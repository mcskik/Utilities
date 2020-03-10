namespace Git8.Views
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
            this.lblCommandLine = new System.Windows.Forms.Label();
            this.cmdRun = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.rtxOutput = new System.Windows.Forms.RichTextBox();
            this.cmdClear = new System.Windows.Forms.Button();
            this.lblCheckoutBranch = new System.Windows.Forms.Label();
            this.lblFileSpec = new System.Windows.Forms.Label();
            this.lblSha = new System.Windows.Forms.Label();
            this.cboCommandLine = new System.Windows.Forms.ComboBox();
            this.lblCommandTemplate = new System.Windows.Forms.Label();
            this.cboCommandTemplate = new System.Windows.Forms.ComboBox();
            this.lblHead = new System.Windows.Forms.Label();
            this.cboHead = new System.Windows.Forms.ComboBox();
            this.cboSha = new System.Windows.Forms.ComboBox();
            this.cboFileSpec = new System.Windows.Forms.ComboBox();
            this.cboCheckoutBranch = new System.Windows.Forms.ComboBox();
            this.lblRemoteBranch = new System.Windows.Forms.Label();
            this.cboRemoteBranch = new System.Windows.Forms.ComboBox();
            this.lblLocalBranch = new System.Windows.Forms.Label();
            this.cboLocalBranch = new System.Windows.Forms.ComboBox();
            this.rtxJournal = new System.Windows.Forms.RichTextBox();
            this.lblJournal = new System.Windows.Forms.Label();
            this.lblComment = new System.Windows.Forms.Label();
            this.cboComment = new System.Windows.Forms.ComboBox();
            this.lblStash = new System.Windows.Forms.Label();
            this.cboStash = new System.Windows.Forms.ComboBox();
            this.cmdSettings = new System.Windows.Forms.Button();
            this.cmdGitStatus = new System.Windows.Forms.Button();
            this.cmdGitLog = new System.Windows.Forms.Button();
            this.cmdParse = new System.Windows.Forms.Button();
            this.cmdGitDiff = new System.Windows.Forms.Button();
            this.cmdGitAdd = new System.Windows.Forms.Button();
            this.cmdGitLogFilter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCommandLine
            // 
            this.lblCommandLine.AutoSize = true;
            this.lblCommandLine.Location = new System.Drawing.Point(12, 9);
            this.lblCommandLine.Name = "lblCommandLine";
            this.lblCommandLine.Size = new System.Drawing.Size(54, 13);
            this.lblCommandLine.TabIndex = 1;
            this.lblCommandLine.Text = "Command";
            // 
            // cmdRun
            // 
            this.cmdRun.Location = new System.Drawing.Point(12, 51);
            this.cmdRun.Name = "cmdRun";
            this.cmdRun.Size = new System.Drawing.Size(75, 23);
            this.cmdRun.TabIndex = 2;
            this.cmdRun.Text = "Run";
            this.cmdRun.UseVisualStyleBackColor = true;
            this.cmdRun.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(9, 129);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(39, 13);
            this.lblOutput.TabIndex = 4;
            this.lblOutput.Text = "Output";
            // 
            // rtxOutput
            // 
            this.rtxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxOutput.Location = new System.Drawing.Point(12, 150);
            this.rtxOutput.Name = "rtxOutput";
            this.rtxOutput.Size = new System.Drawing.Size(748, 701);
            this.rtxOutput.TabIndex = 13;
            this.rtxOutput.Text = "";
            // 
            // cmdClear
            // 
            this.cmdClear.Location = new System.Drawing.Point(12, 77);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(75, 23);
            this.cmdClear.TabIndex = 6;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // lblCheckoutBranch
            // 
            this.lblCheckoutBranch.AutoSize = true;
            this.lblCheckoutBranch.Location = new System.Drawing.Point(93, 56);
            this.lblCheckoutBranch.Name = "lblCheckoutBranch";
            this.lblCheckoutBranch.Size = new System.Drawing.Size(90, 13);
            this.lblCheckoutBranch.TabIndex = 8;
            this.lblCheckoutBranch.Text = "Checkout Branch";
            // 
            // lblFileSpec
            // 
            this.lblFileSpec.AutoSize = true;
            this.lblFileSpec.Location = new System.Drawing.Point(93, 82);
            this.lblFileSpec.Name = "lblFileSpec";
            this.lblFileSpec.Size = new System.Drawing.Size(48, 13);
            this.lblFileSpec.TabIndex = 9;
            this.lblFileSpec.Text = "FileSpec";
            // 
            // lblSha
            // 
            this.lblSha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSha.AutoSize = true;
            this.lblSha.Location = new System.Drawing.Point(1189, 81);
            this.lblSha.Name = "lblSha";
            this.lblSha.Size = new System.Drawing.Size(26, 13);
            this.lblSha.TabIndex = 11;
            this.lblSha.Text = "Sha";
            // 
            // cboCommandLine
            // 
            this.cboCommandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCommandLine.FormattingEnabled = true;
            this.cboCommandLine.Location = new System.Drawing.Point(12, 24);
            this.cboCommandLine.Name = "cboCommandLine";
            this.cboCommandLine.Size = new System.Drawing.Size(748, 21);
            this.cboCommandLine.TabIndex = 0;
            this.cboCommandLine.SelectedIndexChanged += new System.EventHandler(this.cboCommandLine_SelectedIndexChanged);
            this.cboCommandLine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboCommandLine_KeyUp);
            this.cboCommandLine.Leave += new System.EventHandler(this.cboCommandLine_Leave);
            // 
            // lblCommandTemplate
            // 
            this.lblCommandTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCommandTemplate.AutoSize = true;
            this.lblCommandTemplate.Location = new System.Drawing.Point(763, 9);
            this.lblCommandTemplate.Name = "lblCommandTemplate";
            this.lblCommandTemplate.Size = new System.Drawing.Size(51, 13);
            this.lblCommandTemplate.TabIndex = 14;
            this.lblCommandTemplate.Text = "Template";
            // 
            // cboCommandTemplate
            // 
            this.cboCommandTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCommandTemplate.FormattingEnabled = true;
            this.cboCommandTemplate.Location = new System.Drawing.Point(766, 24);
            this.cboCommandTemplate.Name = "cboCommandTemplate";
            this.cboCommandTemplate.Size = new System.Drawing.Size(789, 21);
            this.cboCommandTemplate.TabIndex = 1;
            this.cboCommandTemplate.SelectedIndexChanged += new System.EventHandler(this.cboCommandTemplate_SelectedIndexChanged);
            this.cboCommandTemplate.Leave += new System.EventHandler(this.cboCommandTemplate_Leave);
            // 
            // lblHead
            // 
            this.lblHead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHead.AutoSize = true;
            this.lblHead.Location = new System.Drawing.Point(766, 82);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(33, 13);
            this.lblHead.TabIndex = 16;
            this.lblHead.Text = "Head";
            // 
            // cboHead
            // 
            this.cboHead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboHead.FormattingEnabled = true;
            this.cboHead.Location = new System.Drawing.Point(805, 79);
            this.cboHead.Name = "cboHead";
            this.cboHead.Size = new System.Drawing.Size(378, 21);
            this.cboHead.TabIndex = 8;
            this.cboHead.SelectedIndexChanged += new System.EventHandler(this.cboHead_SelectedIndexChanged);
            this.cboHead.Leave += new System.EventHandler(this.cboHead_Leave);
            // 
            // cboSha
            // 
            this.cboSha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSha.FormattingEnabled = true;
            this.cboSha.Location = new System.Drawing.Point(1268, 78);
            this.cboSha.Name = "cboSha";
            this.cboSha.Size = new System.Drawing.Size(287, 21);
            this.cboSha.TabIndex = 9;
            this.cboSha.SelectedIndexChanged += new System.EventHandler(this.cboSha_SelectedIndexChanged);
            this.cboSha.Leave += new System.EventHandler(this.cboSha_Leave);
            // 
            // cboFileSpec
            // 
            this.cboFileSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFileSpec.FormattingEnabled = true;
            this.cboFileSpec.Location = new System.Drawing.Point(189, 79);
            this.cboFileSpec.Name = "cboFileSpec";
            this.cboFileSpec.Size = new System.Drawing.Size(571, 21);
            this.cboFileSpec.TabIndex = 7;
            this.cboFileSpec.SelectedIndexChanged += new System.EventHandler(this.cboFileSpec_SelectedIndexChanged);
            this.cboFileSpec.Leave += new System.EventHandler(this.cboFileSpec_Leave);
            // 
            // cboCheckoutBranch
            // 
            this.cboCheckoutBranch.FormattingEnabled = true;
            this.cboCheckoutBranch.Location = new System.Drawing.Point(189, 53);
            this.cboCheckoutBranch.Name = "cboCheckoutBranch";
            this.cboCheckoutBranch.Size = new System.Drawing.Size(484, 21);
            this.cboCheckoutBranch.TabIndex = 3;
            this.cboCheckoutBranch.SelectedIndexChanged += new System.EventHandler(this.cboCheckoutBranch_SelectedIndexChanged);
            this.cboCheckoutBranch.Leave += new System.EventHandler(this.cboCheckoutBranch_Leave);
            // 
            // lblRemoteBranch
            // 
            this.lblRemoteBranch.AutoSize = true;
            this.lblRemoteBranch.Location = new System.Drawing.Point(679, 56);
            this.lblRemoteBranch.Name = "lblRemoteBranch";
            this.lblRemoteBranch.Size = new System.Drawing.Size(81, 13);
            this.lblRemoteBranch.TabIndex = 21;
            this.lblRemoteBranch.Text = "Remote Branch";
            // 
            // cboRemoteBranch
            // 
            this.cboRemoteBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRemoteBranch.FormattingEnabled = true;
            this.cboRemoteBranch.Location = new System.Drawing.Point(766, 53);
            this.cboRemoteBranch.Name = "cboRemoteBranch";
            this.cboRemoteBranch.Size = new System.Drawing.Size(417, 21);
            this.cboRemoteBranch.TabIndex = 4;
            this.cboRemoteBranch.SelectedIndexChanged += new System.EventHandler(this.cboRemoteBranch_SelectedIndexChanged);
            this.cboRemoteBranch.Leave += new System.EventHandler(this.cboRemoteBranch_Leave);
            // 
            // lblLocalBranch
            // 
            this.lblLocalBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocalBranch.AutoSize = true;
            this.lblLocalBranch.Location = new System.Drawing.Point(1189, 56);
            this.lblLocalBranch.Name = "lblLocalBranch";
            this.lblLocalBranch.Size = new System.Drawing.Size(70, 13);
            this.lblLocalBranch.TabIndex = 23;
            this.lblLocalBranch.Text = "Local Branch";
            // 
            // cboLocalBranch
            // 
            this.cboLocalBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLocalBranch.FormattingEnabled = true;
            this.cboLocalBranch.Location = new System.Drawing.Point(1268, 53);
            this.cboLocalBranch.Name = "cboLocalBranch";
            this.cboLocalBranch.Size = new System.Drawing.Size(287, 21);
            this.cboLocalBranch.TabIndex = 5;
            this.cboLocalBranch.SelectedIndexChanged += new System.EventHandler(this.cboLocalBranch_SelectedIndexChanged);
            this.cboLocalBranch.Leave += new System.EventHandler(this.cboLocalBranch_Leave);
            // 
            // rtxJournal
            // 
            this.rtxJournal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxJournal.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxJournal.Location = new System.Drawing.Point(769, 150);
            this.rtxJournal.Name = "rtxJournal";
            this.rtxJournal.Size = new System.Drawing.Size(786, 701);
            this.rtxJournal.TabIndex = 14;
            this.rtxJournal.Text = "";
            // 
            // lblJournal
            // 
            this.lblJournal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblJournal.AutoSize = true;
            this.lblJournal.Location = new System.Drawing.Point(766, 129);
            this.lblJournal.Name = "lblJournal";
            this.lblJournal.Size = new System.Drawing.Size(41, 13);
            this.lblJournal.TabIndex = 26;
            this.lblJournal.Text = "Journal";
            // 
            // lblComment
            // 
            this.lblComment.AutoSize = true;
            this.lblComment.Location = new System.Drawing.Point(93, 108);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(51, 13);
            this.lblComment.TabIndex = 27;
            this.lblComment.Text = "Comment";
            // 
            // cboComment
            // 
            this.cboComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboComment.FormattingEnabled = true;
            this.cboComment.Location = new System.Drawing.Point(189, 105);
            this.cboComment.Name = "cboComment";
            this.cboComment.Size = new System.Drawing.Size(571, 21);
            this.cboComment.TabIndex = 11;
            this.cboComment.SelectedIndexChanged += new System.EventHandler(this.cboComment_SelectedIndexChanged);
            this.cboComment.Leave += new System.EventHandler(this.cboComment_Leave);
            // 
            // lblStash
            // 
            this.lblStash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStash.AutoSize = true;
            this.lblStash.Location = new System.Drawing.Point(766, 108);
            this.lblStash.Name = "lblStash";
            this.lblStash.Size = new System.Drawing.Size(34, 13);
            this.lblStash.TabIndex = 29;
            this.lblStash.Text = "Stash";
            // 
            // cboStash
            // 
            this.cboStash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStash.FormattingEnabled = true;
            this.cboStash.Location = new System.Drawing.Point(806, 105);
            this.cboStash.Name = "cboStash";
            this.cboStash.Size = new System.Drawing.Size(749, 21);
            this.cboStash.TabIndex = 12;
            this.cboStash.SelectedIndexChanged += new System.EventHandler(this.cboStash_SelectedIndexChanged);
            this.cboStash.Leave += new System.EventHandler(this.cboStash_Leave);
            // 
            // cmdSettings
            // 
            this.cmdSettings.Location = new System.Drawing.Point(12, 103);
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.Size = new System.Drawing.Size(75, 23);
            this.cmdSettings.TabIndex = 10;
            this.cmdSettings.Text = "Settings";
            this.cmdSettings.UseVisualStyleBackColor = true;
            this.cmdSettings.Click += new System.EventHandler(this.cmdSettings_Click);
            // 
            // cmdGitStatus
            // 
            this.cmdGitStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGitStatus.Location = new System.Drawing.Point(12, 864);
            this.cmdGitStatus.Name = "cmdGitStatus";
            this.cmdGitStatus.Size = new System.Drawing.Size(75, 23);
            this.cmdGitStatus.TabIndex = 30;
            this.cmdGitStatus.Text = "Git Status";
            this.cmdGitStatus.UseVisualStyleBackColor = true;
            this.cmdGitStatus.Click += new System.EventHandler(this.cmdGitStatus_Click);
            // 
            // cmdGitLog
            // 
            this.cmdGitLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGitLog.Location = new System.Drawing.Point(93, 864);
            this.cmdGitLog.Name = "cmdGitLog";
            this.cmdGitLog.Size = new System.Drawing.Size(75, 23);
            this.cmdGitLog.TabIndex = 31;
            this.cmdGitLog.Text = "Git Log";
            this.cmdGitLog.UseVisualStyleBackColor = true;
            this.cmdGitLog.Click += new System.EventHandler(this.cmdGitLog_Click);
            // 
            // cmdParse
            // 
            this.cmdParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdParse.Location = new System.Drawing.Point(174, 864);
            this.cmdParse.Name = "cmdParse";
            this.cmdParse.Size = new System.Drawing.Size(75, 23);
            this.cmdParse.TabIndex = 32;
            this.cmdParse.Text = "Parse";
            this.cmdParse.UseVisualStyleBackColor = true;
            this.cmdParse.Click += new System.EventHandler(this.cmdParse_Click);
            // 
            // cmdGitDiff
            // 
            this.cmdGitDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGitDiff.Location = new System.Drawing.Point(255, 864);
            this.cmdGitDiff.Name = "cmdGitDiff";
            this.cmdGitDiff.Size = new System.Drawing.Size(75, 23);
            this.cmdGitDiff.TabIndex = 33;
            this.cmdGitDiff.Text = "Git Diff";
            this.cmdGitDiff.UseVisualStyleBackColor = true;
            this.cmdGitDiff.Click += new System.EventHandler(this.cmdGitDiff_Click);
            // 
            // cmdGitAdd
            // 
            this.cmdGitAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGitAdd.Location = new System.Drawing.Point(336, 864);
            this.cmdGitAdd.Name = "cmdGitAdd";
            this.cmdGitAdd.Size = new System.Drawing.Size(75, 23);
            this.cmdGitAdd.TabIndex = 34;
            this.cmdGitAdd.Text = "Git Add";
            this.cmdGitAdd.UseVisualStyleBackColor = true;
            this.cmdGitAdd.Click += new System.EventHandler(this.cmdGitAdd_Click);
            // 
            // cmdGitLogFilter
            // 
            this.cmdGitLogFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGitLogFilter.Location = new System.Drawing.Point(417, 864);
            this.cmdGitLogFilter.Name = "cmdGitLogFilter";
            this.cmdGitLogFilter.Size = new System.Drawing.Size(75, 23);
            this.cmdGitLogFilter.TabIndex = 35;
            this.cmdGitLogFilter.Text = "Git Log Filter";
            this.cmdGitLogFilter.UseVisualStyleBackColor = true;
            this.cmdGitLogFilter.Click += new System.EventHandler(this.cmdGitLogFilter_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1567, 899);
            this.Controls.Add(this.cmdGitLogFilter);
            this.Controls.Add(this.cmdGitAdd);
            this.Controls.Add(this.cmdGitDiff);
            this.Controls.Add(this.cmdParse);
            this.Controls.Add(this.cmdGitLog);
            this.Controls.Add(this.cmdGitStatus);
            this.Controls.Add(this.cmdSettings);
            this.Controls.Add(this.cboStash);
            this.Controls.Add(this.lblStash);
            this.Controls.Add(this.cboComment);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.lblJournal);
            this.Controls.Add(this.rtxJournal);
            this.Controls.Add(this.cboLocalBranch);
            this.Controls.Add(this.lblLocalBranch);
            this.Controls.Add(this.cboRemoteBranch);
            this.Controls.Add(this.lblRemoteBranch);
            this.Controls.Add(this.cboCheckoutBranch);
            this.Controls.Add(this.cboFileSpec);
            this.Controls.Add(this.cboSha);
            this.Controls.Add(this.cboHead);
            this.Controls.Add(this.lblHead);
            this.Controls.Add(this.cboCommandTemplate);
            this.Controls.Add(this.lblCommandTemplate);
            this.Controls.Add(this.cboCommandLine);
            this.Controls.Add(this.lblSha);
            this.Controls.Add(this.lblFileSpec);
            this.Controls.Add(this.lblCheckoutBranch);
            this.Controls.Add(this.cmdClear);
            this.Controls.Add(this.rtxOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.cmdRun);
            this.Controls.Add(this.lblCommandLine);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "Commander";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblCommandLine;
        private System.Windows.Forms.Button cmdRun;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.RichTextBox rtxOutput;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.Label lblCheckoutBranch;
        private System.Windows.Forms.Label lblFileSpec;
        private System.Windows.Forms.Label lblSha;
        private System.Windows.Forms.ComboBox cboCommandLine;
        private System.Windows.Forms.Label lblCommandTemplate;
        private System.Windows.Forms.ComboBox cboCommandTemplate;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.ComboBox cboHead;
        private System.Windows.Forms.ComboBox cboSha;
        private System.Windows.Forms.ComboBox cboFileSpec;
        private System.Windows.Forms.ComboBox cboCheckoutBranch;
        private System.Windows.Forms.Label lblRemoteBranch;
        private System.Windows.Forms.ComboBox cboRemoteBranch;
        private System.Windows.Forms.Label lblLocalBranch;
        private System.Windows.Forms.ComboBox cboLocalBranch;
        private System.Windows.Forms.RichTextBox rtxJournal;
        private System.Windows.Forms.Label lblJournal;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.ComboBox cboComment;
        private System.Windows.Forms.Label lblStash;
        private System.Windows.Forms.ComboBox cboStash;
        private System.Windows.Forms.Button cmdSettings;
        private System.Windows.Forms.Button cmdGitStatus;
        private System.Windows.Forms.Button cmdGitLog;
        private System.Windows.Forms.Button cmdParse;
        private System.Windows.Forms.Button cmdGitDiff;
        private System.Windows.Forms.Button cmdGitAdd;
        private System.Windows.Forms.Button cmdGitLogFilter;
    }
}

