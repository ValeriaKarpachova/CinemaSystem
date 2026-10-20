using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cinema.Application.Models;

namespace Cinema.Web.Client;

public class CinemaApiClient(HttpClient http, CustomerSession session)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    // --- авторизация ---
    public Task<ApiResult<AuthResponse>> LoginAsync(string email, string password) =>
        SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/login", new LoginRequest(email, password));

    public async Task<ApiResult> RegisterAsync(RegisterCustomerRequest request) =>
        await SendAsync<object>(HttpMethod.Post, "api/auth/register", request);

    // --- публичные данные ---
    public Task<ApiResult<List<MovieDto>>> GetMoviesAsync(string? title = null, int? genreId = null)
    {
        var q = new List<string>();
        if (!string.IsNullOrWhiteSpace(title)) q.Add($"title={Uri.EscapeDataString(title.Trim())}");
        if (genreId is not null) q.Add($"genreId={genreId}");
        return SendAsync<List<MovieDto>>(HttpMethod.Get, "api/movies" + ToQuery(q));
    }

    public Task<ApiResult<List<SessionDto>>> GetSessionsAsync(int? movieId = null, DateOnly? date = null)
    {
        var q = new List<string>();
        if (movieId is not null) q.Add($"movieId={movieId}");
        if (date is not null) q.Add($"date={date.Value:yyyy-MM-dd}");
        return SendAsync<List<SessionDto>>(HttpMethod.Get, "api/sessions" + ToQuery(q));
    }

    public Task<ApiResult<SeatMap>> GetSeatMapAsync(int sessionId) =>
        SendAsync<SeatMap>(HttpMethod.Get, $"api/sessions/{sessionId}/seats");

    // --- только для вошедшего клиента ---
    public Task<ApiResult<BookingResponse>> BookAsync(int sessionId, IEnumerable<int> seatIds) =>
        SendAsync<BookingResponse>(HttpMethod.Post, "api/bookings",
            new BookingRequest(sessionId, seatIds.ToList()), auth: true);

    public Task<ApiResult<List<TicketDto>>> GetMyTicketsAsync() =>
        SendAsync<List<TicketDto>>(HttpMethod.Get, "api/me/tickets", auth: true);

    public async Task<ApiResult> CancelTicketAsync(int ticketId) =>
        await SendAsync<object>(HttpMethod.Post, $"api/me/tickets/{ticketId}/cancel", auth: true);

    // --- внутренности ---
    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string url, object? body = null, bool auth = false)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null) request.Content = JsonContent.Create(body, options: Json);

            if (auth)
            {
                if (!session.IsAuthenticated) return Fail<T>(401, "Увійдіть в акаунт.");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
            }

            using var response = await http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                var value = string.IsNullOrWhiteSpace(text) ? default : JsonSerializer.Deserialize<T>(text, Json);
                return new ApiResult<T> { Success = true, StatusCode = (int)response.StatusCode, Value = value };
            }

            // токен просрочен или недействителен
            if (auth && response.StatusCode == HttpStatusCode.Unauthorized)
                await session.SignOutAsync();

            return Fail<T>((int)response.StatusCode, await ReadErrorAsync(response));
        }
        catch (HttpRequestException) { return Fail<T>(0, "Сервер недоступний. Спробуйте пізніше."); }
        catch (TaskCanceledException) { return Fail<T>(0, "Сервер не відповів вчасно."); }
    }

    private static ApiResult<T> Fail<T>(int status, string error) =>
        new() { Success = false, StatusCode = status, Error = error };

    private static string ToQuery(List<string> parts) =>
        parts.Count == 0 ? "" : "?" + string.Join("&", parts);

    private static async Task<string> ReadErrorAsync(HttpResponseMessage r)
    {
        try
        {
            using var doc = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                return detail.GetString()!;

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                foreach (var field in errors.EnumerateObject())
                    foreach (var message in field.Value.EnumerateArray())
                        if (message.GetString() is { Length: > 0 } text) return text;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException) { }

        return r.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Потрібно увійти в акаунт.",
            HttpStatusCode.Forbidden => "Недостатньо прав.",
            HttpStatusCode.NotFound => "Не знайдено.",
            _ => "Сталася помилка. Спробуйте ще раз."
        };
    }
}