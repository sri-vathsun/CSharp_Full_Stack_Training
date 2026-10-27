
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories;

public class StudentRepository
{
    public bool EmailExists(string email, int excludeStudentId = 0)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(1)
            FROM Students
            WHERE Email = @Email
              AND StudentId <> @ExcludeStudentId;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@ExcludeStudentId", excludeStudentId);

        connection.Open();

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool ActiveDepartmentExists(int departmentId)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(1)
            FROM Departments
            WHERE DepartmentId = @DepartmentId
              AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@DepartmentId", departmentId);

        connection.Open();

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void AddStudent(Student student)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            INSERT INTO Students (Name, Email, Age, DepartmentId)
            VALUES (@Name, @Email, @Age, @DepartmentId);
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Email", student.Email);
        command.Parameters.AddWithValue("@Age", student.Age);
        command.Parameters.AddWithValue("@DepartmentId", student.DepartmentId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<Student> GetStudents(string? search = null, int? departmentId = null)
    {
        List<Student> students = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT StudentId, Name, Email, Age, DepartmentId,
                   AdmissionDate, IsActive
            FROM Students
            WHERE (@Search IS NULL
                   OR Name LIKE @Pattern
                   OR Email LIKE @Pattern)
              AND (@DepartmentId IS NULL
                   OR DepartmentId = @DepartmentId)
            ORDER BY StudentId;
            """;

        using SqlCommand command = new SqlCommand(query, connection);

        string? cleanedSearch = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        command.Parameters.AddWithValue("@Search", (object?)cleanedSearch ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Pattern",
            cleanedSearch is null ? DBNull.Value : $"%{cleanedSearch}%");
        command.Parameters.AddWithValue(
            "@DepartmentId",
            (object?)departmentId ?? DBNull.Value);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            students.Add(new Student
            {
                StudentId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Age = reader.GetInt32(3),
                DepartmentId = reader.GetInt32(4),
                AdmissionDate = reader.GetDateTime(5),
                IsActive = reader.GetBoolean(6)
            });
        }

        return students;
    }

    public bool UpdateStudent(Student student)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            UPDATE Students
            SET Name = @Name,
                Email = @Email,
                Age = @Age,
                DepartmentId = @DepartmentId
            WHERE StudentId = @StudentId
              AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@StudentId", student.StudentId);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Email", student.Email);
        command.Parameters.AddWithValue("@Age", student.Age);
        command.Parameters.AddWithValue("@DepartmentId", student.DepartmentId);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool DeactivateStudent(int studentId)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            UPDATE Students
            SET IsActive = 0
            WHERE StudentId = @StudentId
              AND IsActive = 1;
            """;

        using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@StudentId", studentId);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}
