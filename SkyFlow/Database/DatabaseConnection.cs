using System;
using Microsoft.Data.SqlClient;

namespace SkyFlow.Database
{
    public static class DatabaseConnection
    {
        private const string DefaultConnectionString =
            @"Server=.\SQLEXPRESS;Database=SkyFlowDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string GetConnectionString()
        {
            string? environmentConnection =
                Environment.GetEnvironmentVariable(
                    "SKYFLOW_CONNECTION_STRING"
                );

            return string.IsNullOrWhiteSpace(environmentConnection)
                ? DefaultConnectionString
                : environmentConnection;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(
                GetConnectionString()
            );
        }

        public static bool TestConnection()
        {
            try
            {
                using SqlConnection connection =
                    GetConnection();

                connection.Open();

                Console.WriteLine(
                    "✓ Connected to SkyFlowDB successfully!"
                );

                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(
                    $"✗ Database connection failed: {ex.Message}"
                );

                return false;
            }
        }
    }
}