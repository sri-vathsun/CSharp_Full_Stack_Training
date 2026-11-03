
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _service;

    public ShowsController(IShowService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create(
        CreateShowRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.ShowId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] int? movieId,
        [FromQuery] int? theatreId)
    {
        var result = await _service.GetAllAsync(
            date, movieId, theatreId);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Show not found." });

        return Ok(result);
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovie(int movieId)
    {
        var result = await _service.GetByMovieIdAsync(movieId);

        return Ok(result);
    }

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatre(int theatreId)
    {
        var result = await _service.GetByTheatreIdAsync(theatreId);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByDate(
        [FromQuery] DateTime date)
    {
        var result = await _service.GetByDateAsync(date);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Update(
        int id,
        UpdateShowRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);

            if (result == null)
                return NotFound(new { message = "Show not found." });

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Show not found." });

            return NoContent();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return Conflict(new
            {
                message = "This show cannot be deleted because it has dependent booking records."
            });
        }
    }

    [HttpGet("{showId:int}/seats")]
    public async Task<IActionResult> GetSeats(int showId)
    {
        var result = await _service.GetShowSeatsAsync(showId);

        if (result == null)
            return NotFound(new { message = "Show not found." });

        return Ok(result);
    }

    [HttpGet("{showId:int}/seats/available")]
    public async Task<IActionResult> GetAvailableSeats(int showId)
    {
        var result = await _service.GetAvailableSeatsAsync(showId);

        if (result == null)
            return NotFound(new { message = "Show not found." });

        return Ok(result);
    }

    [HttpGet("{showId:int}/seats/booked")]
    public async Task<IActionResult> GetBookedSeats(int showId)
    {
        var result = await _service.GetBookedSeatsAsync(showId);

        if (result == null)
            return NotFound(new { message = "Show not found." });

        return Ok(result);
    }
}
