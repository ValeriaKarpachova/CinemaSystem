using System.Security.Claims;
using Cinema.Api.Dtos;
using Cinema.Application.Models;
using Cinema.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Api.Hubs;

public class SeatHub(ISeatLockService locks, ISeatMapService seatMaps) : Hub
{
    private const int MaxSeatsPerLock = 6;

    public static string Group(int sessionId) => $"session-{sessionId}";

    private int CustomerId => int.Parse(Context.User!.FindFirstValue("sub")!);

    public Task JoinSession(int sessionId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, Group(sessionId));

    public Task LeaveSession(int sessionId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(sessionId));

    [Authorize(Roles = "Customer")]
    public async Task<LockResponse> LockSeats(int sessionId, int[] seatIds)
    {
        var ids = seatIds.Distinct().ToList();
        if (ids.Count == 0 || ids.Count > MaxSeatsPerLock)
            return new(false, $"Оберіть від 1 до {MaxSeatsPerLock} місць.", null);

        var map = await seatMaps.GetAsync(sessionId);
        if (map is null) return new(false, "Сеанс не знайдено.", null);

        var byId = map.Seats.ToDictionary(s => s.SeatId);
        if (ids.Any(id => !byId.TryGetValue(id, out var seat)
                          || seat.Status is SeatState.Booked or SeatState.Sold))
            return new(false, "Деякі місця недоступні.", null);

        if (!locks.TryLock(sessionId, ids, CustomerId, out var expiresAt))
            return new(false, "Деякі місця тимчасово утримує інший глядач.", null);

        await Clients.OthersInGroup(Group(sessionId))
            .SendAsync("SeatLocked", new SeatEvent(sessionId, ids, expiresAt));

        return new(true, null, expiresAt);
    }

    [Authorize(Roles = "Customer")]
    public async Task ReleaseSeats(int sessionId, int[] seatIds)
    {
        var released = locks.Release(sessionId, seatIds.Distinct().ToList(), CustomerId);
        if (released.Count > 0)
            await Clients.Group(Group(sessionId))
                .SendAsync("SeatReleased", new SeatEvent(sessionId, released, null));
    }
}