
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _service;

    public SeatsController(ISeatService service)
    {
        _service = service;
    }

    // POST: api/seats
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create(CreateSeatRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.SeatId },
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

    // POST: api/seats/bulk
    [HttpPost("bulk")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> BulkCreate(
        BulkCreateSeatsRequest request)
    {
        try
        {
            var result = await _service.BulkCreateAsync(request);

            return StatusCode(StatusCodes.Status201Created, result);
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

    // GET: api/seats/screen/1
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreenId(int screenId)
    {
        var result = await _service.GetByScreenIdAsync(screenId);

        if (result == null)
            return NotFound(new { message = "Screen not found." });

        return Ok(result);
    }

    // GET: api/seats/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Seat not found." });

        return Ok(result);
    }

    // PUT: api/seats/1
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Update(
        int id,
        UpdateSeatRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);

            if (result == null)
                return NotFound(new { message = "Seat not found." });

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

    // DELETE: api/seats/1
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound(new { message = "Seat not found." });

        return NoContent();
    }
}
