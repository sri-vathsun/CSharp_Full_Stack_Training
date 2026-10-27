
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService()
    {
        _repository = new DepartmentRepository();
    }

    public string AddDepartment(string name, string? location)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            return "Department name cannot be empty.";
        }

        if (name.Length > 100)
        {
            return "Department name cannot exceed 100 characters.";
        }

        location = string.IsNullOrWhiteSpace(location)
            ? null
            : location.Trim();

        if (location is not null && location.Length > 100)
        {
            return "Location cannot exceed 100 characters.";
        }

        if (_repository.DepartmentExists(name))
        {
            return "A department with this name already exists.";
        }

        Department department = new Department
        {
            DepartmentName = name,
            Location = location,
            IsActive = true
        };

        _repository.AddDepartment(department);

        return "Department added successfully.";
    }

    public List<Department> GetAllDepartments()
    {
        return _repository.GetAllDepartments();
    }


    public string UpdateDepartment(int id, string name, string? location)
    {
        if (id <= 0)
            return "Enter a valid department ID.";

        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return "Department name cannot be empty.";

        if (name.Length > 100)
            return "Department name cannot exceed 100 characters.";

        location = string.IsNullOrWhiteSpace(location)
            ? null
            : location.Trim();

        if (location is not null && location.Length > 100)
            return "Location cannot exceed 100 characters.";

        List<Department> departments = _repository.GetAllDepartments();

        if (!departments.Any(d => d.DepartmentId == id))
            return "Department not found.";

        if (departments.Any(d =>
            d.DepartmentId != id &&
            string.Equals(d.DepartmentName, name,
                StringComparison.OrdinalIgnoreCase)))
            return "Another department already uses this name.";

        Department department = new Department
        {
            DepartmentId = id,
            DepartmentName = name,
            Location = location
        };

        return _repository.UpdateDepartment(department)
            ? "Department updated successfully."
            : "Department could not be updated.";
    }

    public string DeactivateDepartment(int id)
    {
        if (id <= 0)
            return "Enter a valid department ID.";

        return _repository.DeactivateDepartment(id)
            ? "Department deactivated successfully."
            : "Department not found or already inactive.";
    }

}
