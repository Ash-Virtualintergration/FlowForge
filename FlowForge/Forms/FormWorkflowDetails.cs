using System;
using System.Windows.Forms;
using FlowForge.Models;

namespace FlowForge
{
    public partial class FormWorkflowDetails : Form
    {
        public Workflow ResultWorkflow { get; private set; }

        private readonly TextBox _txtNotes = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Width = 300,
            Height = 100
        };

        private readonly Label _lblNotes = new Label
        {
            Text = "Notes:",
            AutoSize = true
        };

        public FormWorkflowDetails()
        {
            InitializeComponent();

            // ✅ Add Notes label + box to form dynamically
            _lblNotes.Location = new System.Drawing.Point(30, 110);
            _txtNotes.Location = new System.Drawing.Point(30, 130);

            Controls.Add(_lblNotes);
            Controls.Add(_txtNotes);
        }

        public FormWorkflowDetails(Workflow wf) : this()
        {
            txtName.Text = wf.Name;
            cmbStatus.SelectedItem = wf.Status;
            txtAssignedTo.Text = wf.AssignedTo;   // ✅ load assigned user
            _txtNotes.Text = wf.Notes;            // ✅ load notes
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ResultWorkflow = new Workflow
            {
                Name = txtName.Text,
                Status = cmbStatus.SelectedItem?.ToString() ?? "Not Started",
                AssignedTo = txtAssignedTo.Text,  // ✅ save assigned user
                Notes = _txtNotes.Text            // ✅ save notes
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
