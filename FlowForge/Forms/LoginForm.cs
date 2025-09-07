using FlowForge.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class LoginForm : MaterialForm
    {
        public string LoggedInUser { get; private set; }

        private readonly MaterialTextBox _txtUsername = new MaterialTextBox();
        private readonly MaterialTextBox _txtPassword = new MaterialTextBox();
        private readonly MaterialButton _btnLogin = new MaterialButton();
        private readonly MaterialButton _btnExit = new MaterialButton();

        public LoginForm()
        {
            Text = "FlowForge - Login";
            Size = new Size(400, 250);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeMaterialSkin();
            BuildLayout();
        }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(
                Primary.Blue600,
                Primary.Blue700,
                Primary.Blue200,
                Accent.Pink200,
                TextShade.WHITE
            );
        }

        private void BuildLayout()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 3
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // Username
            panel.Controls.Add(new Label { Text = "Username:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            _txtUsername.Hint = "Enter username";
            _txtUsername.Dock = DockStyle.Fill;
            panel.Controls.Add(_txtUsername, 1, 0);

            // Password
            panel.Controls.Add(new Label { Text = "Password:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            _txtPassword.Hint = "Enter password";
            _txtPassword.Password = true;
            _txtPassword.Dock = DockStyle.Fill;
            panel.Controls.Add(_txtPassword, 1, 1);

            // Buttons
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };

            _btnLogin.Text = "Login";
            _btnLogin.Click += BtnLogin_Click;

            _btnExit.Text = "Exit";
            _btnExit.Click += (s, e) => Application.Exit();

            buttonPanel.Controls.Add(_btnLogin);
            buttonPanel.Controls.Add(_btnExit);

            panel.Controls.Add(buttonPanel, 1, 2);

            Controls.Add(panel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var username = _txtUsername.Text.Trim();
            var password = _txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (UserService.ValidateUser(username, password))
            {
                LoggedInUser = username;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
