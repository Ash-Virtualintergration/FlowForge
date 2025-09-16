using FlowForge.Models;
using FlowForge.Services;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FlowForge.Forms
{
    public class RegisterForm : Form
    {
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnRegister;

        public User RegisteredUser { get; private set; }

        public RegisterForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Register";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(380, 170);

            lblUsername = new Label { Text = "Username:", Location = new System.Drawing.Point(30, 30), AutoSize = true };
            txtUsername = new TextBox { Location = new System.Drawing.Point(130, 27), Size = new System.Drawing.Size(200, 22) };

            lblPassword = new Label { Text = "Password:", Location = new System.Drawing.Point(30, 70), AutoSize = true };
            txtPassword = new TextBox { Location = new System.Drawing.Point(130, 67), Size = new System.Drawing.Size(200, 22), UseSystemPasswordChar = true };

            btnRegister = new Button { Text = "Register", Location = new System.Drawing.Point(130, 110), Size = new System.Drawing.Size(100, 30) };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnRegister);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hashedPassword = HashPassword(password);

            // ✅ Use AddUser instead of Register
            bool success = UserService.AddUser(username, hashedPassword, false);

            if (success)
            {
                RegisteredUser = new User
                {
                    Username = username,
                    Password = hashedPassword,
                    IsAdmin = false
                };

                MessageBox.Show("Registration successful! Logging you in...", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Username may already exist.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
