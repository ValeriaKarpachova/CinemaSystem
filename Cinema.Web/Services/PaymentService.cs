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

    public Task<List<Payment>> SearchAsync(string? method) => _repo.SearchAsync(method);

    public Task<Payment?> GetByTicketIdAsync(int ticketId) => _repo.GetByTicketIdAsync(ticketId);

    public async Task<OperationResult> RegisterAsync(int ticketId, decimal amount, string method)
    {
        var error = Validate(amount, method);
        if (error is not null) return OperationResult.Fail(error);

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
            return OperationResult.Fail("Оплата для цього квитка вже зареєстрована.");
        }

        ticket.Status = TicketStatus.Paid;
        await _tickets.UpdateAsync(ticket);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> UpdateAsync(int paymentId, decimal amount, string method)
    {
        var error = Validate(amount, method);
        if (error is not null) return OperationResult.Fail(error);

        var payment = await _repo.GetByIdAsync(paymentId);
        if (payment is null) return OperationResult.Fail("Платіж не знайдено.");

        payment.Amount = amount;
        payment.PaymentMethod = method;
        await _repo.UpdateAsync(payment);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var payment = await _repo.GetByIdAsync(id);
        if (payment is not null)
        {
            var ticket = await _tickets.GetByIdAsync(payment.TicketId);
            if (ticket is not null && ticket.Status == TicketStatus.Paid)
            {
                ticket.Status = TicketStatus.Booked;
                await _tickets.UpdateAsync(ticket);
            }
        }

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

    private static string? Validate(decimal amount, string method)
    {
        if (!PaymentMethod.All.Contains(method)) return "Невідомий спосіб оплати.";
        if (amount <= 0) return "Сума оплати має бути більшою за нуль.";
        return null;
    }
}