using FlowForge.Models;
using FlowForge.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FlowForge
{
    public partial class MainForm : MaterialForm
    {
        private readonly string _currentUser;
        private readonly List<Workflow> _workflows = new List<Workflow>();
        private AppSettings _settings = new AppSettings();

        private readonly TabControl _tabs = new TabControl();
        private readonly TabPage _tabDashboard = new TabPage("Dashboard");
        private readonly TabPage _tabWorkflows = new TabPage("Workflows");
        private readonly TabPage _tabSettings = new TabPage("Settings");

        private readonly StatusStrip _statusStrip = new StatusStrip();
        private readonly ToolStripStatusLabel _statusLabel = new ToolStripStatusLabel("Ready");
        private readonly Timer _statusResetTimer = new Timer();

        private readonly ListView _lvWorkflows = new ListView();
        private readonly Button _btnAdd = new Button();
        private readonly Button _btnDelete = new Button();

        private readonly Button _btnLogout = new Button();

        private readonly CheckBox _switchDark = new CheckBox { Text = "Use Dark Theme", AutoSize = true };
        private readonly ListView _lvUsers = new ListView();
        private readonly Button _btnAddUser = new Button();
        private readonly Button _btnResetPassword = new Button();
        private readonly Button _btnDeleteUser = new Button();
        private readonly Button _btnRestoreAdmin = new Button();

        public MainForm(string currentUser)
        {
            _currentUser = currentUser;
            Text = "FlowForge";
            Size = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeMaterialSkin();
            BuildLayout();
            Load += MainForm_Load;
        }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700, Primary.Blue200, Accent.Pink200, TextShade.WHITE);
        }

        private void ApplyTheme()
        {
            var manager = MaterialSkinManager.Instance;
            manager.Theme = _settings.UseDarkTheme ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            Invalidate();
            Update();
        }

        private void BuildLayout()
        {
            _tabs.Dock = DockStyle.Fill;
            _tabs.TabPages.Add(_tabDashboard);
            _tabs.TabPages.Add(_tabWorkflows);

            if (_currentUser.Equals("admin", StringComparison.OrdinalIgnoreCase))
                _tabs.TabPages.Add(_tabSettings);

            _btnLogout.Text = "Log Out";
            _btnLogout.Size = new Size(100, 32);
            _btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnLogout.Location = new Point(ClientSize.Width - 120, 10);
            _btnLogout.Click += BtnLogout_Click;
            Controls.Add(_btnLogout);

            Controls.Add(_tabs);
            _statusStrip.Items.Add(_statusLabel);
            _statusStrip.Dock = DockStyle.Bottom;
            Controls.Add(_statusStrip);

            _statusResetTimer.Interval = 2000;
            _statusResetTimer.Tick += (s, e) => { _statusLabel.Text = "Ready"; _statusResetTimer.Stop(); };

            BuildDashboardTab();
            BuildWorkflowsTab();
            if (_currentUser.Equals("admin", StringComparison.OrdinalIgnoreCase)) BuildSettingsTab();
        }

        private void BuildDashboardTab()
        {
            _tabDashboard.Controls.Add(new Label
            {
                Text = "Dashboard coming soon...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
        }

        private void BuildWorkflowsTab()
        {
            _lvWorkflows.Dock = DockStyle.Fill;
            _lvWorkflows.View = View.Details;
            _lvWorkflows.Columns.Add("Name", 200);
            _lvWorkflows.Columns.Add("Status", 100);
            _lvWorkflows.Columns.Add("Assigned To", 150);
            _lvWorkflows.Columns.Add("Notes", 300);
            _tabWorkflows.Controls.Add(_lvWorkflows);
        }

        private void BuildSettingsTab()
        {
            _tabSettings.Padding = new Padding(24);
            _switchDark.Location = new Point(24, 24);
            _switchDark.CheckedChanged += (s, e) =>
            {
                _settings.UseDarkTheme = _switchDark.Checked;
                SettingsService.SaveSettings(_settings);
                ApplyTheme();
                ShowStatus("Theme updated");
            };
            _tabSettings.Controls.Add(_switchDark);

            _lvUsers.Dock = DockStyle.Bottom;
            _lvUsers.Height = 300;
            _lvUsers.View = View.Details;
            _lvUsers.Columns.Add("Username", 300);
            _tabSettings.Controls.Add(_lvUsers);

            var panelButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight
            };

            _btnAddUser.Text = "Add User";
            _btnAddUser.Click += (s, e) =>
            {
                string username = ShowInputDialog("Enter new username:", "Add User");
                string password = ShowInputDialog("Enter password:", "Add User");
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    try
                    {
                        UserService.AddUser(username, password);
                        LoadUsers();
                        ShowStatus("User added");
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            };

            _btnResetPassword.Text = "Reset Password";
            _btnResetPassword.Click += (s, e) =>
            {
                if (_lvUsers.SelectedItems.Count == 0) return;
                string username = _lvUsers.SelectedItems[0].Text;
                string newPass = ShowInputDialog("Enter new password:", "Reset Password");
                if (!string.IsNullOrEmpty(newPass))
                {
                    UserService.ResetPassword(username, newPass);
                    ShowStatus("Password reset");
                }
            };

            _btnDeleteUser.Text = "Delete User";
            _btnDeleteUser.Click += (s, e) =>
            {
                if (_lvUsers.SelectedItems.Count == 0) return;
                string username = _lvUsers.SelectedItems[0].Text;
                UserService.DeleteUser(username);
                LoadUsers();
                ShowStatus("User deleted");
            };

            _btnRestoreAdmin.Text = "Restore Default Admin";
            _btnRestoreAdmin.Click += (s, e) =>
            {
                UserService.RestoreDefaultAdmin();
                LoadUsers();
                ShowStatus("Default admin restored");
            };

            panelButtons.Controls.Add(_btnAddUser);
            panelButtons.Controls.Add(_btnResetPassword);
            panelButtons.Controls.Add(_btnDeleteUser);
            panelButtons.Controls.Add(_btnRestoreAdmin);

            _tabSettings.Controls.Add(panelButtons);
            LoadUsers();
        }

        private void LoadUsers()
        {
            _lvUsers.Items.Clear();
            foreach (var user in UserService.LoadUsers())
                _lvUsers.Items.Add(new ListViewItem(user.Username));
        }

        private void ShowStatus(string message)
        {
            _statusLabel.Text = message;
            _statusResetTimer.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowStatus($"Welcome, {_currentUser}");
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var main = new MainForm(login.LoggedInUser);
                    main.ShowDialog();
                }
            }
            this.Close();
        }

        // -------------------------
        // Custom input dialog method
        // -------------------------
        private string ShowInputDialog(string text, string caption)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.Text = caption;
                inputForm.StartPosition = FormStartPosition.CenterParent;

                Label lbl = new Label() { Left = 10, Top = 10, Text = text, Width = 360 };
                TextBox txt = new TextBox() { Left = 10, Top = 40, Width = 360 };
                Button ok = new Button() { Text = "OK", Left = 220, Width = 75, Top = 70, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancel", Left = 300, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(txt);
                inputForm.Controls.Add(ok);
                inputForm.Controls.Add(cancel);

                inputForm.AcceptButton = ok;
                inputForm.CancelButton = cancel;

                return inputForm.ShowDialog() == DialogResult.OK ? txt.Text : "";
            }
        }
    }
}
