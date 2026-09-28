using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            t.SessionId == sessionId && t.SeatId == seatId && t.TicketId != (excludeId ?? 0));
    }
}