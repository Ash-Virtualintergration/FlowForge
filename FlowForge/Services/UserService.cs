using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace FlowForge.Services
{
    public static class UserService
    {
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

        public static List<UserRecord> LoadUsers()
        {
            if (!File.Exists(_filePath))
            {
                var defaultUsers = new List<UserRecord>
                {
                    new UserRecord { Username = "admin", PasswordHash = HashPassword("admin") }
                };
                SaveUsers(defaultUsers);
                return defaultUsers;
            }

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<UserRecord>>(json) ?? new List<UserRecord>();
        }

        public static void SaveUsers(List<UserRecord> users)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
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
            users.RemoveAll(u => u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase));
            users.Add(new UserRecord { Username = "admin", PasswordHash = HashPassword("admin") });
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
