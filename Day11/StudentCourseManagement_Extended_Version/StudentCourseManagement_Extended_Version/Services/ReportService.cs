
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class ReportService
{
    private readonly ReportRepository _repository = new();

    public void StudentsByDepartment()
        => _repository.StudentsByDepartment();

    public void EnrollmentCountsByCourse()
        => _repository.EnrollmentCountsByCourse();

    public void StudentEnrollmentDetails()
        => _repository.StudentEnrollmentDetails();

    public void CourseFeeSummary()
        => _repository.CourseFeeSummary();
}
