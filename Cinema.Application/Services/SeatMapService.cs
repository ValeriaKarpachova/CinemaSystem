using Cinema.Application.Models;
using Cinema.Domain.Common;
using Cinema.Domain.Interfaces;

namespace Cinema.Application.Services;

public class SeatMapService(ISessionRepository sessions, ISeatRepository seats,
    ITicketRepository tickets, ISeatLockService locks) : ISeatMapService
{
    public async Task<SeatMap?> GetAsync(int sessionId)
    {
        var session = await sessions.GetByIdAsync(sessionId);
        if (session is null) return null;

        var hallSeats = await seats.GetByHallAsync(session.HallId);
        var taken = await tickets.GetActiveSeatStatusesAsync(sessionId);
        var locked = locks.GetLockedSeats(sessionId).ToHashSet();

        var items = hallSeats.Select(s => new SeatMapItem(
            s.SeatId, s.RowNumber, s.SeatNumber, s.Price,
            taken.TryGetValue(s.SeatId, out var status)
                ? (status == TicketStatus.Paid ? SeatState.Sold : SeatState.Booked)
                : locked.Contains(s.SeatId) ? SeatState.Locked : SeatState.Free)).ToList();

        return new SeatMap(sessionId, items);
    }
}