using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Web.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _repo;

    public TicketService(ITicketRepository repo) => _repo = repo;

    public Task<List<Ticket>> SearchAsync(int? customerId, string? status) =>
        _repo.SearchAsync(customerId, status);

    public async Task<OperationResult> ChangeStatusAsync(int ticketId, string newStatus)
    {
        if (!TicketStatus.All.Contains(newStatus))
            return OperationResult.Fail("Невідомий статус квитка.");

        var ticket = await _repo.GetByIdAsync(ticketId);
        if (ticket is null) return OperationResult.Fail("Квиток не знайдено.");

        ticket.Status = newStatus;
        await _repo.UpdateAsync(ticket);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return OperationResult.Ok();
        }
        catch (RelatedRecordsExistException)
        {
            return OperationResult.Fail("Неможливо видалити квиток: на нього вже є платіж.");
        }
    }
}