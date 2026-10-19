using Cinema.Api.Dtos;
using Cinema.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController(IMovieService movies) : ControllerBase
{
    [HttpGet]
    public async Task<List<MovieDto>> Get([FromQuery] string? title, [FromQuery] int? genreId)
    {
        var list = await movies.SearchAsync(title, genreId);
        return list.Select(m => new MovieDto(
         m.MovieId, m.Title, m.GenreId, m.Genre?.GenreName ?? "", m.Duration,
         m.AgeRating, m.Description, m.ReleaseDate)).ToList();
    }
}