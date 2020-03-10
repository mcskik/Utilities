namespace Differenti8.Views
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
            this.txtOldFile = new System.Windows.Forms.TextBox();
            this.txtNewFile = new System.Windows.Forms.TextBox();
            this.lblOldDir = new System.Windows.Forms.Label();
            this.lblNewDir = new System.Windows.Forms.Label();
            this.txtNewBaseDir = new System.Windows.Forms.TextBox();
            this.lblNewBaseDir = new System.Windows.Forms.Label();
            this.grpDirectories = new System.Windows.Forms.GroupBox();
            this.cmdChooseOldBaseDir = new System.Windows.Forms.Button();
            this.txtOldBaseDir = new System.Windows.Forms.TextBox();
            this.lblOldBaseDir = new System.Windows.Forms.Label();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.lblPreviousSelections = new System.Windows.Forms.Label();
            this.cboPreviousSelections = new System.Windows.Forms.ComboBox();
            this.cmdChooseNewFile = new System.Windows.Forms.Button();
            this.cmdChooseOldFile = new System.Windows.Forms.Button();
            this.cmdChooseNewBaseDir = new System.Windows.Forms.Button();
            this.staStatusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdCompare = new System.Windows.Forms.Button();
            this.txtMinChars = new System.Windows.Forms.TextBox();
            this.lblMinChars = new System.Windows.Forms.Label();
            this.chkCompleteLines = new System.Windows.Forms.CheckBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.txtSubLineMatchLimit = new System.Windows.Forms.TextBox();
            this.lblSubLineMatchLimit = new System.Windows.Forms.Label();
            this.txtLimitLines = new System.Windows.Forms.TextBox();
            this.lblLimitLines = new System.Windows.Forms.Label();
            this.txtLimitCharacters = new System.Windows.Forms.TextBox();
            this.lblLimitCharacters = new System.Windows.Forms.Label();
            this.txtMinLines = new System.Windows.Forms.TextBox();
            this.lblMinLines = new System.Windows.Forms.Label();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.pbrOverallProgress = new System.Windows.Forms.ProgressBar();
            this.pbrCurrentActivity = new System.Windows.Forms.ProgressBar();
            this.cmdCompareMeld = new System.Windows.Forms.Button();
            this.grpDirectories.SuspendLayout();
            this.staStatusBar.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOldFile
            // 
            this.txtOldFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOldFile.Location = new System.Drawing.Point(295, 122);
            this.txtOldFile.Name = "txtOldFile";
            this.txtOldFile.Size = new System.Drawing.Size(232, 20);
            this.txtOldFile.TabIndex = 8;
            this.txtOldFile.MouseEnter += new System.EventHandler(this.txtOldFile_MouseEnter);
            this.txtOldFile.MouseLeave += new System.EventHandler(this.txtOldFile_MouseLeave);
            // 
            // txtNewFile
            // 
            this.txtNewFile.Location = new System.Drawing.Point(11, 121);
            this.txtNewFile.Name = "txtNewFile";
            this.txtNewFile.Size = new System.Drawing.Size(246, 20);
            this.txtNewFile.TabIndex = 6;
            this.txtNewFile.MouseEnter += new System.EventHandler(this.txtNewFile_MouseEnter);
            this.txtNewFile.MouseLeave += new System.EventHandler(this.txtNewFile_MouseLeave);
            // 
            // lblOldDir
            // 
            this.lblOldDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOldDir.AutoSize = true;
            this.lblOldDir.Location = new System.Drawing.Point(293, 106);
            this.lblOldDir.Name = "lblOldDir";
            this.lblOldDir.Size = new System.Drawing.Size(23, 13);
            this.lblOldDir.TabIndex = 2;
            this.lblOldDir.Text = "Old";
            // 
            // lblNewDir
            // 
            this.lblNewDir.AutoSize = true;
            this.lblNewDir.Location = new System.Drawing.Point(8, 106);
            this.lblNewDir.Name = "lblNewDir";
            this.lblNewDir.Size = new System.Drawing.Size(29, 13);
            this.lblNewDir.TabIndex = 3;
            this.lblNewDir.Text = "New";
            // 
            // txtNewBaseDir
            // 
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
            this.lblNewBaseDir.Size = new System.Drawing.Size(31, 13);
            this.lblNewBaseDir.TabIndex = 5;
            this.lblNewBaseDir.Text = "Base";
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
            this.grpDirectories.Controls.Add(this.cmdChooseNewFile);
            this.grpDirectories.Controls.Add(this.cmdChooseOldFile);
            this.grpDirectories.Controls.Add(this.cmdChooseNewBaseDir);
            this.grpDirectories.Controls.Add(this.txtNewBaseDir);
            this.grpDirectories.Controls.Add(this.lblNewDir);
            this.grpDirectories.Controls.Add(this.lblNewBaseDir);
            this.grpDirectories.Controls.Add(this.txtNewFile);
            this.grpDirectories.Controls.Add(this.lblOldDir);
            this.grpDirectories.Controls.Add(this.txtOldFile);
            this.grpDirectories.Location = new System.Drawing.Point(12, 12);
            this.grpDirectories.Name = "grpDirectories";
            this.grpDirectories.Size = new System.Drawing.Size(572, 160);
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
            this.txtOldBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.lblOldBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOldBaseDir.AutoSize = true;
            this.lblOldBaseDir.Location = new System.Drawing.Point(292, 64);
            this.lblOldBaseDir.Name = "lblOldBaseDir";
            this.lblOldBaseDir.Size = new System.Drawing.Size(31, 13);
            this.lblOldBaseDir.TabIndex = 16;
            this.lblOldBaseDir.Text = "Base";
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
            // cmdChooseNewFile
            // 
            this.cmdChooseNewFile.Location = new System.Drawing.Point(263, 119);
            this.cmdChooseNewFile.Name = "cmdChooseNewFile";
            this.cmdChooseNewFile.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseNewFile.TabIndex = 7;
            this.cmdChooseNewFile.Text = "...";
            this.cmdChooseNewFile.UseVisualStyleBackColor = true;
            this.cmdChooseNewFile.Click += new System.EventHandler(this.cmdChooseNewFile_Click);
            // 
            // cmdChooseOldFile
            // 
            this.cmdChooseOldFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseOldFile.Location = new System.Drawing.Point(533, 119);
            this.cmdChooseOldFile.Name = "cmdChooseOldFile";
            this.cmdChooseOldFile.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseOldFile.TabIndex = 9;
            this.cmdChooseOldFile.Text = "...";
            this.cmdChooseOldFile.UseVisualStyleBackColor = true;
            this.cmdChooseOldFile.Click += new System.EventHandler(this.cmdChooseOldFile_Click);
            // 
            // cmdChooseNewBaseDir
            // 
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
            this.staStatusBar.Location = new System.Drawing.Point(0, 450);
            this.staStatusBar.Name = "staStatusBar";
            this.staStatusBar.Size = new System.Drawing.Size(599, 22);
            this.staStatusBar.TabIndex = 10;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // cmdCompare
            // 
            this.cmdCompare.Location = new System.Drawing.Point(6, 16);
            this.cmdCompare.Name = "cmdCompare";
            this.cmdCompare.Size = new System.Drawing.Size(83, 23);
            this.cmdCompare.TabIndex = 1;
            this.cmdCompare.Text = "Compare";
            this.cmdCompare.UseVisualStyleBackColor = true;
            this.cmdCompare.Click += new System.EventHandler(this.cmdCompare_Click);
            // 
            // txtMinChars
            // 
            this.txtMinChars.Location = new System.Drawing.Point(118, 16);
            this.txtMinChars.Name = "txtMinChars";
            this.txtMinChars.Size = new System.Drawing.Size(41, 20);
            this.txtMinChars.TabIndex = 1;
            this.txtMinChars.MouseEnter += new System.EventHandler(this.txtMinChars_MouseEnter);
            this.txtMinChars.MouseLeave += new System.EventHandler(this.txtMinChars_MouseLeave);
            // 
            // lblMinChars
            // 
            this.lblMinChars.AutoSize = true;
            this.lblMinChars.Location = new System.Drawing.Point(11, 19);
            this.lblMinChars.Name = "lblMinChars";
            this.lblMinChars.Size = new System.Drawing.Size(101, 13);
            this.lblMinChars.TabIndex = 4;
            this.lblMinChars.Text = "Minimum characters";
            // 
            // chkCompleteLines
            // 
            this.chkCompleteLines.AutoSize = true;
            this.chkCompleteLines.Location = new System.Drawing.Point(14, 143);
            this.chkCompleteLines.Name = "chkCompleteLines";
            this.chkCompleteLines.Size = new System.Drawing.Size(98, 17);
            this.chkCompleteLines.TabIndex = 6;
            this.chkCompleteLines.Text = "Complete Lines";
            this.chkCompleteLines.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            this.grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpOptions.Controls.Add(this.txtSubLineMatchLimit);
            this.grpOptions.Controls.Add(this.lblSubLineMatchLimit);
            this.grpOptions.Controls.Add(this.txtLimitLines);
            this.grpOptions.Controls.Add(this.lblLimitLines);
            this.grpOptions.Controls.Add(this.txtLimitCharacters);
            this.grpOptions.Controls.Add(this.lblLimitCharacters);
            this.grpOptions.Controls.Add(this.txtMinLines);
            this.grpOptions.Controls.Add(this.lblMinLines);
            this.grpOptions.Controls.Add(this.lblMinChars);
            this.grpOptions.Controls.Add(this.chkCompleteLines);
            this.grpOptions.Controls.Add(this.txtMinChars);
            this.grpOptions.Location = new System.Drawing.Point(12, 178);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(287, 175);
            this.grpOptions.TabIndex = 2;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // txtSubLineMatchLimit
            // 
            this.txtSubLineMatchLimit.Location = new System.Drawing.Point(118, 120);
            this.txtSubLineMatchLimit.Name = "txtSubLineMatchLimit";
            this.txtSubLineMatchLimit.Size = new System.Drawing.Size(41, 20);
            this.txtSubLineMatchLimit.TabIndex = 5;
            this.txtSubLineMatchLimit.MouseEnter += new System.EventHandler(this.txtSubLineMatchLimit_MouseEnter);
            this.txtSubLineMatchLimit.MouseLeave += new System.EventHandler(this.txtSubLineMatchLimit_MouseLeave);
            // 
            // lblSubLineMatchLimit
            // 
            this.lblSubLineMatchLimit.AutoSize = true;
            this.lblSubLineMatchLimit.Location = new System.Drawing.Point(11, 123);
            this.lblSubLineMatchLimit.Name = "lblSubLineMatchLimit";
            this.lblSubLineMatchLimit.Size = new System.Drawing.Size(106, 13);
            this.lblSubLineMatchLimit.TabIndex = 11;
            this.lblSubLineMatchLimit.Text = "Sub Line Match Limit";
            // 
            // txtLimitLines
            // 
            this.txtLimitLines.Location = new System.Drawing.Point(118, 94);
            this.txtLimitLines.Name = "txtLimitLines";
            this.txtLimitLines.Size = new System.Drawing.Size(41, 20);
            this.txtLimitLines.TabIndex = 4;
            this.txtLimitLines.MouseEnter += new System.EventHandler(this.txtLimitLines_MouseEnter);
            this.txtLimitLines.MouseLeave += new System.EventHandler(this.txtLimitLines_MouseLeave);
            // 
            // lblLimitLines
            // 
            this.lblLimitLines.AutoSize = true;
            this.lblLimitLines.Location = new System.Drawing.Point(11, 97);
            this.lblLimitLines.Name = "lblLimitLines";
            this.lblLimitLines.Size = new System.Drawing.Size(56, 13);
            this.lblLimitLines.TabIndex = 9;
            this.lblLimitLines.Text = "Limit Lines";
            // 
            // txtLimitCharacters
            // 
            this.txtLimitCharacters.Location = new System.Drawing.Point(118, 68);
            this.txtLimitCharacters.Name = "txtLimitCharacters";
            this.txtLimitCharacters.Size = new System.Drawing.Size(41, 20);
            this.txtLimitCharacters.TabIndex = 3;
            // 
            // lblLimitCharacters
            // 
            this.lblLimitCharacters.AutoSize = true;
            this.lblLimitCharacters.Location = new System.Drawing.Point(11, 71);
            this.lblLimitCharacters.Name = "lblLimitCharacters";
            this.lblLimitCharacters.Size = new System.Drawing.Size(82, 13);
            this.lblLimitCharacters.TabIndex = 7;
            this.lblLimitCharacters.Text = "Limit Characters";
            // 
            // txtMinLines
            // 
            this.txtMinLines.Location = new System.Drawing.Point(118, 42);
            this.txtMinLines.Name = "txtMinLines";
            this.txtMinLines.Size = new System.Drawing.Size(41, 20);
            this.txtMinLines.TabIndex = 2;
            this.txtMinLines.MouseEnter += new System.EventHandler(this.txtMinLines_MouseEnter);
            this.txtMinLines.MouseLeave += new System.EventHandler(this.txtMinLines_MouseLeave);
            // 
            // lblMinLines
            // 
            this.lblMinLines.AutoSize = true;
            this.lblMinLines.Location = new System.Drawing.Point(11, 45);
            this.lblMinLines.Name = "lblMinLines";
            this.lblMinLines.Size = new System.Drawing.Size(72, 13);
            this.lblMinLines.TabIndex = 5;
            this.lblMinLines.Text = "Minimum lines";
            // 
            // grpActions
            // 
            this.grpActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpActions.Controls.Add(this.cmdCompareMeld);
            this.grpActions.Controls.Add(this.cmdCancel);
            this.grpActions.Controls.Add(this.cmdCompare);
            this.grpActions.Location = new System.Drawing.Point(308, 178);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(276, 175);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(6, 74);
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
            this.grpProgress.Location = new System.Drawing.Point(12, 359);
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
            // cmdCompareMeld
            // 
            this.cmdCompareMeld.Location = new System.Drawing.Point(6, 45);
            this.cmdCompareMeld.Name = "cmdCompareMeld";
            this.cmdCompareMeld.Size = new System.Drawing.Size(83, 23);
            this.cmdCompareMeld.TabIndex = 2;
            this.cmdCompareMeld.Text = "Compare Meld";
            this.cmdCompareMeld.UseVisualStyleBackColor = true;
            this.cmdCompareMeld.Click += new System.EventHandler(this.CmdCompareMeld_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 472);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.staStatusBar);
            this.Controls.Add(this.grpDirectories);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(615, 510);
            this.Name = "frmMain";
            this.Text = "Differenti8";
            this.grpDirectories.ResumeLayout(false);
            this.grpDirectories.PerformLayout();
            this.staStatusBar.ResumeLayout(false);
            this.staStatusBar.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtOldFile;
		private System.Windows.Forms.TextBox txtNewFile;
		private System.Windows.Forms.Label lblOldDir;
		private System.Windows.Forms.Label lblNewDir;
		private System.Windows.Forms.TextBox txtNewBaseDir;
		private System.Windows.Forms.Label lblNewBaseDir;
		private System.Windows.Forms.GroupBox grpDirectories;
		private System.Windows.Forms.Button cmdChooseNewBaseDir;
		private System.Windows.Forms.Button cmdChooseOldFile;
		private System.Windows.Forms.Button cmdChooseNewFile;
		private System.Windows.Forms.Label lblPreviousSelections;
		private System.Windows.Forms.ComboBox cboPreviousSelections;
		private System.Windows.Forms.StatusStrip staStatusBar;
		private System.Windows.Forms.Button cmdCompare;
		private System.Windows.Forms.Label lblMinChars;
		private System.Windows.Forms.TextBox txtMinChars;
		private System.Windows.Forms.CheckBox chkCompleteLines;
		private System.Windows.Forms.GroupBox grpOptions;
		private System.Windows.Forms.GroupBox grpActions;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.TextBox txtLimitCharacters;
		private System.Windows.Forms.Label lblLimitCharacters;
		private System.Windows.Forms.TextBox txtMinLines;
		private System.Windows.Forms.Label lblMinLines;
		private System.Windows.Forms.Button cmdChooseOldBaseDir;
		private System.Windows.Forms.TextBox txtOldBaseDir;
		private System.Windows.Forms.Label lblOldBaseDir;
		private System.Windows.Forms.GroupBox grpProgress;
		private System.Windows.Forms.ProgressBar pbrOverallProgress;
		private System.Windows.Forms.ProgressBar pbrCurrentActivity;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.TextBox txtSubLineMatchLimit;
        private System.Windows.Forms.Label lblSubLineMatchLimit;
        private System.Windows.Forms.TextBox txtLimitLines;
        private System.Windows.Forms.Label lblLimitLines;
        private System.Windows.Forms.Button cmdCompareMeld;
    }
}

