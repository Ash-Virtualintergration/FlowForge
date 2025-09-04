using FlowForge;
using FlowForge.Models;
using FlowForge.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class MainForm : MaterialForm
    {
        // Data
        private readonly List<Workflow> _workflows = new List<Workflow>();
        private AppSettings _settings = new AppSettings();

        // UI – Core
        private readonly TabControl _tabs = new TabControl();
        private readonly TabPage _tabDashboard = new TabPage("Dashboard");
        private readonly TabPage _tabWorkflows = new TabPage("Workflows");
        private readonly TabPage _tabSettings = new TabPage("Settings");

        // UI – Status Strip
        private readonly StatusStrip _statusStrip = new StatusStrip();
        private readonly ToolStripStatusLabel _statusLabel = new ToolStripStatusLabel("Ready");
        private readonly Timer _statusResetTimer = new Timer();

        // UI – Workflows
        private readonly ListView _lvWorkflows = new ListView();
        private readonly Button _btnAdd = new Button(); // acts as FAB
        private readonly Button _btnDelete = new Button();

        // UI – Dashboard (3 panels as cards)
        private readonly Panel _cardTotal = new Panel();
        private readonly Panel _cardInProgress = new Panel();
        private readonly Panel _cardCompleted = new Panel();
        private readonly Label _lblTotalNumber = new Label();
        private readonly Label _lblInProgressNumber = new Label();
        private readonly Label _lblCompletedNumber = new Label();

        // UI – Settings
        private readonly CheckBox _switchDark = new CheckBox { Text = "Use Dark Theme", AutoSize = true };

        public MainForm()
        {
            Text = "FlowForge";
            Size = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeMaterialSkin();
            BuildLayout();

            Load += MainForm_Load;
        }

        #region MaterialSkin & Theme
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
                TextShade.WHITE);
        }

        private void ApplyTheme()
        {
            var manager = MaterialSkinManager.Instance;
            manager.Theme = _settings.UseDarkTheme ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;

            if (_settings.UseDarkTheme)
            {
                manager.ColorScheme = new ColorScheme(
                    Primary.Grey800,
                    Primary.Grey900,
                    Primary.Grey700,
                    Accent.Pink200,
                    TextShade.WHITE);
                BackColor = Color.FromArgb(37, 37, 40);
            }
            else
            {
                manager.ColorScheme = new ColorScheme(
                    Primary.Blue600,
                    Primary.Blue700,
                    Primary.Blue200,
                    Accent.Pink200,
                    TextShade.WHITE);
                BackColor = SystemColors.Control;
            }

            Invalidate(true);
            Update();
        }
        #endregion

        #region Layout
        private void BuildLayout()
        {
            _tabs.Dock = DockStyle.Fill;
            _tabs.TabPages.Add(_tabDashboard);
            _tabs.TabPages.Add(_tabWorkflows);
            _tabs.TabPages.Add(_tabSettings);
            Controls.Add(_tabs);

            _statusStrip.Items.Add(_statusLabel);
            _statusStrip.Dock = DockStyle.Bottom;
            Controls.Add(_statusStrip);

            _statusResetTimer.Interval = 2000;
            _statusResetTimer.Tick += (s, e) => { _statusLabel.Text = "Ready"; _statusResetTimer.Stop(); };

            BuildDashboardTab();
            BuildWorkflowsTab();
            BuildSettingsTab();
        }

        private void BuildDashboardTab()
        {
            _tabDashboard.Padding = new Padding(24);

            int cardWidth = 300;
            int cardHeight = 160;
            int left = 24;

            SetupCardPanel(_cardTotal, _lblTotalNumber, "Total Workflows", new Point(left, 24), cardWidth, cardHeight);
            SetupCardPanel(_cardInProgress, _lblInProgressNumber, "In Progress", new Point(left + cardWidth + 24, 24), cardWidth, cardHeight);
            SetupCardPanel(_cardCompleted, _lblCompletedNumber, "Completed", new Point(left + (cardWidth + 24) * 2, 24), cardWidth, cardHeight);

            _tabDashboard.Controls.Add(_cardTotal);
            _tabDashboard.Controls.Add(_cardInProgress);
            _tabDashboard.Controls.Add(_cardCompleted);
        }

        private void SetupCardPanel(Panel panel, Label bigLabel, string smallText, Point location, int w, int h)
        {
            panel.Size = new Size(w, h);
            panel.Location = location;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(16);

            bigLabel.AutoSize = false;
            bigLabel.Height = 64;
            bigLabel.Dock = DockStyle.Top;
            bigLabel.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            bigLabel.TextAlign = ContentAlignment.MiddleLeft;
            bigLabel.Text = "0";

            var smallLabel = new Label
            {
                AutoSize = false,
                Height = 24,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = smallText
            };

            panel.Controls.Add(smallLabel);
            panel.Controls.Add(bigLabel);
        }

        private void BuildWorkflowsTab()
        {
            _tabWorkflows.Padding = new Padding(16);

            _lvWorkflows.Dock = DockStyle.Fill;
            _lvWorkflows.View = View.Details;
            _lvWorkflows.FullRowSelect = true;
            _lvWorkflows.HideSelection = false;
            _lvWorkflows.Columns.Add("Name", 260);
            _lvWorkflows.Columns.Add("Status", 120);
            _lvWorkflows.Columns.Add("Last Modified", 160);
            _lvWorkflows.Columns.Add("Date Created", 160);
            _lvWorkflows.DoubleClick += LvWorkflows_DoubleClick;

            _btnAdd.Text = "+";
            _btnAdd.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            _btnAdd.Size = new Size(56, 56);
            _btnAdd.FlatStyle = FlatStyle.Flat;
            _btnAdd.FlatAppearance.BorderSize = 0;
            _btnAdd.BackColor = Color.FromArgb(46, 134, 171);
            _btnAdd.ForeColor = Color.White;
            _btnAdd.Click += BtnAdd_Click;

            _btnDelete.Text = "Delete Selected";
            _btnDelete.Size = new Size(140, 36);
            _btnDelete.Click += BtnDelete_Click;

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80
            };
            bottomPanel.Controls.Add(_btnDelete);
            bottomPanel.Controls.Add(_btnAdd);

            _btnDelete.Location = new Point(16, 22);
            _btnAdd.Location = new Point(bottomPanel.Width - _btnAdd.Width - 24, 12);
            _btnAdd.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            bottomPanel.Resize += (s, e) =>
            {
                _btnAdd.Location = new Point(bottomPanel.ClientSize.Width - _btnAdd.Width - 24, 12);
            };

            _tabWorkflows.Controls.Add(_lvWorkflows);
            _tabWorkflows.Controls.Add(bottomPanel);
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
        }
        #endregion

        #region Events & Actions
        private void MainForm_Load(object sender, EventArgs e)
        {
            _settings = SettingsService.LoadSettings();
            _switchDark.Checked = _settings.UseDarkTheme;
            ApplyTheme();

            _workflows.Clear();
            _workflows.AddRange(JsonDataService.LoadWorkflows());
            RefreshListView();
            UpdateDashboardCards();
            ShowStatus($"{_workflows.Count} Workflows Loaded");
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormWorkflowDetails())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ResultWorkflow != null)
                {
                    var wf = dlg.ResultWorkflow;
                    wf.Id = Guid.NewGuid();
                    wf.DateCreated = DateTime.Now;
                    wf.LastModified = DateTime.Now;

                    _workflows.Add(wf);
                    JsonDataService.SaveWorkflows(_workflows);

                    RefreshListView();
                    UpdateDashboardCards();
                    ShowStatus("Workflow Saved Successfully");

                    // ✅ Send email async
                    if (!string.IsNullOrEmpty(wf.AssignedTo))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                EmailService.SendEmail(
                                    wf.AssignedTo,
                                    $"[FlowForge] New Workflow Assigned: {wf.Name}",
                                    $"Hello,\n\nYou have been assigned workflow '{wf.Name}'.\n\n" +
                                    $"Status: {wf.Status}\n" +
                                    $"Created on: {wf.DateCreated:dd-MM-yyyy HH:mm}\n\n" +
                                    $"Please log in to FlowForge to take action."
                                );

                                Invoke((MethodInvoker)(() =>
                                    ShowStatus("Workflow saved & email sent")));
                            }
                            catch (Exception ex)
                            {
                                Invoke((MethodInvoker)(() =>
                                    ShowStatus($"Workflow saved, but email failed: {ex.Message}")));
                            }
                        });
                    }
                }
            }
        }

        private void LvWorkflows_DoubleClick(object sender, EventArgs e)
        {
            if (_lvWorkflows.SelectedItems.Count == 0) return;
            var wf = (Workflow)_lvWorkflows.SelectedItems[0].Tag;

            using (var dlg = new FormWorkflowDetails(wf))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ResultWorkflow != null)
                {
                    wf.Name = dlg.ResultWorkflow.Name;
                    wf.Status = dlg.ResultWorkflow.Status;
                    wf.LastModified = DateTime.Now;
                    wf.AssignedTo = dlg.ResultWorkflow.AssignedTo;

                    // ✅ Async email if completed → assign to next user
                    if (!string.IsNullOrEmpty(wf.AssignedTo) &&
                        string.Equals(wf.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                EmailService.SendEmail(
                                    wf.AssignedTo,
                                    $"[FlowForge] Workflow Ready for You: {wf.Name}",
                                    $"Hello,\n\nYou are now assigned workflow '{wf.Name}'.\n\nStatus: In Progress."
                                );

                                Invoke((MethodInvoker)(() =>
                                    ShowStatus("Workflow updated & email sent")));
                            }
                            catch (Exception ex)
                            {
                                Invoke((MethodInvoker)(() =>
                                    ShowStatus($"Workflow updated, but email failed: {ex.Message}")));
                            }
                        });
                    }

                    JsonDataService.SaveWorkflows(_workflows);
                    RefreshListView();
                    UpdateDashboardCards();
                    ShowStatus("Workflow Updated Successfully");
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_lvWorkflows.SelectedItems.Count == 0) return;
            var wf = (Workflow)_lvWorkflows.SelectedItems[0].Tag;

            using (var confirm = new FormDialog($"Delete '{wf.Name}'?"))
            {
                if (confirm.ShowDialog(this) == DialogResult.OK)
                {
                    _workflows.Remove(wf);
                    JsonDataService.SaveWorkflows(_workflows);
                    RefreshListView();
                    UpdateDashboardCards();
                    ShowStatus("Workflow Deleted");
                }
            }
        }
        #endregion

        #region Helpers
        private void RefreshListView()
        {
            _lvWorkflows.BeginUpdate();
            _lvWorkflows.Items.Clear();

            foreach (var wf in _workflows.OrderByDescending(w => w.LastModified))
            {
                var lvi = new ListViewItem(wf.Name);
                lvi.SubItems.Add(wf.Status);
                lvi.SubItems.Add(wf.LastModified.ToString("yyyy-MM-dd HH:mm"));
                lvi.SubItems.Add(wf.DateCreated.ToString("yyyy-MM-dd HH:mm"));
                lvi.Tag = wf;
                _lvWorkflows.Items.Add(lvi);
            }

            _lvWorkflows.EndUpdate();
        }

        private void UpdateDashboardCards()
        {
            int total = _workflows.Count;
            int inProg = _workflows.Count(w => string.Equals(w.Status, "In Progress", StringComparison.OrdinalIgnoreCase));
            int done = _workflows.Count(w => string.Equals(w.Status, "Completed", StringComparison.OrdinalIgnoreCase));

            _lblTotalNumber.Text = total.ToString();
            _lblInProgressNumber.Text = inProg.ToString();
            _lblCompletedNumber.Text = done.ToString();
        }

        private void ShowStatus(string message)
        {
            _statusLabel.Text = message;
            _statusResetTimer.Stop();
            _statusResetTimer.Start();
        }
        #endregion
    }
}
