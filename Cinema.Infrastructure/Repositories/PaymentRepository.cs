using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<Payment?> GetByTicketIdAsync(int ticketId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.TicketId == ticketId);
    }

    public async Task<List<Payment>> SearchAsync(string? method)
    {
        await using var db = await Factory.CreateDbContextAsync();
        IQueryable<Payment> query = db.Payments.AsNoTracking()
            .Include(p => p.Ticket).ThenInclude(t => t.Customer)
            .Include(p => p.Ticket).ThenInclude(t => t.Session).ThenInclude(s => s.Movie);

        if (!string.IsNullOrWhiteSpace(method))
            query = query.Where(p => p.PaymentMethod == method);

        return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
    }
}