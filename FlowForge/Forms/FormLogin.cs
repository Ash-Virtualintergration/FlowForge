using System;
using System.Windows.Forms;
using FlowForge.Services;

namespace FlowForge
{
    public class FormLogin : Form
    {
        private TextBox txtUser = new TextBox();
        private TextBox txtPass = new TextBox();
        private Button btnLogin = new Button();

        public string LoggedInUser { get; private set; }

        public FormLogin()
        {
            Text = "Login - FlowForge";
            Size = new System.Drawing.Size(350, 200);
            StartPosition = FormStartPosition.CenterScreen;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(10) };

            layout.Controls.Add(new Label { Text = "Username:" }, 0, 0);
            layout.Controls.Add(txtUser, 1, 0);

            layout.Controls.Add(new Label { Text = "Password:" }, 0, 1);
            txtPass.PasswordChar = '●';
            layout.Controls.Add(txtPass, 1, 1);

            btnLogin.Text = "Login";
            btnLogin.Dock = DockStyle.Fill;
            btnLogin.Click += BtnLogin_Click;
            layout.Controls.Add(btnLogin, 1, 2);

            Controls.Add(layout);

            // Create a default admin if no users exist
            var users = UserService.LoadUsers();
            if (users.Count == 0)
            {
                UserService.AddUser("admin", "admin");
                MessageBox.Show("Default user created: admin/admin", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (UserService.ValidateUser(txtUser.Text, txtPass.Text))
            {
                LoggedInUser = txtUser.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
