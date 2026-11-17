using Microsoft.EntityFrameworkCore;
using EmployeeManagement.API.Data;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> AddEmployee(
            CreateEmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                Designation = dto.Designation,
                Salary = dto.Salary
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return false;
            }

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.Department = dto.Department;
            employee.Designation = dto.Designation;
            employee.Salary = dto.Salary;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}