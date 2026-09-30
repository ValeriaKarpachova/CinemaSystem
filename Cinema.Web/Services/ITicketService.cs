using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface ITicketService
{
    Task<List<Ticket>> SearchAsync(int? customerId, string? status);
    Task<OperationResult> SaveAsync(Ticket ticket);  
    Task<OperationResult> ChangeStatusAsync(int ticketId, string newStatus);
    Task<OperationResult> DeleteAsync(int id);
}