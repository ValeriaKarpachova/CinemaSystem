using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface IHallRepository : IRepository<Hall>
{
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task CreateWithSeatsAsync(Hall hall, List<Seat> seats);
    Task RecalculateCapacityAsync(int hallId);
}