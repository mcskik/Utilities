namespace Git8.Views
{
    partial class frmRepository
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRepository));
            this.lblRepositoryName = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.lblRemoteUrl = new System.Windows.Forms.Label();
            this.cboRepositoryName = new System.Windows.Forms.ComboBox();
            this.txtRemoteUrl = new System.Windows.Forms.TextBox();
            this.lblLocalPath = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblRepositoryName
            // 
            this.lblRepositoryName.AutoSize = true;
            this.lblRepositoryName.Location = new System.Drawing.Point(12, 14);
            this.lblRepositoryName.Name = "lblRepositoryName";
            this.lblRepositoryName.Size = new System.Drawing.Size(88, 13);
            this.lblRepositoryName.TabIndex = 1;
            this.lblRepositoryName.Text = "Repository Name";
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSave.Location = new System.Drawing.Point(469, 108);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 3;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDelete.Location = new System.Drawing.Point(550, 108);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(75, 23);
            this.cmdDelete.TabIndex = 4;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // lblRemoteUrl
            // 
            this.lblRemoteUrl.AutoSize = true;
            this.lblRemoteUrl.Location = new System.Drawing.Point(12, 44);
            this.lblRemoteUrl.Name = "lblRemoteUrl";
            this.lblRemoteUrl.Size = new System.Drawing.Size(82, 13);
            this.lblRemoteUrl.TabIndex = 8;
            this.lblRemoteUrl.Text = "Repository URL";
            // 
            // cboRepositoryName
            // 
            this.cboRepositoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRepositoryName.FormattingEnabled = true;
            this.cboRepositoryName.Location = new System.Drawing.Point(117, 11);
            this.cboRepositoryName.Name = "cboRepositoryName";
            this.cboRepositoryName.Size = new System.Drawing.Size(506, 21);
            this.cboRepositoryName.TabIndex = 0;
            this.cboRepositoryName.SelectedIndexChanged += new System.EventHandler(this.cboRepositoryName_SelectedIndexChanged);
            // 
            // txtRemoteUrl
            // 
            this.txtRemoteUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteUrl.Location = new System.Drawing.Point(117, 41);
            this.txtRemoteUrl.Name = "txtRemoteUrl";
            this.txtRemoteUrl.Size = new System.Drawing.Size(506, 20);
            this.txtRemoteUrl.TabIndex = 1;
            // 
            // lblLocalPath
            // 
            this.lblLocalPath.AutoSize = true;
            this.lblLocalPath.Location = new System.Drawing.Point(12, 73);
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Size = new System.Drawing.Size(99, 13);
            this.lblLocalPath.TabIndex = 10;
            this.lblLocalPath.Text = "Working Copy Path";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalPath.Location = new System.Drawing.Point(117, 70);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Size = new System.Drawing.Size(506, 20);
            this.txtLocalPath.TabIndex = 2;
            // 
            // frmRepository
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 143);
            this.Controls.Add(this.txtLocalPath);
            this.Controls.Add(this.lblLocalPath);
            this.Controls.Add(this.txtRemoteUrl);
            this.Controls.Add(this.cboRepositoryName);
            this.Controls.Add(this.lblRemoteUrl);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.lblRepositoryName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRepository";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblRepositoryName;
        private System.Windows.Forms.ComboBox cboRepositoryName;
        private System.Windows.Forms.Label lblRemoteUrl;
        private System.Windows.Forms.TextBox txtRemoteUrl;
        private System.Windows.Forms.Label lblLocalPath;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdDelete;
    }
}

