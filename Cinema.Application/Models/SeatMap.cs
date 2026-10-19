
namespace Cinema.Application.Models;

public static class SeatState
{
    public const string Free = "free";
    public const string Booked = "booked";
    public const string Sold = "sold";
}

public record SeatMapItem(int SeatId, int Row, int Number, decimal Price, string Status);
public record SeatMap(int SessionId, IReadOnlyList<SeatMapItem> Seats);
