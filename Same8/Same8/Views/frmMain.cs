using Same8.DataLayer.Profile;
using Same8.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using R = Routines8.Routines;

namespace Same8.Views
{
    /// <summary>
    /// Main form.
    /// </summary>
    /// <remarks>
    /// Main form.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public partial class frmMain : Form
    {
        #region Member variables.
        private CompareEngine moCompare;
        private ListViewItem moListViewItem;
        private string _currentKey = string.Empty;
        public long _newFilesEstimate = 0;
        public long _oldFilesEstimate = 0;
        public long _chgFilesEstimate = 0;
        private bool _monitoredTypesOnly = true;
        #endregion

        #region Properties.
        #region Previous Selection Properties.
        public bool PreviousSelections_IndexChanged_Event_On
        {
            set
            {
                bool eventOn = value;
                if (eventOn)
                {
                    cboPreviousSelections.SelectedIndexChanged += new System.EventHandler(this.cboPreviousSelections_SelectedIndexChanged);
                }
                else
                {
                    cboPreviousSelections.SelectedIndexChanged -= new System.EventHandler(this.cboPreviousSelections_SelectedIndexChanged);
                }
            }
        }

        public object PreviousSelections_DataSource
        {
            set
            {
                cboPreviousSelections.DataSource = value;
            }
        }

        public string PreviousSelections_DisplayMember
        {
            set
            {
                cboPreviousSelections.DisplayMember = value;
            }
        }

        public string PreviousSelections_SelectedString
        {
            set
            {
                try
                {
                    cboPreviousSelections.SelectedIndex = cboPreviousSelections.FindString(value);
                }
                catch
                {
                }
            }
        }

        public UserSetting PreviousSelections_SelectedItem
        {
            get
            {
                return (UserSetting)cboPreviousSelections.SelectedItem;
            }
        }

        public string PreviousSelections_Text
        {
            get
            {
                return cboPreviousSelections.Text;
            }
            set
            {
                cboPreviousSelections.Text = value;
                Application.DoEvents();
            }
        }

        public int PreviousSelections_Count
        {
            get
            {
                return cboPreviousSelections.Items.Count;
            }
        }
        #endregion

        public string NewPath
        {
            get
            {
                return txtNewPath.Text;
            }
            set
            {
                txtNewPath.Text = value;
                txtNewPath.SelectionStart = txtNewPath.Text.Length;
                txtNewPath.ScrollToCaret();
            }
        }

        public string OldPath
        {
            get
            {
                return txtOldPath.Text;
            }
            set
            {
                txtOldPath.Text = value;
                txtOldPath.SelectionStart = txtOldPath.Text.Length;
                txtOldPath.ScrollToCaret();
            }
        }

        public bool IgnoreFileExtension
        {
            get
            {
                return chkIgnoreFileExtension.Checked;
            }
            set
            {
                chkIgnoreFileExtension.Checked = value;
            }
        }

        public string Caption
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public string StatusMessage
        {
            get
            {
                return toolStripStatusLabel.Text;
            }
            set
            {
                toolStripStatusLabel.Text = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Main form constructor.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers.
        /// <summary>
        /// Form load.
        /// </summary>
        private void frmMain_Load(object sender, EventArgs e)
        {
            cmdGo.Enabled = true;
            cmdCancel.Enabled = false;
            lblIdentical.Text = new string(' ', 23);
            lblIdentical.BackColor = Color.LightGray;
            ColumnHeaders();
            LoadParameters();
            RefreshCombo();
            frmMain_Resize(sender, e);
        }

        /// <summary>
        /// Form resize.
        /// </summary>
        private void frmMain_Resize(object sender, EventArgs e)
        {
            const int TGAP = 57;
            const int SGAP = 5;
            const int HGAP = 8;
            const int VGAP = 6;
            lvwResults.Columns[0].Width = 10 * (int)lvwResults.Font.SizeInPoints;
            lvwResults.Columns[1].Width = (int)((lvwResults.Width - lvwResults.Columns[0].Width - (HGAP * 2.75)) / 2);
            lvwResults.Columns[2].Width = (int)((lvwResults.Width - lvwResults.Columns[0].Width - (HGAP * 2.75)) / 2);
            //staStatusStrip.Left = 0;
            //staStatusStrip.Width = pbrPprogressBar.Width + cmdCancel.Width + cmdCancel.Width + cmdGo.Width + (HGAP * 5);
            staStatusStrip.Items[0].Width = pbrPprogressBar.Width + (int)(HGAP * 1.75);
            staStatusStrip.Items[1].Width = cmdCancel.Width + cmdCancel.Width + cmdGo.Width + (HGAP * 5);
        }

        /// <summary>
        /// Start file progress indicator.
        /// </summary>
        void moCompare_EventBeginProgress(object poSender, CompareEngine.EventParameters poEventArgs)
        {
            Cursor.Current = Cursors.WaitCursor;
            cmdGo.Enabled = false;
            cmdCancel.Enabled = true;
            int nMaximum = poEventArgs.Number;
            string sMessage = poEventArgs.Text;
            staStatusStrip.Text = sMessage;
            toolStripStatusLabel.Text = sMessage;
            toolStripProgressBar.Minimum = 0;
            toolStripProgressBar.Maximum = nMaximum;
            toolStripProgressBar.Value = 0;
            pbrPprogressBar.Minimum = 0;
            pbrPprogressBar.Maximum = nMaximum;
            pbrPprogressBar.Value = 0;
            Application.DoEvents();
        }

        /// <summary>
        /// Update file progress indicator.
        /// </summary>
        void moCompare_EventUpdateProgress(object poSender, CompareEngine.EventParameters poEventArgs)
        {
            if (Cursor.Current != Cursors.WaitCursor)
            {
                Cursor.Current = Cursors.WaitCursor;
            }
            int nIncrement = poEventArgs.Number;
            string sMessage = poEventArgs.Text;
            staStatusStrip.Text = sMessage;
            toolStripStatusLabel.Text = sMessage;
            try
            {
                if (pbrPprogressBar.Value + nIncrement > pbrPprogressBar.Maximum)
                {
                    pbrPprogressBar.Maximum += Math.Max(pbrPprogressBar.Maximum, nIncrement);
                    toolStripProgressBar.Maximum += Math.Max(toolStripProgressBar.Maximum, nIncrement);
                }
                pbrPprogressBar.Value += nIncrement;
                toolStripProgressBar.Value += nIncrement;
            }
            catch
            {
            }
            finally
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// End file progress indicator.
        /// </summary>
        void moCompare_EventEndOfProgress(object poSender, CompareEngine.EventParameters poEventArgs)
        {
            pbrPprogressBar.Value = pbrPprogressBar.Maximum;
            string sMessage = poEventArgs.Text;
            staStatusStrip.Text = sMessage;
            toolStripStatusLabel.Text = sMessage;
            toolStripProgressBar.Value = toolStripProgressBar.Maximum;
            cmdGo.Enabled = true;
            cmdCancel.Enabled = false;
            Cursor.Current = Cursors.Default;
            Application.DoEvents();
        }

        /// <summary>
        /// Ignore file extension checked changed.
        /// </summary>
        private void chkIgnoreFileExtension_CheckedChanged(object sender, EventArgs e)
        {
            IgnoreFileExtension = chkIgnoreFileExtension.Checked;
            SaveParametersOnly();
        }

        /// <summary>
        /// Initiate compare.
        /// </summary>
        private void cmdGo_Click(object sender, EventArgs e)
        {
            SaveParameters();
            moCompare.Action = "Compare";
            moCompare.Compare();
            LoadComparisons();
            Refresh_Status();
        }

        /// <summary>
        /// Cancel compare.
        /// </summary>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            moCompare.Action = "Cancel";
        }

        /// <summary>
        /// Initiate fuzzy compare.
        /// </summary>
        private void cmdFzGo_Click(object sender, EventArgs e)
        {
            SaveParameters();
            moCompare.Action = "Compare";
            moCompare.FuzzyCompare();
            LoadComparisons();
        }

        #region Header
        /// <summary>
        /// Select an item from the previous selections ComboBox.
        /// </summary>
        private void cboPreviousSelections_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserSetting userSetting = PreviousSelections_SelectedItem;
            Administrator.ProfileManager.UserSettings.Select(userSetting.Key);
            LoadParameters();
        }

        /// <summary>
        /// Remove the currently selected setting key.
        /// </summary>
        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UserSetting userSetting = new UserSetting();
            if (Administrator.ProfileManager.UserSettings.SelectedItem != null)
            {
                userSetting = Administrator.ProfileManager.UserSettings.SelectedItem;
                Administrator.ProfileManager.UserSettings.Delete(Administrator.ProfileManager.UserSettings.SelectedItem.Key);
            }
            LoadParameters();
            RefreshCombo();
        }

        /// <summary>
        /// Browse for new path.
        /// </summary>
        private void cmdChooseNewPath_Click(object sender, EventArgs e)
        {
            NewPath = BrowseFolder(NewPath, NewPath);
            SetCurrentKey();
        }

        /// <summary>
		/// Browse for old path.
		/// </summary>
        private void cmdChooseOldPath_Click(object sender, EventArgs e)
        {
            OldPath = BrowseFolder(OldPath, OldPath);
            SetCurrentKey();
        }

        /// <summary>
        /// Copy the old path to the new path.
        /// </summary>
        private void txtNewPath_DoubleClick(object sender, EventArgs e)
        {
            NewPath = OldPath;
        }

        /// <summary>
        /// Copy the new path to the old path.
        /// </summary>
        private void txtOldPath_DoubleClick(object sender, EventArgs e)
        {
            OldPath = NewPath;
        }
        #endregion

        /// <summary>
        /// Store the nearest list view item when the mouse button is released.
        /// </summary>
        private void lvwResults_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem oListViewItem = lvwResults.GetItemAt(lvwResults.Left, e.Y);
            if (oListViewItem != null)
            {
                moListViewItem = oListViewItem;
            }
        }

        /// <summary>
        /// Launch Diff application for the selected pair of files.
        /// </summary>
        private void mnuContextCompare_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            switch (sCompare)
            {
                case "Deleted*":
                    MessageBox.Show("Entry has been deleted!");
                    return;
                case "Same":
                case "Same<":
                case "Same>":
                case "Same!":
                case "Diff":
                case "Diff!":
                    break;
                case "New":
                case "New*":
                    MessageBox.Show("Compare requires two files!");
                    return;
                case "Old":
                case "Old*":
                    MessageBox.Show("Compare requires two files!");
                    return;
            }
            moCompare.Diff(sNewSpec, sOldSpec);
            //TODO: Maybe reset the comparison and colour after a compare which proves to be identical.
        }

        private void mnuContextCompareMeld_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            switch (sCompare)
            {
                case "Deleted*":
                    MessageBox.Show("Entry has been deleted!");
                    return;
                case "Same":
                case "Same<":
                case "Same>":
                case "Same!":
                case "Diff":
                case "Diff!":
                    break;
                case "New":
                case "New*":
                    MessageBox.Show("Compare requires two files!");
                    return;
                case "Old":
                case "Old*":
                    MessageBox.Show("Compare requires two files!");
                    return;
            }
            moCompare.MeldCompare(sNewSpec, sOldSpec);
            //TODO: Maybe reset the comparison and colour after a compare which proves to be identical.
        }

        private void mnuContextCheck_Click(object sender, EventArgs e)
        {
            ListViewItem listItem = moListViewItem;
            string newSpec = txtNewPath.Text + listItem.SubItems[1].Text;
            string oldSpec = txtOldPath.Text + listItem.SubItems[2].Text;
            listItem.Text = moCompare.ReCheck(newSpec, oldSpec);
            Refresh_Status();
        }

        /// <summary>
        /// Copy new file to old file.
        /// </summary>
        private void mnuContextCopyNewToOld_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            if (oListItem.SubItems[1].Text != string.Empty)
            {
                if (oListItem.SubItems[2].Text != string.Empty)
                {
                    try
                    {
                        Models.Update.Copy(R.FileStem(sNewSpec), R.FileStem(sOldSpec), R.FileFullName(sNewSpec));
                        oListItem.Text = "Same>";
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        Models.Update.Copy(R.FileStem(sNewSpec), R.FileStem(txtOldPath.Text + oListItem.SubItems[1].Text), R.FileFullName(sNewSpec));
                        oListItem.Text = "Same>";
                        oListItem.SubItems[2].Text = oListItem.SubItems[1].Text;
                    }
                    catch
                    {
                    }
                }
                Refresh_Status();
            }
            else
            {
                MessageBox.Show("No new file available!");
            }
        }

        /// <summary>
        /// Copy old file to new file.
        /// </summary>
        private void mnuContextCopyOldToNew_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            if (oListItem.SubItems[2].Text != string.Empty)
            {
                if (oListItem.SubItems[1].Text != string.Empty)
                {
                    try
                    {
                        Models.Update.Copy(R.FileStem(sOldSpec), R.FileStem(sNewSpec), R.FileFullName(sOldSpec));
                        oListItem.Text = "Same<";
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        Models.Update.Copy(R.FileStem(sOldSpec), R.FileStem(txtNewPath.Text + oListItem.SubItems[2].Text), R.FileFullName(sOldSpec));
                        oListItem.Text = "Same<";
                        oListItem.SubItems[1].Text = oListItem.SubItems[2].Text;
                    }
                    catch
                    {
                    }
                }
                Refresh_Status();
            }
            else
            {
                MessageBox.Show("No old file available!");
            }
        }

        /// <summary>
        /// Delete selected new file.
        /// </summary>
        private void mnuContextDeleteNew_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            if (oListItem.SubItems[1].Text != string.Empty)
            {
                try
                {
                    Models.Update.Delete("New", R.FileStem(sNewSpec), R.FileFullName(sNewSpec));
                    if (oListItem.SubItems[2].Text != string.Empty)
                    {
                        oListItem.Text = "Old*";
                        oListItem.SubItems[1].Text = string.Empty;
                    }
                    else
                    {
                        oListItem.Text = "Deleted*";
                        oListItem.SubItems[1].Text = string.Empty;
                        lvwResults.Items.Remove(oListItem);
                    }
                }
                catch
                {
                }
                Refresh_Status();
            }
            else
            {
                MessageBox.Show("No new file available!");
            }
        }

        /// <summary>
        /// Delete selected old file.
        /// </summary>
        private void mnuContextDeleteOld_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sCompare = oListItem.Text;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            if (oListItem.SubItems[2].Text != string.Empty)
            {
                try
                {
                    Models.Update.Delete("Old", R.FileStem(sOldSpec), R.FileFullName(sOldSpec));
                    if (oListItem.SubItems[1].Text != string.Empty)
                    {
                        oListItem.Text = "New*";
                        oListItem.SubItems[2].Text = string.Empty;
                    }
                    else
                    {
                        oListItem.Text = "Deleted*";
                        oListItem.SubItems[2].Text = string.Empty;
                        lvwResults.Items.Remove(oListItem);
                    }
                }
                catch
                {
                }
                Refresh_Status();
            }
            else
            {
                MessageBox.Show("No old file available!");
            }
        }

        /// <summary>
        /// View new file.
        /// </summary>
        private void viewNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sNewSpec = txtNewPath.Text + oListItem.SubItems[1].Text;
            if (oListItem.SubItems[1].Text != string.Empty)
            {
                moCompare.View(sNewSpec);
            }
            else
            {
                MessageBox.Show("No new file available!");
            }
        }

        /// <summary>
        /// View old file.
        /// </summary>
        private void viewOldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem oListItem = moListViewItem;
            string sOldSpec = txtOldPath.Text + oListItem.SubItems[2].Text;
            if (oListItem.SubItems[2].Text != string.Empty)
            {
                moCompare.View(sOldSpec);
            }
            else
            {
                MessageBox.Show("No old file available!");
            }
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Set up ListView column headers.
        /// </summary>
        private void ColumnHeaders()
        {
            lvwResults.View = View.Details;
            lvwResults.Columns.Clear();
            lvwResults.Columns.Add("Comparison", 10 * (int)lvwResults.Font.SizeInPoints, HorizontalAlignment.Left);
            lvwResults.Columns.Add("New", 10 * (int)lvwResults.Font.SizeInPoints, HorizontalAlignment.Left);
            lvwResults.Columns.Add("Old", 10 * (int)lvwResults.Font.SizeInPoints, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Load comparisons collection into ListView.
        /// </summary>
        private void LoadComparisons()
        {
            if (moCompare.Identical)
            {
                lblIdentical.Text = "    Identical    ";
                lblIdentical.BackColor = Color.LightGray;
            }
            else
            {
                lblIdentical.Text = "    Different    ";
                lblIdentical.BackColor = Color.Gray;
            }
            ListViewItem oListItem;
            lvwResults.Items.Clear();
            for (int nCount = 0; nCount < moCompare.Comparisons.Count; nCount++)
            {
                Comparison comparison = moCompare.Comparisons[nCount];
                oListItem = lvwResults.Items.Add(comparison.Outcome, nCount);
                oListItem.BackColor = Color.AliceBlue;
                oListItem.UseItemStyleForSubItems = false;
                ListViewItem.ListViewSubItem lhs = oListItem.SubItems.Add(comparison.NewEntry);
                ListViewItem.ListViewSubItem rhs = oListItem.SubItems.Add(comparison.OldEntry);
                if (comparison.Outcome == "Diff")
                {
                    if (comparison.NewDate > comparison.OldDate)
                    {
                        oListItem.BackColor = Color.PaleGreen;
                        lhs.BackColor = Color.PaleGreen;
                        rhs.BackColor = Color.MistyRose;
                    }
                    else if (comparison.NewDate < comparison.OldDate)
                    {
                        oListItem.BackColor = Color.MistyRose;
                        lhs.BackColor = Color.MistyRose;
                        rhs.BackColor = Color.PaleGreen;
                    }
                    else
                    {
                        oListItem.BackColor = Color.AliceBlue;
                        lhs.BackColor = Color.AliceBlue;
                        rhs.BackColor = Color.AliceBlue;
                    }
                }
                else if (comparison.Outcome == "New")
                {
                    oListItem.BackColor = Color.LightGreen;
                    lhs.BackColor = Color.LightGreen;
                    rhs.BackColor = Color.LightGreen;
                }
                else if (comparison.Outcome == "Old")
                {
                    oListItem.BackColor = Color.LightPink;
                    lhs.BackColor = Color.LightPink;
                    rhs.BackColor = Color.LightPink;
                }
                else
                {
                    oListItem.BackColor = Color.AliceBlue;
                    lhs.BackColor = Color.AliceBlue;
                    rhs.BackColor = Color.AliceBlue;
                }
            }
        }

        /// <summary>
        /// Load all saved parameters.
        /// </summary>
        private void LoadParameters()
        {
            Administrator.ProfileManager.UserSettings.Load(Administrator.ProfileManager.SystemProfile.CurrentUserSettings, Administrator.ProfileManager.SystemProfile.MasterUserSettings);
            UserSetting userSetting = new UserSetting();
            if (Administrator.ProfileManager.UserSettings.SelectedItem != null)
            {
                userSetting = Administrator.ProfileManager.UserSettings.SelectedItem;
            }
            NewPath = userSetting.NewPath;
            OldPath = userSetting.OldPath;
            _newFilesEstimate = userSetting.NewFilesEstimate;
            _oldFilesEstimate = userSetting.OldFilesEstimate;
            _chgFilesEstimate = userSetting.ChgFilesEstimate;
            IgnoreFileExtension = userSetting.IgnoreFileExtension;
            _monitoredTypesOnly = userSetting.MonitoredTypesOnly;
            SetCurrentKey();
            moCompare = new CompareEngine();
            moCompare.EventBeginProgress += new CompareEngine.EventDelegate(moCompare_EventBeginProgress);
            moCompare.EventUpdateProgress += new CompareEngine.EventDelegate(moCompare_EventUpdateProgress);
            moCompare.EventEndOfProgress += new CompareEngine.EventDelegate(moCompare_EventEndOfProgress);
        }

        /// <summary>
        /// Save all parameters only.
        /// </summary>
        private void SaveParametersOnly()
        {
            SetCurrentKey();
            Administrator.ProfileManager.UserSettings.SelectedItem.NewPath = NewPath; 
            Administrator.ProfileManager.UserSettings.SelectedItem.OldPath = OldPath;
            Administrator.ProfileManager.UserSettings.SelectedItem.NewFilesEstimate = _newFilesEstimate;
            Administrator.ProfileManager.UserSettings.SelectedItem.OldFilesEstimate = _oldFilesEstimate;
            Administrator.ProfileManager.UserSettings.SelectedItem.ChgFilesEstimate = _chgFilesEstimate;
            Administrator.ProfileManager.UserSettings.SelectedItem.IgnoreFileExtension = IgnoreFileExtension;
            Administrator.ProfileManager.UserSettings.Save();
        }

        /// <summary>
        /// Save all parameters.
        /// </summary>
        private void SaveParameters()
        {
            SetCurrentKey();
            moCompare.NewPath = NewPath;
            moCompare.OldPath = OldPath;
            UserSetting userSetting = new UserSetting
            {
                Key = _currentKey,
                NewPath = NewPath,
                OldPath = OldPath,
                NewFilesEstimate = _newFilesEstimate,
                OldFilesEstimate = _oldFilesEstimate,
                ChgFilesEstimate = _chgFilesEstimate,
                IgnoreFileExtension = IgnoreFileExtension,
                MonitoredTypesOnly = _monitoredTypesOnly,
            };
            Administrator.ProfileManager.UserSettings.Persist(userSetting);
            LoadParameters();
            RefreshCombo();
        }

        /// <summary>
        /// Refresh ComboBox at the begining or when a new entry has been added.
        /// </summary>
        private void RefreshCombo()
        {
            PreviousSelections_IndexChanged_Event_On = false;
            PreviousSelections_DataSource = Administrator.ProfileManager.UserSettings.Values.ToList();
            PreviousSelections_DisplayMember = "KeyShrunk";
            UserSetting userSetting = new UserSetting();
            if (Administrator.ProfileManager.UserSettings.SelectedItem != null)
            {
                userSetting = Administrator.ProfileManager.UserSettings.SelectedItem;
            }
            PreviousSelections_SelectedString = userSetting.KeyShrunk;
            if (PreviousSelections_Count == 0)
            {
                PreviousSelections_Text = string.Empty;
            }
            PreviousSelections_IndexChanged_Event_On = true;
        }

        /// <summary>
        /// Set current key.
        /// </summary>
        private void SetCurrentKey()
        {
            if (NewPath.Trim().Length > 0)
            {
                if (OldPath.Trim().Length > 0)
                {
                    _currentKey = "[" + NewPath + "]-[" + OldPath + "]";
                }
                else
                {
                    _currentKey = "[" + NewPath + "]-[?]";
                }
            }
            else
            {
                if (OldPath.Trim().Length > 0)
                {
                    _currentKey = "[?]-[" + OldPath + "]";
                }
                else
                {
                    _currentKey = string.Empty;
                }
            }
        }

        /// <summary>
        /// Refresh status.
        /// </summary>
        private void Refresh_Status()
        {
            int nFirstDifferenceRow = 0;
            int nCount = 0;
            string sStatus = string.Empty;
            bool bIdentical = true;
            for (nCount = 0; nCount < lvwResults.Items.Count; nCount++)
            {
                sStatus = lvwResults.Items[nCount].Text;
                sStatus = sStatus.PadRight(4).ToUpper();
                sStatus = sStatus.Substring(0, 4);
                if (sStatus != "SAME")
                {
                    bIdentical = false;
                    nFirstDifferenceRow = nCount;
                    break;
                }
            }
            if (bIdentical)
            {
                lblIdentical.Text = "    Identical    ";
                lblIdentical.BackColor = Color.LightGray;
            }
            else
            {
                //Scroll to the first row which is different.
                lvwResults.Items[nFirstDifferenceRow].EnsureVisible();
                Application.DoEvents();
                lblIdentical.Text = "    Different    ";
                lblIdentical.BackColor = Color.Gray;
            }
            // Reset colour of items which become the same after a context menu action.
            for (int count = 0; count < lvwResults.Items.Count; count++)
            {
                ListViewItem listItem = lvwResults.Items[count];
                string status = lvwResults.Items[count].Text;
                status = status.PadRight(4).ToUpper();
                status = status.Substring(0, 4);
                if (status == "SAME")
                {
                    listItem.BackColor = Color.AliceBlue;
                    listItem.UseItemStyleForSubItems = false;
                    ListViewItem.ListViewSubItem lhs = listItem.SubItems[1];
                    ListViewItem.ListViewSubItem rhs = listItem.SubItems[2];
                    listItem.BackColor = Color.AliceBlue;
                    lhs.BackColor = Color.AliceBlue;
                    rhs.BackColor = Color.AliceBlue;
                }
            }
        }

        private string BrowseFolder(string initialDirectory, string defaultFolder)
        {
            string folder = defaultFolder;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            dialog.SelectedPath = initialDirectory;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folder = dialog.SelectedPath;
            }
            return folder;
        }

        private string BrowseFile(string initialDirectory, string filter, string defaultFileSpec)
        {
            string fileSpec = defaultFileSpec;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = filter;
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileSpec = openFileDialog.FileName;
            }
            return fileSpec;
        }
        #endregion
    }
}