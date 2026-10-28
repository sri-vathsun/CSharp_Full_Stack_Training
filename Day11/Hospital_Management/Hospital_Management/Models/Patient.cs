namespace Hospital_Management.Models;

public class Patient
{
    public int PatientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }

    public int DepartmentId { get; set; }

    public DateTime RegisteredDate { get; set; }

    public bool IsActive { get; set; } = true;
}