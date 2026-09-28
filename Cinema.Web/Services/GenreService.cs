using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Web.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repo;

    public GenreService(IGenreRepository repo) => _repo = repo;

    public Task<List<Genre>> GetAllAsync() => _repo.GetAllAsync();

    public async Task<OperationResult> SaveAsync(Genre genre)
    {
        genre.GenreName = (genre.GenreName ?? "").Trim();

        if (genre.GenreName.Length == 0)
            return OperationResult.Fail("Назва жанру не може бути порожньою.");
        if (genre.GenreName.Length > 50)
            return OperationResult.Fail("Назва жанру не може перевищувати 50 символів.");

        int? excludeId = genre.GenreId == 0 ? null : genre.GenreId;
        if (await _repo.NameExistsAsync(genre.GenreName, excludeId))
            return OperationResult.Fail("Жанр з такою назвою вже існує.");

        if (genre.GenreId == 0) await _repo.AddAsync(genre);
        else await _repo.UpdateAsync(genre);

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
            return OperationResult.Fail("Неможливо видалити жанр: він використовується у фільмах.");
        }
    }
}