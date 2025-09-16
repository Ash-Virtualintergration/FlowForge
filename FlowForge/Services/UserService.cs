using FlowForge.Models;
using Newtonsoft.Json; // Make sure to install Newtonsoft.Json via NuGet
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FlowForge.Services
{
    public static class UserService
    {
        private static readonly string FilePath = "users.json";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath))
            {
                // Create default admin if file doesn't exist
                var defaultAdmin = new User
                {
                    Username = "admin",
                    Password = HashPassword("admin"),
                    IsAdmin = true
                };
                var users = new List<User> { defaultAdmin };
                SaveUsers(users);
                return users;
            }

            var json = File.ReadAllText(FilePath);
            var usersList = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();

            // Ensure all passwords are hashed (for legacy users with plain-text passwords)
            bool updated = false;
            foreach (var user in usersList)
            {
                if (!IsHashed(user.Password))
                {
                    user.Password = HashPassword(user.Password);
                    updated = true;
                }
            }
            if (updated) SaveUsers(usersList);

            return usersList;
        }

        public static void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static bool Register(string username, string password)
        {
            return AddUser(username, password, false);
        }

        public static bool AddUser(string username, string password, bool isAdmin)
        {
            var users = LoadUsers();
            if (users.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)))
                return false;

            users.Add(new User
            {
                Username = username,
                Password = HashPassword(password),
                IsAdmin = isAdmin
            });
            SaveUsers(users);
            return true;
        }

        public static User Validate(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var users = LoadUsers();
            return users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == hashedPassword);
        }

        public static bool PromoteUser(string username)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            user.IsAdmin = true;
            SaveUsers(users);
            return true;
        }

        public static bool IsUserAdmin(string username)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            return user != null && user.IsAdmin;
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // Detect if string is already a SHA256 hash
        private static bool IsHashed(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return input.Length == 64 && input.All(c => "0123456789abcdef".Contains(char.ToLower(c)));
        }
    }
}
