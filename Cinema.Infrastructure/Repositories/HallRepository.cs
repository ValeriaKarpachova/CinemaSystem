using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class HallRepository : Repository<Hall>, IHallRepository
{
    public HallRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Halls.AnyAsync(h => h.HallName == name && h.HallId != (excludeId ?? 0));
    }

    public async Task CreateWithSeatsAsync(Hall hall, List<Seat> seats)
    {
        await using var db = await Factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        db.Halls.Add(hall);
        await db.SaveChangesAsync();

        foreach (var s in seats) s.HallId = hall.HallId;
        db.Seats.AddRange(seats);
        await db.SaveChangesAsync();

        await tx.CommitAsync();
    }

    public async Task RecalculateCapacityAsync(int hallId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        var count = await db.Seats.CountAsync(s => s.HallId == hallId);
        await db.Halls.Where(h => h.HallId == hallId)
            .ExecuteUpdateAsync(x => x.SetProperty(h => h.Capacity, count));
    }
}