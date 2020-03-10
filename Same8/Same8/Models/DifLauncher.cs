using Same8.DataLayer.Profile;
using System.Diagnostics;

namespace Same8.Models
{
    /// <summary>
    /// Differenti8 launcher.
    /// </summary>
    /// <remarks>
    /// To launch Differenti8 to compare the two files on the selected line.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class DifLauncher
    {
        #region Member Variables.
        private ProfileManager _profileManager;
        private string _currentKey = string.Empty;
        #endregion

        #region Member variables.
        private string _newBaseDir = string.Empty;
        private string _oldBaseDir = string.Empty;
        private string _newFile = string.Empty;
        private string _oldFile = string.Empty;
        private long _minChars = 0;
        private long _minLines = 0;
        private long _limitCharacters = 0;
        private long _limitLines = 0;
        private long _subLineMatchLimit = 0;
        private bool _completeLines = true;
        #endregion

        #region Properties.
        /// <summary>
        /// New base directory.
        /// </summary>
        public string NewBaseDir
        {
            get
            {
                return _newBaseDir;
            }
            set
            {
                _newBaseDir = value;
            }
        }

        /// <summary>
		/// Old base directory.
		/// </summary>
		public string OldBaseDir
        {
            get
            {
                return _oldBaseDir;
            }
            set
            {
                _oldBaseDir = value;
            }
        }

        /// <summary>
		/// New file specification.
		/// </summary>
		public string NewFile
        {
            get
            {
                return _newFile;
            }
            set
            {
                _newFile = value;
            }
        }

        /// <summary>
        /// Old file specification.
        /// </summary>
        public string OldFile
        {
            get
            {
                return _oldFile;
            }
            set
            {
                _oldFile = value;
            }
        }

        /// <summary>
        /// Minimum number of characters needed to be considered a match.
        /// </summary>
        private long MinChars
        {
            get
            {
                return _minChars;
            }
            set
            {
                _minChars = value;
            }
        }

        /// <summary>
        /// Minimum number of lines needed to be considered a match.
        /// </summary>
        private long MinLines
        {
            get
            {
                return _minLines;
            }
            set
            {
                _minLines = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long LimitCharacters
        {
            get
            {
                return _limitCharacters;
            }
            set
            {
                _limitCharacters = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long LimitLines
        {
            get
            {
                return _limitLines;
            }
            set
            {
                _limitLines = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long SubLineMatchLimit
        {
            get
            {
                return _subLineMatchLimit;
            }
            set
            {
                _subLineMatchLimit = value;
            }
        }

        /// <summary>
		/// Synchronise matches with complete lines.
		/// </summary>
		private bool CompleteLines
        {
            get
            {
                return _completeLines;
            }
            set
            {
                _completeLines = value;
            }
        }
        #endregion

        #region Properties.
        #endregion

        #region Constructors.
        internal DifLauncher(ProfileManager profileManager)
        {
            _profileManager = profileManager;
            LoadParameters();
        }
        #endregion

        #region Public Methods.
        /// <summary>
        /// Compare the two files on the selected line.
        /// </summary>
        public void Compare()
        {
            SaveParameters();
            Launch();
        }
        #endregion

        #region Private Methods.
        /// <summary>
        /// Load all saved parameters.
        /// </summary>
        private void LoadParameters()
        {
            _profileManager.DiffUserSettings.Load(_profileManager.SystemProfile.DiffCurrentUserSettings, _profileManager.SystemProfile.DiffMasterUserSettings);
            DiffUserSetting diffUserSetting = new DiffUserSetting();
            if (_profileManager.DiffUserSettings.SelectedItem != null)
            {
                diffUserSetting = _profileManager.DiffUserSettings.SelectedItem;
            }
            NewBaseDir = diffUserSetting.NewBaseDir;
            OldBaseDir = diffUserSetting.OldBaseDir;
            NewFile = diffUserSetting.NewFile;
            OldFile = diffUserSetting.OldFile;
            MinChars = diffUserSetting.MinChars;
            MinLines = diffUserSetting.MinLines;
            LimitCharacters = diffUserSetting.LimitCharacters;
            LimitLines = diffUserSetting.LimitLines;
            SubLineMatchLimit = diffUserSetting.SubLineMatchLimit;
            CompleteLines = diffUserSetting.CompleteLines;
            SetCurrentKey();
        }

        /// <summary>
        /// Save all parameters.
        /// </summary>
        private void SaveParameters()
        {
            SetCurrentKey();
            DiffUserSetting diffUserSetting = new DiffUserSetting
            {
                Key = _currentKey,
                NewBaseDir = NewBaseDir,
                OldBaseDir = OldBaseDir,
                NewFile = NewFile,
                OldFile = OldFile,
                MinChars = MinChars,
                MinLines = MinLines,
                LimitCharacters = LimitCharacters,
                LimitLines = LimitLines,
                SubLineMatchLimit = SubLineMatchLimit,
                CompleteLines = CompleteLines
            };
            _profileManager.DiffUserSettings.Persist(diffUserSetting);
        }

        /// <summary>
		/// Set current key.
		/// </summary>
		private void SetCurrentKey()
        {
            if (NewFile.Trim().Length > 0)
            {
                if (OldFile.Trim().Length > 0)
                {
                    _currentKey = "[" + NewFile + "]-[" + OldFile + "]";
                }
                else
                {
                    _currentKey = "[" + NewFile + "]-[?]";
                }
            }
            else
            {
                if (OldFile.Trim().Length > 0)
                {
                    _currentKey = "[?]-[" + OldFile + "]";
                }
                else
                {
                    _currentKey = string.Empty;
                }
            }
        }

        /// <summary>
        /// Launch Differenti8 to compare the two files on the selected line.
        /// </summary>
        private void Launch()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = _profileManager.SystemProfile.DiffProgram;
            proc.StartInfo.Arguments = string.Empty;
            proc.EnableRaisingEvents = false;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            proc.Start();
        }
        #endregion
    }
}