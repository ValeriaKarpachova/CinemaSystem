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
}