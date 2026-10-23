
using Employee_Management_WebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_WebApplication.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(
            DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
