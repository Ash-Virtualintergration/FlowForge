using FlowForge.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class FormWorkflowDetails : Form
    {
        public Workflow ResultWorkflow { get; private set; }

        private readonly TextBox _txtNotes = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Width = 350,
            Height = 120,
            Font = new Font("Segoe UI", 10),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        private readonly Label _lblNotes = new Label
        {
            Text = "Notes:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        public FormWorkflowDetails()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Text = "Workflow Details";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(450, 450);

            // --- Dynamically position Notes below txtAssignedTo ---
            int notesTop = txtAssignedTo.Bottom + 20; // 20 px spacing below AssignedTo
            _lblNotes.Location = new Point(30, notesTop);
            _txtNotes.Location = new Point(30, _lblNotes.Bottom + 5);

            Controls.Add(_lblNotes);
            Controls.Add(_txtNotes);

            // --- Move OK / Cancel buttons below Notes ---
            int buttonsTop = _txtNotes.Bottom + 20;
            if (btnOk != null)
            {
                btnOk.Location = new Point(30, buttonsTop);
                StyleButton(btnOk, Color.FromArgb(0, 120, 215));
            }

            if (btnCancel != null)
            {
                btnCancel.Location = new Point(150, buttonsTop);
                StyleButton(btnCancel, Color.Gray);
            }
        }

        public FormWorkflowDetails(Workflow wf) : this()
        {
            txtName.Text = wf.Name;
            cmbStatus.SelectedItem = wf.Status;
            txtAssignedTo.Text = wf.AssignedTo;
            _txtNotes.Text = wf.Notes;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ResultWorkflow = new Workflow
            {
                Name = txtName.Text,
                Status = cmbStatus.SelectedItem?.ToString() ?? "Not Started",
                AssignedTo = txtAssignedTo.Text,
                Notes = _txtNotes.Text
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(color);
            btn.MouseLeave += (s, e) => btn.BackColor = color;
        }
    }
}
