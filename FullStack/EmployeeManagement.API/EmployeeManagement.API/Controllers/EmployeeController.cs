using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeeController(EmployeeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(
            CreateEmployeeDto dto)
        {
            var employee = await _service.AddEmployee(dto);

            return CreatedAtAction(
                nameof(AddEmployee),
                new { id = employee.Id },
                employee
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _service.GetEmployees();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _service.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeDto dto)
        {
            var updated = await _service.UpdateEmployee(id, dto);

            if (!updated)
            {
                return NotFound("Employee not found");
            }

            return Ok(new { message = "Employee updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _service.DeleteEmployee(id);

            if (!deleted)
            {
                return NotFound("Employee not found");
            }

            return Ok(new { message = "Employee deleted successfully" });
        }
    }
}