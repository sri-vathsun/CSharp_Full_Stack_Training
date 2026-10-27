
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories;

public class AuditLogRepository
{
    public List<AuditLog> GetAllAuditLogs()
    {
        List<AuditLog> logs = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            SELECT AuditId, EntityName, EntityId, Action, Details, CreatedAt
            FROM AuditLogs
            ORDER BY CreatedAt DESC;
            """;

        using SqlCommand command = new(query, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            logs.Add(new AuditLog
            {
                AuditId = reader.GetInt64(0),
                EntityName = reader.GetString(1),
                EntityId = reader.GetInt32(2),
                Action = reader.GetString(3),
                Details = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }

        return logs;
    }

    public void AddAuditLog(
        string entityName, int entityId, string action, string? details)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();

        string query = """
            INSERT INTO AuditLogs
                (EntityName, EntityId, Action, Details, CreatedAt)
            VALUES
                (@EntityName, @EntityId, @Action, @Details, SYSDATETIME());
            """;

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@EntityName", entityName);
        command.Parameters.AddWithValue("@EntityId", entityId);
        command.Parameters.AddWithValue("@Action", action);
        command.Parameters.AddWithValue(
            "@Details", (object?)details ?? DBNull.Value);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
