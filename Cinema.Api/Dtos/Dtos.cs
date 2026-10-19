namespace Cinema.Api.Dtos;

public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string FullName);

public record MovieDto(int Id, string Title, string Genre, int DurationMinutes,
    string AgeRating, string? Description, DateOnly? ReleaseDate);

public record SessionDto(int Id, int MovieId, string MovieTitle, int HallId,
    string HallName, string ScreenType, DateOnly Date, TimeOnly Time);

public record BookingRequest(int SessionId, List<int> SeatIds);
public record BookingResponse(IReadOnlyList<int> TicketIds, decimal TotalPrice);

public record SeatEvent(int SessionId, IReadOnlyCollection<int> SeatIds, DateTimeOffset? ExpiresAt);
public record LockResponse(bool Success, string? Error, DateTimeOffset? ExpiresAt);