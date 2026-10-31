using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;

namespace HospitalAppointmentManagement.Repositories;

public class DoctorRepository
{
    private readonly string _connectionString;

    public DoctorRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddAsync(Doctor doctor)
    {
        const string sql = @"
            INSERT INTO Doctors
            (
                DoctorName,
                Specialization,
                ConsultationFee,
                SlotMinutes,
                DepartmentId
            )
            OUTPUT INSERTED.DoctorId
            VALUES
            (
                @DoctorName,
                @Specialization,
                @ConsultationFee,
                @SlotMinutes,
                @DepartmentId
            );";

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        AddDoctorParameters(command, doctor);

        await connection.OpenAsync();

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<List<Doctor>> GetAllAsync()
    {
        const string sql = @"
            SELECT DoctorId, DoctorName, Specialization,
                   ConsultationFee, SlotMinutes,
                   DepartmentId, IsActive
            FROM Doctors
            ORDER BY DoctorId;";

        var doctors = new List<Doctor>();

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            doctors.Add(MapDoctor(reader));
        }

        return doctors;
    }

    public async Task<Doctor?> GetByIdAsync(int doctorId)
    {
        const string sql = @"
            SELECT DoctorId, DoctorName, Specialization,
                   ConsultationFee, SlotMinutes,
                   DepartmentId, IsActive
            FROM Doctors
            WHERE DoctorId = @DoctorId;";

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@DoctorId", doctorId);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapDoctor(reader);
        }

        return null;
    }

    public async Task<List<Doctor>> SearchByNameAsync(string name)
    {
        const string sql = @"
            SELECT DoctorId, DoctorName, Specialization,
                   ConsultationFee, SlotMinutes,
                   DepartmentId, IsActive
            FROM Doctors
            WHERE DoctorName LIKE @Name
            ORDER BY DoctorName;";

        var doctors = new List<Doctor>();

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Name", $"%{name}%");

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            doctors.Add(MapDoctor(reader));
        }

        return doctors;
    }

    public async Task<List<Doctor>> GetByDepartmentAsync(
        int departmentId)
    {
        const string sql = @"
            SELECT DoctorId, DoctorName, Specialization,
                   ConsultationFee, SlotMinutes,
                   DepartmentId, IsActive
            FROM Doctors
            WHERE DepartmentId = @DepartmentId
            ORDER BY DoctorName;";

        var doctors = new List<Doctor>();

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@DepartmentId", departmentId);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            doctors.Add(MapDoctor(reader));
        }

        return doctors;
    }

    public async Task<bool> UpdateAsync(Doctor doctor)
    {
        const string sql = @"
            UPDATE Doctors
            SET DoctorName = @DoctorName,
                Specialization = @Specialization,
                ConsultationFee = @ConsultationFee,
                SlotMinutes = @SlotMinutes,
                DepartmentId = @DepartmentId
            WHERE DoctorId = @DoctorId
              AND IsActive = 1;";

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        AddDoctorParameters(command, doctor);

        command.Parameters.AddWithValue(
            "@DoctorId", doctor.DoctorId);

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> DeactivateAsync(int doctorId)
    {
        const string sql = @"
            UPDATE Doctors
            SET IsActive = 0
            WHERE DoctorId = @DoctorId
              AND IsActive = 1;";

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@DoctorId", doctorId);

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    private static void AddDoctorParameters(
        SqlCommand command,
        Doctor doctor)
    {
        command.Parameters.AddWithValue(
            "@DoctorName", doctor.DoctorName);

        command.Parameters.AddWithValue(
            "@Specialization", doctor.Specialization);

        command.Parameters.AddWithValue(
            "@ConsultationFee", doctor.ConsultationFee);

        command.Parameters.AddWithValue(
            "@SlotMinutes", doctor.SlotMinutes);

        command.Parameters.AddWithValue(
            "@DepartmentId", doctor.DepartmentId);
    }

    private static Doctor MapDoctor(SqlDataReader reader)
    {
        return new Doctor
        {
            DoctorId = reader.GetInt32(
                reader.GetOrdinal("DoctorId")),

            DoctorName = reader.GetString(
                reader.GetOrdinal("DoctorName")),

            Specialization = reader.GetString(
                reader.GetOrdinal("Specialization")),

            ConsultationFee = reader.GetDecimal(
                reader.GetOrdinal("ConsultationFee")),

            SlotMinutes = reader.GetInt32(
                reader.GetOrdinal("SlotMinutes")),

            DepartmentId = reader.GetInt32(
                reader.GetOrdinal("DepartmentId")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}