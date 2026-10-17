using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Application.Services;

public interface ISessionService
{
    Task<List<Session>> SearchAsync(DateOnly? date, int? movieId, int? hallId);
    Task<OperationResult> SaveAsync(Session session);
    Task<OperationResult> DeleteAsync(int id);
}