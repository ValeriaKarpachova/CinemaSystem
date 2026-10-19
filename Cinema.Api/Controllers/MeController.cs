using System.Security.Claims;
using Cinema.Api.Dtos;
using Cinema.Api.Hubs;
using Cinema.Application.Services;
using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize(Roles = "Customer")]
public class MeController(ITicketService tickets, IHubContext<SeatHub> hub) : ControllerBase
{
    private int CustomerId => int.Parse(User.FindFirstValue("sub")!);

    [HttpGet("tickets")]
    public async Task<List<TicketDto>> Tickets()
    {
        var now = DateTime.Now;   // TODO: київський часовий пояс
        var list = await tickets.SearchAsync(CustomerId, null);
        return list.Select(t => ToDto(t, now))
            .OrderByDescending(t => t.Date).ThenByDescending(t => t.Time)
            .ToList();
    }

    [HttpPost("tickets/{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await tickets.CancelOwnAsync(CustomerId, id);
        if (!result.Success)
            return Problem(detail: result.Error,
                statusCode: result.NotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest);

        await hub.Clients.Group(SeatHub.Group(result.SessionId))
            .SendAsync("SeatReleased", new SeatEvent(result.SessionId, new[] { result.SeatId }, null));

        return NoContent();
    }

    private static TicketDto ToDto(Ticket t, DateTime now)
    {
        var start = t.Session.SessionDate.ToDateTime(t.Session.SessionTime);
        var code = t.Status == TicketStatus.Paid ? "paid"
                 : t.Status == TicketStatus.Cancelled ? "cancelled"
                 : "booked";

        return new TicketDto(
            t.TicketId, t.SessionId, t.Session.Movie.Title,
            t.Session.SessionDate, t.Session.SessionTime,
            t.Seat.RowNumber, t.Seat.SeatNumber, t.Price,
            code, t.Status, t.Status == TicketStatus.Booked && start > now);
    }
}