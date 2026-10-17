using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Cinema.Domain.Exceptions;
using Npgsql;

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

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
    }

    public override async Task AddAsync(Customer entity)
    {
        try { await base.AddAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new DuplicateEmailException(ex);
        }
    }

    public override async Task UpdateAsync(Customer entity)
    {
        try { await base.UpdateAsync(entity); }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new DuplicateEmailException(ex);
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}