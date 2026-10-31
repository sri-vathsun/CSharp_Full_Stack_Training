using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;

namespace HospitalAppointmentManagement.Repositories;

public class AppointmentRepository
{
    private readonly string _connectionString;

    public AppointmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddAsync(Appointment appointment)
    {
        const string sql = @"
            INSERT INTO Appointments
                (PatientId, DoctorId, AppointmentDate)
            OUTPUT INSERTED.AppointmentId
            VALUES (@PatientId, @DoctorId, @AppointmentDate);";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
        command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
        command.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);

        await connection.OpenAsync();
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        const string sql = @"
            SELECT AppointmentId, PatientId, DoctorId,
                   AppointmentDate, BookedAt, Status
            FROM Appointments
            ORDER BY AppointmentDate;";

        var appointments = new List<Appointment>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            appointments.Add(MapAppointment(reader));

        return appointments;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT AppointmentId, PatientId, DoctorId,
                   AppointmentDate, BookedAt, Status
            FROM Appointments
            WHERE AppointmentId = @Id;";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapAppointment(reader) : null;
    }

    public async Task<List<Appointment>> GetByPatientAsync(int patientId)
    {
        const string sql = @"
            SELECT AppointmentId, PatientId, DoctorId,
                   AppointmentDate, BookedAt, Status
            FROM Appointments
            WHERE PatientId = @PatientId
            ORDER BY AppointmentDate;";

        var appointments = new List<Appointment>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@PatientId", patientId);
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            appointments.Add(MapAppointment(reader));

        return appointments;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        const string sql = @"
            UPDATE Appointments
            SET Status = @Status
            WHERE AppointmentId = @Id;";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Status", status);

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Appointment MapAppointment(SqlDataReader reader)
    {
        return new Appointment
        {
            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
            DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
            AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
            BookedAt = reader.GetDateTime(reader.GetOrdinal("BookedAt")),
            Status = reader.GetString(reader.GetOrdinal("Status"))
        };
    }
}