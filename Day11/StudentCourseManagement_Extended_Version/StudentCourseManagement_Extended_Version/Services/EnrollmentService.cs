
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class EnrollmentService
{
    private readonly EnrollmentRepository _repository = new();

    public string EnrollStudent(int studentId, int courseId)
    {
        if (studentId <= 0 || courseId <= 0)
            return "Enter valid student and course IDs.";

        try
        {
            _repository.EnrollStudent(studentId, courseId);
            return "Student enrolled successfully.";
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
            when (ex.Number == 2601 || ex.Number == 2627)
        {
            return "This student is already enrolled in this course.";
        }
    }

    public List<Enrollment> GetEnrollments()
        => _repository.GetEnrollments();

    public string ChangeStatus(int id, string status)
    {
        if (id <= 0)
            return "Enter a valid enrollment ID.";

        if (status is not ("Enrolled" or "Completed" or "Cancelled"))
            return "Status must be Enrolled, Completed, or Cancelled.";

        return _repository.ChangeStatus(id, status)
            ? "Enrollment status updated."
            : "Enrollment not found.";
    }
}
