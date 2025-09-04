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
using System.Windows.Forms.DataVisualization.Charting;

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
        private readonly Button _btnAdd = new Button(); // FAB
        private readonly Button _btnDelete = new Button();

        // FAB Animation
        private readonly Timer _fabAnimationTimer = new Timer();
        private int _fabPulseStep = 0;
        private bool _fabGrowing = true;

        // UI – Dashboard (cards)
        private readonly Panel _cardTotal = new Panel();
        private readonly Panel _cardInProgress = new Panel();
        private readonly Panel _cardCompleted = new Panel();
        private readonly Label _lblTotalNumber = new Label();
        private readonly Label _lblInProgressNumber = new Label();
        private readonly Label _lblCompletedNumber = new Label();
        private readonly Label _lblTotalPercent = new Label();
        private readonly Label _lblInProgressPercent = new Label();
        private readonly Label _lblCompletedPercent = new Label();

        // UI – Dashboard Chart
        private readonly Chart _chartStatus = new Chart();

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

            SetupCardPanel(_cardTotal, _lblTotalNumber, _lblTotalPercent, "Total Workflows", new Point(left, 24), cardWidth, cardHeight, Color.FromArgb(66, 135, 245), Color.FromArgb(33, 80, 180));
            SetupCardPanel(_cardInProgress, _lblInProgressNumber, _lblInProgressPercent, "In Progress", new Point(left + cardWidth + 24, 24), cardWidth, cardHeight, Color.FromArgb(245, 166, 35), Color.FromArgb(200, 120, 20));
            SetupCardPanel(_cardCompleted, _lblCompletedNumber, _lblCompletedPercent, "Completed", new Point(left + (cardWidth + 24) * 2, 24), cardWidth, cardHeight, Color.FromArgb(40, 167, 69), Color.FromArgb(20, 100, 40));

            _chartStatus.Dock = DockStyle.Bottom;
            _chartStatus.Height = 280;

            var chartArea = new ChartArea("Main");
            chartArea.BackColor = Color.Transparent;
            _chartStatus.ChartAreas.Add(chartArea);

            var series = new Series("Workflows")
            {
                ChartType = SeriesChartType.Pie,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                IsValueShownAsLabel = true
            };
            _chartStatus.Series.Add(series);

            _chartStatus.Legends.Add(new Legend("Legend")
            {
                Docking = Docking.Right,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            });

            _tabDashboard.Controls.Add(_cardTotal);
            _tabDashboard.Controls.Add(_cardInProgress);
            _tabDashboard.Controls.Add(_cardCompleted);
            _tabDashboard.Controls.Add(_chartStatus);
        }

        private void SetupCardPanel(Panel panel, Label bigLabel, Label percentLabel, string smallText, Point location, int w, int h, Color startColor, Color endColor)
        {
            panel.Size = new Size(w, h);
            panel.Location = location;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(16);

            panel.Paint += (s, e) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(panel.ClientRectangle, startColor, endColor, 45F))
                {
                    e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                }
            };

            bigLabel.AutoSize = false;
            bigLabel.Height = 72;
            bigLabel.Dock = DockStyle.Top;
            bigLabel.Font = new Font("Segoe UI", 40, FontStyle.Bold);
            bigLabel.ForeColor = Color.White;
            bigLabel.TextAlign = ContentAlignment.MiddleLeft;
            bigLabel.Text = "0";

            percentLabel.AutoSize = false;
            percentLabel.Height = 28;
            percentLabel.Dock = DockStyle.Top;
            percentLabel.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            percentLabel.ForeColor = Color.White;
            percentLabel.TextAlign = ContentAlignment.MiddleLeft;
            percentLabel.Text = "0%";

            var smallLabel = new Label
            {
                AutoSize = false,
                Height = 28,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = smallText
            };

            panel.Controls.Add(smallLabel);
            panel.Controls.Add(percentLabel);
            panel.Controls.Add(bigLabel);
        }
        #endregion

        #region Workflows Tab
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

            // FAB
            _btnAdd.Text = "Create\nTask";
            _btnAdd.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _btnAdd.Size = new Size(120, 120);
            _btnAdd.FlatStyle = FlatStyle.Flat;
            _btnAdd.FlatAppearance.BorderSize = 0;
            _btnAdd.BackColor = Color.FromArgb(0, 123, 255);
            _btnAdd.ForeColor = Color.White;
            _btnAdd.Cursor = Cursors.Hand;

            _btnAdd.Paint += (s, e) =>
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, _btnAdd.Width, _btnAdd.Height);
                    _btnAdd.Region = new Region(path);
                }
            };

            _btnAdd.MouseEnter += (s, e) => _btnAdd.BackColor = Color.FromArgb(0, 105, 217);
            _btnAdd.MouseLeave += (s, e) => _btnAdd.BackColor = Color.FromArgb(0, 123, 255);
            _btnAdd.Click += BtnAdd_Click;

            // FAB Animation
            _fabAnimationTimer.Interval = 50;
            _fabAnimationTimer.Tick += (s, e) =>
            {
                int pulseRange = 10;
                int stepSize = 2;

                if (_fabGrowing)
                {
                    _fabPulseStep += stepSize;
                    if (_fabPulseStep >= pulseRange) _fabGrowing = false;
                }
                else
                {
                    _fabPulseStep -= stepSize;
                    if (_fabPulseStep <= 0) _fabGrowing = true;
                }

                _btnAdd.Invalidate();
            };

            _btnAdd.Paint += (s, e) =>
            {
                int glowSize = _fabPulseStep;
                using (var glowBrush = new SolidBrush(Color.FromArgb(80, 0, 123, 255)))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(glowBrush, -glowSize / 2, -glowSize / 2, _btnAdd.Width + glowSize, _btnAdd.Height + glowSize);
                }
            };

            Load += (s, e) => _fabAnimationTimer.Start();

            _btnDelete.Text = "Delete Selected";
            _btnDelete.Size = new Size(140, 36);
            _btnDelete.Click += BtnDelete_Click;

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };
            bottomPanel.Controls.Add(_btnDelete);
            bottomPanel.Controls.Add(_btnAdd);

            _btnDelete.Location = new Point(16, 30);
            _btnAdd.Location = new Point(bottomPanel.Width - _btnAdd.Width - 24, 0);
            _btnAdd.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            bottomPanel.Resize += (s, e) =>
            {
                _btnAdd.Location = new Point(bottomPanel.ClientSize.Width - _btnAdd.Width - 24, 0);
            };

            _tabWorkflows.Controls.Add(_lvWorkflows);
            _tabWorkflows.Controls.Add(bottomPanel);
        }
        #endregion

        #region Settings Tab
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

        #region Events
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

            _lblTotalPercent.Text = total > 0 ? "100%" : "0%";
            _lblInProgressPercent.Text = total > 0 ? $"{(inProg * 100 / total)}%" : "0%";
            _lblCompletedPercent.Text = total > 0 ? $"{(done * 100 / total)}%" : "0%";

            var series = _chartStatus.Series["Workflows"];
            series.Points.Clear();
            if (total > 0)
            {
                series.Points.AddXY("In Progress", inProg);
                series.Points.AddXY("Completed", done);
                series.Points.AddXY("Other", total - (inProg + done));
            }
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
