using Cinema.Application.Models;

namespace Cinema.Application.Services;

public class InMemorySeatLockService(TimeProvider clock, TimeSpan holdDuration) : ISeatLockService
{
    private record SeatLock(int CustomerId, DateTimeOffset ExpiresAt);

    private readonly object _sync = new();
    private readonly Dictionary<(int Session, int Seat), SeatLock> _locks = new();

    public bool TryLock(int sessionId, IReadOnlyCollection<int> seatIds, int customerId, out DateTimeOffset expiresAt)
    {
        var now = clock.GetUtcNow();
        expiresAt = now + holdDuration;

        lock (_sync)
        {
            foreach (var seatId in seatIds)
            {
                if (_locks.TryGetValue((sessionId, seatId), out var l)
                    && l.ExpiresAt > now && l.CustomerId != customerId)
                    return false;
            }

            foreach (var seatId in seatIds)
                _locks[(sessionId, seatId)] = new SeatLock(customerId, expiresAt);

            return true;
        }
    }

    public IReadOnlyCollection<int> Release(int sessionId, IReadOnlyCollection<int> seatIds, int customerId)
    {
        var released = new List<int>();
        lock (_sync)
        {
            foreach (var seatId in seatIds)
            {
                if (_locks.TryGetValue((sessionId, seatId), out var l) && l.CustomerId == customerId)
                {
                    _locks.Remove((sessionId, seatId));
                    released.Add(seatId);
                }
            }
        }
        return released;
    }

    public bool IsLockedByOther(int sessionId, IReadOnlyCollection<int> seatIds, int customerId)
    {
        var now = clock.GetUtcNow();
        lock (_sync)
        {
            return seatIds.Any(id => _locks.TryGetValue((sessionId, id), out var l)
                && l.ExpiresAt > now && l.CustomerId != customerId);
        }
    }

    public IReadOnlyCollection<int> GetLockedSeats(int sessionId)
    {
        var now = clock.GetUtcNow();
        lock (_sync)
        {
            return _locks
                .Where(p => p.Key.Session == sessionId && p.Value.ExpiresAt > now)
                .Select(p => p.Key.Seat)
                .ToList();
        }
    }

    public IReadOnlyList<ExpiredLocks> RemoveExpired()
    {
        var now = clock.GetUtcNow();
        lock (_sync)
        {
            var expired = _locks.Where(p => p.Value.ExpiresAt <= now).Select(p => p.Key).ToList();
            foreach (var key in expired) _locks.Remove(key);

            return expired
                .GroupBy(k => k.Session)
                .Select(g => new ExpiredLocks(g.Key, g.Select(k => k.Seat).ToList()))
                .ToList();
        }
    }
}