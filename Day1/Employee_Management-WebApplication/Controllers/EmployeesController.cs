
using Employee_Management_WebApplication.Data;
using Employee_Management_WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_WebApplication.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (await _context.Employees.AnyAsync(
                e => e.Email == employee.Email))
            {
                return Conflict(new { message = "Email already exists." });
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
        }

        // READ ALL / FILTER
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? department)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(
                    e => e.Department == department);
            }

            return Ok(await query.ToListAsync());
        }

        // READ ONE
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            if (await _context.Employees.AnyAsync(
                e => e.Email == updatedEmployee.Email && e.Id != id))
            {
                return Conflict(new { message = "Email already exists." });
            }

            employee.FullName = updatedEmployee.FullName;
            employee.Email = updatedEmployee.Email;
            employee.Department = updatedEmployee.Department;
            employee.JobTitle = updatedEmployee.JobTitle;
            employee.Salary = updatedEmployee.Salary;

            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
