using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Web.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly ITicketRepository _tickets;

    public PaymentService(IPaymentRepository repo, ITicketRepository tickets)
    {
        _repo = repo;
        _tickets = tickets;
    }

    public Task<Payment?> GetByTicketIdAsync(int ticketId) => _repo.GetByTicketIdAsync(ticketId);

    public async Task<OperationResult> RegisterAsync(int ticketId, decimal amount, string method)
    {
        if (!PaymentMethod.All.Contains(method))
            return OperationResult.Fail("Невідомий спосіб оплати.");
        if (amount <= 0)
            return OperationResult.Fail("Сума оплати має бути більшою за нуль.");

        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return OperationResult.Fail("Квиток не знайдено.");

        if (await _repo.GetByTicketIdAsync(ticketId) is not null)
            return OperationResult.Fail("Оплата для цього квитка вже зареєстрована.");

        var payment = new Payment
        {
            TicketId = ticketId,
            Amount = amount,
            PaymentMethod = method,
            PaymentDate = DateTime.Now
        };

        try
        {
            await _repo.AddAsync(payment);
        }
        catch (DbUpdateException)
        {
            // підстраховка UNIQUE (ticket_id), якщо оплату зареєстрували одночасно
            return OperationResult.Fail("Оплата для цього квитка вже зареєстрована.");
        }

        ticket.Status = TicketStatus.Paid;
        await _tickets.UpdateAsync(ticket);

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
            return OperationResult.Fail("Неможливо видалити платіж.");
        }
    }
}