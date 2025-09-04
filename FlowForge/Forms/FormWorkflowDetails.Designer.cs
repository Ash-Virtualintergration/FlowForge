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
            this.txtName.Location = new System.Drawing.Point(12, 38);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(300, 26);
            this.txtName.TabIndex = 0;
            // 
            // cmbStatus
            // 
            this.cmbStatus.Items.AddRange(new object[] {
            "Not Started",
            "In Progress",
            "Completed"});
            this.cmbStatus.Location = new System.Drawing.Point(12, 70);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(200, 28);
            this.cmbStatus.TabIndex = 1;
            // 
            // txtAssignedTo
            // 
            this.txtAssignedTo.Location = new System.Drawing.Point(12, 136);
            this.txtAssignedTo.Name = "txtAssignedTo";
            this.txtAssignedTo.Size = new System.Drawing.Size(300, 26);
            this.txtAssignedTo.TabIndex = 3;
            // 
            // lblAssignedTo
            // 
            this.lblAssignedTo.Location = new System.Drawing.Point(12, 110);
            this.lblAssignedTo.Name = "lblAssignedTo";
            this.lblAssignedTo.Size = new System.Drawing.Size(150, 23);
            this.lblAssignedTo.TabIndex = 2;
            this.lblAssignedTo.Text = "Assign To (Email):";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 181);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(104, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
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
            this.Name = "FormWorkflowDetails";
            this.Text = "Workflow Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
