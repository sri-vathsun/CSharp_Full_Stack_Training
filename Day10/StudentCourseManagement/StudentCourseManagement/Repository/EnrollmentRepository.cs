
using Microsoft.Data.SqlClient;

namespace StudentCourseManagement.Repository;

public class EnrollmentRepository
{
    private readonly string _connectionString;

    public EnrollmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnrollStudentAsync(int studentId, int courseId)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var transaction =
            (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            const string enrollmentSql = """
                INSERT INTO Enrollments (StudentId, CourseId)
                VALUES (@StudentId, @CourseId);
                """;

            await using (var command = new SqlCommand(
                enrollmentSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@StudentId", studentId);
                command.Parameters.AddWithValue("@CourseId", courseId);

                await command.ExecuteNonQueryAsync();
            }

            const string auditSql = """
                INSERT INTO EnrollmentAudit
                    (StudentId, CourseId, ActionName)
                VALUES
                    (@StudentId, @CourseId, @ActionName);
                """;

            await using (var command = new SqlCommand(
                auditSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@StudentId", studentId);
                command.Parameters.AddWithValue("@CourseId", courseId);
                command.Parameters.AddWithValue("@ActionName", "ENROLLED");

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            Console.WriteLine("Enrollment completed successfully.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
