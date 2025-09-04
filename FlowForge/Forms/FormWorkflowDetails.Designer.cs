namespace FlowForge
{
    partial class FormWorkflowDetails
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.TextBox txtAssignedTo;   // ✅ new
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblAssignedTo;     // ✅ new

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.txtAssignedTo = new System.Windows.Forms.TextBox();
            this.lblAssignedTo = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(30, 30);
            this.txtName.Size = new System.Drawing.Size(300, 23);
            // 
            // cmbStatus
            // 
            this.cmbStatus.Location = new System.Drawing.Point(30, 70);
            this.cmbStatus.Size = new System.Drawing.Size(200, 23);
            this.cmbStatus.Items.AddRange(new object[] { "Not Started", "In Progress", "Completed" });
            // 
            // lblAssignedTo
            // 
            this.lblAssignedTo.Text = "Assign To (Email):";
            this.lblAssignedTo.Location = new System.Drawing.Point(30, 110);
            this.lblAssignedTo.Size = new System.Drawing.Size(150, 23);
            // 
            // txtAssignedTo
            // 
            this.txtAssignedTo.Location = new System.Drawing.Point(30, 140);
            this.txtAssignedTo.Size = new System.Drawing.Size(300, 23);
            // 
            // btnOk
            // 
            this.btnOk.Text = "OK";
            this.btnOk.Location = new System.Drawing.Point(180, 190);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(260, 190);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormWorkflowDetails
            // 
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblAssignedTo);
            this.Controls.Add(this.txtAssignedTo);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Text = "Workflow Details";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
