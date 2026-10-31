
using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;

namespace HospitalAppointmentManagement.Repositories;

public class AuditLogRepository
{
    private readonly string _connectionString;

    public AuditLogRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Insert an audit log into the database
    public async Task LogAsync(AuditLog auditLog)
    {
        const string query = @"
            INSERT INTO AuditLogs
                (EntityName, EntityId, Action, Details)
            VALUES
                (@EntityName, @EntityId, @Action, @Details);";

        await using SqlConnection connection =
            new SqlConnection(_connectionString);

        await using SqlCommand command =
            new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@EntityName", auditLog.EntityName);

        command.Parameters.AddWithValue(
            "@EntityId", auditLog.EntityId);

        command.Parameters.AddWithValue(
            "@Action", auditLog.Action);

        command.Parameters.AddWithValue(
            "@Details",
            (object?)auditLog.Details ?? DBNull.Value);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    // Retrieve all audit logs
    public async Task<List<AuditLog>> GetAllAsync()
    {
        const string query = @"
            SELECT AuditId, EntityName, EntityId,
                   Action, Details, CreatedAt
            FROM AuditLogs
            ORDER BY CreatedAt DESC;";

        List<AuditLog> logs = new();

        await using SqlConnection connection =
            new SqlConnection(_connectionString);

        await using SqlCommand command =
            new SqlCommand(query, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            logs.Add(new AuditLog
            {
                AuditId = reader.GetInt64(0),
                EntityName = reader.GetString(1),
                EntityId = reader.GetInt32(2),
                Action = reader.GetString(3),
                Details = reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }

        return logs;
    }
}
