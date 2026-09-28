using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface ISeatRepository : IRepository<Seat>
{
    Task<List<Seat>> GetByHallAsync(int hallId);
    Task<bool> PositionExistsAsync(int hallId, int row, int number, int? excludeId = null);
    Task<int> CountByHallAsync(int hallId);
    Task<int> SetRowPriceAsync(int hallId, int row, decimal price);
}