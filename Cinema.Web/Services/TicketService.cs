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

    public async Task<OperationResult> SaveAsync(Ticket ticket)
    {
        if (ticket.SessionId <= 0) return OperationResult.Fail("Оберіть сеанс.");
        if (ticket.SeatId <= 0) return OperationResult.Fail("Оберіть місце.");
        if (ticket.CustomerId <= 0) return OperationResult.Fail("Оберіть клієнта.");
        if (ticket.Price < 0) return OperationResult.Fail("Ціна не може бути від'ємною.");
        if (!TicketStatus.All.Contains(ticket.Status))
            return OperationResult.Fail("Невідомий статус квитка.");

        // унікальність «сеанс + місце»: перевіряємо заздалегідь, щоб показати зрозуміле повідомлення
        int? excludeId = ticket.TicketId == 0 ? null : ticket.TicketId;
        if (await _repo.SeatTakenAsync(ticket.SessionId, ticket.SeatId, excludeId))
            return OperationResult.Fail("Це місце на обраний сеанс вже зайняте.");

        if (ticket.TicketId == 0)
        {
            await _repo.AddAsync(ticket);   
            return OperationResult.Ok();
        }

        var existing = await _repo.GetByIdAsync(ticket.TicketId);
        if (existing is null) return OperationResult.Fail("Квиток не знайдено.");

        existing.SessionId = ticket.SessionId;
        existing.SeatId = ticket.SeatId;
        existing.CustomerId = ticket.CustomerId;
        existing.Price = ticket.Price;
        existing.Status = ticket.Status;

        await _repo.UpdateAsync(existing);
        return OperationResult.Ok();
    }

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