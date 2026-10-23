
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_WebApplication.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Range(0.01, 999999999)]
        public decimal Salary { get; set; }
    }
}
