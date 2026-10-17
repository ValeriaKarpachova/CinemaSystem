using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Cinema.Domain.Common;
using Cinema.Domain.Exceptions;
using Npgsql;

namespace Cinema.Infrastructure.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<List<Ticket>> SearchAsync(int? customerId, string? status)
    {
        await using var db = await Factory.CreateDbContextAsync();
        IQueryable<Ticket> query = db.Tickets.AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.Seat)
            .Include(t => t.Session).ThenInclude(s => s.Movie);

        if (customerId.HasValue) query = query.Where(t => t.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(t => t.Status == status);

        return await query.OrderByDescending(t => t.TicketId).ToListAsync();
    }

    public async Task<bool> SeatTakenAsync(int sessionId, int seatId, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Tickets.AnyAsync(t =>
            t.SessionId == sessionId && t.SeatId == seatId
            && t.Status != TicketStatus.Cancelled
            && t.TicketId != (excludeId ?? 0));
    }

    public override async Task AddAsync(Ticket entity)
    {
        try { await base.AddAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new SeatAlreadyTakenException(ex);
        }
    }

    public override async Task UpdateAsync(Ticket entity)
    {
        try { await base.UpdateAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new SeatAlreadyTakenException(ex);
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}