using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _screenService;

    public ScreensController(IScreenService screenService)
    {
        _screenService = screenService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateScreenRequest request)
    {
        try
        {
            var result = await _screenService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.ScreenId },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _screenService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _screenService.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound(new { message = "Screen not found." });
        }

        return Ok(result);
    }

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatreId(int theatreId)
    {
        var result =
            await _screenService.GetByTheatreIdAsync(theatreId);

        if (result == null)
        {
            return NotFound(new { message = "Theatre not found." });
        }

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateScreenRequest request)
    {
        try
        {
            var result = await _screenService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new { message = "Screen not found." });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _screenService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Screen not found." });
        }

        return NoContent();
    }
}