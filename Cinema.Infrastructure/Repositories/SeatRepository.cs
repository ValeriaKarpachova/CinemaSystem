using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class SeatRepository : Repository<Seat>, ISeatRepository
{
    public SeatRepository(IDbContextFactory<CinemaDbContext> factory) : base(factory) { }

    public async Task<List<Seat>> GetByHallAsync(int hallId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Seats.AsNoTracking()
            .Where(s => s.HallId == hallId)
            .OrderBy(s => s.RowNumber).ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<bool> PositionExistsAsync(int hallId, int row, int number, int? excludeId = null)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Seats.AnyAsync(s => s.HallId == hallId && s.RowNumber == row
            && s.SeatNumber == number && s.SeatId != (excludeId ?? 0));
    }

    public async Task<int> CountByHallAsync(int hallId)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Seats.CountAsync(s => s.HallId == hallId);
    }

    public async Task<int> SetRowPriceAsync(int hallId, int row, decimal price)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Seats.Where(s => s.HallId == hallId && s.RowNumber == row)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.Price, price));
    }
}