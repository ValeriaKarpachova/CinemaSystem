namespace Cinema.Application.Models;

public record BookingResult(
    bool Success, string? Error = null,
    IReadOnlyList<int>? TicketIds = null, decimal TotalPrice = 0)
{
    public static BookingResult Fail(string error) => new(false, error);
    public static BookingResult Ok(IReadOnlyList<int> ids, decimal total) => new(true, null, ids, total);
}