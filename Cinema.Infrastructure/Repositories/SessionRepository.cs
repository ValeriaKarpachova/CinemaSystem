using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cinema.Infrastructure.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public override async Task AddAsync(Session entity)
    {
        try { await base.AddAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new ScheduleConflictException(ex);
        }
    }

    public override async Task UpdateAsync(Session entity)
    {
        try { await base.UpdateAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new ScheduleConflictException(ex);
        }
    }

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

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}