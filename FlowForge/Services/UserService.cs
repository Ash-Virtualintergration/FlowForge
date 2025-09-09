using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using FlowForge.Models;

namespace FlowForge.Services
{
    public static class UserService
    {
        private static readonly string FilePath = "users.json";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath)) return new List<User>();
            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }

        public static void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool ValidateUser(string username, string password)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;
            return user.PasswordHash == HashPassword(password);
        }

        public static void AddUser(string username, string password)
        {
            var users = LoadUsers();
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("User already exists.");

            users.Add(new User { Username = username, PasswordHash = HashPassword(password) });
            SaveUsers(users);
        }

        public static void DeleteUser(string username)
        {
            var users = LoadUsers();
            users.RemoveAll(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            SaveUsers(users);
        }

        public static void ResetPassword(string username, string newPassword)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new Exception("User not found.");
            user.PasswordHash = HashPassword(newPassword);
            SaveUsers(users);
        }
    }
}
