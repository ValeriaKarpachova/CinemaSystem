using Cinema.Api.Dtos;
using Cinema.Application.Models;
using Cinema.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController(ISessionService sessions, ISeatMapService seatMaps) : ControllerBase
{
    [HttpGet]
    public async Task<List<SessionDto>> Get([FromQuery] int? movieId, [FromQuery] DateOnly? date)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);   // TODO: київський часовий пояс
        var list = await sessions.SearchAsync(date, movieId, null);
        return list
            .Where(s => s.SessionDate >= today)
            .Select(s => new SessionDto(
                s.SessionId, s.MovieId, s.Movie.Title, s.HallId,
                s.Hall.HallName, s.Hall.ScreenType, s.SessionDate, s.SessionTime))
            .ToList();
    }

    [HttpGet("{id:int}/seats")]
    public async Task<ActionResult<SeatMap>> Seats(int id)
    {
        var map = await seatMaps.GetAsync(id);
        return map is null ? NotFound() : map;
    }
}