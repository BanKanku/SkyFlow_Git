using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Dapper;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class SqlUserRepository
    {
        public List<User> GetAll()
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var records = connection.Query<UserRecord>(
                @"SELECT UserID, Username, PasswordHash, FullName, Role
                  FROM Users
                  ORDER BY UserID"
            ).ToList();

            return records.Select(MapUser).ToList();
        }

        public User? GetById(int id)
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var record = connection.QueryFirstOrDefault<UserRecord>(
                @"SELECT UserID, Username, PasswordHash, FullName, Role
                  FROM Users
                  WHERE UserID = @UserID",
                new
                {
                    UserID = id
                }
            );

            return record == null
                ? null
                : MapUser(record);
        }

        public User? Authenticate(
            string username,
            string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var record = connection.QueryFirstOrDefault<UserRecord>(
                @"SELECT UserID, Username, PasswordHash, FullName, Role
                  FROM Users
                  WHERE LOWER(Username) = LOWER(@Username)",
                new
                {
                    Username = username.Trim()
                }
            );

            if (record == null)
                return null;

            if (!VerifyPassword(
                    password,
                    record.PasswordHash))
            {
                return null;
            }

            return MapUser(record);
        }

        public void Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException(
                    "Username is required."
                );
            }

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException(
                    "Full name is required."
                );
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException(
                    "Password is required."
                );
            }

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                throw new ArgumentException(
                    "Role is required."
                );
            }

            if (!user.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) &&
                !user.Role.Equals(
                    "GateAgent",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Role must be Admin or GateAgent."
                );
            }

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int existingUser =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Users
                      WHERE LOWER(Username) =
                            LOWER(@Username)",
                    new
                    {
                        Username =
                            user.Username.Trim()
                    }
                );

            if (existingUser > 0)
            {
                throw new InvalidOperationException(
                    "Username already exists."
                );
            }

            string passwordHash =
                HashPassword(user.PasswordHash);

            string normalizedRole =
                user.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : "GateAgent";

            int newId =
                connection.QuerySingle<int>(
                    @"INSERT INTO Users
                        (
                            Username,
                            PasswordHash,
                            FullName,
                            Role
                        )
                      OUTPUT INSERTED.UserID
                      VALUES
                        (
                            @Username,
                            @PasswordHash,
                            @FullName,
                            @Role
                        )",
                    new
                    {
                        Username =
                            user.Username.Trim(),

                        PasswordHash =
                            passwordHash,

                        FullName =
                            user.FullName.Trim(),

                        Role =
                            normalizedRole
                    }
                );

            user.UserID = newId;
            user.PasswordHash = passwordHash;
            user.Role = normalizedRole;
        }

        public void Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException(
                    "Username is required."
                );
            }

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException(
                    "Full name is required."
                );
            }

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                throw new ArgumentException(
                    "Role is required."
                );
            }

            if (!user.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) &&
                !user.Role.Equals(
                    "GateAgent",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Role must be Admin or GateAgent."
                );
            }

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int duplicateUsername =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Users
                      WHERE LOWER(Username) =
                            LOWER(@Username)
                        AND UserID <> @UserID",
                    new
                    {
                        Username =
                            user.Username.Trim(),

                        user.UserID
                    }
                );

            if (duplicateUsername > 0)
            {
                throw new InvalidOperationException(
                    "Username already exists."
                );
            }

            string normalizedRole =
                user.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : "GateAgent";

            int affectedRows =
                connection.Execute(
                    @"UPDATE Users
                      SET Username = @Username,
                          FullName = @FullName,
                          Role = @Role
                      WHERE UserID = @UserID",
                    new
                    {
                        user.UserID,

                        Username =
                            user.Username.Trim(),

                        FullName =
                            user.FullName.Trim(),

                        Role =
                            normalizedRole
                    }
                );

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    "User was not found."
                );
            }

            user.Role = normalizedRole;
        }

        public void Delete(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            connection.Execute(
                @"DELETE FROM Users
                  WHERE UserID = @UserID",
                new
                {
                    UserID = id
                }
            );
        }

        private static User MapUser(
            UserRecord record)
        {
            User user;

            if (record.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                user = new Admin();
            }
            else
            {
                user = new GateAgent();
            }

            user.UserID = record.UserID;
            user.Username = record.Username;
            user.PasswordHash = record.PasswordHash;
            user.FullName = record.FullName;
            user.Role = record.Role;

            return user;
        }

        private static string HashPassword(
            string password)
        {
            byte[] salt =
                RandomNumberGenerator.GetBytes(16);

            byte[] hash =
                Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256,
                    32
                );

            return
                Convert.ToBase64String(salt) +
                ":" +
                Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(
            string password,
            string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(
                    storedPasswordHash))
            {
                return false;
            }

            string[] parts =
                storedPasswordHash.Split(':');

            if (parts.Length != 2)
                return false;

            try
            {
                byte[] salt =
                    Convert.FromBase64String(
                        parts[0]
                    );

                byte[] storedHash =
                    Convert.FromBase64String(
                        parts[1]
                    );

                byte[] calculatedHash =
                    Rfc2898DeriveBytes.Pbkdf2(
                        password,
                        salt,
                        100000,
                        HashAlgorithmName.SHA256,
                        32
                    );

                return CryptographicOperations
                    .FixedTimeEquals(
                        storedHash,
                        calculatedHash
                    );
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private sealed class UserRecord
        {
            public int UserID { get; set; }

            public string Username { get; set; }
                = "";

            public string PasswordHash { get; set; }
                = "";

            public string FullName { get; set; }
                = "";

            public string Role { get; set; }
                = "";
        }
    }
}