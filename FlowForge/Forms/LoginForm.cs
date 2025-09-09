using FlowForge.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class LoginForm : Form
    {
        public string LoggedInUser { get; private set; }

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblMessage;

        public LoginForm()
        {
            InitializeComponent();
            BuildLayout();
        }

        private void BuildLayout()
        {
            this.Text = "Login - FlowForge";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblUser = new Label { Text = "Username:", Location = new Point(50, 40), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(150, 40), Width = 180 };

            Label lblPass = new Label { Text = "Password:", Location = new Point(50, 80), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(150, 80), Width = 180, UseSystemPasswordChar = true };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(150, 130),
                Width = 100,
                Height = 30
            };
            btnLogin.Click += BtnLogin_Click;

            lblMessage = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(50, 170),
                Width = 280,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblMessage);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (UserService.ValidateUser(username, password))
            {
                LoggedInUser = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblMessage.Text = "Invalid username or password!";
            }
        }
    }
}
