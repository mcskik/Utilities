namespace Same8.Views
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lvwResults = new System.Windows.Forms.ListView();
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuContextCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextCompareMeld = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextCopyNewToOld = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextCopyOldToNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextDeleteNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextDeleteOld = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdFzGo = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdGo = new System.Windows.Forms.Button();
            this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.pbrPprogressBar = new System.Windows.Forms.ProgressBar();
            this.staStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblIdentical = new System.Windows.Forms.Label();
            this.grpDirectories = new System.Windows.Forms.GroupBox();
            this.cmdChooseOldPath = new System.Windows.Forms.Button();
            this.txtOldPath = new System.Windows.Forms.TextBox();
            this.lblOldPath = new System.Windows.Forms.Label();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.lblPreviousSelections = new System.Windows.Forms.Label();
            this.cboPreviousSelections = new System.Windows.Forms.ComboBox();
            this.cmdChooseNewPath = new System.Windows.Forms.Button();
            this.txtNewPath = new System.Windows.Forms.TextBox();
            this.lblNewPath = new System.Windows.Forms.Label();
            this.mnuContextCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContext.SuspendLayout();
            this.staStatusStrip.SuspendLayout();
            this.grpDirectories.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwResults
            // 
            this.lvwResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwResults.ContextMenuStrip = this.mnuContext;
            this.lvwResults.Location = new System.Drawing.Point(12, 146);
            this.lvwResults.Name = "lvwResults";
            this.lvwResults.Size = new System.Drawing.Size(904, 434);
            this.lvwResults.TabIndex = 4;
            this.lvwResults.UseCompatibleStateImageBehavior = false;
            this.lvwResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvwResults_MouseUp);
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContextCompare,
            this.mnuContextCompareMeld,
            this.mnuContextCopyNewToOld,
            this.mnuContextCopyOldToNew,
            this.mnuContextDeleteNew,
            this.mnuContextDeleteOld,
            this.viewNewToolStripMenuItem,
            this.viewOldToolStripMenuItem,
            this.mnuContextCheck});
            this.mnuContext.Name = "mnuContext";
            this.mnuContext.Size = new System.Drawing.Size(162, 224);
            // 
            // mnuContextCompare
            // 
            this.mnuContextCompare.Name = "mnuContextCompare";
            this.mnuContextCompare.Size = new System.Drawing.Size(161, 22);
            this.mnuContextCompare.Text = "Compare Diff8";
            this.mnuContextCompare.Click += new System.EventHandler(this.mnuContextCompare_Click);
            // 
            // mnuContextCompareMeld
            // 
            this.mnuContextCompareMeld.Name = "mnuContextCompareMeld";
            this.mnuContextCompareMeld.Size = new System.Drawing.Size(161, 22);
            this.mnuContextCompareMeld.Text = "Compare Meld";
            this.mnuContextCompareMeld.Click += new System.EventHandler(this.mnuContextCompareMeld_Click);
            // 
            // mnuContextCopyNewToOld
            // 
            this.mnuContextCopyNewToOld.Name = "mnuContextCopyNewToOld";
            this.mnuContextCopyNewToOld.Size = new System.Drawing.Size(161, 22);
            this.mnuContextCopyNewToOld.Text = "Copy new to old";
            this.mnuContextCopyNewToOld.Click += new System.EventHandler(this.mnuContextCopyNewToOld_Click);
            // 
            // mnuContextCopyOldToNew
            // 
            this.mnuContextCopyOldToNew.Name = "mnuContextCopyOldToNew";
            this.mnuContextCopyOldToNew.Size = new System.Drawing.Size(161, 22);
            this.mnuContextCopyOldToNew.Text = "Copy old to new";
            this.mnuContextCopyOldToNew.Click += new System.EventHandler(this.mnuContextCopyOldToNew_Click);
            // 
            // mnuContextDeleteNew
            // 
            this.mnuContextDeleteNew.Name = "mnuContextDeleteNew";
            this.mnuContextDeleteNew.Size = new System.Drawing.Size(161, 22);
            this.mnuContextDeleteNew.Text = "Delete new";
            this.mnuContextDeleteNew.Click += new System.EventHandler(this.mnuContextDeleteNew_Click);
            // 
            // mnuContextDeleteOld
            // 
            this.mnuContextDeleteOld.Name = "mnuContextDeleteOld";
            this.mnuContextDeleteOld.Size = new System.Drawing.Size(161, 22);
            this.mnuContextDeleteOld.Text = "Delete old";
            this.mnuContextDeleteOld.Click += new System.EventHandler(this.mnuContextDeleteOld_Click);
            // 
            // viewNewToolStripMenuItem
            // 
            this.viewNewToolStripMenuItem.Name = "viewNewToolStripMenuItem";
            this.viewNewToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.viewNewToolStripMenuItem.Text = "View new";
            this.viewNewToolStripMenuItem.Click += new System.EventHandler(this.viewNewToolStripMenuItem_Click);
            // 
            // viewOldToolStripMenuItem
            // 
            this.viewOldToolStripMenuItem.Name = "viewOldToolStripMenuItem";
            this.viewOldToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.viewOldToolStripMenuItem.Text = "View old";
            this.viewOldToolStripMenuItem.Click += new System.EventHandler(this.viewOldToolStripMenuItem_Click);
            // 
            // cmdFzGo
            // 
            this.cmdFzGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdFzGo.Location = new System.Drawing.Point(489, 595);
            this.cmdFzGo.Name = "cmdFzGo";
            this.cmdFzGo.Size = new System.Drawing.Size(102, 24);
            this.cmdFzGo.TabIndex = 6;
            this.cmdFzGo.Text = "&Name Match";
            this.cmdFzGo.UseVisualStyleBackColor = true;
            this.cmdFzGo.Click += new System.EventHandler(this.cmdFzGo_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(706, 595);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(102, 24);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdGo
            // 
            this.cmdGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGo.Location = new System.Drawing.Point(814, 595);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(102, 24);
            this.cmdGo.TabIndex = 8;
            this.cmdGo.Text = "&Go";
            this.cmdGo.UseVisualStyleBackColor = true;
            this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
            // 
            // pbrPprogressBar
            // 
            this.pbrPprogressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrPprogressBar.Location = new System.Drawing.Point(12, 595);
            this.pbrPprogressBar.Name = "pbrPprogressBar";
            this.pbrPprogressBar.Size = new System.Drawing.Size(471, 24);
            this.pbrPprogressBar.TabIndex = 13;
            // 
            // staStatusStrip
            // 
            this.staStatusStrip.AutoSize = false;
            this.staStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.staStatusStrip.Location = new System.Drawing.Point(0, 632);
            this.staStatusStrip.Name = "staStatusStrip";
            this.staStatusStrip.Size = new System.Drawing.Size(928, 22);
            this.staStatusStrip.TabIndex = 14;
            this.staStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(510, 17);
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(318, 16);
            // 
            // lblIdentical
            // 
            this.lblIdentical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIdentical.AutoSize = true;
            this.lblIdentical.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblIdentical.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblIdentical.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdentical.Location = new System.Drawing.Point(597, 596);
            this.lblIdentical.Name = "lblIdentical";
            this.lblIdentical.Size = new System.Drawing.Size(103, 22);
            this.lblIdentical.TabIndex = 15;
            this.lblIdentical.Text = "    Identical    ";
            // 
            // grpDirectories
            // 
            this.grpDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDirectories.Controls.Add(this.cmdChooseOldPath);
            this.grpDirectories.Controls.Add(this.txtOldPath);
            this.grpDirectories.Controls.Add(this.lblOldPath);
            this.grpDirectories.Controls.Add(this.cmdRemove);
            this.grpDirectories.Controls.Add(this.lblPreviousSelections);
            this.grpDirectories.Controls.Add(this.cboPreviousSelections);
            this.grpDirectories.Controls.Add(this.cmdChooseNewPath);
            this.grpDirectories.Controls.Add(this.txtNewPath);
            this.grpDirectories.Controls.Add(this.lblNewPath);
            this.grpDirectories.Location = new System.Drawing.Point(12, 12);
            this.grpDirectories.Name = "grpDirectories";
            this.grpDirectories.Size = new System.Drawing.Size(904, 117);
            this.grpDirectories.TabIndex = 16;
            this.grpDirectories.TabStop = false;
            this.grpDirectories.Text = "Directories";
            // 
            // cmdChooseOldPath
            // 
            this.cmdChooseOldPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseOldPath.Location = new System.Drawing.Point(865, 77);
            this.cmdChooseOldPath.Name = "cmdChooseOldPath";
            this.cmdChooseOldPath.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseOldPath.TabIndex = 5;
            this.cmdChooseOldPath.Text = "...";
            this.cmdChooseOldPath.UseVisualStyleBackColor = true;
            this.cmdChooseOldPath.Click += new System.EventHandler(this.cmdChooseOldPath_Click);
            // 
            // txtOldPath
            // 
            this.txtOldPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOldPath.Location = new System.Drawing.Point(446, 77);
            this.txtOldPath.Name = "txtOldPath";
            this.txtOldPath.Size = new System.Drawing.Size(413, 20);
            this.txtOldPath.TabIndex = 4;
            this.txtOldPath.DoubleClick += new System.EventHandler(this.txtOldPath_DoubleClick);
            // 
            // lblOldPath
            // 
            this.lblOldPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOldPath.AutoSize = true;
            this.lblOldPath.Location = new System.Drawing.Point(446, 64);
            this.lblOldPath.Name = "lblOldPath";
            this.lblOldPath.Size = new System.Drawing.Size(48, 13);
            this.lblOldPath.TabIndex = 16;
            this.lblOldPath.Text = "Old Path";
            // 
            // cmdRemove
            // 
            this.cmdRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemove.Location = new System.Drawing.Point(865, 29);
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
            this.cboPreviousSelections.Size = new System.Drawing.Size(850, 21);
            this.cboPreviousSelections.TabIndex = 0;
            this.cboPreviousSelections.SelectedIndexChanged += new System.EventHandler(this.cboPreviousSelections_SelectedIndexChanged);
            // 
            // cmdChooseNewPath
            // 
            this.cmdChooseNewPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseNewPath.Location = new System.Drawing.Point(416, 75);
            this.cmdChooseNewPath.Name = "cmdChooseNewPath";
            this.cmdChooseNewPath.Size = new System.Drawing.Size(24, 23);
            this.cmdChooseNewPath.TabIndex = 3;
            this.cmdChooseNewPath.Text = "...";
            this.cmdChooseNewPath.UseVisualStyleBackColor = true;
            this.cmdChooseNewPath.Click += new System.EventHandler(this.cmdChooseNewPath_Click);
            // 
            // txtNewPath
            // 
            this.txtNewPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewPath.Location = new System.Drawing.Point(9, 77);
            this.txtNewPath.Name = "txtNewPath";
            this.txtNewPath.Size = new System.Drawing.Size(401, 20);
            this.txtNewPath.TabIndex = 2;
            this.txtNewPath.DoubleClick += new System.EventHandler(this.txtNewPath_DoubleClick);
            // 
            // lblNewPath
            // 
            this.lblNewPath.AutoSize = true;
            this.lblNewPath.Location = new System.Drawing.Point(6, 64);
            this.lblNewPath.Name = "lblNewPath";
            this.lblNewPath.Size = new System.Drawing.Size(54, 13);
            this.lblNewPath.TabIndex = 5;
            this.lblNewPath.Text = "New Path";
            // 
            // mnuContextCheck
            // 
            this.mnuContextCheck.Name = "mnuContextCheck";
            this.mnuContextCheck.Size = new System.Drawing.Size(161, 22);
            this.mnuContextCheck.Text = "Check";
            this.mnuContextCheck.Click += new System.EventHandler(this.mnuContextCheck_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 654);
            this.Controls.Add(this.grpDirectories);
            this.Controls.Add(this.lblIdentical);
            this.Controls.Add(this.staStatusStrip);
            this.Controls.Add(this.pbrPprogressBar);
            this.Controls.Add(this.cmdGo);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdFzGo);
            this.Controls.Add(this.lvwResults);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "Same 8";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.mnuContext.ResumeLayout(false);
            this.staStatusStrip.ResumeLayout(false);
            this.staStatusStrip.PerformLayout();
            this.grpDirectories.ResumeLayout(false);
            this.grpDirectories.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView lvwResults;
        private System.Windows.Forms.Button cmdFzGo;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdGo;
        private System.Windows.Forms.FolderBrowserDialog dlgFolder;
        private System.Windows.Forms.ProgressBar pbrPprogressBar;
        private System.Windows.Forms.StatusStrip staStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.Label lblIdentical;
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.ToolStripMenuItem mnuContextCompare;
        private System.Windows.Forms.ToolStripMenuItem mnuContextCopyNewToOld;
        private System.Windows.Forms.ToolStripMenuItem mnuContextCopyOldToNew;
        private System.Windows.Forms.ToolStripMenuItem mnuContextDeleteNew;
        private System.Windows.Forms.ToolStripMenuItem mnuContextDeleteOld;
        private System.Windows.Forms.ToolStripMenuItem viewNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOldToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpDirectories;
        private System.Windows.Forms.Button cmdChooseOldPath;
        private System.Windows.Forms.TextBox txtOldPath;
        private System.Windows.Forms.Label lblOldPath;
        private System.Windows.Forms.Button cmdRemove;
        private System.Windows.Forms.Label lblPreviousSelections;
        private System.Windows.Forms.ComboBox cboPreviousSelections;
        private System.Windows.Forms.Button cmdChooseNewPath;
        private System.Windows.Forms.TextBox txtNewPath;
        private System.Windows.Forms.Label lblNewPath;
        private System.Windows.Forms.ToolStripMenuItem mnuContextCompareMeld;
        private System.Windows.Forms.ToolStripMenuItem mnuContextCheck;
    }
}