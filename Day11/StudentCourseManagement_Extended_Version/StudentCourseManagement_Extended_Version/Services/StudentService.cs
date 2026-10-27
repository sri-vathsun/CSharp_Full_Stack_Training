
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class StudentService
{
    private readonly StudentRepository _repository = new();

    private static string? ValidateStudent(
        string name, string email, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Student name cannot be empty.";

        if (name.Trim().Length > 100)
            return "Student name cannot exceed 100 characters.";

        if (string.IsNullOrWhiteSpace(email) ||
            email.Trim().Length > 255 ||
            !System.Net.Mail.MailAddress.TryCreate(email.Trim(), out var address) ||
            address.Address != email.Trim())
            return "Enter a valid email address.";

        if (age < 17 || age > 60)
            return "Age must be between 17 and 60.";

        return null;
    }

    public string AddStudent(
        string name, string email, int age, int departmentId)
    {
        name = name.Trim();
        email = email.Trim();

        string? error = ValidateStudent(name, email, age);
        if (error is not null)
            return error;

        if (!_repository.ActiveDepartmentExists(departmentId))
            return "The department does not exist or is inactive.";

        if (_repository.EmailExists(email))
            return "That email address is already registered.";

        _repository.AddStudent(new Student
        {
            Name = name,
            Email = email,
            Age = age,
            DepartmentId = departmentId
        });

        return "Student added successfully.";
    }

    public List<Student> GetStudents(
        string? search = null, int? departmentId = null)
    {
        return _repository.GetStudents(search, departmentId);
    }

    public string UpdateStudent(
        int id, string name, string email, int age, int departmentId)
    {
        if (id <= 0)
            return "Enter a valid student ID.";

        name = name.Trim();
        email = email.Trim();

        string? error = ValidateStudent(name, email, age);
        if (error is not null)
            return error;

        if (!_repository.ActiveDepartmentExists(departmentId))
            return "The department does not exist or is inactive.";

        if (_repository.EmailExists(email, id))
            return "Another student already uses that email.";

        Student? existing = _repository.GetStudents()
            .FirstOrDefault(s => s.StudentId == id && s.IsActive);

        if (existing is null)
            return "Active student not found.";

        existing.Name = name;
        existing.Email = email;
        existing.Age = age;
        existing.DepartmentId = departmentId;

        return _repository.UpdateStudent(existing)
            ? "Student updated successfully."
            : "Student could not be updated.";
    }

    public string DeactivateStudent(int id)
    {
        if (id <= 0)
            return "Enter a valid student ID.";

        return _repository.DeactivateStudent(id)
            ? "Student deactivated successfully."
            : "Student not found or already inactive.";
    }
}
