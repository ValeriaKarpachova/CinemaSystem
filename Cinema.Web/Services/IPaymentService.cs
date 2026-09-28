using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface IPaymentService
{
    Task<Payment?> GetByTicketIdAsync(int ticketId);
    Task<OperationResult> RegisterAsync(int ticketId, decimal amount, string method);
    Task<OperationResult> DeleteAsync(int id);
}