using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Web.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repo;

    public MovieService(IMovieRepository repo) => _repo = repo;

    public IReadOnlyList<string> AgeRatings { get; } = new[] { "0+", "6+", "12+", "16+", "18+" };

    public Task<List<Movie>> SearchAsync(string? title, int? genreId) => _repo.SearchAsync(title, genreId);

    public async Task<OperationResult> SaveAsync(Movie movie)
    {
        movie.Title = (movie.Title ?? "").Trim();
        movie.Description = string.IsNullOrWhiteSpace(movie.Description) ? null : movie.Description.Trim();

        if (movie.Title.Length == 0)
            return OperationResult.Fail("Назва фільму не може бути порожньою.");
        if (movie.Title.Length > 255)
            return OperationResult.Fail("Назва фільму не може перевищувати 255 символів.");
        if (movie.GenreId == 0)
            return OperationResult.Fail("Оберіть жанр.");
        if (movie.Duration is < 1 or > 300)
            return OperationResult.Fail("Тривалість має бути від 1 до 300 хвилин.");
        if (!AgeRatings.Contains(movie.AgeRating))
            return OperationResult.Fail("Оберіть віковий рейтинг.");

        if (movie.MovieId == 0) await _repo.AddAsync(movie);
        else await _repo.UpdateAsync(movie);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return OperationResult.Ok();
        }
        catch (RelatedRecordsExistException)
        {
            return OperationResult.Fail("Неможливо видалити фільм: для нього існують сеанси.");
        }
    }
}