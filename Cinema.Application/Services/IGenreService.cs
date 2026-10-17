using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Application.Services;

public interface IGenreService
{
    Task<List<Genre>> GetAllAsync();
    Task<OperationResult> SaveAsync(Genre genre);
    Task<OperationResult> DeleteAsync(int id);
}