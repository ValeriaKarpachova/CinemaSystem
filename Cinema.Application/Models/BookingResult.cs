namespace Cinema.Application.Models;

public record BookingResult(
    bool Success, string? Error = null,
    IReadOnlyList<int>? TicketIds = null, decimal TotalPrice = 0, bool Conflict = false)
{
    public static BookingResult Fail(string error, bool conflict = false) =>
        new(false, error, null, 0, conflict);
    public static BookingResult Ok(IReadOnlyList<int> ids, decimal total) => new(true, null, ids, total);
}