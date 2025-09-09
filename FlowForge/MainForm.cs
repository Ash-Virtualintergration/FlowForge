using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using FlowForge.Models;   // ✅ now using the shared model

namespace FlowForge.Services
{
    public static class UserService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

        public static List<UserRecord> LoadUsers()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    var defaultUsers = new List<UserRecord>
                    {
                        new UserRecord { Username = "admin", PasswordHash = HashPassword("admin") }
                    };
                    SaveUsers(defaultUsers);
                    return defaultUsers;
                }

                string json = File.ReadAllText(FilePath);

                var users = JsonConvert.DeserializeObject<List<UserRecord>>(json);

                // If file was empty or corrupted, restore admin
                if (users == null || users.Count == 0)
                {
                    users = new List<UserRecord>
                    {
                        new UserRecord { Username = "admin", PasswordHash = HashPassword("admin") }
                    };
                    SaveUsers(users);
                }

                return users;
            }
            catch
            {
                // In case of corrupted file, recreate with admin
                var defaultUsers = new List<UserRecord>
                {
                    new UserRecord { Username = "admin", PasswordHash = HashPassword("admin") }
                };
                SaveUsers(defaultUsers);
                return defaultUsers;
            }
        }

        public static void SaveUsers(List<UserRecord> users)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static void AddUser(string username, string password)
        {
            var users = LoadUsers();
            if (users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("User already exists.");

            users.Add(new UserRecord { Username = username, PasswordHash = HashPassword(password) });
            SaveUsers(users);
        }

        public static void ResetPassword(string username, string newPassword)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new Exception("User not found.");

            user.PasswordHash = HashPassword(newPassword);
            SaveUsers(users);
        }

        public static void DeleteUser(string username)
        {
            var users = LoadUsers();
            users.RemoveAll(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            SaveUsers(users);
        }

        public static bool ValidateUser(string username, string password)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            return user.PasswordHash == HashPassword(password);
        }

        public static void RestoreDefaultAdmin()
        {
            var users = LoadUsers();

            // Remove any existing admin account
            users.RemoveAll(u => u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase));

            // Add default admin/admin
            users.Add(new UserRecord
            {
                Username = "admin",
                PasswordHash = HashPassword("admin")
            });

            SaveUsers(users);
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }

    public class UserRecord
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
