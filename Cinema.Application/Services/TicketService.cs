
using Cinema.Application.Models;
using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Application.Services;

public class TicketService : ITicketService
{
    private const int MaxSeatsPerBooking = 6;
    private readonly ITicketRepository _repo;
    private readonly ISeatRepository _seats;
    private readonly ISessionRepository _sessions;
    private readonly ICustomerRepository _customers;

    public TicketService(
        ITicketRepository repo,
        ISeatRepository seats,
        ISessionRepository sessions,
        ICustomerRepository customers)
    {
        _repo = repo;
        _seats = seats;
        _sessions = sessions;
        _customers = customers;
    }

    public Task<List<Ticket>> SearchAsync(int? customerId, string? status) =>
        _repo.SearchAsync(customerId, status);

    public async Task<OperationResult> SaveAsync(Ticket ticket)
    {
        if (ticket.SessionId <= 0)
            return OperationResult.Fail("Оберіть сеанс.");

        if (ticket.SeatId <= 0)
            return OperationResult.Fail("Оберіть місце.");

        if (ticket.CustomerId <= 0)
            return OperationResult.Fail("Оберіть клієнта.");

        if (ticket.Price < 0)
            return OperationResult.Fail("Ціна не може бути від'ємною.");

        if (!TicketStatus.All.Contains(ticket.Status))
            return OperationResult.Fail("Невідомий статус квитка.");

        if (ticket.Status != TicketStatus.Cancelled)
        {
            int? excludeId = ticket.TicketId == 0
                ? null
                : ticket.TicketId;

            if (await _repo.SeatTakenAsync(
                    ticket.SessionId,
                    ticket.SeatId,
                    excludeId))
            {
                return OperationResult.Fail(
                    "Це місце на обраний сеанс вже зайняте.");
            }
        }

        if (ticket.TicketId == 0)
        {
            try
            {
                await _repo.AddAsync(ticket);
                return OperationResult.Ok();
            }
            catch (SeatAlreadyTakenException)
            {
                return OperationResult.Fail(
                    "Це місце на обраний сеанс вже зайняте.");
            }
        }

        var existing = await _repo.GetByIdAsync(ticket.TicketId);

        if (existing is null)
            return OperationResult.Fail("Квиток не знайдено.");

        existing.SessionId = ticket.SessionId;
        existing.SeatId = ticket.SeatId;
        existing.CustomerId = ticket.CustomerId;
        existing.Price = ticket.Price;
        existing.Status = ticket.Status;

        try
        {
            await _repo.UpdateAsync(existing);
            return OperationResult.Ok();
        }
        catch (SeatAlreadyTakenException)
        {
            return OperationResult.Fail(
                "Це місце на обраний сеанс вже зайняте.");
        }
    }

    public async Task<BookingResult> BookAsync(
        int customerId,
        int sessionId,
        IReadOnlyCollection<int> seatIds)
    {
        var ids = seatIds.Distinct().ToList();

        if (ids.Count == 0)
            return BookingResult.Fail("Оберіть хоча б одне місце.");

        if (ids.Count > MaxSeatsPerBooking)
        {
            return BookingResult.Fail(
                $"Можна забронювати не більше {MaxSeatsPerBooking} місць за раз.");
        }

        if (await _customers.GetByIdAsync(customerId) is null)
            return BookingResult.Fail("Клієнта не знайдено.");

        var session = await _sessions.GetByIdAsync(sessionId);

        if (session is null)
            return BookingResult.Fail("Сеанс не знайдено.");

        if (session.SessionDate.ToDateTime(session.SessionTime) <= DateTime.Now)
        {
            return BookingResult.Fail(
                "Сеанс уже розпочався або минув.");
        }

        var hallSeats = await _seats.GetByHallAsync(session.HallId);

        var chosen = hallSeats
            .Where(s => ids.Contains(s.SeatId))
            .ToList();

        if (chosen.Count != ids.Count)
        {
            return BookingResult.Fail(
                "Деякі місця не належать залу цього сеансу.");
        }

        var tickets = chosen.Select(s => new Ticket
        {
            SessionId = sessionId,
            SeatId = s.SeatId,
            CustomerId = customerId,
            Price = s.Price,
            Status = TicketStatus.Booked
        }).ToList();

        try
        {
            await _repo.AddRangeAsync(tickets);
        }
        catch (SeatAlreadyTakenException)
        {
            return BookingResult.Fail(
                "Одне або кілька обраних місць щойно забронював інший глядач. Оновіть схему залу.",
                conflict: true);
        }

        return BookingResult.Ok(
            tickets.Select(t => t.TicketId).ToList(),
            tickets.Sum(t => t.Price));
    }

    public async Task<OperationResult> ChangeStatusAsync(
        int ticketId,
        string newStatus)
    {
        if (!TicketStatus.All.Contains(newStatus))
            return OperationResult.Fail("Невідомий статус квитка.");

        var ticket = await _repo.GetByIdAsync(ticketId);

        if (ticket is null)
            return OperationResult.Fail("Квиток не знайдено.");

        ticket.Status = newStatus;

        try
        {
            await _repo.UpdateAsync(ticket);
        }
        catch (SeatAlreadyTakenException)
        {
            return OperationResult.Fail(
                "Неможливо відновити квиток: місце вже зайняте іншим.");
        }

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
            return OperationResult.Fail(
                "Неможливо видалити квиток: на нього вже є платіж.");
        }
    }
}
