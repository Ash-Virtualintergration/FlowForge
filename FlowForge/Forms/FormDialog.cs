using System;
using System.Windows.Forms;

namespace FlowForge
{
    public partial class FormDialog : Form
    {
        public FormDialog(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
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
