
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories;

public class DepartmentRepository
{
    public bool DepartmentExists(string departmentName)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT COUNT(1)
            FROM Departments
            WHERE DepartmentName = @DepartmentName;
            """;

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@DepartmentName", departmentName);

        connection.Open();

        int count = Convert.ToInt32(command.ExecuteScalar());

        return count > 0;
    }

    public void AddDepartment(Department department)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            INSERT INTO Departments (DepartmentName, Location)
            VALUES (@DepartmentName, @Location);
            """;

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@DepartmentName", department.DepartmentName);

        command.Parameters.AddWithValue(
            "@Location", (object?)department.Location ?? DBNull.Value);

        connection.Open();

        command.ExecuteNonQuery();
    }

    public List<Department> GetAllDepartments()
    {
        List<Department> departments = new List<Department>();

        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT DepartmentId, DepartmentName, Location, IsActive
            FROM Departments
            ORDER BY DepartmentId;
            """;

        using SqlCommand command = new SqlCommand(query, connection);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Department department = new Department
            {
                DepartmentId = reader.GetInt32(0),
                DepartmentName = reader.GetString(1),
                Location = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            };

            departments.Add(department);
        }

        return departments;

    }


    public bool UpdateDepartment(Department department)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
        UPDATE Departments
        SET DepartmentName = @DepartmentName,
            Location = @Location
        WHERE DepartmentId = @DepartmentId;
        """;

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@DepartmentId", department.DepartmentId);
        command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
        command.Parameters.AddWithValue(
            "@Location", (object?)department.Location ?? DBNull.Value);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool DeactivateDepartment(int departmentId)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
        UPDATE Departments
        SET IsActive = 0
        WHERE DepartmentId = @DepartmentId
          AND IsActive = 1;
        """;

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@DepartmentId", departmentId);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

}
