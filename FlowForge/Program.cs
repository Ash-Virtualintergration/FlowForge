using System;
using System.Windows.Forms;
using FlowForge.Services;

namespace FlowForge
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Ensure default admin user exists
            EnsureDefaultAdmin();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the login form first
            Application.Run(new LoginForm());
        }

        private static void EnsureDefaultAdmin()
        {
            var users = UserService.LoadUsers();

            if (!users.Exists(u => u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase)))
            {
                UserService.AddUser("admin", "admin");
                Console.WriteLine("Default admin user created: admin/admin");
            }
        }
    }
}
