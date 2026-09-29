using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByTicketIdAsync(int ticketId);
    Task<List<Payment>> SearchAsync(string? method);
}