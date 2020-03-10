using Lookout.DataLayer.Profile;
using Lookout.Presenters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lookout.Views
{
    public partial class frmMain : Form, ILookoutViewer
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

        public string TargetOutlookFolder
        {
            get
            {
                return txtTargetOutlookFolder.Text;
            }
            set
            {
                txtTargetOutlookFolder.Text = value;
                txtTargetOutlookFolder.SelectionStart = txtTargetOutlookFolder.Text.Length;
                txtTargetOutlookFolder.ScrollToCaret();
            }
        }

        public string CriteriaName
        {
            get
            {
                return txtCriteriaName.Text;
            }
            set
            {
                txtCriteriaName.Text = value;
            }
        }

        public string SenderCriteria
        {
            get
            {
                return txtSenderCriteria.Text;
            }
            set
            {
                txtSenderCriteria.Text = value;
            }
        }

        public string SubjectCriteria
        {
            get
            {
                return txtSubjectCriteria.Text;
            }
            set
            {
                txtSubjectCriteria.Text = value;
            }
        }

        public string BodyCriteria
        {
            get
            {
                return txtBodyCriteria.Text;
            }
            set
            {
                txtBodyCriteria.Text = value;
            }
        }

        /// <summary>
        /// Not used at present.
        /// </summary>
        public bool UseMapping
        {
            get
            {
                return true;
            }
            set
            {
                bool temp = value;
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
        public event Presenter.ChooseSourceFolderHandler OnChooseSourceFolder;
        public event Presenter.ChooseTargetFolderHandler OnChooseApkFolder;
        public event Presenter.CopySourceFolderHandler OnCopySourceFolder;
        public event Presenter.CopyTargetFolderHandler OnCopyTargetFolder;
        public event Presenter.ChooseReflateFolderHandler OnChooseReflateFolder;
        public event Presenter.ChooseTargetFileHandler OnChooseApkFile;
        public event Presenter.CopyActionHandler OnCompressAction;
        public event Presenter.MoveActionHandler OnMoveAction;
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
        /// Browse for source folder.
        /// </summary>
        private void cmdChooseSourceFolder_Click(object sender, EventArgs e)
        {
            if (OnChooseSourceFolder != null)
            {
                OnChooseSourceFolder(new EventArgs());
            }
        }

        /// <summary>
		/// Browse for target folder which is the default parent directory for the target file.
		/// </summary>
		private void cmdChooseTargetFolder_Click(object sender, EventArgs e)
        {
            if (OnChooseApkFolder != null)
            {
                OnChooseApkFolder(new EventArgs());
            }
        }

        /// <summary>
        /// Copy the source folder to the target folder.
        /// </summary>
        private void txtSourceFolder_DoubleClick(object sender, EventArgs e)
        {
            if (OnCopyTargetFolder != null)
            {
                OnCopyTargetFolder(new EventArgs());
            }
        }

        /// <summary>
        /// Copy the target folder to the source folder.
        /// </summary>
        private void txtTargetFolder_DoubleClick(object sender, EventArgs e)
        {
            if (OnCopySourceFolder != null)
            {
                OnCopySourceFolder(new EventArgs());
            }
        }

        /// <summary>
        /// Choose reflate folder to test that the deflate and reflate worked.
        /// </summary>
        private void cmdChooseReflateFolder_Click(object sender, EventArgs e)
        {
            if (OnChooseReflateFolder != null)
            {
                OnChooseReflateFolder(new EventArgs());
            }
        }

        /// <summary>
        /// Choose the target file where the compressed folder contents are stored.
        /// </summary>
        private void cmdChooseTargetFile_Click(object sender, EventArgs e)
        {
            if (OnChooseApkFile != null)
            {
                OnChooseApkFile(new EventArgs());
            }
        }

        /// <summary>
        /// Run the compress using the specified locations.
        /// </summary>
        private void cmdCompress_Click(object sender, EventArgs e)
        {
            if (OnCompressAction != null)
            {
                OnCompressAction(new EventArgs());
            }
        }

        /// <summary>
        /// Move all junk mail to designated folders.
        /// </summary>
        private void cmdMove_Click(object sender, EventArgs e)
        {
            if (OnMoveAction != null)
            {
                OnMoveAction(new EventArgs());
            }
        }

        /// <summary>
        /// Cancel the process.
        /// </summary>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (OnCancelAction != null)
            {
                OnCancelAction(new EventArgs());
            }
        }

        #region Mouse Event Handlers.
        private void txtApkFolder_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("TargetOutlookFolder");
        }

        private void txtApkFolder_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }

        private void txtApkFile_MouseEnter(object sender, EventArgs e)
        {
            ShowFieldError("ApkFile");
        }

        private void txtApkFile_MouseLeave(object sender, EventArgs e)
        {
            RestoreFirstError();
        }
        #endregion
        #endregion

        #region Public Methods.
        public void EnableControls(bool enabled)
        {
            grpDirectories.Enabled = enabled;
            grpCriteria.Enabled = enabled;
            cmdMove.Enabled = enabled;
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
            _fields.Add("TargetOutlookFolder", new ControlMessage(txtTargetOutlookFolder));
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
    }
}