using System;
using System.Windows.Forms;
using FlowForge.Models;

namespace FlowForge
{
    public partial class FormWorkflowDetails : Form
    {
        public Workflow ResultWorkflow { get; private set; }

        public FormWorkflowDetails()
        {
            InitializeComponent();
        }

        public FormWorkflowDetails(Workflow wf) : this()
        {
            txtName.Text = wf.Name;
            cmbStatus.SelectedItem = wf.Status;
            txtAssignedTo.Text = wf.AssignedTo;  // ✅ load assigned user
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ResultWorkflow = new Workflow
            {
                Name = txtName.Text,
                Status = cmbStatus.SelectedItem?.ToString() ?? "Not Started",
                AssignedTo = txtAssignedTo.Text  // ✅ save assigned user
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

