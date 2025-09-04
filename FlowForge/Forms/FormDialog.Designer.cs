namespace FlowForge
{
    partial class FormDialog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

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
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(20, 20);
            this.lblMessage.Size = new System.Drawing.Size(300, 40);
            // 
            // btnOk
            // 
            this.btnOk.Text = "Yes";
            this.btnOk.Location = new System.Drawing.Point(140, 80);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Text = "No";
            this.btnCancel.Location = new System.Drawing.Point(220, 80);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormDialog
            // 
            this.ClientSize = new System.Drawing.Size(360, 140);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Text = "Confirm";
            this.ResumeLayout(false);
        }
    }
}
