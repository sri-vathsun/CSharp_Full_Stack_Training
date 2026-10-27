
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class CourseService
{
    private readonly CourseRepository _repository = new();

    public string AddCourse(
        string name, int duration, decimal fee, int departmentId)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return "Course name is required and cannot exceed 100 characters.";

        if (duration <= 0)
            return "Duration must be greater than zero.";

        if (fee < 0)
            return "Fee cannot be negative.";

        if (!_repository.ActiveDepartmentExists(departmentId))
            return "The department does not exist or is inactive.";

        if (_repository.CourseNameExists(name))
            return "A course with this name already exists.";

        _repository.AddCourse(new Course
        {
            CourseName = name,
            DurationMonths = duration,
            Fee = fee,
            DepartmentId = departmentId,
            IsActive = true
        });

        return "Course added successfully.";
    }

    public List<Course> GetCourses(
        string? search = null, int? departmentId = null)
        => _repository.GetCourses(search, departmentId);

    public string UpdateCourse(
        int id, string name, int duration, decimal fee, int departmentId)
    {
        if (id <= 0)
            return "Enter a valid course ID.";

        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return "Course name is required and cannot exceed 100 characters.";

        if (duration <= 0)
            return "Duration must be greater than zero.";

        if (fee < 0)
            return "Fee cannot be negative.";

        if (!_repository.ActiveDepartmentExists(departmentId))
            return "The department does not exist or is inactive.";

        if (_repository.CourseNameExists(name, id))
            return "Another course already uses this name.";

        Course? existing = _repository.GetCourses()
            .FirstOrDefault(c => c.CourseId == id && c.IsActive);

        if (existing is null)
            return "Active course not found.";

        existing.CourseName = name;
        existing.DurationMonths = duration;
        existing.Fee = fee;
        existing.DepartmentId = departmentId;

        return _repository.UpdateCourse(existing)
            ? "Course updated successfully."
            : "Course could not be updated.";
    }

    public string DeactivateCourse(int id)
    {
        if (id <= 0)
            return "Enter a valid course ID.";

        return _repository.DeactivateCourse(id)
            ? "Course deactivated successfully."
            : "Course not found or already inactive.";
    }
}
