using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface ISessionRepository : IRepository<Session>
{
    Task<List<Session>> SearchAsync(DateOnly? date, int? movieId, int? hallId);
    Task<bool> ConflictExistsAsync(int hallId, DateOnly date, TimeOnly time, int? excludeId = null);
}