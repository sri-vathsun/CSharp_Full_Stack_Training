
using Microsoft.Data.SqlClient;

namespace HospitalAppointmentManagement.Helpers;

public static class DatabaseHelper
{
    private const string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=HospitalAppointmentDB;" +
        @"Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
