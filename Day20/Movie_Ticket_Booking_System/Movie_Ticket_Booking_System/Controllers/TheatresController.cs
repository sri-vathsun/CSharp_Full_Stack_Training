using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _theatreService;

    public TheatresController(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    // POST: api/theatres
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateTheatre(
        CreateTheatreRequest request)
    {
        var theatre = await _theatreService.CreateTheatreAsync(request);

        return CreatedAtAction(
            nameof(GetTheatreById),
            new { id = theatre.TheatreId },
            theatre);
    }

    // GET: api/theatres
    [HttpGet]
    public async Task<IActionResult> GetAllTheatres()
    {
        var theatres = await _theatreService.GetAllTheatresAsync();

        return Ok(theatres);
    }

    // GET: api/theatres/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTheatreById(int id)
    {
        var theatre = await _theatreService.GetTheatreByIdAsync(id);

        if (theatre == null)
            return NotFound(new { message = "Theatre not found." });

        return Ok(theatre);
    }

    // PUT: api/theatres/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTheatre(
        int id,
        UpdateTheatreRequest request)
    {
        var theatre = await _theatreService.UpdateTheatreAsync(id, request);

        if (theatre == null)
            return NotFound(new { message = "Theatre not found." });

        return Ok(theatre);
    }

    // DELETE: api/theatres/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTheatre(int id)
    {
        var deleted = await _theatreService.DeleteTheatreAsync(id);

        if (!deleted)
            return NotFound(new { message = "Theatre not found." });

        return NoContent();
    }
}