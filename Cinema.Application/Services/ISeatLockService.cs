using Cinema.Application.Models;

namespace Cinema.Application.Services;

public interface ISeatLockService
{
    bool TryLock(int sessionId, IReadOnlyCollection<int> seatIds, int customerId, out DateTimeOffset expiresAt);
    IReadOnlyCollection<int> Release(int sessionId, IReadOnlyCollection<int> seatIds, int customerId);
    bool IsLockedByOther(int sessionId, IReadOnlyCollection<int> seatIds, int customerId);
    IReadOnlyCollection<int> GetLockedSeats(int sessionId);
    IReadOnlyList<ExpiredLocks> RemoveExpired();
}