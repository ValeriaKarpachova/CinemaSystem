using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface IMovieRepository : IRepository<Movie>
{
    Task<List<Movie>> SearchAsync(string? title, int? genreId);
}