using System.Drawing;

namespace Copy8.Views
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
            this.txtNewBaseDir = new System.Windows.Forms.TextBox();
            this.lblNewBaseDir = new System.Windows.Forms.Label();
            this.grpDirectories = new System.Windows.Forms.GroupBox();
            this.cmdChooseOldBaseDir = new System.Windows.Forms.Button();
            this.txtOldBaseDir = new System.Windows.Forms.TextBox();
            this.lblOldBaseDir = new System.Windows.Forms.Label();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.lblPreviousSelections = new System.Windows.Forms.Label();
            this.cboPreviousSelections = new System.Windows.Forms.ComboBox();
            this.cmdChooseNewBaseDir = new System.Windows.Forms.Button();
            this.staStatusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdCopy = new System.Windows.Forms.Button();
            this.chkMonitoredTypesOnly = new System.Windows.Forms.CheckBox();
            this.rdoReplacePreservingNewest = new System.Windows.Forms.RadioButton();
            this.rdoReplaceOnlyWithNewer = new System.Windows.Forms.RadioButton();
            this.rdoReplaceMatches = new System.Windows.Forms.RadioButton();
            this.rdoSkipMatches = new System.Windows.Forms.RadioButton();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.cmdSynchronizePlusAll = new System.Windows.Forms.Button();
            this.cmdSynchronizePlus = new System.Windows.Forms.Button();
            this.cmdSynchronizeAll = new System.Windows.Forms.Button();
            this.cmdSynchronize = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.pbrOverallProgress = new System.Windows.Forms.ProgressBar();
            this.pbrCurrentActivity = new System.Windows.Forms.ProgressBar();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.cmdCopySynch = new System.Windows.Forms.Button();
            this.grpDirectories.SuspendLayout();
            this.staStatusBar.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpProgress.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNewBaseDir
            // 
            this.txtNewBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewBaseDir.Location = new System.Drawing.Point(9, 77);
            this.txtNewBaseDir.Name = "txtNewBaseDir";
            this.txtNewBaseDir.Size = new System.Drawing.Size(248, 20);
            this.txtNewBaseDir.TabIndex = 2;
            this.txtNewBaseDir.DoubleClick += new System.EventHandler(this.txtNewBaseDir_DoubleClick);
            this.txtNewBaseDir.MouseEnter += new System.EventHandler(this.txtNewBaseDir_MouseEnter);
            this.txtNewBaseDir.MouseLeave += new System.EventHandler(this.txtNewBaseDir_MouseLeave);
            // 
            // lblNewBaseDir
            // 
            this.lblNewBaseDir.AutoSize = true;
            this.lblNewBaseDir.Location = new System.Drawing.Point(6, 64);
            this.lblNewBaseDir.Name = "lblNewBaseDir";
            this.lblNewBaseDir.Size = new System.Drawing.Size(102, 13);
            this.lblNewBaseDir.TabIndex = 5;
            this.lblNewBaseDir.Text = "Copy From Directory";
            // 
            // grpDirectories
            // 
            this.grpDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDirectories.Controls.Add(this.cmdChooseOldBaseDir);
            this.grpDirectories.Controls.Add(this.txtOldBaseDir);
            this.grpDirectories.Controls.Add(this.lblOldBaseDir);
            this.grpDirectories.Controls.Add(this.cmdRemove);
            this.grpDirectories.Controls.Add(this.lblPreviousSelections);
            this.grpDirectories.Controls.Add(this.cboPreviousSelections);
            this.grpDirectories.Controls.Add(this.cmdChooseNewBaseDir);
            this.grpDirectories.Controls.Add(this.txtNewBaseDir);
            this.grpDirectories.Controls.Add(this.lblNewBaseDir);
            this.grpDirectories.Location = new System.Drawing.Point(12, 12);
            this.grpDirectories.Name = "grpDirectories";
            this.grpDirectories.Size = new System.Drawing.Size(572, 117);
            this.grpDirectories.TabIndex = 1;
            this.grpDirectories.TabStop = false;
            this.grpDirectories.Text = "Directories";
            // 
            // cmdChooseOldBaseDir
            // 
            this.cmdChooseOldBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseOldBaseDir.Location = new System.Drawing.Point(533, 77);
            this.cmdChooseOldBaseDir.Name = "cmdChooseOldBaseDir";
            this.cmdChooseOldBaseDir.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseOldBaseDir.TabIndex = 5;
            this.cmdChooseOldBaseDir.Text = "...";
            this.cmdChooseOldBaseDir.UseVisualStyleBackColor = true;
            this.cmdChooseOldBaseDir.Click += new System.EventHandler(this.cmdChooseOldBaseDir_Click);
            // 
            // txtOldBaseDir
            // 
            this.txtOldBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOldBaseDir.Location = new System.Drawing.Point(295, 77);
            this.txtOldBaseDir.Name = "txtOldBaseDir";
            this.txtOldBaseDir.Size = new System.Drawing.Size(232, 20);
            this.txtOldBaseDir.TabIndex = 4;
            this.txtOldBaseDir.DoubleClick += new System.EventHandler(this.txtOldBaseDir_DoubleClick);
            this.txtOldBaseDir.MouseEnter += new System.EventHandler(this.txtOldBaseDir_MouseEnter);
            this.txtOldBaseDir.MouseLeave += new System.EventHandler(this.txtOldBaseDir_MouseLeave);
            // 
            // lblOldBaseDir
            // 
            this.lblOldBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOldBaseDir.AutoSize = true;
            this.lblOldBaseDir.Location = new System.Drawing.Point(292, 64);
            this.lblOldBaseDir.Name = "lblOldBaseDir";
            this.lblOldBaseDir.Size = new System.Drawing.Size(92, 13);
            this.lblOldBaseDir.TabIndex = 16;
            this.lblOldBaseDir.Text = "Copy To Directory";
            // 
            // cmdRemove
            // 
            this.cmdRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemove.Location = new System.Drawing.Point(533, 29);
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
            this.cboPreviousSelections.FormattingEnabled = true;
            this.cboPreviousSelections.Location = new System.Drawing.Point(9, 31);
            this.cboPreviousSelections.Name = "cboPreviousSelections";
            this.cboPreviousSelections.Size = new System.Drawing.Size(518, 21);
            this.cboPreviousSelections.TabIndex = 0;
            this.cboPreviousSelections.SelectedIndexChanged += new System.EventHandler(this.cboPreviousSelections_SelectedIndexChanged);
            // 
            // cmdChooseNewBaseDir
            // 
            this.cmdChooseNewBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseNewBaseDir.Location = new System.Drawing.Point(263, 77);
            this.cmdChooseNewBaseDir.Name = "cmdChooseNewBaseDir";
            this.cmdChooseNewBaseDir.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseNewBaseDir.TabIndex = 3;
            this.cmdChooseNewBaseDir.Text = "...";
            this.cmdChooseNewBaseDir.UseVisualStyleBackColor = true;
            this.cmdChooseNewBaseDir.Click += new System.EventHandler(this.cmdChooseNewBaseDir_Click);
            // 
            // staStatusBar
            // 
            this.staStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.staStatusBar.Location = new System.Drawing.Point(0, 410);
            this.staStatusBar.Name = "staStatusBar";
            this.staStatusBar.Size = new System.Drawing.Size(599, 22);
            this.staStatusBar.TabIndex = 10;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // cmdCopy
            // 
            this.cmdCopy.Location = new System.Drawing.Point(6, 16);
            this.cmdCopy.Name = "cmdCopy";
            this.cmdCopy.Size = new System.Drawing.Size(72, 23);
            this.cmdCopy.TabIndex = 1;
            this.cmdCopy.Text = "Copy";
            this.cmdCopy.UseVisualStyleBackColor = true;
            this.cmdCopy.Click += new System.EventHandler(this.cmdCopy_Click);
            // 
            // chkMonitoredTypesOnly
            // 
            this.chkMonitoredTypesOnly.AutoSize = true;
            this.chkMonitoredTypesOnly.Location = new System.Drawing.Point(9, 123);
            this.chkMonitoredTypesOnly.Name = "chkMonitoredTypesOnly";
            this.chkMonitoredTypesOnly.Size = new System.Drawing.Size(129, 17);
            this.chkMonitoredTypesOnly.TabIndex = 9;
            this.chkMonitoredTypesOnly.Text = "Monitored Types Only";
            // 
            // rdoReplacePreservingNewest
            // 
            this.rdoReplacePreservingNewest.AutoSize = true;
            this.rdoReplacePreservingNewest.Location = new System.Drawing.Point(9, 91);
            this.rdoReplacePreservingNewest.Name = "rdoReplacePreservingNewest";
            this.rdoReplacePreservingNewest.Size = new System.Drawing.Size(157, 17);
            this.rdoReplacePreservingNewest.TabIndex = 8;
            this.rdoReplacePreservingNewest.TabStop = true;
            this.rdoReplacePreservingNewest.Text = "Replace Preserving Newest";
            this.rdoReplacePreservingNewest.UseVisualStyleBackColor = true;
            // 
            // rdoReplaceOnlyWithNewer
            // 
            this.rdoReplaceOnlyWithNewer.AutoSize = true;
            this.rdoReplaceOnlyWithNewer.Location = new System.Drawing.Point(9, 68);
            this.rdoReplaceOnlyWithNewer.Name = "rdoReplaceOnlyWithNewer";
            this.rdoReplaceOnlyWithNewer.Size = new System.Drawing.Size(148, 17);
            this.rdoReplaceOnlyWithNewer.TabIndex = 7;
            this.rdoReplaceOnlyWithNewer.TabStop = true;
            this.rdoReplaceOnlyWithNewer.Text = "Replace Only With Newer";
            this.rdoReplaceOnlyWithNewer.UseVisualStyleBackColor = true;
            // 
            // rdoReplaceMatches
            // 
            this.rdoReplaceMatches.AutoSize = true;
            this.rdoReplaceMatches.Location = new System.Drawing.Point(9, 45);
            this.rdoReplaceMatches.Name = "rdoReplaceMatches";
            this.rdoReplaceMatches.Size = new System.Drawing.Size(109, 17);
            this.rdoReplaceMatches.TabIndex = 6;
            this.rdoReplaceMatches.TabStop = true;
            this.rdoReplaceMatches.Text = "Replace Matches";
            this.rdoReplaceMatches.UseVisualStyleBackColor = true;
            // 
            // rdoSkipMatches
            // 
            this.rdoSkipMatches.AutoSize = true;
            this.rdoSkipMatches.Location = new System.Drawing.Point(9, 22);
            this.rdoSkipMatches.Name = "rdoSkipMatches";
            this.rdoSkipMatches.Size = new System.Drawing.Size(90, 17);
            this.rdoSkipMatches.TabIndex = 5;
            this.rdoSkipMatches.TabStop = true;
            this.rdoSkipMatches.Text = "Skip Matches";
            this.rdoSkipMatches.UseVisualStyleBackColor = true;
            // 
            // grpActions
            // 
            this.grpActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpActions.Controls.Add(this.cmdCopySynch);
            this.grpActions.Controls.Add(this.cmdSynchronizePlusAll);
            this.grpActions.Controls.Add(this.cmdSynchronizePlus);
            this.grpActions.Controls.Add(this.cmdSynchronizeAll);
            this.grpActions.Controls.Add(this.cmdSynchronize);
            this.grpActions.Controls.Add(this.cmdDelete);
            this.grpActions.Controls.Add(this.cmdCancel);
            this.grpActions.Controls.Add(this.cmdCopy);
            this.grpActions.Location = new System.Drawing.Point(411, 135);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(172, 176);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // cmdSynchronizePlusAll
            // 
            this.cmdSynchronizePlusAll.Location = new System.Drawing.Point(86, 79);
            this.cmdSynchronizePlusAll.Name = "cmdSynchronizePlusAll";
            this.cmdSynchronizePlusAll.Size = new System.Drawing.Size(70, 23);
            this.cmdSynchronizePlusAll.TabIndex = 6;
            this.cmdSynchronizePlusAll.Text = "Synch All +";
            this.cmdSynchronizePlusAll.UseVisualStyleBackColor = true;
            this.cmdSynchronizePlusAll.Click += new System.EventHandler(this.cmdSynchronizePlusAll_Click);
            // 
            // cmdSynchronizePlus
            // 
            this.cmdSynchronizePlus.Location = new System.Drawing.Point(6, 79);
            this.cmdSynchronizePlus.Name = "cmdSynchronizePlus";
            this.cmdSynchronizePlus.Size = new System.Drawing.Size(72, 23);
            this.cmdSynchronizePlus.TabIndex = 5;
            this.cmdSynchronizePlus.Text = "Synch +";
            this.cmdSynchronizePlus.UseVisualStyleBackColor = true;
            this.cmdSynchronizePlus.Click += new System.EventHandler(this.cmdSynchronizePlus_Click);
            // 
            // cmdSynchronizeAll
            // 
            this.cmdSynchronizeAll.Location = new System.Drawing.Point(86, 46);
            this.cmdSynchronizeAll.Name = "cmdSynchronizeAll";
            this.cmdSynchronizeAll.Size = new System.Drawing.Size(70, 23);
            this.cmdSynchronizeAll.TabIndex = 4;
            this.cmdSynchronizeAll.Text = "Synch All";
            this.cmdSynchronizeAll.UseVisualStyleBackColor = true;
            this.cmdSynchronizeAll.Click += new System.EventHandler(this.cmdSynchronizeAll_Click);
            // 
            // cmdSynchronize
            // 
            this.cmdSynchronize.Location = new System.Drawing.Point(6, 46);
            this.cmdSynchronize.Name = "cmdSynchronize";
            this.cmdSynchronize.Size = new System.Drawing.Size(72, 23);
            this.cmdSynchronize.TabIndex = 3;
            this.cmdSynchronize.Text = "Synch";
            this.cmdSynchronize.UseVisualStyleBackColor = true;
            this.cmdSynchronize.Click += new System.EventHandler(this.cmdSynchronize_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(6, 137);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(72, 23);
            this.cmdDelete.TabIndex = 8;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(6, 108);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(72, 23);
            this.cmdCancel.TabIndex = 7;
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
            this.grpProgress.Location = new System.Drawing.Point(12, 319);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(572, 81);
            this.grpProgress.TabIndex = 4;
            this.grpProgress.TabStop = false;
            // 
            // pbrOverallProgress
            // 
            this.pbrOverallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrOverallProgress.Location = new System.Drawing.Point(6, 48);
            this.pbrOverallProgress.Name = "pbrOverallProgress";
            this.pbrOverallProgress.Size = new System.Drawing.Size(551, 23);
            this.pbrOverallProgress.TabIndex = 1;
            // 
            // pbrCurrentActivity
            // 
            this.pbrCurrentActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrCurrentActivity.Location = new System.Drawing.Point(6, 19);
            this.pbrCurrentActivity.Name = "pbrCurrentActivity";
            this.pbrCurrentActivity.Size = new System.Drawing.Size(551, 23);
            this.pbrCurrentActivity.TabIndex = 0;
            // 
            // grpOptions
            // 
            this.grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOptions.BackColor = System.Drawing.SystemColors.Control;
            this.grpOptions.Controls.Add(this.rdoSkipMatches);
            this.grpOptions.Controls.Add(this.rdoReplaceMatches);
            this.grpOptions.Controls.Add(this.chkMonitoredTypesOnly);
            this.grpOptions.Controls.Add(this.rdoReplaceOnlyWithNewer);
            this.grpOptions.Controls.Add(this.rdoReplacePreservingNewest);
            this.grpOptions.Location = new System.Drawing.Point(12, 135);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(393, 176);
            this.grpOptions.TabIndex = 12;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // cmdCopySynch
            // 
            this.cmdCopySynch.Location = new System.Drawing.Point(84, 16);
            this.cmdCopySynch.Name = "cmdCopySynch";
            this.cmdCopySynch.Size = new System.Drawing.Size(72, 23);
            this.cmdCopySynch.TabIndex = 2;
            this.cmdCopySynch.Text = "Copy Synch";
            this.cmdCopySynch.UseVisualStyleBackColor = true;
            this.cmdCopySynch.Click += new System.EventHandler(this.cmdCopySynch_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 432);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.staStatusBar);
            this.Controls.Add(this.grpDirectories);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(615, 451);
            this.Name = "frmMain";
            this.Text = "Copy8";
            this.grpDirectories.ResumeLayout(false);
            this.grpDirectories.PerformLayout();
            this.staStatusBar.ResumeLayout(false);
            this.staStatusBar.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpProgress.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtNewBaseDir;
		private System.Windows.Forms.Label lblNewBaseDir;
		private System.Windows.Forms.GroupBox grpDirectories;
		private System.Windows.Forms.Button cmdChooseNewBaseDir;
		private System.Windows.Forms.Label lblPreviousSelections;
		private System.Windows.Forms.ComboBox cboPreviousSelections;
		private System.Windows.Forms.StatusStrip staStatusBar;
		private System.Windows.Forms.Button cmdCopy;
		private System.Windows.Forms.GroupBox grpActions;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Button cmdChooseOldBaseDir;
		private System.Windows.Forms.TextBox txtOldBaseDir;
		private System.Windows.Forms.Label lblOldBaseDir;
		private System.Windows.Forms.GroupBox grpProgress;
		private System.Windows.Forms.ProgressBar pbrOverallProgress;
		private System.Windows.Forms.ProgressBar pbrCurrentActivity;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.RadioButton rdoReplacePreservingNewest;
        private System.Windows.Forms.RadioButton rdoReplaceOnlyWithNewer;
        private System.Windows.Forms.RadioButton rdoReplaceMatches;
        private System.Windows.Forms.RadioButton rdoSkipMatches;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.CheckBox chkMonitoredTypesOnly;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Button cmdSynchronize;
        private System.Windows.Forms.Button cmdSynchronizeAll;
        private System.Windows.Forms.Button cmdSynchronizePlus;
        private System.Windows.Forms.Button cmdSynchronizePlusAll;
        private System.Windows.Forms.Button cmdCopySynch;
    }
}

