namespace Search8.Views
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
            this.chkWinAll = new System.Windows.Forms.CheckBox();
            this.grpWindows = new System.Windows.Forms.GroupBox();
            this.chkKotlin = new System.Windows.Forms.CheckBox();
            this.chkJava = new System.Windows.Forms.CheckBox();
            this.chkSql = new System.Windows.Forms.CheckBox();
            this.chkVb = new System.Windows.Forms.CheckBox();
            this.chkCs = new System.Windows.Forms.CheckBox();
            this.grpWeb = new System.Windows.Forms.GroupBox();
            this.chkCss = new System.Windows.Forms.CheckBox();
            this.chkXml = new System.Windows.Forms.CheckBox();
            this.chkAsp = new System.Windows.Forms.CheckBox();
            this.chkHtm = new System.Windows.Forms.CheckBox();
            this.chkWebAll = new System.Windows.Forms.CheckBox();
            this.grpInclude = new System.Windows.Forms.GroupBox();
            this.chkAllTypes = new System.Windows.Forms.CheckBox();
            this.grpExecutables = new System.Windows.Forms.GroupBox();
            this.chkDll = new System.Windows.Forms.CheckBox();
            this.chkExe = new System.Windows.Forms.CheckBox();
            this.chkExeAll = new System.Windows.Forms.CheckBox();
            this.grpOffice = new System.Windows.Forms.GroupBox();
            this.chkOffAll = new System.Windows.Forms.CheckBox();
            this.chkXls = new System.Windows.Forms.CheckBox();
            this.chkDoc = new System.Windows.Forms.CheckBox();
            this.chkTxt = new System.Windows.Forms.CheckBox();
            this.grpEncoding = new System.Windows.Forms.GroupBox();
            this.chkUnicode = new System.Windows.Forms.CheckBox();
            this.chkAscii = new System.Windows.Forms.CheckBox();
            this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.txtCriteria = new System.Windows.Forms.TextBox();
            this.lblCriteria = new System.Windows.Forms.Label();
            this.lblResults = new System.Windows.Forms.Label();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.pbrPprogressBar = new System.Windows.Forms.ProgressBar();
            this.staStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.lblFilePattern = new System.Windows.Forms.Label();
            this.txtFilePattern = new System.Windows.Forms.TextBox();
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.txtExcludeDirectories = new System.Windows.Forms.TextBox();
            this.lblExcludeDirectories = new System.Windows.Forms.Label();
            this.chkMethodBlocks = new System.Windows.Forms.CheckBox();
            this.grpWindows.SuspendLayout();
            this.grpWeb.SuspendLayout();
            this.grpInclude.SuspendLayout();
            this.grpExecutables.SuspendLayout();
            this.grpOffice.SuspendLayout();
            this.grpEncoding.SuspendLayout();
            this.staStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkWinAll
            // 
            this.chkWinAll.AutoSize = true;
            this.chkWinAll.Location = new System.Drawing.Point(24, 19);
            this.chkWinAll.Name = "chkWinAll";
            this.chkWinAll.Size = new System.Drawing.Size(37, 17);
            this.chkWinAll.TabIndex = 0;
            this.chkWinAll.Text = "All";
            this.chkWinAll.UseVisualStyleBackColor = true;
            this.chkWinAll.CheckedChanged += new System.EventHandler(this.chkWinAll_CheckedChanged);
            // 
            // grpWindows
            // 
            this.grpWindows.Controls.Add(this.chkKotlin);
            this.grpWindows.Controls.Add(this.chkJava);
            this.grpWindows.Controls.Add(this.chkSql);
            this.grpWindows.Controls.Add(this.chkVb);
            this.grpWindows.Controls.Add(this.chkCs);
            this.grpWindows.Controls.Add(this.chkWinAll);
            this.grpWindows.Location = new System.Drawing.Point(17, 42);
            this.grpWindows.Name = "grpWindows";
            this.grpWindows.Size = new System.Drawing.Size(92, 158);
            this.grpWindows.TabIndex = 1;
            this.grpWindows.TabStop = false;
            this.grpWindows.Text = "Windows";
            // 
            // chkKotlin
            // 
            this.chkKotlin.AutoSize = true;
            this.chkKotlin.Location = new System.Drawing.Point(24, 87);
            this.chkKotlin.Name = "chkKotlin";
            this.chkKotlin.Size = new System.Drawing.Size(35, 17);
            this.chkKotlin.TabIndex = 5;
            this.chkKotlin.Text = "kt";
            this.chkKotlin.UseVisualStyleBackColor = true;
            // 
            // chkJava
            // 
            this.chkJava.AutoSize = true;
            this.chkJava.Location = new System.Drawing.Point(24, 65);
            this.chkJava.Name = "chkJava";
            this.chkJava.Size = new System.Drawing.Size(46, 17);
            this.chkJava.TabIndex = 4;
            this.chkJava.Text = "java";
            this.chkJava.UseVisualStyleBackColor = true;
            // 
            // chkSql
            // 
            this.chkSql.AutoSize = true;
            this.chkSql.Location = new System.Drawing.Point(24, 134);
            this.chkSql.Name = "chkSql";
            this.chkSql.Size = new System.Drawing.Size(39, 17);
            this.chkSql.TabIndex = 7;
            this.chkSql.Text = "sql";
            this.chkSql.UseVisualStyleBackColor = true;
            // 
            // chkVb
            // 
            this.chkVb.AutoSize = true;
            this.chkVb.Location = new System.Drawing.Point(24, 110);
            this.chkVb.Name = "chkVb";
            this.chkVb.Size = new System.Drawing.Size(38, 17);
            this.chkVb.TabIndex = 6;
            this.chkVb.Text = "vb";
            this.chkVb.UseVisualStyleBackColor = true;
            this.chkVb.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkCs
            // 
            this.chkCs.AutoSize = true;
            this.chkCs.Location = new System.Drawing.Point(24, 42);
            this.chkCs.Name = "chkCs";
            this.chkCs.Size = new System.Drawing.Size(37, 17);
            this.chkCs.TabIndex = 1;
            this.chkCs.Text = "cs";
            this.chkCs.UseVisualStyleBackColor = true;
            this.chkCs.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // grpWeb
            // 
            this.grpWeb.Controls.Add(this.chkCss);
            this.grpWeb.Controls.Add(this.chkXml);
            this.grpWeb.Controls.Add(this.chkAsp);
            this.grpWeb.Controls.Add(this.chkHtm);
            this.grpWeb.Controls.Add(this.chkWebAll);
            this.grpWeb.Location = new System.Drawing.Point(115, 42);
            this.grpWeb.Name = "grpWeb";
            this.grpWeb.Size = new System.Drawing.Size(92, 158);
            this.grpWeb.TabIndex = 2;
            this.grpWeb.TabStop = false;
            this.grpWeb.Text = "Web";
            // 
            // chkCss
            // 
            this.chkCss.AutoSize = true;
            this.chkCss.Location = new System.Drawing.Point(25, 111);
            this.chkCss.Name = "chkCss";
            this.chkCss.Size = new System.Drawing.Size(42, 17);
            this.chkCss.TabIndex = 4;
            this.chkCss.Text = "css";
            this.chkCss.UseVisualStyleBackColor = true;
            this.chkCss.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkXml
            // 
            this.chkXml.AutoSize = true;
            this.chkXml.Location = new System.Drawing.Point(25, 88);
            this.chkXml.Name = "chkXml";
            this.chkXml.Size = new System.Drawing.Size(41, 17);
            this.chkXml.TabIndex = 3;
            this.chkXml.Text = "xml";
            this.chkXml.UseVisualStyleBackColor = true;
            this.chkXml.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkAsp
            // 
            this.chkAsp.AutoSize = true;
            this.chkAsp.Location = new System.Drawing.Point(24, 65);
            this.chkAsp.Name = "chkAsp";
            this.chkAsp.Size = new System.Drawing.Size(43, 17);
            this.chkAsp.TabIndex = 2;
            this.chkAsp.Text = "asp";
            this.chkAsp.UseVisualStyleBackColor = true;
            this.chkAsp.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkHtm
            // 
            this.chkHtm.AutoSize = true;
            this.chkHtm.Location = new System.Drawing.Point(24, 42);
            this.chkHtm.Name = "chkHtm";
            this.chkHtm.Size = new System.Drawing.Size(43, 17);
            this.chkHtm.TabIndex = 1;
            this.chkHtm.Text = "htm";
            this.chkHtm.UseVisualStyleBackColor = true;
            this.chkHtm.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkWebAll
            // 
            this.chkWebAll.AutoSize = true;
            this.chkWebAll.Location = new System.Drawing.Point(24, 19);
            this.chkWebAll.Name = "chkWebAll";
            this.chkWebAll.Size = new System.Drawing.Size(37, 17);
            this.chkWebAll.TabIndex = 0;
            this.chkWebAll.Text = "All";
            this.chkWebAll.UseVisualStyleBackColor = true;
            this.chkWebAll.CheckedChanged += new System.EventHandler(this.chkWebAll_CheckedChanged);
            // 
            // grpInclude
            // 
            this.grpInclude.Controls.Add(this.chkAllTypes);
            this.grpInclude.Controls.Add(this.grpExecutables);
            this.grpInclude.Controls.Add(this.grpOffice);
            this.grpInclude.Controls.Add(this.grpWindows);
            this.grpInclude.Controls.Add(this.grpWeb);
            this.grpInclude.Location = new System.Drawing.Point(12, 3);
            this.grpInclude.Name = "grpInclude";
            this.grpInclude.Size = new System.Drawing.Size(421, 217);
            this.grpInclude.TabIndex = 7;
            this.grpInclude.TabStop = false;
            this.grpInclude.Text = "Include";
            // 
            // chkAllTypes
            // 
            this.chkAllTypes.AutoSize = true;
            this.chkAllTypes.Location = new System.Drawing.Point(17, 19);
            this.chkAllTypes.Name = "chkAllTypes";
            this.chkAllTypes.Size = new System.Drawing.Size(65, 17);
            this.chkAllTypes.TabIndex = 0;
            this.chkAllTypes.Text = "All types";
            this.chkAllTypes.UseVisualStyleBackColor = true;
            this.chkAllTypes.CheckedChanged += new System.EventHandler(this.chkAllTypes_CheckedChanged);
            // 
            // grpExecutables
            // 
            this.grpExecutables.Controls.Add(this.chkDll);
            this.grpExecutables.Controls.Add(this.chkExe);
            this.grpExecutables.Controls.Add(this.chkExeAll);
            this.grpExecutables.Location = new System.Drawing.Point(311, 42);
            this.grpExecutables.Name = "grpExecutables";
            this.grpExecutables.Size = new System.Drawing.Size(92, 158);
            this.grpExecutables.TabIndex = 4;
            this.grpExecutables.TabStop = false;
            this.grpExecutables.Text = "Executables";
            // 
            // chkDll
            // 
            this.chkDll.AutoSize = true;
            this.chkDll.Location = new System.Drawing.Point(24, 65);
            this.chkDll.Name = "chkDll";
            this.chkDll.Size = new System.Drawing.Size(36, 17);
            this.chkDll.TabIndex = 2;
            this.chkDll.Text = "dll";
            this.chkDll.UseVisualStyleBackColor = true;
            this.chkDll.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkExe
            // 
            this.chkExe.AutoSize = true;
            this.chkExe.Location = new System.Drawing.Point(24, 42);
            this.chkExe.Name = "chkExe";
            this.chkExe.Size = new System.Drawing.Size(43, 17);
            this.chkExe.TabIndex = 1;
            this.chkExe.Text = "exe";
            this.chkExe.UseVisualStyleBackColor = true;
            this.chkExe.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkExeAll
            // 
            this.chkExeAll.AutoSize = true;
            this.chkExeAll.Location = new System.Drawing.Point(24, 19);
            this.chkExeAll.Name = "chkExeAll";
            this.chkExeAll.Size = new System.Drawing.Size(37, 17);
            this.chkExeAll.TabIndex = 0;
            this.chkExeAll.Text = "All";
            this.chkExeAll.UseVisualStyleBackColor = true;
            this.chkExeAll.CheckedChanged += new System.EventHandler(this.chkExeAll_CheckedChanged);
            // 
            // grpOffice
            // 
            this.grpOffice.Controls.Add(this.chkOffAll);
            this.grpOffice.Controls.Add(this.chkXls);
            this.grpOffice.Controls.Add(this.chkDoc);
            this.grpOffice.Controls.Add(this.chkTxt);
            this.grpOffice.Location = new System.Drawing.Point(213, 42);
            this.grpOffice.Name = "grpOffice";
            this.grpOffice.Size = new System.Drawing.Size(92, 158);
            this.grpOffice.TabIndex = 3;
            this.grpOffice.TabStop = false;
            this.grpOffice.Text = "Office";
            // 
            // chkOffAll
            // 
            this.chkOffAll.AutoSize = true;
            this.chkOffAll.Location = new System.Drawing.Point(24, 19);
            this.chkOffAll.Name = "chkOffAll";
            this.chkOffAll.Size = new System.Drawing.Size(37, 17);
            this.chkOffAll.TabIndex = 0;
            this.chkOffAll.Text = "All";
            this.chkOffAll.UseVisualStyleBackColor = true;
            this.chkOffAll.CheckedChanged += new System.EventHandler(this.chkOffAll_CheckedChanged);
            // 
            // chkXls
            // 
            this.chkXls.AutoSize = true;
            this.chkXls.Location = new System.Drawing.Point(23, 88);
            this.chkXls.Name = "chkXls";
            this.chkXls.Size = new System.Drawing.Size(38, 17);
            this.chkXls.TabIndex = 3;
            this.chkXls.Text = "xls";
            this.chkXls.UseVisualStyleBackColor = true;
            this.chkXls.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkDoc
            // 
            this.chkDoc.AutoSize = true;
            this.chkDoc.Location = new System.Drawing.Point(23, 65);
            this.chkDoc.Name = "chkDoc";
            this.chkDoc.Size = new System.Drawing.Size(44, 17);
            this.chkDoc.TabIndex = 2;
            this.chkDoc.Text = "doc";
            this.chkDoc.UseVisualStyleBackColor = true;
            this.chkDoc.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // chkTxt
            // 
            this.chkTxt.AutoSize = true;
            this.chkTxt.Location = new System.Drawing.Point(24, 42);
            this.chkTxt.Name = "chkTxt";
            this.chkTxt.Size = new System.Drawing.Size(37, 17);
            this.chkTxt.TabIndex = 1;
            this.chkTxt.Text = "txt";
            this.chkTxt.UseVisualStyleBackColor = true;
            this.chkTxt.CheckedChanged += new System.EventHandler(this.chkAllChecked_CheckedChanged);
            // 
            // grpEncoding
            // 
            this.grpEncoding.Controls.Add(this.chkUnicode);
            this.grpEncoding.Controls.Add(this.chkAscii);
            this.grpEncoding.Location = new System.Drawing.Point(450, 22);
            this.grpEncoding.Name = "grpEncoding";
            this.grpEncoding.Size = new System.Drawing.Size(98, 82);
            this.grpEncoding.TabIndex = 8;
            this.grpEncoding.TabStop = false;
            this.grpEncoding.Text = "Encoding";
            // 
            // chkUnicode
            // 
            this.chkUnicode.AutoSize = true;
            this.chkUnicode.Location = new System.Drawing.Point(16, 42);
            this.chkUnicode.Name = "chkUnicode";
            this.chkUnicode.Size = new System.Drawing.Size(66, 17);
            this.chkUnicode.TabIndex = 1;
            this.chkUnicode.Text = "Unicode";
            this.chkUnicode.UseVisualStyleBackColor = true;
            // 
            // chkAscii
            // 
            this.chkAscii.AutoSize = true;
            this.chkAscii.Location = new System.Drawing.Point(16, 19);
            this.chkAscii.Name = "chkAscii";
            this.chkAscii.Size = new System.Drawing.Size(48, 17);
            this.chkAscii.TabIndex = 0;
            this.chkAscii.Text = "Ascii";
            this.chkAscii.UseVisualStyleBackColor = true;
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(12, 239);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(412, 20);
            this.txtPath.TabIndex = 0;
            this.txtPath.Text = "C:\\_\\Main\\D";
            this.txtPath.DoubleClick += new System.EventHandler(this.txtPath_DoubleClick);
            // 
            // lblPath
            // 
            this.lblPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(9, 223);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(29, 13);
            this.lblPath.TabIndex = 9;
            this.lblPath.Text = "Path";
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResults.Location = new System.Drawing.Point(12, 317);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(598, 185);
            this.txtResults.TabIndex = 2;
            // 
            // txtCriteria
            // 
            this.txtCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCriteria.Location = new System.Drawing.Point(12, 278);
            this.txtCriteria.Name = "txtCriteria";
            this.txtCriteria.Size = new System.Drawing.Size(598, 20);
            this.txtCriteria.TabIndex = 2;
            // 
            // lblCriteria
            // 
            this.lblCriteria.AutoSize = true;
            this.lblCriteria.Location = new System.Drawing.Point(9, 262);
            this.lblCriteria.Name = "lblCriteria";
            this.lblCriteria.Size = new System.Drawing.Size(39, 13);
            this.lblCriteria.TabIndex = 1;
            this.lblCriteria.Text = "Criteria";
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(9, 301);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(42, 13);
            this.lblResults.TabIndex = 3;
            this.lblResults.Text = "Results";
            // 
            // cmdSearch
            // 
            this.cmdSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSearch.Location = new System.Drawing.Point(428, 508);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(56, 24);
            this.cmdSearch.TabIndex = 3;
            this.cmdSearch.Text = "Search";
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(552, 508);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(58, 24);
            this.cmdClose.TabIndex = 5;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // pbrPprogressBar
            // 
            this.pbrPprogressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrPprogressBar.Location = new System.Drawing.Point(12, 508);
            this.pbrPprogressBar.Name = "pbrPprogressBar";
            this.pbrPprogressBar.Size = new System.Drawing.Size(412, 24);
            this.pbrPprogressBar.TabIndex = 10;
            // 
            // staStatusStrip
            // 
            this.staStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.staStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.staStatusStrip.Location = new System.Drawing.Point(0, 540);
            this.staStatusStrip.Name = "staStatusStrip";
            this.staStatusStrip.Size = new System.Drawing.Size(622, 26);
            this.staStatusStrip.TabIndex = 11;
            this.staStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(428, 21);
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.RightToLeftLayout = true;
            this.toolStripProgressBar.Size = new System.Drawing.Size(172, 20);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(490, 508);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(56, 24);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // lblFilePattern
            // 
            this.lblFilePattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilePattern.AutoSize = true;
            this.lblFilePattern.Location = new System.Drawing.Point(427, 223);
            this.lblFilePattern.Name = "lblFilePattern";
            this.lblFilePattern.Size = new System.Drawing.Size(59, 13);
            this.lblFilePattern.TabIndex = 12;
            this.lblFilePattern.Text = "File pattern";
            // 
            // txtFilePattern
            // 
            this.txtFilePattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePattern.Location = new System.Drawing.Point(430, 239);
            this.txtFilePattern.Name = "txtFilePattern";
            this.txtFilePattern.Size = new System.Drawing.Size(180, 20);
            this.txtFilePattern.TabIndex = 1;
            this.txtFilePattern.Text = "C:\\_\\Main\\D";
            // 
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(466, 127);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(92, 17);
            this.chkRegex.TabIndex = 13;
            this.chkRegex.Text = "Regex Criteria";
            this.chkRegex.UseVisualStyleBackColor = true;
            // 
            // txtExcludeDirectories
            // 
            this.txtExcludeDirectories.Location = new System.Drawing.Point(439, 163);
            this.txtExcludeDirectories.Name = "txtExcludeDirectories";
            this.txtExcludeDirectories.Size = new System.Drawing.Size(171, 20);
            this.txtExcludeDirectories.TabIndex = 14;
            // 
            // lblExcludeDirectories
            // 
            this.lblExcludeDirectories.AutoSize = true;
            this.lblExcludeDirectories.Location = new System.Drawing.Point(439, 147);
            this.lblExcludeDirectories.Name = "lblExcludeDirectories";
            this.lblExcludeDirectories.Size = new System.Drawing.Size(151, 13);
            this.lblExcludeDirectories.TabIndex = 15;
            this.lblExcludeDirectories.Text = "Exclude Directories Containing";
            // 
            // chkMethodBlocks
            // 
            this.chkMethodBlocks.AutoSize = true;
            this.chkMethodBlocks.Location = new System.Drawing.Point(466, 189);
            this.chkMethodBlocks.Name = "chkMethodBlocks";
            this.chkMethodBlocks.Size = new System.Drawing.Size(97, 17);
            this.chkMethodBlocks.TabIndex = 15;
            this.chkMethodBlocks.Text = "Method Blocks";
            this.chkMethodBlocks.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 566);
            this.Controls.Add(this.chkMethodBlocks);
            this.Controls.Add(this.lblExcludeDirectories);
            this.Controls.Add(this.txtExcludeDirectories);
            this.Controls.Add(this.chkRegex);
            this.Controls.Add(this.txtFilePattern);
            this.Controls.Add(this.lblFilePattern);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.staStatusStrip);
            this.Controls.Add(this.pbrPprogressBar);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdSearch);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.lblCriteria);
            this.Controls.Add(this.txtCriteria);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.grpEncoding);
            this.Controls.Add(this.grpInclude);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(638, 502);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search8";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpWindows.ResumeLayout(false);
            this.grpWindows.PerformLayout();
            this.grpWeb.ResumeLayout(false);
            this.grpWeb.PerformLayout();
            this.grpInclude.ResumeLayout(false);
            this.grpInclude.PerformLayout();
            this.grpExecutables.ResumeLayout(false);
            this.grpExecutables.PerformLayout();
            this.grpOffice.ResumeLayout(false);
            this.grpOffice.PerformLayout();
            this.grpEncoding.ResumeLayout(false);
            this.grpEncoding.PerformLayout();
            this.staStatusStrip.ResumeLayout(false);
            this.staStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkWinAll;
        private System.Windows.Forms.GroupBox grpWindows;
        private System.Windows.Forms.CheckBox chkVb;
        private System.Windows.Forms.CheckBox chkCs;
        private System.Windows.Forms.GroupBox grpWeb;
        private System.Windows.Forms.CheckBox chkAsp;
        private System.Windows.Forms.CheckBox chkHtm;
        private System.Windows.Forms.CheckBox chkWebAll;
        private System.Windows.Forms.GroupBox grpInclude;
        private System.Windows.Forms.GroupBox grpExecutables;
        private System.Windows.Forms.CheckBox chkDll;
        private System.Windows.Forms.CheckBox chkExe;
        private System.Windows.Forms.CheckBox chkExeAll;
        private System.Windows.Forms.GroupBox grpOffice;
        private System.Windows.Forms.CheckBox chkXls;
        private System.Windows.Forms.CheckBox chkDoc;
        private System.Windows.Forms.CheckBox chkTxt;
        private System.Windows.Forms.CheckBox chkOffAll;
        private System.Windows.Forms.GroupBox grpEncoding;
        private System.Windows.Forms.CheckBox chkUnicode;
        private System.Windows.Forms.CheckBox chkAscii;
        private System.Windows.Forms.FolderBrowserDialog dlgFolder;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtCriteria;
        private System.Windows.Forms.Label lblCriteria;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.Button cmdSearch;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ProgressBar pbrPprogressBar;
        private System.Windows.Forms.StatusStrip staStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.CheckBox chkXml;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label lblFilePattern;
        private System.Windows.Forms.TextBox txtFilePattern;
        private System.Windows.Forms.CheckBox chkRegex;
		private System.Windows.Forms.CheckBox chkCss;
		private System.Windows.Forms.CheckBox chkAllTypes;
        private System.Windows.Forms.CheckBox chkSql;
        private System.Windows.Forms.CheckBox chkJava;
        private System.Windows.Forms.TextBox txtExcludeDirectories;
        private System.Windows.Forms.Label lblExcludeDirectories;
        private System.Windows.Forms.CheckBox chkKotlin;
        private System.Windows.Forms.CheckBox chkMethodBlocks;
    }
}

