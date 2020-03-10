using Differenti8.DataLayer.Profile;
using Differenti8.Presenters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Differenti8.Views
{
    public partial class frmMain : Form, ICompareViewer
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

		public string NewFile
		{
			get
			{
				return txtNewFile.Text;
			}
			set
			{
				txtNewFile.Text = value;
				txtNewFile.SelectionStart = txtNewFile.Text.Length;
				txtNewFile.ScrollToCaret();
			}
		}

		public string OldFile
		{
			get
			{
				return txtOldFile.Text;
			}
			set
			{
				txtOldFile.Text = value;
				txtOldFile.SelectionStart = txtOldFile.Text.Length;
				txtOldFile.ScrollToCaret();
			}
		}

		public long MinChars
		{
			get
			{
				return GetLongFromTextBox(txtMinChars, 0);
			}
			set
			{
				SetTextBoxFromLong(txtMinChars, value);
			}
		}

		public long MinLines
		{
			get
			{
				return GetLongFromTextBox(txtMinLines, 0);
			}
			set
			{
				SetTextBoxFromLong(txtMinLines, value);
			}
		}

        public long LimitCharacters
		{
			get
			{
				return GetLongFromTextBox(txtLimitCharacters, 0);
			}
			set
			{
				SetTextBoxFromLong(txtLimitCharacters, value);
			}
		}

        public long LimitLines
        {
            get
            {
                return GetLongFromTextBox(txtLimitLines, 0);
            }
            set
            {
                SetTextBoxFromLong(txtLimitLines, value);
            }
        }

        public long SubLineMatchLimit
        {
            get
            {
                return GetLongFromTextBox(txtSubLineMatchLimit, 0);
            }
            set
            {
                SetTextBoxFromLong(txtSubLineMatchLimit, value);
            }
        }

        public bool CompleteLines
		{
			get
			{
				return chkCompleteLines.Checked;
			}
			set
			{
				chkCompleteLines.Checked = value;
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
        public event Presenter.ChooseNewFileHandler OnChooseNewFile;
		public event Presenter.ChooseOldFileHandler OnChooseOldFile;
		public event Presenter.CompareActionHandler OnCompareAction;
        public event Presenter.CompareMeldActionHandler OnCompareMeldAction;
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

        /// <summary>
		/// Browse for new file starting at the base directory if present.
		/// </summary>
		private void cmdChooseNewFile_Click(object sender, EventArgs e)
		{
			if (OnChooseNewFile != null)
			{
				OnChooseNewFile(new EventArgs());
			}
		}

		/// <summary>
		/// Browse for old file starting at the base directory if present.
		/// </summary>
		private void cmdChooseOldFile_Click(object sender, EventArgs e)
		{
			if (OnChooseOldFile != null)
			{
				OnChooseOldFile(new EventArgs());
			}
		}

		/// <summary>
		/// Run the comparison using the specified files.
		/// </summary>
		private void cmdCompare_Click(object sender, EventArgs e)
		{
			if (OnCompareAction != null)
			{
				OnCompareAction(new EventArgs());
			}
		}

        /// <summary>
        /// Run the comparison with "Meld" using the specified files.
        /// </summary>
        private void CmdCompareMeld_Click(object sender, EventArgs e)
        {
            if (OnCompareMeldAction != null)
            {
                OnCompareMeldAction(new EventArgs());
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
            cmdCompare.Enabled = enabled;
            cmdCompareMeld.Enabled = enabled;
            cmdCancel.Enabled = !enabled;
        }

        public void ClearErrors()
		{
			StatusMessage = string.Empty;
			foreach (KeyValuePair<string,ControlMessage> pair in _fields)
			{
				ControlMessage controlMessage = pair.Value;
				controlMessage.Message = string.Empty;
				Control control = controlMessage.Control;
				control.BackColor = Color.White;
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
			_fields.Add("NewFile", new ControlMessage(txtNewFile));
			_fields.Add("OldFile", new ControlMessage(txtOldFile));
			_fields.Add("MinChars", new ControlMessage(txtMinChars));
			_fields.Add("MinLines", new ControlMessage(txtMinLines));
			_fields.Add("LimitCharacters", new ControlMessage(txtLimitCharacters));
            _fields.Add("LimitLines", new ControlMessage(txtLimitLines));
            _fields.Add("SubLineMatchLimit", new ControlMessage(txtSubLineMatchLimit));
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