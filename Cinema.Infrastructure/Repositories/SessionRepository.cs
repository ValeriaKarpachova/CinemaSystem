using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<List<Session>> SearchAsync(DateOnly? date, int? movieId, int? hallId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        IQueryable<Session> query = db.Sessions.AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall);

        if (date.HasValue) query = query.Where(s => s.SessionDate == date.Value);
        if (movieId.HasValue) query = query.Where(s => s.MovieId == movieId.Value);
        if (hallId.HasValue) query = query.Where(s => s.HallId == hallId.Value);

        return await query
            .OrderBy(s => s.SessionDate).ThenBy(s => s.SessionTime)
            .ToListAsync();
    }

    public async Task<bool> ConflictExistsAsync(int hallId, DateOnly date, TimeOnly time, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Sessions.AnyAsync(s =>
            s.HallId == hallId && s.SessionDate == date && s.SessionTime == time
            && s.SessionId != (excludeId ?? 0));
    }
}