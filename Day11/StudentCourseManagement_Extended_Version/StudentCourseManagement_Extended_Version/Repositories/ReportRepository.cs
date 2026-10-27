
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;

namespace StudentCourseManagement.Repositories;

public class ReportRepository
{
    public void StudentsByDepartment()
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT d.DepartmentName, COUNT(s.StudentId) AS StudentCount
            FROM Departments d
            LEFT JOIN Students s
                ON d.DepartmentId = s.DepartmentId
               AND s.IsActive = 1
            GROUP BY d.DepartmentId, d.DepartmentName
            ORDER BY d.DepartmentName;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader.GetString(0)}: {reader.GetInt32(1)} students");
        }
    }

    public void EnrollmentCountsByCourse()
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT c.CourseName,
                   COUNT(e.EnrollmentId) AS EnrollmentCount
            FROM Courses c
            LEFT JOIN Enrollments e
                ON c.CourseId = e.CourseId
               AND e.Status = N'Enrolled'
            GROUP BY c.CourseId, c.CourseName
            ORDER BY c.CourseName;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader.GetString(0)}: {reader.GetInt32(1)} enrolled");
        }
    }

    public void StudentEnrollmentDetails()
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT s.Name, s.Email, c.CourseName, e.Status, e.EnrolledAt
            FROM Enrollments e
            INNER JOIN Students s ON e.StudentId = s.StudentId
            INNER JOIN Courses c ON e.CourseId = c.CourseId
            ORDER BY s.Name, c.CourseName;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader.GetString(0)} ({reader.GetString(1)}) | " +
                $"{reader.GetString(2)} | {reader.GetString(3)} | " +
                $"{reader.GetDateTime(4):g}");
        }
    }

    public void CourseFeeSummary()
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(*) AS CourseCount,
                   COALESCE(SUM(Fee), 0) AS TotalFees,
                   COALESCE(AVG(Fee), 0) AS AverageFee
            FROM Courses
            WHERE IsActive = 1;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            Console.WriteLine($"Active courses: {reader.GetInt32(0)}");
            Console.WriteLine($"Total course fees: {reader.GetDecimal(1):C}");
            Console.WriteLine($"Average course fee: {reader.GetDecimal(2):C}");
        }
    }
}
