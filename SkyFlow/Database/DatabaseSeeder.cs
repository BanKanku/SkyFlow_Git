using System;
using Dapper;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public static class DatabaseSeeder
    {
        public static void SeedUsers()
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int userCount =
                connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Users"
                );

            if (userCount > 0)
            {
                return;
            }

            SqlUserRepository userRepository =
                new SqlUserRepository();

            Admin admin = new Admin
            {
                Username = "admin",
                FullName = "System Administrator",
                PasswordHash = "admin123",
                Role = "Admin"
            };

            GateAgent agent1 = new GateAgent
            {
                Username = "agent1",
                FullName = "Gate Agent One",
                PasswordHash = "agent123",
                Role = "GateAgent"
            };

            GateAgent agent2 = new GateAgent
            {
                Username = "agent2",
                FullName = "Gate Agent Two",
                PasswordHash = "agent123",
                Role = "GateAgent"
            };

            userRepository.Add(admin);
            userRepository.Add(agent1);
            userRepository.Add(agent2);

            Console.WriteLine(
                "✓ Demo user accounts created securely."
            );
        }
    }
}