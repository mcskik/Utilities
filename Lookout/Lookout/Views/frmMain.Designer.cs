using System.Drawing;

namespace Lookout.Views
{
	partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.grpDirectories = new System.Windows.Forms.GroupBox();
            this.cmdChooseTargetOutlookFolder = new System.Windows.Forms.Button();
            this.txtTargetOutlookFolder = new System.Windows.Forms.TextBox();
            this.lblTargetOutlookFolder = new System.Windows.Forms.Label();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.lblPreviousSelections = new System.Windows.Forms.Label();
            this.cboPreviousSelections = new System.Windows.Forms.ComboBox();
            this.staStatusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.cmdMove = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.pbrOverallProgress = new System.Windows.Forms.ProgressBar();
            this.pbrCurrentActivity = new System.Windows.Forms.ProgressBar();
            this.grpCriteria = new System.Windows.Forms.GroupBox();
            this.lblCriteriaName = new System.Windows.Forms.Label();
            this.txtCriteriaName = new System.Windows.Forms.TextBox();
            this.txtBodyCriteria = new System.Windows.Forms.TextBox();
            this.lblBodyCriteria = new System.Windows.Forms.Label();
            this.txtSubjectCriteria = new System.Windows.Forms.TextBox();
            this.lblSubjectCriteria = new System.Windows.Forms.Label();
            this.lblSenderCriteria = new System.Windows.Forms.Label();
            this.txtSenderCriteria = new System.Windows.Forms.TextBox();
            this.grpDirectories.SuspendLayout();
            this.staStatusBar.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpProgress.SuspendLayout();
            this.grpCriteria.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDirectories
            // 
            this.grpDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDirectories.Controls.Add(this.cmdChooseTargetOutlookFolder);
            this.grpDirectories.Controls.Add(this.txtTargetOutlookFolder);
            this.grpDirectories.Controls.Add(this.lblTargetOutlookFolder);
            this.grpDirectories.Controls.Add(this.cmdRemove);
            this.grpDirectories.Controls.Add(this.lblPreviousSelections);
            this.grpDirectories.Controls.Add(this.cboPreviousSelections);
            this.grpDirectories.Location = new System.Drawing.Point(12, 12);
            this.grpDirectories.Name = "grpDirectories";
            this.grpDirectories.Size = new System.Drawing.Size(694, 119);
            this.grpDirectories.TabIndex = 1;
            this.grpDirectories.TabStop = false;
            this.grpDirectories.Text = "Directories";
            // 
            // cmdChooseTargetOutlookFolder
            // 
            this.cmdChooseTargetOutlookFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseTargetOutlookFolder.Location = new System.Drawing.Point(655, 77);
            this.cmdChooseTargetOutlookFolder.Name = "cmdChooseTargetOutlookFolder";
            this.cmdChooseTargetOutlookFolder.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseTargetOutlookFolder.TabIndex = 5;
            this.cmdChooseTargetOutlookFolder.Text = "...";
            this.cmdChooseTargetOutlookFolder.UseVisualStyleBackColor = true;
            this.cmdChooseTargetOutlookFolder.Click += new System.EventHandler(this.cmdChooseTargetFolder_Click);
            // 
            // txtTargetOutlookFolder
            // 
            this.txtTargetOutlookFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetOutlookFolder.Location = new System.Drawing.Point(11, 79);
            this.txtTargetOutlookFolder.Name = "txtTargetOutlookFolder";
            this.txtTargetOutlookFolder.Size = new System.Drawing.Size(638, 20);
            this.txtTargetOutlookFolder.TabIndex = 4;
            this.txtTargetOutlookFolder.DoubleClick += new System.EventHandler(this.txtTargetFolder_DoubleClick);
            this.txtTargetOutlookFolder.MouseEnter += new System.EventHandler(this.txtApkFolder_MouseEnter);
            this.txtTargetOutlookFolder.MouseLeave += new System.EventHandler(this.txtApkFolder_MouseLeave);
            // 
            // lblTargetOutlookFolder
            // 
            this.lblTargetOutlookFolder.AutoSize = true;
            this.lblTargetOutlookFolder.Location = new System.Drawing.Point(8, 63);
            this.lblTargetOutlookFolder.Name = "lblTargetOutlookFolder";
            this.lblTargetOutlookFolder.Size = new System.Drawing.Size(110, 13);
            this.lblTargetOutlookFolder.TabIndex = 16;
            this.lblTargetOutlookFolder.Text = "Target Outlook Folder";
            // 
            // cmdRemove
            // 
            this.cmdRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemove.Location = new System.Drawing.Point(655, 29);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(24, 23);
            this.cmdRemove.TabIndex = 1;
            this.cmdRemove.Text = "X";
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // lblPreviousSelections
            // 
            this.lblPreviousSelections.AutoSize = true;
            this.lblPreviousSelections.Location = new System.Drawing.Point(6, 16);
            this.lblPreviousSelections.Name = "lblPreviousSelections";
            this.lblPreviousSelections.Size = new System.Drawing.Size(100, 13);
            this.lblPreviousSelections.TabIndex = 13;
            this.lblPreviousSelections.Text = "Previous Selections";
            // 
            // cboPreviousSelections
            // 
            this.cboPreviousSelections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPreviousSelections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPreviousSelections.FormattingEnabled = true;
            this.cboPreviousSelections.Location = new System.Drawing.Point(9, 31);
            this.cboPreviousSelections.Name = "cboPreviousSelections";
            this.cboPreviousSelections.Size = new System.Drawing.Size(640, 21);
            this.cboPreviousSelections.TabIndex = 0;
            this.cboPreviousSelections.SelectedIndexChanged += new System.EventHandler(this.cboPreviousSelections_SelectedIndexChanged);
            // 
            // staStatusBar
            // 
            this.staStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.staStatusBar.Location = new System.Drawing.Point(0, 384);
            this.staStatusBar.Name = "staStatusBar";
            this.staStatusBar.Size = new System.Drawing.Size(721, 22);
            this.staStatusBar.TabIndex = 10;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // grpActions
            // 
            this.grpActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpActions.Controls.Add(this.cmdMove);
            this.grpActions.Controls.Add(this.cmdCancel);
            this.grpActions.Location = new System.Drawing.Point(609, 137);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(96, 155);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // cmdMove
            // 
            this.cmdMove.Location = new System.Drawing.Point(6, 22);
            this.cmdMove.Name = "cmdMove";
            this.cmdMove.Size = new System.Drawing.Size(83, 23);
            this.cmdMove.TabIndex = 2;
            this.cmdMove.Text = "Move";
            this.cmdMove.UseVisualStyleBackColor = true;
            this.cmdMove.Click += new System.EventHandler(this.cmdMove_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(6, 51);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(83, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // grpProgress
            // 
            this.grpProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProgress.Controls.Add(this.pbrOverallProgress);
            this.grpProgress.Controls.Add(this.pbrCurrentActivity);
            this.grpProgress.Location = new System.Drawing.Point(12, 293);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(694, 81);
            this.grpProgress.TabIndex = 4;
            this.grpProgress.TabStop = false;
            // 
            // pbrOverallProgress
            // 
            this.pbrOverallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrOverallProgress.Location = new System.Drawing.Point(13, 19);
            this.pbrOverallProgress.Name = "pbrOverallProgress";
            this.pbrOverallProgress.Size = new System.Drawing.Size(673, 23);
            this.pbrOverallProgress.TabIndex = 1;
            // 
            // pbrCurrentActivity
            // 
            this.pbrCurrentActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrCurrentActivity.Location = new System.Drawing.Point(13, 48);
            this.pbrCurrentActivity.Name = "pbrCurrentActivity";
            this.pbrCurrentActivity.Size = new System.Drawing.Size(673, 23);
            this.pbrCurrentActivity.TabIndex = 0;
            // 
            // grpCriteria
            // 
            this.grpCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCriteria.BackColor = System.Drawing.SystemColors.Control;
            this.grpCriteria.Controls.Add(this.lblCriteriaName);
            this.grpCriteria.Controls.Add(this.txtCriteriaName);
            this.grpCriteria.Controls.Add(this.txtBodyCriteria);
            this.grpCriteria.Controls.Add(this.lblBodyCriteria);
            this.grpCriteria.Controls.Add(this.txtSubjectCriteria);
            this.grpCriteria.Controls.Add(this.lblSubjectCriteria);
            this.grpCriteria.Controls.Add(this.lblSenderCriteria);
            this.grpCriteria.Controls.Add(this.txtSenderCriteria);
            this.grpCriteria.Location = new System.Drawing.Point(12, 137);
            this.grpCriteria.Name = "grpCriteria";
            this.grpCriteria.Size = new System.Drawing.Size(591, 155);
            this.grpCriteria.TabIndex = 2;
            this.grpCriteria.TabStop = false;
            this.grpCriteria.Text = "Criteria";
            // 
            // lblCriteriaName
            // 
            this.lblCriteriaName.AutoSize = true;
            this.lblCriteriaName.Location = new System.Drawing.Point(8, 22);
            this.lblCriteriaName.Name = "lblCriteriaName";
            this.lblCriteriaName.Size = new System.Drawing.Size(70, 13);
            this.lblCriteriaName.TabIndex = 6;
            this.lblCriteriaName.Text = "Criteria Name";
            // 
            // txtCriteriaName
            // 
            this.txtCriteriaName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCriteriaName.Location = new System.Drawing.Point(107, 19);
            this.txtCriteriaName.Name = "txtCriteriaName";
            this.txtCriteriaName.Size = new System.Drawing.Size(467, 20);
            this.txtCriteriaName.TabIndex = 0;
            // 
            // txtBodyCriteria
            // 
            this.txtBodyCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBodyCriteria.Location = new System.Drawing.Point(107, 121);
            this.txtBodyCriteria.Name = "txtBodyCriteria";
            this.txtBodyCriteria.Size = new System.Drawing.Size(467, 20);
            this.txtBodyCriteria.TabIndex = 3;
            // 
            // lblBodyCriteria
            // 
            this.lblBodyCriteria.AutoSize = true;
            this.lblBodyCriteria.Location = new System.Drawing.Point(6, 124);
            this.lblBodyCriteria.Name = "lblBodyCriteria";
            this.lblBodyCriteria.Size = new System.Drawing.Size(66, 13);
            this.lblBodyCriteria.TabIndex = 4;
            this.lblBodyCriteria.Text = "Body Criteria";
            // 
            // txtSubjectCriteria
            // 
            this.txtSubjectCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubjectCriteria.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSubjectCriteria.Location = new System.Drawing.Point(107, 87);
            this.txtSubjectCriteria.Name = "txtSubjectCriteria";
            this.txtSubjectCriteria.Size = new System.Drawing.Size(467, 20);
            this.txtSubjectCriteria.TabIndex = 2;
            // 
            // lblSubjectCriteria
            // 
            this.lblSubjectCriteria.AutoSize = true;
            this.lblSubjectCriteria.Location = new System.Drawing.Point(6, 90);
            this.lblSubjectCriteria.Name = "lblSubjectCriteria";
            this.lblSubjectCriteria.Size = new System.Drawing.Size(78, 13);
            this.lblSubjectCriteria.TabIndex = 2;
            this.lblSubjectCriteria.Text = "Subject Criteria";
            // 
            // lblSenderCriteria
            // 
            this.lblSenderCriteria.AutoSize = true;
            this.lblSenderCriteria.Location = new System.Drawing.Point(8, 56);
            this.lblSenderCriteria.Name = "lblSenderCriteria";
            this.lblSenderCriteria.Size = new System.Drawing.Size(76, 13);
            this.lblSenderCriteria.TabIndex = 1;
            this.lblSenderCriteria.Text = "Sender Criteria";
            // 
            // txtSenderCriteria
            // 
            this.txtSenderCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSenderCriteria.Location = new System.Drawing.Point(107, 53);
            this.txtSenderCriteria.Name = "txtSenderCriteria";
            this.txtSenderCriteria.Size = new System.Drawing.Size(467, 20);
            this.txtSenderCriteria.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 406);
            this.Controls.Add(this.grpCriteria);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.staStatusBar);
            this.Controls.Add(this.grpDirectories);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(737, 300);
            this.Name = "frmMain";
            this.Text = "Lookout";
            this.grpDirectories.ResumeLayout(false);
            this.grpDirectories.PerformLayout();
            this.staStatusBar.ResumeLayout(false);
            this.staStatusBar.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpProgress.ResumeLayout(false);
            this.grpCriteria.ResumeLayout(false);
            this.grpCriteria.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.GroupBox grpDirectories;
		private System.Windows.Forms.Label lblPreviousSelections;
		private System.Windows.Forms.ComboBox cboPreviousSelections;
		private System.Windows.Forms.StatusStrip staStatusBar;
		private System.Windows.Forms.GroupBox grpActions;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Button cmdChooseTargetOutlookFolder;
		private System.Windows.Forms.TextBox txtTargetOutlookFolder;
		private System.Windows.Forms.Label lblTargetOutlookFolder;
		private System.Windows.Forms.GroupBox grpProgress;
		private System.Windows.Forms.ProgressBar pbrOverallProgress;
		private System.Windows.Forms.ProgressBar pbrCurrentActivity;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox grpCriteria;
        private System.Windows.Forms.Button cmdMove;
        private System.Windows.Forms.TextBox txtSubjectCriteria;
        private System.Windows.Forms.Label lblSubjectCriteria;
        private System.Windows.Forms.Label lblSenderCriteria;
        private System.Windows.Forms.TextBox txtSenderCriteria;
        private System.Windows.Forms.Label lblBodyCriteria;
        private System.Windows.Forms.TextBox txtBodyCriteria;
        private System.Windows.Forms.Label lblCriteriaName;
        private System.Windows.Forms.TextBox txtCriteriaName;
    }
}

