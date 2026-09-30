using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<CinemaDbContext> _factory;

    public UserRepository(IDbContextFactory<CinemaDbContext> factory) => _factory = factory;

    public async Task<AppUser?> GetByLoginAsync(string login)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<bool> AnyAdminExistsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Role == "Admin");
    }

    public async Task AddAsync(AppUser user)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}