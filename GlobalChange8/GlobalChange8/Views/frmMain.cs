using GlobalChange8.Models;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;

namespace GlobalChange8.Views
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
        private SearchEngine moSearch;
        private SearchKotlinEngine moSearchKotlin;
        private CloneEngine moClone;
        private SearchReplaceMultipleEngine moSearchReplaceMultipleEngine;
        private ReproEngine moRepro;
        private CloneDroid moCloneDroid;
        private ReplaceDroid moReplaceDroid;
        private ModelsEngine moModels;
        private ModelsKotlinEngine moModelsKotlin;
        private bool mbSuppressCheckAllChanged = false;
	    private bool mbSuppressCheckAll = false;
        private bool _resultsDisplayLimitReached = false;
		#endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers.
        /// <summary>
        /// Form load event handler.
        /// </summary>
        private void frmMain_Load(object sender, EventArgs e)
        {
            cmdSearch.Enabled = true;
            cmdCancel.Enabled = false;
            cmdClose.Enabled = true;
            LoadParameters();
        }

        /// <summary>
        /// Check or uncheck all windows extensions.
        /// </summary>
        private void chkWinAll_CheckedChanged(object sender, EventArgs e)
        {
			if (!mbSuppressCheckAllChanged)
			{
				mbSuppressCheckAll = true;
				CheckChanged(grpWindows, chkWinAll);
				mbSuppressCheckAll = false;
			}
        }

        /// <summary>
        /// Check or uncheck all web extensions.
        /// </summary>
        private void chkWebAll_CheckedChanged(object sender, EventArgs e)
        {
			if (!mbSuppressCheckAllChanged)
			{
				mbSuppressCheckAll = true;
				CheckChanged(grpWeb, chkWebAll);
				mbSuppressCheckAll = false;
			}
        }

        /// <summary>
        /// Check or uncheck all office extensions.
        /// </summary>
        private void chkOffAll_CheckedChanged(object sender, EventArgs e)
        {
			if (!mbSuppressCheckAllChanged)
			{
				mbSuppressCheckAll = true;
				CheckChanged(grpOffice, chkOffAll);
				mbSuppressCheckAll = false;
			}
        }

        /// <summary>
        /// Check or uncheck all executable extensions.
        /// </summary>
        private void chkExeAll_CheckedChanged(object sender, EventArgs e)
        {
			if (!mbSuppressCheckAllChanged)
			{
				mbSuppressCheckAll = true;
				CheckChanged(grpExecutables, chkExeAll);
				mbSuppressCheckAll = false;
			}
        }

		/// <summary>
		/// Use a wildcard for all file extensions and ignore individual file extentions.
		/// </summary>
		private void chkAllTypes_CheckedChanged(object sender, EventArgs e)
		{
			ToggleFileTypes(!chkAllTypes.Checked);
		}

		/// <summary>
		/// Check if all individual CheckBoxes in each group are checked or not.
		/// If they are then check the "All" CheckBox for that group.
		/// </summary>
		private void chkAllChecked_CheckedChanged(object sender, EventArgs e)
		{
			if (!mbSuppressCheckAll)
			{
				mbSuppressCheckAllChanged = true;
				CheckAll();
				mbSuppressCheckAllChanged = false;
			}
		}

        /// <summary>
        /// Initiate search.
        /// </summary>
        private void cmdSearch_Click(object sender, EventArgs e)
        {
            if (!cboMode.Text.StartsWith("Clone"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moSearch.Action = "Search";
                    moSearch.Search();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot search when mode is set to ""Clone*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        /// <summary>
        /// Initiate search for Kotlin.
        /// </summary>
        private void cmdSearchKt_Click(object sender, EventArgs e)
        {
            if (!cboMode.Text.StartsWith("Clone"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moSearchKotlin.Action = "Search";
                    moSearchKotlin.Search();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot search when mode is set to ""Clone*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdClone_Click(object sender, EventArgs e)
        {
            if (cboMode.Text.StartsWith("Clone"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moClone.Action = "Clone";
                    moClone.LoadConfiguration(cboMode.Text);
                    moClone.Run();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot clone unless mode is set to ""Clone*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void CmdSearchReplaceMultiple_Click(object sender, EventArgs e)
        {
            if (cboMode.Text.StartsWith("Replace"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moSearchReplaceMultipleEngine.Action = "Replace";
                    moSearchReplaceMultipleEngine.LoadConfiguration(cboMode.Text);
                    moSearchReplaceMultipleEngine.Run();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot search replace multiple unless mode is set to ""Replace*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdRepro_Click(object sender, EventArgs e)
        {
            if (cboMode.Text.StartsWith("Repro"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moRepro.Action = "Repro";
                    moRepro.LoadConfiguration(cboMode.Text);
                    moRepro.Run();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot repro unless mode is set to ""Repro*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdCloneDroid_Click(object sender, EventArgs e)
        {
            if (cboMode.Text.StartsWith("CloneDroid"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moCloneDroid.Action = "CloneDroid";
                    moCloneDroid.LoadConfiguration(cboMode.Text);
                    moCloneDroid.Run();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot clone droid unless mode is set to ""CloneDroid*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdReplaceAsterisk_Click(object sender, EventArgs e)
        {
            if (cboMode.Text.StartsWith("ReplaceDroid"))
            {
                _resultsDisplayLimitReached = false;
                if (System.IO.Directory.Exists(txtPath.Text))
                {
                    SaveParameters();
                    txtResults.Text = string.Empty;
                    moReplaceDroid.Action = "ReplaceDroid";
                    moReplaceDroid.LoadConfiguration(cboMode.Text);
                    moReplaceDroid.Run();
                }
                else
                {
                    string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                    staStatusStrip.Text = sMessage;
                    toolStripStatusLabel.Text = sMessage;
                }
            }
            else
            {
                string sMessage = @"Cannot replace droid unless mode is set to ""ReplaceDroid*""";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdModels_Click(object sender, EventArgs e)
        {
            _resultsDisplayLimitReached = false;
            if (System.IO.Directory.Exists(txtPath.Text))
            {
                SaveParameters();
                txtResults.Text = string.Empty;
                moModels.Action = "Models";
                moModels.LoadConfiguration(cboMode.Text);
                moModels.Run();
            }
            else
            {
                string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        private void cmdModelsKotlin_Click(object sender, EventArgs e)
        {
            _resultsDisplayLimitReached = false;
            if (System.IO.Directory.Exists(txtPath.Text))
            {
                SaveParameters();
                txtResults.Text = string.Empty;
                moModelsKotlin.Action = "Models";
                moModelsKotlin.LoadConfiguration(cboMode.Text);
                moModelsKotlin.Run();
            }
            else
            {
                string sMessage = @"Directory """ + txtPath.Text + @""" does not exist.";
                staStatusStrip.Text = sMessage;
                toolStripStatusLabel.Text = sMessage;
            }
        }

        /// <summary>
        /// Cancel run which is in progress.
        /// </summary>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            moSearch.Action = "Cancel";
            moSearchKotlin.Action = "Cancel";
        }

        /// <summary>
        /// Close application.
        /// </summary>
        private void cmdClose_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        /// <summary>
        /// Show folder selector.
        /// </summary>
        private void txtPath_DoubleClick(object sender, EventArgs e)
        {
            dlgFolder.RootFolder = System.Environment.SpecialFolder.Desktop;
            DialogResult oResult = dlgFolder.ShowDialog();
            if (oResult == DialogResult.OK)
            {
                txtPath.Text = dlgFolder.SelectedPath;
            }
        }

        /// <summary>
        /// Start file progress indicator.
        /// </summary>
        void moSearch_EventBeginProgress(object poSender, SearchEngine.EventParameters poEventArgs)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            cmdSearch.Enabled = false;
            cmdCancel.Enabled = true;
            cmdClose.Enabled = false;
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
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Update file progress indicator.
        /// </summary>
        void moSearch_EventUpdateProgress(object poSender, SearchEngine.EventParameters poEventArgs)
        {
            if (System.Windows.Forms.Cursor.Current != System.Windows.Forms.Cursors.WaitCursor)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
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
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// End file progress indicator.
        /// </summary>
        void moSearch_EventEndOfProgress(object poSender, SearchEngine.EventParameters poEventArgs)
        {
            pbrPprogressBar.Value = pbrPprogressBar.Maximum;
            string sMessage = poEventArgs.Text;
            staStatusStrip.Text = sMessage;
            toolStripStatusLabel.Text = sMessage;
            toolStripProgressBar.Value = toolStripProgressBar.Maximum;
            cmdSearch.Enabled = true;
            cmdCancel.Enabled = false;
            cmdClose.Enabled = true;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Display found data in context.
        /// </summary>
        void moSearch_EventCriteriaPass(object poSender, SearchEngine.EventParameters poEventArgs)
        {
            string sData = poEventArgs.Text;
            if (txtResults.Text.Length <= 8192)
            {
                txtResults.Text += sData + Environment.NewLine;
            }
            else
            {
                if (!_resultsDisplayLimitReached)
                {
                    txtResults.Text += "*************************" + Environment.NewLine;
                    txtResults.Text += "* Display Limit Reached *" + Environment.NewLine;
                    txtResults.Text += "*************************" + Environment.NewLine;
                    _resultsDisplayLimitReached = true;
                }
            }
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Start file progress indicator.
        /// </summary>
        void moSearchKotlin_EventBeginProgress(object poSender, SearchKotlinEngine.EventParameters poEventArgs)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            cmdSearch.Enabled = false;
            cmdCancel.Enabled = true;
            cmdClose.Enabled = false;
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
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Update file progress indicator.
        /// </summary>
        void moSearchKotlin_EventUpdateProgress(object poSender, SearchKotlinEngine.EventParameters poEventArgs)
        {
            if (System.Windows.Forms.Cursor.Current != System.Windows.Forms.Cursors.WaitCursor)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
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
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// End file progress indicator.
        /// </summary>
        void moSearchKotlin_EventEndOfProgress(object poSender, SearchKotlinEngine.EventParameters poEventArgs)
        {
            pbrPprogressBar.Value = pbrPprogressBar.Maximum;
            string sMessage = poEventArgs.Text;
            staStatusStrip.Text = sMessage;
            toolStripStatusLabel.Text = sMessage;
            toolStripProgressBar.Value = toolStripProgressBar.Maximum;
            cmdSearch.Enabled = true;
            cmdCancel.Enabled = false;
            cmdClose.Enabled = true;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Display found data in context.
        /// </summary>
        void moSearchKotlin_EventCriteriaPass(object poSender, SearchKotlinEngine.EventParameters poEventArgs)
        {
            string sData = poEventArgs.Text;
            if (txtResults.Text.Length <= 8192)
            {
                txtResults.Text += sData + Environment.NewLine;
            }
            else
            {
                if (!_resultsDisplayLimitReached)
                {
                    txtResults.Text += "*************************" + Environment.NewLine;
                    txtResults.Text += "* Display Limit Reached *" + Environment.NewLine;
                    txtResults.Text += "*************************" + Environment.NewLine;
                    _resultsDisplayLimitReached = true;
                }
            }
            System.Windows.Forms.Application.DoEvents();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Load all saved parameters.
        /// </summary>
        private void LoadParameters()
        {
            moSearch = new SearchEngine();
            moSearchKotlin = new SearchKotlinEngine();
            moClone = new CloneEngine();
            moSearchReplaceMultipleEngine = new SearchReplaceMultipleEngine();
            moRepro = new ReproEngine();
            moCloneDroid = new CloneDroid();
            moReplaceDroid = new ReplaceDroid();
            moModels = new ModelsEngine();
            moModelsKotlin = new ModelsKotlinEngine();
            moSearch.EventCriteriaPass += new SearchEngine.EventDelegate(moSearch_EventCriteriaPass);
            moSearch.EventBeginProgress += new SearchEngine.EventDelegate(moSearch_EventBeginProgress);
            moSearch.EventUpdateProgress += new SearchEngine.EventDelegate(moSearch_EventUpdateProgress);
            moSearch.EventEndOfProgress += new SearchEngine.EventDelegate(moSearch_EventEndOfProgress);
            moSearchKotlin.EventCriteriaPass += new SearchKotlinEngine.EventDelegate(moSearchKotlin_EventCriteriaPass);
            moSearchKotlin.EventBeginProgress += new SearchKotlinEngine.EventDelegate(moSearchKotlin_EventBeginProgress);
            moSearchKotlin.EventUpdateProgress += new SearchKotlinEngine.EventDelegate(moSearchKotlin_EventUpdateProgress);
            moSearchKotlin.EventEndOfProgress += new SearchKotlinEngine.EventDelegate(moSearchKotlin_EventEndOfProgress);
            txtPath.Text = moSearch.Path;
            txtFilePattern.Text = moSearch.FilePattern;
            txtCriteria.Text = moSearch.SearchCriteria;
            txtFind.Text = moSearch.Find;
            txtReplacement.Text = moSearch.Replacement;
            string[] modeNames = Enum.GetNames(typeof(RunMode));
            cboMode.DataSource = modeNames;
            cboMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMode.Text = moSearch.Mode;
            chkRegex.Checked = moSearch.Regex;
			mbSuppressCheckAllChanged = true;
			mbSuppressCheckAll = true;
			GetOptions();
            GetExtensions();
            GetDirectoryExclusions();
			CheckAll();
			mbSuppressCheckAllChanged = false;
			mbSuppressCheckAll = false;
			chkAllTypes.Checked = moSearch.AllTypes;
		}

        /// <summary>
        /// Save all parameters.
        /// </summary>
        private void SaveParameters()
        {
            moSearch.Path = txtPath.Text;
            moSearch.FilePattern = txtFilePattern.Text;
            moSearch.SearchCriteria = txtCriteria.Text;
            moSearch.Find = txtFind.Text;
            moSearch.Replacement = txtReplacement.Text;
            moSearch.Mode = cboMode.Text;
            moSearch.Regex = chkRegex.Checked;
            moSearchKotlin.Path = txtPath.Text;
            moSearchKotlin.FilePattern = txtFilePattern.Text;
            moSearchKotlin.SearchCriteria = txtCriteria.Text;
            moSearchKotlin.Find = txtFind.Text;
            moSearchKotlin.Replacement = txtReplacement.Text;
            moSearchKotlin.Mode = cboMode.Text;
            moSearchKotlin.Regex = chkRegex.Checked;
            SetDirectoryExclusions();
            SetOptions();
            SetExtensions();
            moSearch.AllTypes = chkAllTypes.Checked;
            moSearchKotlin.AllTypes = chkAllTypes.Checked;
            moClone.Mode = cboMode.Text;
            moSearchReplaceMultipleEngine.Mode = cboMode.Text;
            moRepro.Mode = cboMode.Text;
            moCloneDroid.Mode = cboMode.Text;
            moReplaceDroid.Mode = cboMode.Text;
            moModels.Mode = cboMode.Text;
            moModelsKotlin.Mode = cboMode.Text;
            moSearch.Save();
            moSearchKotlin.Save();
        }

        /// <summary>
        /// Set CheckBox values from saved list of extensions.
        /// </summary>
        private void GetExtensions()
        {
            Hashtable cExtensions = new Hashtable();
            foreach (string sExtension in moSearch.Extensions)
            {
                cExtensions.Add(sExtension, sExtension);
            }
            foreach (Control oElement in this.Controls)
            {
                GroupBox oVessel = oElement as GroupBox;
                if (oVessel != null)
                {
                    foreach (Control oGroup in oVessel.Controls)
                    {
                        GroupBox oGroupBox = oGroup as GroupBox;
                        if (oGroupBox != null)
                        {
                            foreach (Control oControl in oGroupBox.Controls)
                            {
                                CheckBox oCheckBox = oControl as CheckBox;
                                if (oCheckBox != null)
                                {
                                    try
                                    {
                                        string sExtension = cExtensions[oCheckBox.Text].ToString();
                                        oCheckBox.Checked = true;
                                    }
                                    catch
                                    {
                                        oCheckBox.Checked = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set CheckBox values from saved list of options.
        /// </summary>
        private void GetOptions()
        {
            Hashtable cOptions = new Hashtable();
            foreach (string sOption in moSearch.Options)
            {
                cOptions.Add(sOption, sOption);
            }
            foreach (Control oElement in this.Controls)
            {
                GroupBox oGroupBox = oElement as GroupBox;
                if (oGroupBox != null)
                {
                    foreach (Control oControl in oGroupBox.Controls)
                    {
                        CheckBox oCheckBox = oControl as CheckBox;
                        if (oCheckBox != null)
                        {
                            try
                            {
                                string sOption = cOptions[oCheckBox.Text].ToString();
                                oCheckBox.Checked = true;
                            }
                            catch
                            {
                                oCheckBox.Checked = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set TextBox values from saved list of directory exclusions.
        /// </summary>
        private void GetDirectoryExclusions()
        {
            string delimiter = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string exclusion in moSearch.DirectoryExclusions)
            {
                sb.Append(String.Format(@"{0}""{1}""", delimiter, exclusion));
                delimiter = ",";
            }
            txtExcludeDirectories.Text = sb.ToString();
            txtExcludeDirectories.SelectionStart = 0;
            txtExcludeDirectories.SelectionLength = 0;
        }

        /// <summary>
        /// Check if all file extensions in all groups have been selected.
        /// </summary>
        private void CheckAll()
        {
            CheckAll(grpWindows, chkWinAll);
            CheckAll(grpWeb, chkWebAll);
            CheckAll(grpOffice, chkOffAll);
            CheckAll(grpExecutables, chkExeAll);
        }

        /// <summary>
        /// Check if all file extensions in the specified group have been selected.
        /// </summary>
        private void CheckAll(GroupBox poGroupBox, CheckBox poCheckBoxAll)
        {
            bool bAllChecked = true;
            foreach (Control oControl in poGroupBox.Controls)
            {
                CheckBox oCheckBox = oControl as CheckBox;
                if (oCheckBox != null)
                {
                    if (oCheckBox != poCheckBoxAll)
                    {
                        if (oCheckBox.Checked == false)
                        {
                            bAllChecked = false;
                            break;
                        }
                    }
                }
            }
            poCheckBoxAll.Checked = bAllChecked;
        }

        /// <summary>
        /// Set all associated CheckBoxes to the same value as the Chk???All CheckBox.
        /// </summary>
        private void CheckChanged(GroupBox poGroupBox, CheckBox poCheckBoxAll)
        {
            foreach (Control oControl in poGroupBox.Controls)
            {
                CheckBox oCheckBox = oControl as CheckBox;
                if (oCheckBox != null)
                {
                    oCheckBox.Checked = poCheckBoxAll.Checked;
                }
            }
        }

        /// <summary>
        /// Build a list of file extensions to search based on checked values.
        /// </summary>
        private void SetExtensions()
        {
            moSearch.Extensions = new StringCollection();
            foreach (Control oElement in this.Controls)
            {
                GroupBox oVessel = oElement as GroupBox;
                if (oVessel != null)
                {
                    foreach (Control oGroup in oVessel.Controls)
                    {
                        GroupBox oGroupBox = oGroup as GroupBox;
                        if (oGroupBox != null)
                        {
                            foreach (Control oControl in oGroupBox.Controls)
                            {
                                CheckBox oCheckBox = oControl as CheckBox;
                                if (oCheckBox != null)
                                {
                                    if (oCheckBox.Checked)
                                    {
                                        string sExtension = oCheckBox.Text;
                                        if (sExtension != "All")
                                        {
                                            moSearch.Extensions.Add(sExtension);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build a list of search options based on checked values.
        /// </summary>
        private void SetOptions()
        {
            moSearch.Options = new StringCollection();
            foreach (Control oElement in this.Controls)
            {
                GroupBox oGroupBox = oElement as GroupBox;
                if (oGroupBox != null)
                {
                    foreach (Control oControl in oGroupBox.Controls)
                    {
                        CheckBox oCheckBox = oControl as CheckBox;
                        if (oCheckBox != null)
                        {
							if (oCheckBox.Text != "All types")
							{
								if (oCheckBox.Checked)
								{
									string sOption = oCheckBox.Text;
									moSearch.Options.Add(sOption);
								}
							}
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build a list of directory exclusions based on the comma separated values in the exclude directories field.
        /// </summary>
        private void SetDirectoryExclusions()
        {
            moSearch.DirectoryExclusions = new StringCollection();
            var directoryExclusions = CsvSplitter.Split(txtExcludeDirectories.Text);
            foreach (var exclusion in directoryExclusions)
            {
                moSearch.DirectoryExclusions.Add(exclusion);
            }
        }

		/// <summary>
		/// Toggle individual file extension CheckBoxes between enabled and disabled.
		/// </summary>
		/// <param name="pbEnabled"></param>
		private void ToggleFileTypes(bool pbEnabled)
		{
			moSearch.Extensions = new StringCollection();
			foreach (Control oElement in this.Controls)
			{
				GroupBox oVessel = oElement as GroupBox;
				if (oVessel != null)
				{
					foreach (Control oGroup in oVessel.Controls)
					{
						GroupBox oGroupBox = oGroup as GroupBox;
						if (oGroupBox != null)
						{
							oGroupBox.Enabled = pbEnabled;
						}
					}
				}
			}
		}
        #endregion
    }
}
