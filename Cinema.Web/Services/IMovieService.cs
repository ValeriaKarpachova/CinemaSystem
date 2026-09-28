using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface IMovieService
{
    IReadOnlyList<string> AgeRatings { get; }
    Task<List<Movie>> SearchAsync(string? title, int? genreId);
    Task<OperationResult> SaveAsync(Movie movie);
    Task<OperationResult> DeleteAsync(int id);
}