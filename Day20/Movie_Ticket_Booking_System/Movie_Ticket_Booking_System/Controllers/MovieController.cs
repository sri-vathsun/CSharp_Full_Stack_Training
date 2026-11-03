using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Constants;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    // POST: api/movies
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateMovie(
        CreateMovieRequest request)
    {
        try
        {
            var movie = await _movieService.CreateMovieAsync(request);

            return CreatedAtAction(
                nameof(GetMovieById),
                new { id = movie.MovieId },
                movie);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/movies
    [HttpGet]
    public async Task<IActionResult> GetAllMovies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination values.");

        var movies = await _movieService.GetAllMoviesAsync(
            pageNumber, pageSize, title);

        return Ok(movies);
    }

    // GET: api/movies/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
            return NotFound("Movie not found.");

        return Ok(movie);
    }

    // PUT: api/movies/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(
        int id,
        UpdateMovieRequest request)
    {
        try
        {
            var movie = await _movieService.UpdateMovieAsync(id, request);

            if (movie == null)
                return NotFound("Movie not found.");

            return Ok(movie);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/movies/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            var deleted = await _movieService.DeleteMovieAsync(id);

            if (!deleted)
                return NotFound("Movie not found.");

            return NoContent();
        }
        catch (Exception)
        {
            return Conflict(
                "Movie cannot be deleted because dependent records exist.");
        }
    }

    // GET: api/movies/search?title=Leo
    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(
        [FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Movie title is required.");

        var movies = await _movieService.GetAllMoviesAsync(
            1, 100, title);

        return Ok(movies);
    }
}