
using Campus_Plcement_Management_Web_Api.DTOs.College;
using Campus_Plcement_Management_Web_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/colleges")]
public class CollegeController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegeController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    // Public: College registration
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        CreateCollegeDto dto)
    {
        try
        {
            var result = await _collegeService.RegisterAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.CollegeId },
                result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // SuperAdmin: Get all colleges
    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _collegeService.GetAllAsync();

        return Ok(result);
    }

    // SuperAdmin: Get pending colleges
    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _collegeService.GetPendingAsync();

        return Ok(result);
    }

    // SuperAdmin: Get college by ID
    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _collegeService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // SuperAdmin: Approve college
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var result = await _collegeService.ApproveAsync(id);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // SuperAdmin: Reject college
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var result = await _collegeService.RejectAsync(id);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // SuperAdmin: Suspend college
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        try
        {
            var result = await _collegeService.SuspendAsync(id);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // SuperAdmin: Update college
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCollegeDto dto)
    {
        try
        {
            var result = await _collegeService.UpdateAsync(id, dto);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
