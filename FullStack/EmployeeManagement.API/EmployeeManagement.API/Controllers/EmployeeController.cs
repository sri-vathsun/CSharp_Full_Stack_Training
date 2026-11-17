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
    }
}