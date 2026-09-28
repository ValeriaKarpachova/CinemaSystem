using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly IDbContextFactory<CinemaDbContext> Factory;

    public Repository(IDbContextFactory<CinemaDbContext> factory) => Factory = factory;

    public async Task<List<T>> GetAllAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await using var db = await Factory.CreateDbContextAsync();
        db.Set<T>().Add(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await using var db = await Factory.CreateDbContextAsync();
        db.Set<T>().Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await Factory.CreateDbContextAsync();
        var entity = await db.Set<T>().FindAsync(id);
        if (entity is null) return;

        db.Set<T>().Remove(entity);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new RelatedRecordsExistException();
        }
    }
}