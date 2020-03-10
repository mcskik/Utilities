using Differenti8.DataLayer.Profile;
using Differenti8.Models;
using Differenti8.Views;
using Differenti8Engine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Differenti8.Presenters
{
    public class Presenter
    {
        #region Constants.
        const string FILE_FILTER = "All files (*.*)|*.*|Text files (*.txt)|*.txt";
        #endregion

        #region Enumerations.
        public enum MsgIcon
        {
            Asterisk = 1,
            Error = 2,
            Exclamation = 3,
            Hand = 4,
            Information = 5,
            None = 6,
            Question = 7,
            Stop = 8,
            Warning = 9
        }

        public enum MsgButtons
        {
            AbortRetryIgnore = 1,
            OK = 2,
            OKCancel = 3,
            RetryCancel = 4,
            YesNo = 5,
            YesNoCancel = 6
        }

        public enum MsgResult
        {
            Abort = 1,
            Cancel = 2,
            Ignore = 3,
            No = 4,
            None = 5,
            OK = 6,
            Retry = 7,
            Yes = 8
        }

        public enum CursorModes
        {
            Normal = 1,
            Wait = 2
        }
        #endregion

        #region Member Variables.
        private ICompareEngine _compareEngine;
        private ICompareViewer _compareViewer;
        private IProfileManager _profileManager;
        //TODO: May need to change to more general ReportEngine in future.
        private ReportEngine _reportEngine;
        private string _currentKey = string.Empty;
        private ProgressLink _currentActivityProgress;
        private ProgressLink _overallProgress;
        private long _originalMaximum = 0;
        #endregion

        #region Properties.
        private string NewBaseDir
        {
            get
            {
                string initialPath = Directory.Exists(_compareViewer.NewBaseDir) ? _compareViewer.NewBaseDir : string.Empty;
                string newBaseDir = initialPath.Length > 0 ? initialPath : _profileManager.SystemProfile.HomePath;
                return newBaseDir;
            }
        }

        private string OldBaseDir
        {
            get
            {
                string initialPath = Directory.Exists(_compareViewer.OldBaseDir) ? _compareViewer.OldBaseDir : string.Empty;
                string oldBaseDir = initialPath.Length > 0 ? initialPath : _profileManager.SystemProfile.HomePath;
                return oldBaseDir;
            }
        }
        #endregion

        #region Constructors.
        internal Presenter(ICompareEngine compareEngine, ICompareViewer compareViewer, IProfileManager profileManager)
        {
            _compareEngine = compareEngine;
            _compareViewer = compareViewer;
            _compareViewer.OnPreviousSelectionSelectedIndexChanged += new PreviousSelectionSelectedIndexChangedHandler(_compareViewer_OnPreviousSelectionSelectedIndexChanged);
            _compareViewer.OnRemovePreviousSelection += new RemovePreviousSelectionHandler(_compareViewer_OnRemovePreviousSelection);
            _compareViewer.OnChooseNewBaseDirectory += new ChooseNewBaseDirectoryHandler(_compareViewer_OnChooseNewBaseDirectory);
            _compareViewer.OnChooseOldBaseDirectory += new ChooseOldBaseDirectoryHandler(_compareViewer_OnChooseOldBaseDirectory);
            _compareViewer.OnCopyNewBaseDirectory += new CopyNewBaseDirectoryHandler(_compareViewer_OnCopyNewBaseDirectory);
            _compareViewer.OnCopyOldBaseDirectory += new CopyOldBaseDirectoryHandler(_compareViewer_OnCopyOldBaseDirectory);
            _compareViewer.OnChooseNewFile += new ChooseNewFileHandler(_compareViewer_OnChooseNewFile);
            _compareViewer.OnChooseOldFile += new ChooseOldFileHandler(_compareViewer_OnChooseOldFile);
            _compareViewer.OnCompareAction += new CompareActionHandler(_compareViewer_OnCompareAction);
            _compareViewer.OnCompareMeldAction += new CompareMeldActionHandler(_compareViewer_OnCompareMeldAction);
            _compareViewer.OnCancelAction += new CancelActionHandler(_compareViewer_OnCancelAction);
            _profileManager = profileManager;
            _currentActivityProgress = new ProgressLink();
            _overallProgress = new ProgressLink();
            _compareViewer.EnableControls(true);
            LoadParameters();
            RefreshCombo();
        }
        #endregion

        #region Custom Event Arguments.
        //public class NodeRequestEventArgs : EventArgs
        //{
        //    public string ChosenNode { get; set; }
        //    public NodeRequestEventArgs(string node)
        //    {
        //        ChosenNode = node;
        //    }
        //}
        #endregion

        #region Delegates.
        public delegate void PreviousSelectionSelectedIndexChangedHandler(EventArgs e);
        public delegate void RemovePreviousSelectionHandler(EventArgs e);
        public delegate void ChooseNewBaseDirectoryHandler(EventArgs e);
        public delegate void ChooseOldBaseDirectoryHandler(EventArgs e);
        public delegate void CopyNewBaseDirectoryHandler(EventArgs e);
        public delegate void CopyOldBaseDirectoryHandler(EventArgs e);
        public delegate void ChooseNewFileHandler(EventArgs e);
        public delegate void ChooseOldFileHandler(EventArgs e);
        public delegate void CompareActionHandler(EventArgs e);
        public delegate void CompareMeldActionHandler(EventArgs e);
        public delegate void CancelActionHandler(EventArgs e);
        #endregion

        #region Event Handlers.
        #region Main Form Event Handlers.
        void _compareViewer_OnPreviousSelectionSelectedIndexChanged(EventArgs e)
        {
            _compareViewer.ClearErrors();
            UserSetting oSetting = _compareViewer.PreviousSelections_SelectedItem;
            _profileManager.UserSettings.Select(oSetting.Key);
            LoadParameters();
        }

        void _compareViewer_OnRemovePreviousSelection(EventArgs e)
        {
            _compareViewer.ClearErrors();
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
                _profileManager.UserSettings.Delete(_profileManager.UserSettings.SelectedItem.Key);
            }
            LoadParameters();
            RefreshCombo();
        }

        void _compareViewer_OnChooseNewBaseDirectory(EventArgs e)
        {
            _compareViewer.ClearErrors();
            _compareViewer.NewBaseDir = _compareViewer.BrowseFolder(NewBaseDir, _compareViewer.NewBaseDir);
        }

        void _compareViewer_OnChooseOldBaseDirectory(EventArgs e)
        {
            _compareViewer.ClearErrors();
            _compareViewer.OldBaseDir = _compareViewer.BrowseFolder(OldBaseDir, _compareViewer.OldBaseDir);
        }

        void _compareViewer_OnCopyNewBaseDirectory(EventArgs e)
        {
            _compareViewer.OldBaseDir = _compareViewer.NewBaseDir;
        }

        void _compareViewer_OnCopyOldBaseDirectory(EventArgs e)
        {
            _compareViewer.NewBaseDir = _compareViewer.OldBaseDir;
        }

        void _compareViewer_OnChooseNewFile(EventArgs e)
        {
            _compareViewer.ClearErrors();
            _compareViewer.NewFile = _compareViewer.BrowseFile(NewBaseDir, FILE_FILTER, _compareViewer.NewFile);
            SetCurrentKey();
        }

        void _compareViewer_OnChooseOldFile(EventArgs e)
        {
            _compareViewer.ClearErrors();
            _compareViewer.OldFile = _compareViewer.BrowseFile(OldBaseDir, FILE_FILTER, _compareViewer.OldFile);
            SetCurrentKey();
        }

        void _compareViewer_OnCompareAction(EventArgs e)
        {
            _compareViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _compareViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _compareViewer.CurrentActivityProgress = _currentActivityProgress;
            if (_compareEngine.Interrupt != null)
            {
                _compareEngine.Interrupt.Reason = "OK";
            }

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            SaveParameters();
            RefreshCombo();

            //Comparison.
            _compareEngine = new CompareEngine();
            _compareEngine.OnBeginSegment += new CompareEngine.BeginSegmentHandler(_compareEngine_OnBeginSegment);
            _compareEngine.OnUpdateSegment += new CompareEngine.UpdateSegmentHandler(_compareEngine_OnUpdateSegment);
            _compareEngine.OnEndOfSegment += new CompareEngine.EndOfSegmentHandler(_compareEngine_OnEndOfSegment);
            _compareEngine.OnBeginCompare += new CompareEngine.BeginCompareHandler(_compareEngine_OnBeginCompare);
            _compareEngine.OnUpdateCompare += new CompareEngine.UpdateCompareHandler(_compareEngine_OnUpdateCompare);
            _compareEngine.OnEndOfCompare += new CompareEngine.EndOfCompareHandler(_compareEngine_OnEndOfCompare);
            _compareEngine.OnBeginReport += new CompareEngine.BeginReportHandler(_compareEngine_OnBeginReport);
            _compareEngine.OnUpdateReport += new CompareEngine.UpdateReportHandler(_compareEngine_OnUpdateReport);
            _compareEngine.OnEndOfReport += new CompareEngine.EndOfReportHandler(_compareEngine_OnEndOfReport);
            _compareEngine.MinChars = _profileManager.UserSettings.SelectedItem.MinChars;
            _compareEngine.MinLines = _profileManager.UserSettings.SelectedItem.MinLines;
            _compareEngine.LimitCharacters = _profileManager.UserSettings.SelectedItem.LimitCharacters;
            _compareEngine.LimitLines = _profileManager.UserSettings.SelectedItem.LimitLines;
            _compareEngine.SubLineMatchLimit = _profileManager.UserSettings.SelectedItem.SubLineMatchLimit;
            _compareEngine.CompleteLines = _profileManager.UserSettings.SelectedItem.CompleteLines;
            _compareEngine.Interrupt = _profileManager.Interrupt;
            try
            {
                _compareEngine.CompareFiles(_profileManager.UserSettings.SelectedItem.NewFile, _profileManager.UserSettings.SelectedItem.OldFile);
                string reportViewer = _profileManager.ProfileCache.Fetch("ReportViewer");
                switch (reportViewer)
                {
                    case "ViewerHtml":
                        _reportEngine = new ReportEngineHtml(_compareEngine, _profileManager);
                        _reportEngine.OnBeginReport += new ReportEngine.BeginReportHandler(_reportEngine_OnBeginReport);
                        _reportEngine.OnUpdateReport += new ReportEngine.UpdateReportHandler(_reportEngine_OnUpdateReport);
                        _reportEngine.OnEndOfReport += new ReportEngine.EndOfReportHandler(_reportEngine_OnEndOfReport);
                        _reportEngine.Report();
                        View(_profileManager.ProfileCache.Fetch("ViewerHtml"), _profileManager.ProfileCache.Fetch("HtmlOutputFile"));
                        break;
                    case "ViewerUnix":
                        _reportEngine = new ReportEngineText(_compareEngine, _profileManager);
                        _reportEngine.OnBeginReport += new ReportEngine.BeginReportHandler(_reportEngine_OnBeginReport);
                        _reportEngine.OnUpdateReport += new ReportEngine.UpdateReportHandler(_reportEngine_OnUpdateReport);
                        _reportEngine.OnEndOfReport += new ReportEngine.EndOfReportHandler(_reportEngine_OnEndOfReport);
                        _reportEngine.Report();
                        break;
                    case "Both":
                        _reportEngine = new ReportEngineText(_compareEngine, _profileManager);
                        _reportEngine.OnBeginReport += new ReportEngine.BeginReportHandler(_reportEngine_OnBeginReport);
                        _reportEngine.OnUpdateReport += new ReportEngine.UpdateReportHandler(_reportEngine_OnUpdateReport);
                        _reportEngine.OnEndOfReport += new ReportEngine.EndOfReportHandler(_reportEngine_OnEndOfReport);
                        _reportEngine.Report();
                        _reportEngine = new ReportEngineHtml(_compareEngine, _profileManager);
                        _reportEngine.OnBeginReport += new ReportEngine.BeginReportHandler(_reportEngine_OnBeginReport);
                        _reportEngine.OnUpdateReport += new ReportEngine.UpdateReportHandler(_reportEngine_OnUpdateReport);
                        _reportEngine.OnEndOfReport += new ReportEngine.EndOfReportHandler(_reportEngine_OnEndOfReport);
                        _reportEngine.Report();
                        View(_profileManager.ProfileCache.Fetch("ViewerHtml"), _profileManager.ProfileCache.Fetch("HtmlOutputFile"));
                        break;
                    default:
                        _reportEngine = new ReportEngineText(_compareEngine, _profileManager);
                        _reportEngine.OnBeginReport += new ReportEngine.BeginReportHandler(_reportEngine_OnBeginReport);
                        _reportEngine.OnUpdateReport += new ReportEngine.UpdateReportHandler(_reportEngine_OnUpdateReport);
                        _reportEngine.OnEndOfReport += new ReportEngine.EndOfReportHandler(_reportEngine_OnEndOfReport);
                        _reportEngine.Report();
                        break;
                }
            }
            catch (ParameterException pe)
            {
                _compareViewer.SetFieldError(pe.Parameter, pe.Message);
                //_compareViewer.DisplayMessageBox(ae.Message, _compareViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _compareViewer.EnableControls(true);
            }
        }

        void _compareViewer_OnCompareMeldAction(EventArgs e)
        {
            _compareViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _compareViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _compareViewer.CurrentActivityProgress = _currentActivityProgress;
            if (_compareEngine.Interrupt != null)
            {
                _compareEngine.Interrupt.Reason = "OK";
            }

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            SaveParameters();
            RefreshCombo();

            //Comparison with Meld.
            try
            {
                MeldCompare(_profileManager.UserSettings.SelectedItem.NewFile, _profileManager.UserSettings.SelectedItem.OldFile);
            }
            catch (ParameterException pe)
            {
                _compareViewer.SetFieldError(pe.Parameter, pe.Message);
                //_compareViewer.DisplayMessageBox(ae.Message, _compareViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _compareViewer.EnableControls(true);
            }
        }



        void _compareViewer_OnCancelAction(EventArgs e)
        {
            _compareViewer.EnableControls(true);
            _compareEngine.Interrupt.Reason = "Cancel";
        }
        #endregion

        #region Progress Event Handlers.
        void _compareEngine_OnBeginSegment(CompareEngine.BeginSegmentEventArgs e)
        {
            ActionBeginProgress(_currentActivityProgress, e.SegmentLength);
            _compareViewer.CurrentActivityProgress = _currentActivityProgress;
        }

        void _compareEngine_OnUpdateSegment(CompareEngine.UpdateSegmentEventArgs e)
        {
            ActionUpdateProgress(_currentActivityProgress, e.Increment);
            _compareViewer.CurrentActivityProgress = _currentActivityProgress;
        }

        void _compareEngine_OnEndOfSegment(CompareEngine.EndOfSegmentEventArgs e)
        {
            ActionEndOfProgress(_currentActivityProgress);
            _compareViewer.CurrentActivityProgress = _currentActivityProgress;
        }

        void _compareEngine_OnBeginCompare(CompareEngine.BeginCompareEventArgs e)
        {
            _compareViewer.CursorMode = CursorModes.Wait;
            _compareViewer.StatusMessage = "Comparing ...";
            ActionBeginProgress(_overallProgress, e.CompareLength);
            _compareViewer.OverallProgress = _overallProgress;
        }

        void _compareEngine_OnUpdateCompare(CompareEngine.UpdateCompareEventArgs e)
        {
            ActionUpdateProgress(_overallProgress, e.Increment);
            _compareViewer.OverallProgress = _overallProgress;
            if (_overallProgress.Maximum > 897631)
            {
                string temp = string.Empty;
            }
            _compareViewer.StatusMessage = String.Format("Comparing: {0} of {1}", _overallProgress.Value, _overallProgress.Maximum);
        }

        void _compareEngine_OnEndOfCompare(CompareEngine.EndOfCompareEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _compareViewer.OverallProgress = _overallProgress;
            _compareViewer.StatusMessage = "End of Compare";
            _compareViewer.CursorMode = CursorModes.Normal;
        }

        void _compareEngine_OnBeginReport(CompareEngine.BeginReportEventArgs e)
        {
            _currentActivityProgress = new ProgressLink();
            _overallProgress = new ProgressLink();
            _compareViewer.CursorMode = CursorModes.Wait;
            _compareViewer.StatusMessage = "Reporting ...";
            ActionBeginProgress(_overallProgress, e.ReportLength);
            _compareViewer.OverallProgress = _overallProgress;
        }

        void _compareEngine_OnUpdateReport(CompareEngine.UpdateReportEventArgs e)
        {
            ActionUpdateProgress(_overallProgress, e.Increment);
            _compareViewer.OverallProgress = _overallProgress;
            _compareViewer.StatusMessage = String.Format("Reporting: {0} of {1}", _overallProgress.Value, _overallProgress.Maximum);
        }

        void _compareEngine_OnEndOfReport(CompareEngine.EndOfReportEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _compareViewer.OverallProgress = _overallProgress;
            _compareViewer.StatusMessage = "Complete";
            _compareViewer.CursorMode = CursorModes.Normal;
        }

        void _reportEngine_OnBeginReport(ReportEngine.BeginReportEventArgs e)
        {
            _currentActivityProgress = new ProgressLink();
            _overallProgress = new ProgressLink();
            _compareViewer.CursorMode = CursorModes.Wait;
            _compareViewer.StatusMessage = "Writing ...";
            ActionBeginProgress(_overallProgress, e.ReportLength);
            _compareViewer.OverallProgress = _overallProgress;
        }

        void _reportEngine_OnUpdateReport(ReportEngine.UpdateReportEventArgs e)
        {
            ActionUpdateProgress(_overallProgress, e.Increment);
            _compareViewer.OverallProgress = _overallProgress;
            _compareViewer.StatusMessage = String.Format("Writing: {0} of {1}", _overallProgress.Value, _overallProgress.Maximum);
        }

        void _reportEngine_OnEndOfReport(ReportEngine.EndOfReportEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _compareViewer.OverallProgress = _overallProgress;
            _compareViewer.StatusMessage = "Complete";
            _compareViewer.CursorMode = CursorModes.Normal;
        }
        #endregion

        #region Progress Event Helper Methods.
        private void ActionBeginProgress(ProgressLink progressLink, long length)
        {
            _originalMaximum = length;
            progressLink.Minimum = 0;
            progressLink.Maximum = (int)_originalMaximum;
            progressLink.Value = 0;
        }

        private void ActionUpdateProgress(ProgressLink progressLink, long increment)
        {
            if (progressLink.Value + (int)increment <= progressLink.Maximum)
            {
                progressLink.Value += (int)increment;
            }
            else
            {
                progressLink.Maximum += (int)increment;
                //progressLink.Maximum += (int)_originalMaximum;
                progressLink.Value += (int)increment;
            }
        }

        private void ActionEndOfProgress(ProgressLink progressLink)
        {
            progressLink.Value = progressLink.Maximum;
        }
        #endregion
        #endregion

        #region Private Methods.
        /// <summary>
        /// Launch Meld to compare the two specified files.
        /// </summary>
        public void MeldCompare(string fileSpecNew, string fileSpecOld)
        {
            try
            {
                Process oProc = new Process();
                oProc.StartInfo.FileName = _profileManager.SystemProfile.MeldProgram;
                oProc.StartInfo.Arguments = String.Format(@"""{0}"" ""{1}""", fileSpecNew, fileSpecOld);
                //oProc.StartInfo.WorkingDirectory = @"C:\Temp\";
                oProc.EnableRaisingEvents = false;
                oProc.StartInfo.UseShellExecute = true;
                oProc.StartInfo.CreateNoWindow = false;
                oProc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                oProc.Start();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Load all saved parameters.
        /// </summary>
        private void LoadParameters()
        {
            _profileManager.UserSettings.Load(_profileManager.SystemProfile.CurrentUserSettings, _profileManager.SystemProfile.MasterUserSettings);
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
            }
            _compareViewer.NewBaseDir = userSetting.NewBaseDir;
            _compareViewer.OldBaseDir = userSetting.OldBaseDir;
            _compareViewer.NewFile = userSetting.NewFile;
            _compareViewer.OldFile = userSetting.OldFile;
            _compareViewer.MinChars = userSetting.MinChars;
            _compareViewer.MinLines = userSetting.MinLines;
            _compareViewer.LimitCharacters = userSetting.LimitCharacters;
            _compareViewer.LimitLines = userSetting.LimitLines;
            _compareViewer.SubLineMatchLimit = userSetting.SubLineMatchLimit;
            _compareViewer.CompleteLines = userSetting.CompleteLines;
            SetCurrentKey();
        }

        /// <summary>
        /// Save all parameters.
        /// </summary>
        private void SaveParameters()
        {
            SetCurrentKey();
            UserSetting userSetting = new UserSetting
            {
                Key = _currentKey,
                NewBaseDir = _compareViewer.NewBaseDir,
                OldBaseDir = _compareViewer.OldBaseDir,
                NewFile = _compareViewer.NewFile,
                OldFile = _compareViewer.OldFile,
                MinChars = _compareViewer.MinChars,
                MinLines = _compareViewer.MinLines,
                LimitCharacters = _compareViewer.LimitCharacters,
                LimitLines = _compareViewer.LimitLines,
                SubLineMatchLimit = _compareViewer.SubLineMatchLimit,
                CompleteLines = _compareViewer.CompleteLines
            };
            _profileManager.UserSettings.Persist(userSetting);
            LoadParameters();
        }

        /// <summary>
        /// Refresh ComboBox at the begining or when a new entry has been added.
        /// </summary>
        private void RefreshCombo()
        {
            _compareViewer.PreviousSelections_IndexChanged_Event_On = false;
            _compareViewer.PreviousSelections_DataSource = _profileManager.UserSettings.Values.ToList();
            _compareViewer.PreviousSelections_DisplayMember = "KeyShrunk";
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
            }
            _compareViewer.PreviousSelections_SelectedString = userSetting.KeyShrunk;
            if (_compareViewer.PreviousSelections_Count == 0)
            {
                _compareViewer.PreviousSelections_Text = string.Empty;
            }
            _compareViewer.PreviousSelections_IndexChanged_Event_On = true;
        }

        /// <summary>
        /// Set current key.
        /// </summary>
        private void SetCurrentKey()
        {
            if (_compareViewer.NewFile.Trim().Length > 0)
            {
                if (_compareViewer.OldFile.Trim().Length > 0)
                {
                    _currentKey = "[" + _compareViewer.NewFile + "]-[" + _compareViewer.OldFile + "]";
                }
                else
                {
                    _currentKey = "[" + _compareViewer.NewFile + "]-[?]";
                }
            }
            else
            {
                if (_compareViewer.OldFile.Trim().Length > 0)
                {
                    _currentKey = "[?]-[" + _compareViewer.OldFile + "]";
                }
                else
                {
                    _currentKey = string.Empty;
                }
            }
        }

        /// <summary>
        /// Validate all fields.
        /// </summary>
        private int ValidateFields()
        {
            _compareViewer.ClearErrors();
            int errorCount = 0;
            if (!Directory.Exists(_compareViewer.NewBaseDir))
            {
                errorCount++;
                _compareViewer.SetFieldError("NewBaseDir", "New base directory not found");
            }
            if (!Directory.Exists(_compareViewer.OldBaseDir))
            {
                errorCount++;
                _compareViewer.SetFieldError("OldBaseDir", "Old base directory not found");
            }
            if (!File.Exists(_compareViewer.NewFile))
            {
                errorCount++;
                _compareViewer.SetFieldError("NewFile", "New file not found");
            }
            if (!File.Exists(_compareViewer.OldFile))
            {
                errorCount++;
                _compareViewer.SetFieldError("OldFile", "Old file not found");
            }
            if (_compareViewer.MinChars < 1 || _compareViewer.MinChars > 80)
            {
                errorCount++;
                _compareViewer.SetFieldError("MinChars", "The minimum number of characters to match must be between 1 and 80 inclusive");
            }
            if (_compareViewer.MinLines < 1 || _compareViewer.MinLines > 100)
            {
                errorCount++;
                _compareViewer.SetFieldError("MinLines", "The minimum number of lines to match must be between 1 and 100 inclusive");
            }
            if (_compareViewer.LimitCharacters < 1 || _compareViewer.LimitCharacters > 4096)
            {
                errorCount++;
                _compareViewer.SetFieldError("LimitCharacters", "The character limit beyond which the best match is considered to have been found must be between 1 and 4096 inclusive");
            }
            if (_compareViewer.LimitLines < 1 || _compareViewer.LimitLines > 255)
            {
                errorCount++;
                _compareViewer.SetFieldError("LimitLines", "The line limit beyond which the best match is considered to have been found must be between 1 and 255 inclusive");
            }
            if (_compareViewer.SubLineMatchLimit < 1 || _compareViewer.SubLineMatchLimit > 4096)
            {
                errorCount++;
                _compareViewer.SetFieldError("SubLineMatchLimit", "The sub line match limit must be between 1 and 4096 inclusive");
            }
            return errorCount;
        }

        public void View(string viewerProgram, string fileSpec)
        {
            Process process = new Process();
            process.StartInfo.FileName = viewerProgram;
            process.StartInfo.Arguments = fileSpec;
            process.EnableRaisingEvents = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            process.Start();
        }
        #endregion
    }
}