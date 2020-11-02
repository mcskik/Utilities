using Copy9.DataLayer.Profile;
using Copy9.Presenters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Copy9.Views
{
    public partial class frmMain : Form, ICopyViewer
    {
        #region Member Variables.
        private int _errorCount;
        private Dictionary<string, ControlMessage> _fields;
        private readonly object _progressLock = new object();
        private readonly object _cursorLock = new object();
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

        public string NewBaseDir
        {
            get
            {
                return txtNewBaseDir.Text;
            }
            set
            {
                txtNewBaseDir.Text = value;
                txtNewBaseDir.SelectionStart = txtNewBaseDir.Text.Length;
                txtNewBaseDir.ScrollToCaret();
            }
        }

        public string OldBaseDir
        {
            get
            {
                return txtOldBaseDir.Text;
            }
            set
            {
                txtOldBaseDir.Text = value;
                txtOldBaseDir.SelectionStart = txtOldBaseDir.Text.Length;
                txtOldBaseDir.ScrollToCaret();
            }
        }

        public UserSetting.CopyRuleEnum CopyRule
        {
            get
            {
                return GetCopyRuleFromRadioGroup(grpOptions, UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer);
            }
            set
            {
                SetRadioGroupFromCopyRule(grpOptions, value);
            }
        }

        public bool MonitoredTypesOnly
        {
            get
            {
                return chkMonitoredTypesOnly.Checked;
            }
            set
            {
                chkMonitoredTypesOnly.Checked = value;
            }
        }

        protected DateTime _dateThreshold = DateTime.MinValue;
        public DateTime DateThreshold
        {
            get
            {
                return _dateThreshold;
            }
            set
            {
                if (value >= dpkDateThreshold.MinDate)
                {
                    try
                    {
                        _dateThreshold = value;
                        dpkDateThreshold.Value = _dateThreshold;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    _dateThreshold = dpkDateThreshold.MinDate;
                    dpkDateThreshold.Value = _dateThreshold;
                }
            }
        }

        public Presenter.CursorModes CursorMode
        {
            get
            {
                if (Cursor.Current == Cursors.WaitCursor)
                {
                    return Presenter.CursorModes.Wait;
                }
                else
                {
                    return Presenter.CursorModes.Normal;
                }
            }
            set
            {
                if (value == Presenter.CursorModes.Wait)
                {
                    Cursor.Current = Cursors.WaitCursor;
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                }
                Application.DoEvents();
            }
        }

        public ProgressLink CurrentActivityProgress
        {
            get
            {
                return new ProgressLink(pbrCurrentActivity.Minimum, pbrCurrentActivity.Maximum, pbrCurrentActivity.Value);
            }
            set
            {
                pbrCurrentActivity.Minimum = value.Minimum;
                pbrCurrentActivity.Maximum = value.Maximum;
                pbrCurrentActivity.Value = value.Value;
                Application.DoEvents();
            }
        }

        public ProgressLink OverallProgress
        {
            get
            {
                return new ProgressLink(pbrOverallProgress.Minimum, pbrOverallProgress.Maximum, pbrOverallProgress.Value);
            }
            set
            {
                pbrOverallProgress.Minimum = value.Minimum;
                pbrOverallProgress.Maximum = value.Maximum;
                pbrOverallProgress.Value = value.Value;
                Application.DoEvents();
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

        #region Event Declarations.
        public event Presenter.PreviousSelectionSelectedIndexChangedHandler OnPreviousSelectionSelectedIndexChanged;
        public event Presenter.RemovePreviousSelectionHandler OnRemovePreviousSelection;
        public event Presenter.ChooseNewBaseDirectoryHandler OnChooseNewBaseDirectory;
        public event Presenter.ChooseOldBaseDirectoryHandler OnChooseOldBaseDirectory;
        public event Presenter.CopyNewBaseDirectoryHandler OnCopyNewBaseDirectory;
        public event Presenter.CopyOldBaseDirectoryHandler OnCopyOldBaseDirectory;
        public event Presenter.CopyActionHandler OnCopyAction;
        public event Presenter.DeleteActionHandler OnDeleteAction;
        public event Presenter.SynchronizePreventDeleteActionHandler OnSynchronizePreventDeleteAction;
        public event Presenter.SynchronizeActionHandler OnSynchronizeAction;
        public event Presenter.SynchronizeAllActionHandler OnSynchronizeAllAction;
        public event Presenter.SynchronizePlusActionHandler OnSynchronizePlusAction;
        public event Presenter.SynchronizePlusAllActionHandler OnSynchronizePlusAllAction;
        public event Presenter.SynchronizePlusPlusActionHandler OnSynchronizePlusPlusAction;
        public event Presenter.SynchronizePlusPlusAllActionHandler OnSynchronizePlusPlusAllAction;
        public event Presenter.ResetActionHandler OnResetAction;
        public event Presenter.CancelActionHandler OnCancelAction;
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            InitialiseFields();
            ClearErrors();
        }
        #endregion

        #region Event handlers.
        /// <summary>
        /// Select an item from the previous selections ComboBox.
        /// </summary>
        private void cboPreviousSelections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnPreviousSelectionSelectedIndexChanged != null)
            {
                OnPreviousSelectionSelectedIndexChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Remove the currently selected setting key.
        /// </summary>
        private void cmdRemove_Click(object sender, EventArgs e)
        {
            if (OnRemovePreviousSelection != null)
            {
                OnRemovePreviousSelection(new EventArgs());
            }
        }

        /// <summary>
        /// Browse for new base directory which is the default parent directory for the new file.
        /// </summary>
        private void cmdChooseNewBaseDir_Click(object sender, EventArgs e)
        {
            if (OnChooseNewBaseDirectory != null)
            {
                OnChooseNewBaseDirectory(new EventArgs());
            }
        }

        /// <summary>
		/// Browse for new base directory which is the default parent directory for the new file.
		/// </summary>
		private void cmdChooseOldBaseDir_Click(object sender, EventArgs e)
        {
            if (OnChooseOldBaseDirectory != null)
            {
                OnChooseOldBaseDirectory(new EventArgs());
            }
        }

        /// <summary>
        /// Copy the old base directory to the new base directory.
        /// </summary>
        private void txtNewBaseDir_DoubleClick(object sender, EventArgs e)
        {
            if (OnCopyOldBaseDirectory != null)
            {
                OnCopyOldBaseDirectory(new EventArgs());
            }
        }

        /// <summary>
        /// Copy the new base directory to the old base directory.
        /// </summary>
        private void txtOldBaseDir_DoubleClick(object sender, EventArgs e)
        {
            if (OnCopyNewBaseDirectory != null)
            {
                OnCopyNewBaseDirectory(new EventArgs());
            }
        }

        private void cmdDateThresholdMin_Click(object sender, EventArgs e)
        {
            dpkDateThreshold.Value = dpkDateThreshold.MinDate;
        }

        private void dpkDateThreshold_ValueChanged(object sender, EventArgs e)
        {
            DateThreshold = dpkDateThreshold.Value;
        }

        private void cmdDateThresholdMax_Click(object sender, EventArgs e)
        {
            dpkDateThreshold.Value = dpkDateThreshold.MaxDate;
        }

        /// <summary>
        /// Run the XCopy using the specified directories.
        /// </summary>
        private void cmdCopy_Click(object sender, EventArgs e)
        {
            if (OnCopyAction != null)
            {
                string lastPortionCopyFrom = LastPortion(NewBaseDir);
                string lastPortionCopyTo = LastPortion(OldBaseDir);
                if (lastPortionCopyTo != lastPortionCopyFrom)
                {
                    DialogResult boxResult = MessageBox.Show("Create missing directory for last portion and copy into that?", "Create missing subdirectory", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (boxResult == DialogResult.Yes)
                    {
                        if (!OldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            OldBaseDir += Path.DirectorySeparatorChar;
                        }
                        OldBaseDir += lastPortionCopyFrom;
                        Application.DoEvents();
                        OnCopyAction(new EventArgs());
                    }
                    else if (boxResult == DialogResult.No)
                    {
                        OnCopyAction(new EventArgs());
                    }
                }
                else
                {
                    OnCopyAction(new EventArgs());
                }
            }
        }

        private string LastPortion(string path)
        {
            string lastPortion = path;
            int pos = path.LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > -1)
            {
                lastPortion = lastPortion.Substring(pos + 1);
            }
            return lastPortion;
        }

        /// <summary>
        /// Run the XDelete using the specified copy from directories.
        /// </summary>
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (OnDeleteAction != null)
            {
                DialogResult boxResult = MessageBox.Show("Are you absolutely sure you want to delete everything including all sub directories from the Copy From directory?", "Delete everything from the Copy From directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (boxResult == DialogResult.Yes)
                {
                    OnDeleteAction(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Use the synchronize process to copy files and directories only, but to prevent deletion of files or removal of directories.
        /// </summary>
        private void cmdCopySynch_Click(object sender, EventArgs e)
        {
            if (OnSynchronizePreventDeleteAction != null)
            {
                string lastPortionCopyFrom = LastPortion(NewBaseDir);
                string lastPortionCopyTo = LastPortion(OldBaseDir);
                if (lastPortionCopyTo != lastPortionCopyFrom)
                {
                    DialogResult boxResult = MessageBox.Show("Create missing directory for last portion and synchronize into that?", "Create missing subdirectory (Prevent Delete)", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (boxResult == DialogResult.Yes)
                    {
                        if (!OldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            OldBaseDir += Path.DirectorySeparatorChar;
                        }
                        OldBaseDir += lastPortionCopyFrom;
                        Application.DoEvents();
                        OnSynchronizePreventDeleteAction(new EventArgs());
                    }
                    else if (boxResult == DialogResult.No)
                    {
                        OnSynchronizePreventDeleteAction(new EventArgs());
                    }
                }
                else
                {
                    OnSynchronizePreventDeleteAction(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Use the synchronize process to copy and delete files by comparing the source and target directory trees first.
        /// </summary>
        private void cmdSynchronize_Click(object sender, EventArgs e)
        {
            if (OnSynchronizeAction != null)
            {
                string lastPortionCopyFrom = LastPortion(NewBaseDir);
                string lastPortionCopyTo = LastPortion(OldBaseDir);
                if (lastPortionCopyTo != lastPortionCopyFrom)
                {
                    DialogResult boxResult = MessageBox.Show("Create missing directory for last portion and synchronize into that?", "Create missing subdirectory", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (boxResult == DialogResult.Yes)
                    {
                        if (!OldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            OldBaseDir += Path.DirectorySeparatorChar;
                        }
                        OldBaseDir += lastPortionCopyFrom;
                        Application.DoEvents();
                        OnSynchronizeAction(new EventArgs());
                    }
                    else if (boxResult == DialogResult.No)
                    {
                        OnSynchronizeAction(new EventArgs());
                    }
                }
                else
                {
                    OnSynchronizeAction(new EventArgs());
                }
            }
        }

        private void cmdSynchronizeAll_Click(object sender, EventArgs e)
        {
            if (OnSynchronizeAllAction != null)
            {
                OnSynchronizeAllAction(new EventArgs());
            }
        }

        /// <summary>
        /// Use the synchronize process to copy and delete files by comparing the source and target directory trees first.
        /// This fast version uses a previously saved snapshot to avoid having to re-scan the target directory each time.
        /// It is worth using the original synchronize method periodically just in case the target directory is changed independently of this process.
        /// The original synchronize method has been modified to take a snapshot but it doesn't read it.
        /// So if the new synchronize method is used immediately after the original one it will be working on the very latest snapshot.
        /// </summary>
        private void cmdSynchronizePlus_Click(object sender, EventArgs e)
        {
            if (OnSynchronizePlusAction != null)
            {
                string lastPortionCopyFrom = LastPortion(NewBaseDir);
                string lastPortionCopyTo = LastPortion(OldBaseDir);
                if (lastPortionCopyTo != lastPortionCopyFrom)
                {
                    DialogResult boxResult = MessageBox.Show("Create missing directory for last portion and synchronize into that?", "Create missing subdirectory", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (boxResult == DialogResult.Yes)
                    {
                        if (!OldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            OldBaseDir += Path.DirectorySeparatorChar;
                        }
                        OldBaseDir += lastPortionCopyFrom;
                        Application.DoEvents();
                        OnSynchronizePlusAction(new EventArgs());
                    }
                    else if (boxResult == DialogResult.No)
                    {
                        OnSynchronizePlusAction(new EventArgs());
                    }
                }
                else
                {
                    OnSynchronizePlusAction(new EventArgs());
                }
            }
        }

        private void cmdSynchronizePlusAll_Click(object sender, EventArgs e)
        {
            if (OnSynchronizePlusAllAction != null)
            {
                OnSynchronizePlusAllAction(new EventArgs());
            }
        }

        /// <summary>
        /// Use the synchronize process to copy and delete files by comparing the source and target directory trees first.
        /// The Synch+ version uses a previously saved snapshot to avoid having to re-scan the target directory each time.
        /// This Synch++ version uploads the comparrisons file so that the same comparisons can be downloaded
        /// and used again later in the other direction.  To avoid the comparisons file lying around and getting out of date
        /// it gets deleted as soon as it is used.  If multiple comparisons are performed in one direction then each one
        /// builds on and appends to the comparisons file until the comparisons file has the opportunity to run in the opposite direction
        /// when it gets deleted meaning that subsequent comparisons start from the beginning again.
        /// It is worth using the original synchronize method periodically just in case the target directory is changed independently of this process.
        /// The original synchronize method has been modified to take a snapshot but it doesn't read it.
        /// So if the new synchronize method is used immediately after the original one it will be working on the very latest snapshot.
        /// </summary>
        private void cmdSynchronizePlusPlus_Click(object sender, EventArgs e)
        {
            if (OnSynchronizePlusPlusAction != null)
            {
                string lastPortionCopyFrom = LastPortion(NewBaseDir);
                string lastPortionCopyTo = LastPortion(OldBaseDir);
                if (lastPortionCopyTo != lastPortionCopyFrom)
                {
                    DialogResult boxResult = MessageBox.Show("Create missing directory for last portion and synchronize into that?", "Create missing subdirectory", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (boxResult == DialogResult.Yes)
                    {
                        if (!OldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            OldBaseDir += Path.DirectorySeparatorChar;
                        }
                        OldBaseDir += lastPortionCopyFrom;
                        Application.DoEvents();
                        OnSynchronizePlusPlusAction(new EventArgs());
                    }
                    else if (boxResult == DialogResult.No)
                    {
                        OnSynchronizePlusPlusAction(new EventArgs());
                    }
                }
                else
                {
                    OnSynchronizePlusPlusAction(new EventArgs());
                }
            }
        }

        private void cmdSynchronizePlusPlusAll_Click(object sender, EventArgs e)
        {
            if (OnSynchronizePlusPlusAllAction != null)
            {
                OnSynchronizePlusPlusAllAction(new EventArgs());
            }
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            if (OnResetAction != null)
            {
                OnResetAction(new EventArgs());
            }
        }

        /// <summary>
        /// Cancel the comparison.
        /// </summary>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (OnCancelAction != null)
            {
                OnCancelAction(new EventArgs());
            }
        }

        #region Mouse Event Handlers.
        private void txtNewBaseDir_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("NewBaseDir");
        }

        private void txtNewBaseDir_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtOldBaseDir_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("OldBaseDir");
        }

        private void txtOldBaseDir_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtNewFile_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("NewFile");
        }

        private void txtNewFile_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtOldFile_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("OldFile");
        }

        private void txtOldFile_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtMinChars_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("MinChars");
        }

        private void txtMinChars_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtMinLines_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("MinLines");
        }

        private void txtMinLines_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtLimitCharacters_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("LimitCharacters");
        }

        private void txtLimitCharacters_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtLimitLines_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("LimitLines");
        }

        private void txtLimitLines_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtSubLineMatchLimit_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("SubLineMatchLimit");
        }

        private void txtSubLineMatchLimit_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }
        #endregion
        #endregion

        #region Public Methods.
        public void EnableControls(bool enabled)
        {
            grpDirectories.Enabled = enabled;
            grpOptions.Enabled = enabled;
            cmdCopy.Enabled = enabled;
            cmdCopySynch.Enabled = enabled;
            cmdSynchronize.Enabled = enabled;
            cmdSynchronizeAll.Enabled = enabled;
            cmdSynchronizePlus.Enabled = enabled;
            cmdSynchronizePlusAll.Enabled = enabled;
            cmdSynchronizePlusPlus.Enabled = enabled;
            cmdSynchronizePlusPlusAll.Enabled = enabled;
            cmdDelete.Enabled = enabled;
            cmdCancel.Enabled = !enabled;
        }

        public void ClearErrors()
        {
            StatusMessage = string.Empty;
            foreach (KeyValuePair<string, ControlMessage> pair in _fields)
            {
                ControlMessage controlMessage = pair.Value;
                controlMessage.Message = string.Empty;
                Control control = controlMessage.Control;
                control.BackColor = SystemColors.Control;
            }
            _errorCount = 0;
        }

        public void SetFieldError(string fieldName, string message)
        {
            ControlMessage controlMessage = _fields[fieldName];
            controlMessage.Message = message;
            Control control = controlMessage.Control;
            control.BackColor = Color.Pink;
            _errorCount++;
            if (_errorCount == 1)
            {
                control.Focus();
                StatusMessage = message;
            }
        }

        public string BrowseFolder(string initialDirectory, string defaultFolder)
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

        public string BrowseFile(string initialDirectory, string filter, string defaultFileSpec)
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

        public Presenter.MsgResult DisplayMessageBox(string message, string caption, Presenter.MsgButtons buttons, Presenter.MsgIcon icon)
        {
            MessageBoxButtons boxButtons;
            switch (buttons)
            {
                case Presenter.MsgButtons.AbortRetryIgnore:
                    boxButtons = MessageBoxButtons.AbortRetryIgnore;
                    break;
                case Presenter.MsgButtons.OK:
                    boxButtons = MessageBoxButtons.OK;
                    break;
                case Presenter.MsgButtons.OKCancel:
                    boxButtons = MessageBoxButtons.OKCancel;
                    break;
                case Presenter.MsgButtons.RetryCancel:
                    boxButtons = MessageBoxButtons.RetryCancel;
                    break;
                case Presenter.MsgButtons.YesNo:
                    boxButtons = MessageBoxButtons.YesNo;
                    break;
                case Presenter.MsgButtons.YesNoCancel:
                    boxButtons = MessageBoxButtons.YesNoCancel;
                    break;
                default:
                    boxButtons = MessageBoxButtons.OK;
                    break;
            }
            MessageBoxIcon boxIcon;
            switch (icon)
            {
                case Presenter.MsgIcon.Asterisk:
                    boxIcon = MessageBoxIcon.Asterisk;
                    break;
                case Presenter.MsgIcon.Error:
                    boxIcon = MessageBoxIcon.Error;
                    break;
                case Presenter.MsgIcon.Exclamation:
                    boxIcon = MessageBoxIcon.Exclamation;
                    break;
                case Presenter.MsgIcon.Hand:
                    boxIcon = MessageBoxIcon.Hand;
                    break;
                case Presenter.MsgIcon.Information:
                    boxIcon = MessageBoxIcon.Information;
                    break;
                case Presenter.MsgIcon.None:
                    boxIcon = MessageBoxIcon.None;
                    break;
                case Presenter.MsgIcon.Question:
                    boxIcon = MessageBoxIcon.Question;
                    break;
                case Presenter.MsgIcon.Stop:
                    boxIcon = MessageBoxIcon.Stop;
                    break;
                case Presenter.MsgIcon.Warning:
                    boxIcon = MessageBoxIcon.Warning;
                    break;
                default:
                    boxIcon = MessageBoxIcon.Information;
                    break;
            }
            Presenter.MsgResult result;
            DialogResult boxResult = MessageBox.Show(message, caption, boxButtons, boxIcon);
            switch (boxResult)
            {
                case DialogResult.Abort:
                    result = Presenter.MsgResult.Abort;
                    break;
                case DialogResult.Cancel:
                    result = Presenter.MsgResult.Cancel;
                    break;
                case DialogResult.Ignore:
                    result = Presenter.MsgResult.Ignore;
                    break;
                case DialogResult.No:
                    result = Presenter.MsgResult.No;
                    break;
                case DialogResult.None:
                    result = Presenter.MsgResult.None;
                    break;
                case DialogResult.OK:
                    result = Presenter.MsgResult.OK;
                    break;
                case DialogResult.Retry:
                    result = Presenter.MsgResult.Retry;
                    break;
                case DialogResult.Yes:
                    result = Presenter.MsgResult.Yes;
                    break;
                default:
                    result = Presenter.MsgResult.None;
                    break;
            }
            return result;
        }
        #endregion

        #region Private Methods.
        private void InitialiseFields()
        {
            _fields = new Dictionary<string, ControlMessage>();
            _fields.Add("NewBaseDir", new ControlMessage(txtNewBaseDir));
            _fields.Add("OldBaseDir", new ControlMessage(txtOldBaseDir));
            _fields.Add("CopyRule", new ControlMessage(grpOptions));
            dpkDateThreshold.MinDate = new DateTime(1997, 1, 1);
            dpkDateThreshold.MaxDate = DateTime.Today;
        }

        private void ShowFieldError(string fieldName)
        {
            ControlMessage controlMessage = _fields[fieldName];
            string message = controlMessage.Message;
            if (message.Trim().Length > 0)
            {
                StatusMessage = message;
            }
        }

        private void RestoreFirstError()
        {
            foreach (KeyValuePair<string, ControlMessage> pair in _fields)
            {
                ControlMessage controlMessage = pair.Value;
                string message = controlMessage.Message;
                if (message.Trim().Length > 0)
                {
                    StatusMessage = message;
                    break;
                }
            }
        }

        private long GetLongFromTextBox(TextBox textBox, long defaultValue)
        {
            long number;
            if (long.TryParse(textBox.Text, out number))
            {
                return number;
            }
            else
            {
                return defaultValue;
            }
        }

        private void SetTextBoxFromLong(TextBox textBox, long numericValue)
        {
            textBox.Text = numericValue.ToString();
        }
        #endregion

        private UserSetting.CopyRuleEnum GetCopyRuleFromRadioGroup(GroupBox groupBox, UserSetting.CopyRuleEnum defaultValue)
        {
            UserSetting.CopyRuleEnum copyRule = defaultValue;
            foreach (Control control in groupBox.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radioButton = (RadioButton)control;
                    if (radioButton.Checked)
                    {
                        string rule = radioButton.Name.Substring(3);
                        Enum.TryParse(rule, out copyRule);
                        break;
                    }
                }
            }
            return copyRule;
        }

        private void SetRadioGroupFromCopyRule(GroupBox groupBox, UserSetting.CopyRuleEnum copyRule)
        {
            string name = copyRule.ToString();
            foreach (Control control in groupBox.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radioButton = (RadioButton)control;
                    string rule = radioButton.Name.Substring(3);
                    if (rule == name)
                    {
                        radioButton.Checked = true;
                        break;
                    }
                }
            }
        }
    }
}