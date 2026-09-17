using System;
using Microsoft.Data.SqlClient;

namespace SkyFlow.Database
{
    public static class DatabaseConnection
    {
        private const string DefaultConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=SkyFlowDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string GetConnectionString()
        {
            string? environmentConnection =
                Environment.GetEnvironmentVariable("SKYFLOW_CONNECTION_STRING");

            return string.IsNullOrWhiteSpace(environmentConnection)
                ? DefaultConnectionString
                : environmentConnection;
        }

        public static bool TestConnection()
        {
            try
            {
                using SqlConnection connection =
                    new SqlConnection(GetConnectionString());

                connection.Open();

                Console.WriteLine("✓ Database connected successfully!");

                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(
                    $"✗ Database connection failed: {ex.Message}");

                return false;
            }
        }
    }
}