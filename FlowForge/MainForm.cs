using FlowForge.Models;
using FlowForge.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class MainForm : MaterialForm
    {
        private readonly string _currentUser;
        private MaterialTabControl tabControl;
        private DataGridView dgvWorkflows;
        private DataGridView dgvUsers;
        private TextBox txtWorkflowSearch;
        private List<Workflow> workflows = new List<Workflow>();
        private List<User> users = new List<User>();
        private readonly string workflowFile = "workflows.json";

        public MainForm(string loggedInUser)
        {
            _currentUser = loggedInUser;
            InitializeComponent();
            InitializeMaterialSkin();
            BuildLayout();
            LoadWorkflowsFromFile();
            LoadUsers();
            RefreshWorkflowGrid();
            RefreshUserGrid();
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
            this.Text = $"FlowForge Dashboard - {_currentUser}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            tabControl = new MaterialTabControl { Dock = DockStyle.Fill };

            var tabWorkflows = new TabPage("Workflows");
            var tabSettings = new TabPage("Settings");

            tabControl.TabPages.Add(tabWorkflows);

            // Only admins get settings tab
            var currentUserObj = UserService.LoadUsers()
                .FirstOrDefault(u => string.Equals(u.Username, _currentUser, StringComparison.OrdinalIgnoreCase));
            if (currentUserObj != null && currentUserObj.IsAdmin)
                tabControl.TabPages.Add(tabSettings);

            this.Controls.Add(tabControl);

            BuildWorkflowTab(tabWorkflows);
            BuildSettingsTab(tabSettings);
        }

        #region Workflow Tab
        private void BuildWorkflowTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var btnNewWorkflow = new MaterialButton
            {
                Text = "Create New Workflow",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnNewWorkflow.Click += BtnNewWorkflow_Click;
            panel.Controls.Add(btnNewWorkflow);

            txtWorkflowSearch = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Text = "Search workflow..."
            };
            txtWorkflowSearch.GotFocus += (s, e) =>
            {
                if (txtWorkflowSearch.Text == "Search workflow...")
                {
                    txtWorkflowSearch.Text = "";
                    txtWorkflowSearch.ForeColor = Color.Black;
                }
            };
            txtWorkflowSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtWorkflowSearch.Text))
                {
                    txtWorkflowSearch.Text = "Search workflow...";
                    txtWorkflowSearch.ForeColor = Color.Gray;
                }
            };
            txtWorkflowSearch.TextChanged += (s, e) => RefreshWorkflowGrid();
            panel.Controls.Add(txtWorkflowSearch);

            dgvWorkflows = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvWorkflows.CellContentClick += DgvWorkflows_CellContentClick;
            panel.Controls.Add(dgvWorkflows);

            panel.Controls.SetChildIndex(txtWorkflowSearch, 0);
            panel.Controls.SetChildIndex(btnNewWorkflow, 0);

            tab.Controls.Add(panel);
        }

        private void LoadWorkflowsFromFile()
        {
            if (File.Exists(workflowFile))
            {
                string json = File.ReadAllText(workflowFile);
                workflows = JsonSerializer.Deserialize<List<Workflow>>(json) ?? new List<Workflow>();
            }
        }

        private void SaveWorkflowsToFile()
        {
            string json = JsonSerializer.Serialize(workflows, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(workflowFile, json);
        }

        private void RefreshWorkflowGrid()
        {
            if (dgvWorkflows == null) return;

            dgvWorkflows.Columns.Clear();
            dgvWorkflows.DataSource = null;

            var filtered = workflows
                .Where(w => string.IsNullOrWhiteSpace(txtWorkflowSearch.Text) || txtWorkflowSearch.ForeColor == Color.Gray ||
                            w.Name.IndexOf(txtWorkflowSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            w.Status.IndexOf(txtWorkflowSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            dgvWorkflows.DataSource = filtered.Select(w => new
            {
                w.Name,
                w.Status,
                w.AssignedTo,
                w.Notes
            }).ToList();

            if (!dgvWorkflows.Columns.Contains("Edit"))
            {
                var editBtn = new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true
                };
                dgvWorkflows.Columns.Add(editBtn);
            }

            if (!dgvWorkflows.Columns.Contains("Delete"))
            {
                var delBtn = new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true
                };
                dgvWorkflows.Columns.Add(delBtn);
            }

            foreach (DataGridViewRow row in dgvWorkflows.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status == "Completed")
                        row.Cells["Status"].Style.BackColor = Color.LightGreen;
                    else if (status == "In Progress")
                        row.Cells["Status"].Style.BackColor = Color.Orange;
                    else
                        row.Cells["Status"].Style.BackColor = Color.IndianRed;
                }
            }
        }

        private void BtnNewWorkflow_Click(object sender, EventArgs e)
        {
            using (var wfForm = new FormWorkflowDetails())
            {
                if (wfForm.ShowDialog() == DialogResult.OK)
                {
                    workflows.Add(wfForm.ResultWorkflow);
                    SaveWorkflowsToFile();
                    RefreshWorkflowGrid();
                }
            }
        }

        private void DgvWorkflows_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var selectedWorkflow = workflows[e.RowIndex];

            if (dgvWorkflows.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (var wfForm = new FormWorkflowDetails(selectedWorkflow))
                {
                    if (wfForm.ShowDialog() == DialogResult.OK)
                    {
                        workflows[e.RowIndex] = wfForm.ResultWorkflow;
                        SaveWorkflowsToFile();
                        RefreshWorkflowGrid();
                    }
                }
            }
            else if (dgvWorkflows.Columns[e.ColumnIndex].Name == "Delete")
            {
                var confirm = MessageBox.Show($"Delete workflow '{selectedWorkflow.Name}'?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    workflows.RemoveAt(e.RowIndex);
                    SaveWorkflowsToFile();
                    RefreshWorkflowGrid();
                }
            }
        }
        #endregion

        #region Settings Tab
        private void BuildSettingsTab(TabPage tab)
        {
            if (tab == null) return;

            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            panel.Controls.Add(dgvUsers);

            var btnAddUser = new MaterialButton { Text = "Add User", Dock = DockStyle.Top, Height = 35 };
            btnAddUser.Click += BtnAddUser_Click;

            var btnDeleteUser = new MaterialButton { Text = "Delete User", Dock = DockStyle.Top, Height = 35 };
            btnDeleteUser.Click += BtnDeleteUser_Click;

            var btnPromoteUser = new MaterialButton { Text = "Promote to Admin", Dock = DockStyle.Top, Height = 35 };
            btnPromoteUser.Click += BtnPromoteUser_Click;

            panel.Controls.Add(btnPromoteUser);
            panel.Controls.Add(btnDeleteUser);
            panel.Controls.Add(btnAddUser);

            panel.Controls.SetChildIndex(btnAddUser, 0);
            panel.Controls.SetChildIndex(btnDeleteUser, 1);
            panel.Controls.SetChildIndex(btnPromoteUser, 2);

            tab.Controls.Add(panel);
        }

        private void LoadUsers()
        {
            users = UserService.LoadUsers();
        }

        private void RefreshUserGrid()
        {
            if (dgvUsers == null) return;

            dgvUsers.Columns.Clear();
            dgvUsers.DataSource = null;

            dgvUsers.DataSource = users.Select(u => new { u.Username, u.IsAdmin }).ToList();
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            string username = Microsoft.VisualBasic.Interaction.InputBox("Enter username:", "Add User", "");
            string password = Microsoft.VisualBasic.Interaction.InputBox("Enter password:", "Add User", "");

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                bool added = UserService.AddUser(username, password, false); // isAdmin = false by default
                if (added)
                {
                    LoadUsers();
                    RefreshUserGrid();
                    MessageBox.Show($"User '{username}' added.");
                }
                else
                {
                    MessageBox.Show($"User '{username}' already exists.");
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;

            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            var confirm = MessageBox.Show($"Delete user '{username}'?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                if (UserService.RemoveUser(username)) // use RemoveUser instead of DeleteUser
                {
                    LoadUsers();
                    RefreshUserGrid();
                }
                else
                {
                    MessageBox.Show("Cannot delete user.");
                }

            }
        }

        private void BtnPromoteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;

            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            try
            {
                UserService.PromoteUser(username);
                LoadUsers();
                RefreshUserGrid();
                MessageBox.Show($"{username} promoted to admin!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
