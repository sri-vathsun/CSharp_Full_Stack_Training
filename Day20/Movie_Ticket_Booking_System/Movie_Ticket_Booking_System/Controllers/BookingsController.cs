using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;
using System.Security.Claims;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }

    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    // POST: api/bookings
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var customerName = User.FindFirstValue(ClaimTypes.Name);
            var customerEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userId is null || customerName is null || customerEmail is null)
                return Unauthorized();

            var result = await _service.CreateAsync(
                request,
                userId.Value,
                customerName,
                customerEmail);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.BookingId },
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/bookings
    // Optional filters: ?showId=1&status=Confirmed
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? showId,
        [FromQuery] string? status)
    {
        var result = await _service.GetAllAsync(showId, status);

        return Ok(result);
    }

    // GET: api/bookings/1
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.IsInRole(UserRoles.Admin) ? null : GetCurrentUserId();
        if (userId is null && !User.IsInRole(UserRoles.Admin))
            return Unauthorized();

        var result = await _service.GetByIdAsync(id, userId);

        if (result == null)
            return NotFound(new { message = "Booking not found." });

        return Ok(result);
    }

    // GET: api/bookings/show/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("show/{showId:int}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        try
        {
            var result = await _service.GetByShowIdAsync(showId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PUT: api/bookings/1/cancel
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var userId = User.IsInRole(UserRoles.Admin) ? null : GetCurrentUserId();
            if (userId is null && !User.IsInRole(UserRoles.Admin))
                return Unauthorized();

            var result = await _service.CancelAsync(id, userId);

            if (result == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
