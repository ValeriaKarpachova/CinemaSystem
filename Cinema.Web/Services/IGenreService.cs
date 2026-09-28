using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface IGenreService
{
    Task<List<Genre>> GetAllAsync();
    Task<OperationResult> SaveAsync(Genre genre);
    Task<OperationResult> DeleteAsync(int id);
}