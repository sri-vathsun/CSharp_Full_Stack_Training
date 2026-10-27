
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories;

public class EnrollmentRepository
{
    public void EnrollStudent(int studentId, int courseId)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            string validationQuery = """
                SELECT
                    (SELECT COUNT(1) FROM Students
                     WHERE StudentId = @StudentId AND IsActive = 1),
                    (SELECT COUNT(1) FROM Courses
                     WHERE CourseId = @CourseId AND IsActive = 1);
                """;

            using (SqlCommand validation = new(
                validationQuery, connection, transaction))
            {
                validation.Parameters.AddWithValue("@StudentId", studentId);
                validation.Parameters.AddWithValue("@CourseId", courseId);

                using SqlDataReader reader = validation.ExecuteReader();
                reader.Read();

                bool studentExists = reader.GetInt32(0) > 0;
                bool courseExists = reader.GetInt32(1) > 0;

                reader.Close();

                if (!studentExists || !courseExists)
                    throw new InvalidOperationException(
                        "The student or course does not exist or is inactive.");
            }

            string insertQuery = """
                INSERT INTO Enrollments (StudentId, CourseId, Status)
                VALUES (@StudentId, @CourseId, N'Enrolled');

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            int enrollmentId;

            using (SqlCommand insert = new(
                insertQuery, connection, transaction))
            {
                insert.Parameters.AddWithValue("@StudentId", studentId);
                insert.Parameters.AddWithValue("@CourseId", courseId);

                enrollmentId = Convert.ToInt32(insert.ExecuteScalar());
            }

            string auditQuery = """
                INSERT INTO EnrollmentAudit
                    (StudentId, CourseId, ActionName, CreatedAt)
                VALUES
                    (@StudentId, @CourseId, N'Enrolled', SYSDATETIME());
                """;

            using (SqlCommand audit = new(
                auditQuery, connection, transaction))
            {
                audit.Parameters.AddWithValue("@StudentId", studentId);
                audit.Parameters.AddWithValue("@CourseId", courseId);
                audit.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<Enrollment> GetEnrollments()
    {
        List<Enrollment> enrollments = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT EnrollmentId, StudentId, CourseId, EnrolledAt, Status
            FROM Enrollments
            ORDER BY EnrollmentId;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            enrollments.Add(new Enrollment
            {
                EnrollmentId = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                CourseId = reader.GetInt32(2),
                EnrolledAt = reader.GetDateTime(3),
                Status = reader.GetString(4)
            });
        }

        return enrollments;
    }

    public bool ChangeStatus(int enrollmentId, string status)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            string findQuery = """
                SELECT StudentId, CourseId
                FROM Enrollments
                WHERE EnrollmentId = @Id;
                """;

            int studentId;
            int courseId;

            using (SqlCommand find = new(
                findQuery, connection, transaction))
            {
                find.Parameters.AddWithValue("@Id", enrollmentId);

                using SqlDataReader reader = find.ExecuteReader();

                if (!reader.Read())
                    return RollbackAndReturnFalse(transaction);

                studentId = reader.GetInt32(0);
                courseId = reader.GetInt32(1);
            }

            string updateQuery = """
                UPDATE Enrollments
                SET Status = @Status
                WHERE EnrollmentId = @Id;
                """;

            using (SqlCommand update = new(
                updateQuery, connection, transaction))
            {
                update.Parameters.AddWithValue("@Id", enrollmentId);
                update.Parameters.AddWithValue("@Status", status);
                update.ExecuteNonQuery();
            }

            string auditQuery = """
                INSERT INTO EnrollmentAudit
                    (StudentId, CourseId, ActionName, CreatedAt)
                VALUES
                    (@StudentId, @CourseId, @ActionName, SYSDATETIME());
                """;

            using (SqlCommand audit = new(
                auditQuery, connection, transaction))
            {
                audit.Parameters.AddWithValue("@StudentId", studentId);
                audit.Parameters.AddWithValue("@CourseId", courseId);
                audit.Parameters.AddWithValue(
                    "@ActionName", "Status changed to " + status);
                audit.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static bool RollbackAndReturnFalse(SqlTransaction transaction)
    {
        transaction.Rollback();
        return false;
    }
}
