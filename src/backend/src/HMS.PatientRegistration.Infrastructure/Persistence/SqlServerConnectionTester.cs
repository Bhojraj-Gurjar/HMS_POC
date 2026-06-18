using Microsoft.Data.SqlClient;

namespace HMS.PatientRegistration.Infrastructure.Persistence;

internal static class SqlServerConnectionTester
{
    public static bool CanConnect(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
