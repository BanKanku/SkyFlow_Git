using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class UserRepository
    {
        private static readonly List<User> users = new List<User>();

        public UserRepository()
        {
            if (users.Count == 0)
            {
                AddDefaultUser(
                    1,
                    "admin",
                    "admin123",
                    "System Administrator",
                    "Admin");

                AddDefaultUser(
                    2,
                    "agent1",
                    "agent123",
                    "John Smith",
                    "GateAgent");

                AddDefaultUser(
                    3,
                    "agent2",
                    "agent123",
                    "Sarah Johnson",
                    "GateAgent");
            }
        }

        private void AddDefaultUser(
            int id,
            string username,
            string password,
            string fullName,
            string role)
        {
            User user;

            if (role == "Admin")
            {
                user = new Admin();
            }
            else
            {
                user = new GateAgent();
            }

            user.UserID = id;
            user.Username = username;
            user.PasswordHash = HashPassword(password);
            user.FullName = fullName;
            user.Role = role;

            users.Add(user);
        }

        public List<User> GetAll()
        {
            return users;
        }

        public User? GetById(int id)
        {
            return users.FirstOrDefault(u => u.UserID == id);
        }

        public User? Authenticate(string username, string password)
        {
            User? user = users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            return VerifyPassword(password, user.PasswordHash)
                ? user
                : null;
        }

        public void Add(User user)
        {
            user.UserID = users.Count == 0
                ? 1
                : users.Max(u => u.UserID) + 1;

            user.PasswordHash = HashPassword(user.PasswordHash);

            users.Add(user);
        }

        public void Update(User user)
        {
            User? existing =
                users.FirstOrDefault(u => u.UserID == user.UserID);

            if (existing != null)
            {
                existing.Username = user.Username;
                existing.FullName = user.FullName;
                existing.Role = user.Role;
            }
        }

        public void Delete(int id)
        {
            User? user =
                users.FirstOrDefault(u => u.UserID == id);

            if (user != null)
            {
                users.Remove(user);
            }
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(
            string password,
            string storedPassword)
        {
            string[] parts = storedPassword.Split(':');

            if (parts.Length != 2)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256,
                    32);

                return CryptographicOperations.FixedTimeEquals(
                    enteredHash,
                    storedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}