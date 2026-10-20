namespace Cinema.Web.Client;

public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string FullName);

public record MovieDto(int Id, string Title, int GenreId, string Genre, int DurationMinutes,
    string AgeRating, string? Description, DateOnly? ReleaseDate, string? PosterUrl);

public record SessionDto(int Id, int MovieId, string MovieTitle, int HallId,
    string HallName, string ScreenType, DateOnly Date, TimeOnly Time);

public record BookingRequest(int SessionId, List<int> SeatIds);
public record BookingResponse(List<int> TicketIds, decimal TotalPrice);

public record TicketDto(int Id, int SessionId, string MovieTitle, DateOnly Date, TimeOnly Time,
    int Row, int Seat, decimal Price, string Status, string StatusLabel, bool CanCancel);

public record SeatEvent(int SessionId, List<int> SeatIds, DateTimeOffset? ExpiresAt);
public record LockResponse(bool Success, string? Error, DateTimeOffset? ExpiresAt);
