namespace EmployeeManagement.API.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public decimal Salary { get; set; }
    }
}