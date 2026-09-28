using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<List<Ticket>> SearchAsync(int? customerId, string? status);
    Task<bool> SeatTakenAsync(int sessionId, int seatId, int? excludeId = null);
}