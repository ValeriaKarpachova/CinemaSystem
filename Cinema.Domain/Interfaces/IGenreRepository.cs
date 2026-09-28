using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface IGenreRepository : IRepository<Genre>
{
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
}