
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories;

public class CourseRepository
{
    public bool CourseNameExists(string name, int excludeId = 0)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(1)
            FROM Courses
            WHERE CourseName = @Name
              AND CourseId <> @ExcludeId;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ExcludeId", excludeId);

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool ActiveDepartmentExists(int departmentId)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(1)
            FROM Departments
            WHERE DepartmentId = @Id AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", departmentId);

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void AddCourse(Course course)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            INSERT INTO Courses
                (CourseName, DurationMonths, Fee, DepartmentId)
            VALUES
                (@Name, @Duration, @Fee, @DepartmentId);
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", course.CourseName);
        command.Parameters.AddWithValue("@Duration", course.DurationMonths);
        command.Parameters.AddWithValue("@Fee", course.Fee);
        command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<Course> GetCourses(string? search = null, int? departmentId = null)
    {
        List<Course> courses = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT CourseId, CourseName, DurationMonths, Fee,
                   DepartmentId, IsActive
            FROM Courses
            WHERE (@Search IS NULL OR CourseName LIKE @Pattern)
              AND (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
            ORDER BY CourseId;
            """;

        using SqlCommand command = new SqlCommand(query, connection);

        string? cleanedSearch = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        command.Parameters.AddWithValue("@Search",
            (object?)cleanedSearch ?? DBNull.Value);
        command.Parameters.AddWithValue("@Pattern",
            cleanedSearch is null ? DBNull.Value : $"%{cleanedSearch}%");
        command.Parameters.AddWithValue("@DepartmentId",
            (object?)departmentId ?? DBNull.Value);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            courses.Add(new Course
            {
                CourseId = reader.GetInt32(0),
                CourseName = reader.GetString(1),
                DurationMonths = reader.GetInt32(2),
                Fee = reader.GetDecimal(3),
                DepartmentId = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            });
        }

        return courses;
    }

    public bool UpdateCourse(Course course)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            UPDATE Courses
            SET CourseName = @Name,
                DurationMonths = @Duration,
                Fee = @Fee,
                DepartmentId = @DepartmentId
            WHERE CourseId = @Id AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", course.CourseId);
        command.Parameters.AddWithValue("@Name", course.CourseName);
        command.Parameters.AddWithValue("@Duration", course.DurationMonths);
        command.Parameters.AddWithValue("@Fee", course.Fee);
        command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeactivateCourse(int id)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            UPDATE Courses SET IsActive = 0
            WHERE CourseId = @Id AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}
