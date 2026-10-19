using System.Security.Claims;
using Cinema.Api.Dtos;
using Cinema.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = "Customer")]
public class BookingsController(ITicketService tickets) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create(BookingRequest request)
    {
        var customerId = int.Parse(User.FindFirstValue("sub")!);
        var result = await tickets.BookAsync(customerId, request.SessionId, request.SeatIds);

        if (!result.Success)
            return Problem(detail: result.Error,
                statusCode: result.Conflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest);

        return new BookingResponse(result.TicketIds!, result.TotalPrice);
    }
}