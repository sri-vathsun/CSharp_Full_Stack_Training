using Microsoft.Data.SqlClient;
using Hospital_Management.Models;

namespace Hospital_Management.Repositories;

public class PatientRepository
{
    private readonly string _connectionString;

    public PatientRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 1. Register a patient
    public void AddPatient(Patient patient)
    {
        const string query = @"
            INSERT INTO Patients (Name, Email, Age, DepartmentId)
            VALUES (@Name, @Email, @Age, @DepartmentId);";

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Name", patient.Name);
        command.Parameters.AddWithValue("@Email", patient.Email);
        command.Parameters.AddWithValue("@Age", patient.Age);
        command.Parameters.AddWithValue(
            "@DepartmentId", patient.DepartmentId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // 2. View all patients
    public List<Patient> GetAllPatients()
    {
        const string query = @"
            SELECT PatientId, Name, Email, Age, DepartmentId,
                   RegisteredDate, IsActive
            FROM Patients
            ORDER BY PatientId;";

        List<Patient> patients = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            patients.Add(MapPatient(reader));
        }

        return patients;
    }

    // 3. Search by patient ID
    public Patient? GetPatientById(int patientId)
    {
        const string query = @"
            SELECT PatientId, Name, Email, Age, DepartmentId,
                   RegisteredDate, IsActive
            FROM Patients
            WHERE PatientId = @PatientId;";

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@PatientId", patientId);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapPatient(reader);
        }

        return null;
    }

    // 4. Search by patient name
    public List<Patient> SearchPatientsByName(string name)
    {
        const string query = @"
            SELECT PatientId, Name, Email, Age, DepartmentId,
                   RegisteredDate, IsActive
            FROM Patients
            WHERE Name LIKE @Name
            ORDER BY Name;";

        List<Patient> patients = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Name", $"%{name}%");

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            patients.Add(MapPatient(reader));
        }

        return patients;
    }

    // 5. Filter patients by department
    public List<Patient> GetPatientsByDepartment(int departmentId)
    {
        const string query = @"
            SELECT PatientId, Name, Email, Age, DepartmentId,
                   RegisteredDate, IsActive
            FROM Patients
            WHERE DepartmentId = @DepartmentId
            ORDER BY Name;";

        List<Patient> patients = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@DepartmentId", departmentId);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            patients.Add(MapPatient(reader));
        }

        return patients;
    }

    // 6. Update patient details
    public bool UpdatePatient(Patient patient)
    {
        const string query = @"
            UPDATE Patients
            SET Name = @Name,
                Email = @Email,
                Age = @Age,
                DepartmentId = @DepartmentId
            WHERE PatientId = @PatientId
              AND IsActive = 1;";

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@PatientId", patient.PatientId);
        command.Parameters.AddWithValue("@Name", patient.Name);
        command.Parameters.AddWithValue("@Email", patient.Email);
        command.Parameters.AddWithValue("@Age", patient.Age);
        command.Parameters.AddWithValue(
            "@DepartmentId", patient.DepartmentId);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    // 7. Deactivate a patient
    public bool DeactivatePatient(int patientId)
    {
        const string query = @"
            UPDATE Patients
            SET IsActive = 0
            WHERE PatientId = @PatientId
              AND IsActive = 1;";

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@PatientId", patientId);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    // Convert database row into a Patient object
    private static Patient MapPatient(SqlDataReader reader)
    {
        return new Patient
        {
            PatientId = reader.GetInt32(
                reader.GetOrdinal("PatientId")),

            Name = reader.GetString(
                reader.GetOrdinal("Name")),

            Email = reader.GetString(
                reader.GetOrdinal("Email")),

            Age = reader.GetInt32(
                reader.GetOrdinal("Age")),

            DepartmentId = reader.GetInt32(
                reader.GetOrdinal("DepartmentId")),

            RegisteredDate = reader.GetDateTime(
                reader.GetOrdinal("RegisteredDate")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}