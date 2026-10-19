namespace Cinema.Application.Models;

public record CancelResult(
    bool Success, string? Error = null, int SessionId = 0, int SeatId = 0, bool NotFound = false)
{
    public static CancelResult Fail(string error, bool notFound = false) =>
        new(false, error, NotFound: notFound);
    public static CancelResult Ok(int sessionId, int seatId) => new(true, null, sessionId, seatId);
}