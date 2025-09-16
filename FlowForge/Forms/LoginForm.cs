using FlowForge.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowForge.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnExit;

        public string LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "FlowForge - Login";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(420, 210);
            this.MaximizeBox = false;

            var lblUser = new Label { Text = "Username:", Location = new Point(24, 28), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(120, 24), Width = 260 };

            var lblPass = new Label { Text = "Password:", Location = new Point(24, 68), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(120, 64), Width = 260, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "LOGIN", Location = new Point(220, 120), Size = new Size(80, 32) };
            btnRegister = new Button { Text = "REGISTER", Location = new Point(120, 120), Size = new Size(90, 32) };
            btnExit = new Button { Text = "EXIT", Location = new Point(320, 120), Size = new Size(60, 32) };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblUser, txtUsername, lblPass, txtPassword, btnLogin, btnRegister, btnExit });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = UserService.Validate(username, password);
            if (user != null)
            {
                LoggedInUser = user.Username;
                // Open main form
                Hide();
                var main = new MainForm(LoggedInUser); // your MainForm expects currentUser
                main.ShowDialog();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (var reg = new RegisterForm())
            {
                reg.ShowDialog();
            }
        }
    }
}
