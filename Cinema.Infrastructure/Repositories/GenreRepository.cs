using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Genres.AnyAsync(g => g.GenreName == name && g.GenreId != (excludeId ?? 0));
    }
}