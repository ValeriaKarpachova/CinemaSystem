using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<List<Customer>> SearchAsync(string? name)
    {
        await using var db = await Factory.CreateDbContextAsync();
        IQueryable<Customer> query = db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var term = name.Trim();
            query = query.Where(c =>
                EF.Functions.ILike(c.FirstName, $"%{term}%") ||
                EF.Functions.ILike(c.LastName, $"%{term}%") ||
                EF.Functions.ILike(c.Email, $"%{term}%"));
        }

        return await query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Customers.AnyAsync(c => c.Email == email && c.CustomerId != (excludeId ?? 0));
    }
}