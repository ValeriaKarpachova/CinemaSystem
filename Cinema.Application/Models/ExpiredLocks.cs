namespace Cinema.Application.Models;

public record ExpiredLocks(int SessionId, IReadOnlyList<int> SeatIds);