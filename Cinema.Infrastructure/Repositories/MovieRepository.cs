using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<List<Movie>> SearchAsync(string? title, int? genreId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        IQueryable<Movie> query = db.Movies.AsNoTracking().Include(m => m.Genre);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(m => EF.Functions.ILike(m.Title, $"%{title.Trim()}%"));

        if (genreId.HasValue)
            query = query.Where(m => m.GenreId == genreId.Value);

        return await query.OrderBy(m => m.Title).ToListAsync();
    }
}