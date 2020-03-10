using Copy8.DataLayer.Profile;
using Copy8.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Copy8.Presenters
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
        private ICopyEngine _copyEngine;
        private ICopyViewer _copyViewer;
        private DirectoryEngine _directoryEngine;
        private SynchronizeEngine _synchronizeEngine;
        private IProfileManager _profileManager;
        //TODO: May need to change to more general ReportEngine in future.
        private string _currentKey = string.Empty;
        private ProgressLink _currentActivityProgress;
        private ProgressLink _overallProgress;
        private long _TotalBytes = 0;
        private long _SoFarBytes = 0;

        //TODO: Added to support the DirectoryEngine.
        private Comparisons<Comparison> _comparisons;
        private bool mbIdentical = false;
        private long mnNewFilesEstimate = 0;
        private long mnOldFilesEstimate = 0;
        private long mnChgFilesEstimate = 0;
        private bool _processingNew = false;
        DirectoryEntries _OldDirectoryListing;
        DirectoryEntries _NewDirectoryListing;
        DirectoryEntries _ChangedDirectoryListing;
        #endregion

        #region Properties.
        private string NewBaseDir
        {
            get
            {
                string initialPath = Directory.Exists(_copyViewer.NewBaseDir) ? _copyViewer.NewBaseDir : string.Empty;
                string newBaseDir = initialPath.Length > 0 ? initialPath : _profileManager.SystemProfile.HomePath;
                return newBaseDir;
            }
        }

        private string OldBaseDir
        {
            get
            {
                string initialPath = Directory.Exists(_copyViewer.OldBaseDir) ? _copyViewer.OldBaseDir : string.Empty;
                string oldBaseDir = initialPath.Length > 0 ? initialPath : _profileManager.SystemProfile.HomePath;
                return oldBaseDir;
            }
        }
        #endregion

        #region Constructors.
        internal Presenter(ICopyEngine copyEngine, ICopyViewer copyViewer, DirectoryEngine directoryEngine, SynchronizeEngine synchronizeEngine, IProfileManager profileManager)
        {
            _copyEngine = copyEngine;
            _copyViewer = copyViewer;
            _directoryEngine = directoryEngine;
            _synchronizeEngine = synchronizeEngine;
            _copyViewer.OnPreviousSelectionSelectedIndexChanged += new PreviousSelectionSelectedIndexChangedHandler(_copyViewer_OnPreviousSelectionSelectedIndexChanged);
            _copyViewer.OnRemovePreviousSelection += new RemovePreviousSelectionHandler(_copyViewer_OnRemovePreviousSelection);
            _copyViewer.OnChooseNewBaseDirectory += new ChooseNewBaseDirectoryHandler(_copyViewer_OnChooseNewBaseDirectory);
            _copyViewer.OnChooseOldBaseDirectory += new ChooseOldBaseDirectoryHandler(_copyViewer_OnChooseOldBaseDirectory);
            _copyViewer.OnCopyNewBaseDirectory += new CopyNewBaseDirectoryHandler(_copyViewer_OnCopyNewBaseDirectory);
            _copyViewer.OnCopyOldBaseDirectory += new CopyOldBaseDirectoryHandler(_copyViewer_OnCopyOldBaseDirectory);
            _copyViewer.OnCopyAction += new CopyActionHandler(_copyViewer_OnCopyAction);
            _copyViewer.OnDeleteAction += new DeleteActionHandler(_copyViewer_OnDeleteAction);
            _copyViewer.OnSynchronizePreventDeleteAction += new SynchronizePreventDeleteActionHandler(_copyViewer_OnSynchronizePreventDeleteAction);
            _copyViewer.OnSynchronizeAction += new SynchronizeActionHandler(_copyViewer_OnSynchronizeAction);
            _copyViewer.OnSynchronizeAllAction += new SynchronizeAllActionHandler(_copyViewer_OnSynchronizeAllAction);
            _copyViewer.OnSynchronizePlusAction += new SynchronizePlusActionHandler(_copyViewer_OnSynchronizePlusAction);
            _copyViewer.OnSynchronizePlusAllAction += new SynchronizePlusAllActionHandler(_copyViewer_OnSynchronizePlusAllAction);
            _copyViewer.OnCancelAction += new CancelActionHandler(_copyViewer_OnCancelAction);
            _profileManager = profileManager;
            _currentActivityProgress = new ProgressLink();
            _overallProgress = new ProgressLink();
            _copyViewer.EnableControls(true);
            LoadParameters();
            RefreshCombo();
        }
        #endregion

        #region Delegates.
        public delegate void PreviousSelectionSelectedIndexChangedHandler(EventArgs e);
        public delegate void RemovePreviousSelectionHandler(EventArgs e);
        public delegate void ChooseNewBaseDirectoryHandler(EventArgs e);
        public delegate void ChooseOldBaseDirectoryHandler(EventArgs e);
        public delegate void CopyNewBaseDirectoryHandler(EventArgs e);
        public delegate void CopyOldBaseDirectoryHandler(EventArgs e);
        public delegate void CopyActionHandler(EventArgs e);
        public delegate void DeleteActionHandler(EventArgs e);
        public delegate void SynchronizePreventDeleteActionHandler(EventArgs e);
        public delegate void SynchronizeActionHandler(EventArgs e);
        public delegate void SynchronizeAllActionHandler(EventArgs e);
        public delegate void SynchronizePlusActionHandler(EventArgs e);
        public delegate void SynchronizePlusAllActionHandler(EventArgs e);
        public delegate void CancelActionHandler(EventArgs e);
        #endregion

        #region Event Handlers.
        #region Main Form Event Handlers.
        void _copyViewer_OnPreviousSelectionSelectedIndexChanged(EventArgs e)
        {
            _copyViewer.ClearErrors();
            UserSetting oSetting = _copyViewer.PreviousSelections_SelectedItem;
            _profileManager.UserSettings.Select(oSetting.Key);
            LoadParameters();
        }

        void _copyViewer_OnRemovePreviousSelection(EventArgs e)
        {
            _copyViewer.ClearErrors();
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
                _profileManager.UserSettings.Delete(_profileManager.UserSettings.SelectedItem.Key);
            }
            LoadParameters();
            RefreshCombo();
        }

        void _copyViewer_OnChooseNewBaseDirectory(EventArgs e)
        {
            _copyViewer.ClearErrors();
            _copyViewer.NewBaseDir = _copyViewer.BrowseFolder(NewBaseDir, _copyViewer.NewBaseDir);
            SetCurrentKey();
        }

        void _copyViewer_OnChooseOldBaseDirectory(EventArgs e)
        {
            _copyViewer.ClearErrors();
            _copyViewer.OldBaseDir = _copyViewer.BrowseFolder(OldBaseDir, _copyViewer.OldBaseDir);
            SetCurrentKey();
        }

        void _copyViewer_OnCopyNewBaseDirectory(EventArgs e)
        {
            _copyViewer.OldBaseDir = _copyViewer.NewBaseDir;
        }

        void _copyViewer_OnCopyOldBaseDirectory(EventArgs e)
        {
            _copyViewer.NewBaseDir = _copyViewer.OldBaseDir;
        }

        void _copyViewer_OnCopyAction(EventArgs e)
        {
            _copyViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _copyViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Copy.
            _copyEngine = new CopyEngine();
            _copyEngine.OnBeginCount += new CopyEngine.BeginCountHandler(_copyEngine_OnBeginCount);
            _copyEngine.OnUpdateCount += new CopyEngine.UpdateCountHandler(_copyEngine_OnUpdateCount);
            _copyEngine.OnEndOfCount += new CopyEngine.EndOfCountHandler(_copyEngine_OnEndOfCount);
            _copyEngine.OnBeginCopy += new CopyEngine.BeginCopyHandler(_copyEngine_OnBeginCopy);
            _copyEngine.OnUpdateCopy += new CopyEngine.UpdateCopyHandler(_copyEngine_OnUpdateCopy);
            _copyEngine.OnEndOfCopy += new CopyEngine.EndOfCopyHandler(_copyEngine_OnEndOfCopy);
            _copyEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _copyEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _copyEngine.XCopy(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _copyViewer.SetFieldError(pe.Parameter, pe.Message);
                //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _copyViewer.EnableControls(true);
            }
        }

        void _copyViewer_OnDeleteAction(EventArgs e)
        {
            _copyViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _copyViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Copy.
            _copyEngine = new CopyEngine();
            _copyEngine.OnBeginCount += new CopyEngine.BeginCountHandler(_copyEngine_OnBeginCount);
            _copyEngine.OnUpdateCount += new CopyEngine.UpdateCountHandler(_copyEngine_OnUpdateCount);
            _copyEngine.OnEndOfCount += new CopyEngine.EndOfCountHandler(_copyEngine_OnEndOfCount);
            _copyEngine.OnBeginDelete += new CopyEngine.BeginDeleteHandler(_copyEngine_OnBeginDelete);
            _copyEngine.OnUpdateDelete += new CopyEngine.UpdateDeleteHandler(_copyEngine_OnUpdateDelete);
            _copyEngine.OnEndOfDelete += new CopyEngine.EndOfDeleteHandler(_copyEngine_OnEndOfDelete);
            _copyEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _copyEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _copyEngine.XDelete(_profileManager.UserSettings.SelectedItem.NewBaseDir);
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _copyViewer.SetFieldError(pe.Parameter, pe.Message);
                //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _copyViewer.EnableControls(true);
            }
        }

        void _copyViewer_OnCancelAction(EventArgs e)
        {
            _copyViewer.EnableControls(true);
            _copyEngine.Interrupt.Reason = "Cancel";
        }


        void _copyViewer_OnSynchronizePreventDeleteAction(EventArgs e)
        {
            Administrator.Reset();
            _copyViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _copyViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _directoryEngine = new DirectoryEngine();
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _directoryEngine.EventBeginProgress += new Copy8.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _directoryEngine.EventUpdateProgress += new Copy8.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _directoryEngine.EventEndOfProgress += new Copy8.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _directoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _directoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            _directoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            _OldDirectoryListing = _directoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            _directoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _directoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _directoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine();
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                //THe crucial difference is that allowDelete is set to false which prevents any deletion of files or removal of directories.
                _synchronizeEngine.Process(_comparisons, startAllDateTime, false);
                //After synchronization the target directory is synchronized with the source directory.
                _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _copyViewer.SetFieldError(pe.Parameter, pe.Message);
                //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _copyViewer.EnableControls(true);
            }
        }

        void _copyViewer_OnSynchronizeAction(EventArgs e)
        {
            Administrator.Reset();
            _copyViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _copyViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _directoryEngine = new DirectoryEngine();
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _directoryEngine.EventBeginProgress += new Copy8.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _directoryEngine.EventUpdateProgress += new Copy8.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _directoryEngine.EventEndOfProgress += new Copy8.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _directoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _directoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            _directoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            _OldDirectoryListing = _directoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            _directoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _directoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _directoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine();
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _synchronizeEngine.Process(_comparisons, startAllDateTime);
                //After synchronization the target directory is synchronized with the source directory.
                _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _copyViewer.SetFieldError(pe.Parameter, pe.Message);
                //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _copyViewer.EnableControls(true);
            }
        }

        void _copyViewer_OnSynchronizeAllAction(EventArgs e)
        {
            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Loop through all saved UserSettings.
            foreach (var pair in _profileManager.UserSettings.Entries)
            {
                Thread.Sleep(3000);
                UserSetting setting = pair.Value;
                _copyViewer.ClearErrors();
                _profileManager.UserSettings.Select(setting.Key);
                LoadParameters();
                _copyViewer_OnSynchronizeAction(e);
            }
        }

        void _copyViewer_OnSynchronizePlusAction(EventArgs e)
        {
            Administrator.Reset();
            _copyViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _copyViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _directoryEngine = new DirectoryEngine();
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _directoryEngine.EventBeginProgress += new Copy8.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _directoryEngine.EventUpdateProgress += new Copy8.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _directoryEngine.EventEndOfProgress += new Copy8.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _directoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _directoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _directoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            _directoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            // Instead of scanning the target directory.
            // Load the previously synchronized snapshot to compare against.
            if (File.Exists(getSnapshotFileSpec(targetDirectory)))
            {
                _OldDirectoryListing = new DirectoryEntries();
                _OldDirectoryListing.Load(getSnapshotFileSpec(targetDirectory), DirectoryEntries.InterpretationEnum.Target);
            }
            else
            {
                _OldDirectoryListing = _directoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            }
            _directoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _directoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _directoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine();
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _synchronizeEngine.Process(_comparisons, startAllDateTime);
                //After synchronization the target directory is synchronized with the source directory.
                _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _copyViewer.SetFieldError(pe.Parameter, pe.Message);
                //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
            }
            finally
            {
                _copyViewer.EnableControls(true);
            }
        }

        void _copyViewer_OnSynchronizePlusAllAction(EventArgs e)
        {
            //Validate all fields.
            int errorCount = ValidateFields();
            if (errorCount > 0)
            {
                return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Loop through all saved UserSettings.
            foreach (var pair in _profileManager.UserSettings.Entries)
            {
                Thread.Sleep(3000);
                UserSetting setting = pair.Value;
                _copyViewer.ClearErrors();
                _profileManager.UserSettings.Select(setting.Key);
                LoadParameters();
                _copyViewer_OnSynchronizePlusAction(e);
            }
        }

        private string getSnapshotFileSpec(string targetDirectory)
        {
            string fileSpec = string.Empty;
            string targetName = getTargetName(targetDirectory);
            if (targetName.Length > 0)
            {
                fileSpec = Administrator.ProfileManager.SystemProfile.CurrentSnapshotPath + targetName + ".snap";
            }
            return fileSpec;
        }

        private string getTargetName(string targetDirectory)
        {
            StringBuilder sb = new StringBuilder();
            string[] parts = targetDirectory.Split(new string[] { Path.DirectorySeparatorChar.ToString() }, StringSplitOptions.RemoveEmptyEntries);
            string delimiter = string.Empty;
            if (parts.Length > 0)
            {
                String part = parts[0];
                part = part.Replace(":", string.Empty);
                sb = new StringBuilder(part);
                if (parts.Length > 1)
                {
                    for (int index = 1; index < parts.Length; index++)
                    {
                        part = parts[index];
                        part = part.Replace(":", string.Empty);
                        sb.Append(delimiter + part);
                        delimiter = "_";
                    }
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Log file housekeeping.
        protected void DeleteOldLogFiles()
        {
            Thread.Sleep(2000);
            //Create list of deletes and other.
            DirectoryEntries oldLogEntries = new DirectoryEntries();
            DirectoryEntries otherEntries = new DirectoryEntries();
            int logFileKeepDays = Administrator.ProfileManager.SystemProfile.LogFileKeepDays;
            if (_NewDirectoryListing != null && _NewDirectoryListing.Count > 0)
            {
                foreach (DirectoryEntry entry in _NewDirectoryListing)
                {
                    bool isOldLogFile = false;
                    if (entry.StdType != "dir")
                    {
                        bool isLogFile = false;
                        if (entry.StdType == "log" && !entry.StdFile.StartsWith("Git"))
                        {
                            string[] nameParts = entry.StdFile.Split('_');
                            foreach (string namePart in nameParts)
                            {
                                if (namePart.Contains("@"))
                                {
                                    isLogFile = true;
                                    break;
                                }
                            }
                        }
                        if (isLogFile)
                        {
                            DateTime now = DateTime.Now;
                            DateTime LastWriteTime = entry.StdDate;
                            TimeSpan ts = now.Subtract(LastWriteTime);
                            int daysOld = (int)ts.TotalDays;
                            if (Math.Abs(daysOld) > logFileKeepDays)
                            {
                                isOldLogFile = true;
                            }
                        }
                    }
                    if (isOldLogFile)
                    {
                        oldLogEntries.Add(entry);
                    }
                    else
                    {
                        otherEntries.Add(entry);
                    }
                }
            }
            //Effectively remove deletes from list.
            _NewDirectoryListing = otherEntries;
            //Perform deletes.
            int deletedCount = 0;
            string DateTimeFormat = @"yyyy/MM/dd HH:mm";
            string separator = Path.DirectorySeparatorChar.ToString();
            try
            {
                if (oldLogEntries != null && oldLogEntries.Count > 0)
                {
                    foreach (DirectoryEntry entry in oldLogEntries)
                    {
                        string fileSpec = entry.StdHlq;
                        if (entry.StdDir != string.Empty)
                        {
                            fileSpec += entry.StdDir;
                        }
                        fileSpec += separator + entry.StdFile;
                        try
                        {
                            DateTime now = DateTime.Now;
                            DateTime lastWriteTime = entry.StdDate;
                            TimeSpan ts = now.Subtract(lastWriteTime);
                            int daysOld = (int)ts.TotalDays;
                            Administrator.OldLogsTracer.WriteTimedMsg("I", String.Format(@"Last Written : {0} Days Old : {1} File : {2} [Deleted]", lastWriteTime.ToString(DateTimeFormat), daysOld.ToString().Trim().PadLeft(3), fileSpec));
                            Delete(fileSpec);
                            deletedCount++;
                        }
                        catch (Exception ex)
                        {
                            Administrator.OldLogsTracer.WriteTimedMsg("E", ex.Message);
                        }
                    }
                    if (deletedCount > 0)
                    {
                        Administrator.OldLogsTracer.WriteTimedMsg("I", String.Format(@"{0} Log Files Deleted", deletedCount));
                    }
                    else
                    {
                        Administrator.OldLogsTracer.WriteTimedMsg("I", @"No Log Files Deleted");
                    }
                }
                else
                {
                    Administrator.OldLogsTracer.WriteTimedMsg("I", @"No Log Files Found");
                }
            }
            catch (Exception ex)
            {
                Administrator.OldLogsTracer.WriteTimedMsg("E", ex.Message);
            }
            finally
            {
                Thread.Sleep(2000);
            }
        }

        private void Delete(string fileSpec)
        {
            if (File.Exists(fileSpec))
            {
                bool isReadOnly = ((File.GetAttributes(fileSpec) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                if (isReadOnly)
                {
                    File.SetAttributes(fileSpec, FileAttributes.Normal);
                }
                File.Delete(fileSpec);
            }
        }
        #endregion

        #region Progress Event Handlers.
        void _copyEngine_OnBeginCount(CopyEngine.BeginCopyEventArgs e)
        {
            _copyViewer.CursorMode = CursorModes.Wait;
            _copyViewer.StatusMessage = "Counting ...";
            _TotalBytes = 0;
            _SoFarBytes = 0;
            _currentActivityProgress = new ProgressLink();
            ActionBeginProgress(_currentActivityProgress);
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;
        }

        void _copyEngine_OnUpdateCount(CopyEngine.UpdateCopyEventArgs e)
        {
            ActionUpdateProgress(_currentActivityProgress, e.increment);
            _copyViewer.StatusMessage = String.Format("Counting: {0} %, Bytes So Far = {1}, Estimated total Bytes = {2}", _currentActivityProgress.Value, _SoFarBytes.ToString("#,##0"), _TotalBytes.ToString("#,##0"));
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;
        }

        void _copyEngine_OnEndOfCount(CopyEngine.EndOfCopyEventArgs e)
        {
            ActionEndOfProgress(_currentActivityProgress);
            _currentActivityProgress.Maximum = _currentActivityProgress.Value;
            _copyViewer.CurrentActivityProgress = _currentActivityProgress;
            _copyViewer.StatusMessage = "End of Count";
            _copyViewer.CursorMode = CursorModes.Normal;
        }

        void _copyEngine_OnBeginCopy(CopyEngine.BeginCopyEventArgs e)
        {
            _copyViewer.CursorMode = CursorModes.Wait;
            _copyViewer.StatusMessage = "Copying ...";
            ActionBeginProgress(_overallProgress);
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _copyEngine_OnUpdateCopy(CopyEngine.UpdateCopyEventArgs e)
        {
            ActionUpdateProgress(_overallProgress, e.increment);
            _copyViewer.StatusMessage = String.Format("Copying: {0} %, Bytes So Far = {1}, Total Bytes = {2}", _overallProgress.Value, _SoFarBytes.ToString("#,##0"), _TotalBytes.ToString("#,##0"));
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _copyEngine_OnEndOfCopy(CopyEngine.EndOfCopyEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _overallProgress.Value = _overallProgress.Maximum;
            _copyViewer.OverallProgress = _overallProgress;
            _copyViewer.StatusMessage = "End of Copy";
            _copyViewer.CursorMode = CursorModes.Normal;
        }

        void _copyEngine_OnBeginDelete(CopyEngine.BeginCopyEventArgs e)
        {
            _copyViewer.CursorMode = CursorModes.Wait;
            _copyViewer.StatusMessage = "Deleting ...";
            ActionBeginProgress(_overallProgress);
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _copyEngine_OnUpdateDelete(CopyEngine.UpdateCopyEventArgs e)
        {
            ActionUpdateProgress(_overallProgress, e.increment);
            _copyViewer.StatusMessage = String.Format("Deleting: {0} %, Bytes So Far = {1}, Total Bytes = {2}", _overallProgress.Value, _SoFarBytes.ToString("#,##0"), _TotalBytes.ToString("#,##0"));
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _copyEngine_OnEndOfDelete(CopyEngine.EndOfCopyEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _overallProgress.Value = _overallProgress.Maximum;
            _copyViewer.OverallProgress = _overallProgress;
            _copyViewer.StatusMessage = "End of Delete";
            _copyViewer.CursorMode = CursorModes.Normal;
        }


        /// <summary>
        /// Start directory scan progress indicator.
        /// </summary>
        void _directoryEngine_OnBeginDirectoryScan(object sender, EventParameters2 eventArgs)
        {
            switch (eventArgs.Method)
            {
                case "DirList":
                    _copyViewer.CursorMode = CursorModes.Wait;
                    _copyViewer.StatusMessage = "Scanning directories ...";
                    _SoFarBytes = 0;
                    if (_processingNew)
                    {
                        _TotalBytes = 0;
                        _currentActivityProgress = new ProgressLink();
                        ActionBeginProgress(_currentActivityProgress);
                        _copyViewer.CurrentActivityProgress = _currentActivityProgress;
                    }
                    else
                    {
                        _overallProgress = new ProgressLink();
                        ActionBeginProgress(_overallProgress);
                        _copyViewer.OverallProgress = _overallProgress;
                    }
                    break;
                case "Changes":
                    break;
                case "Comparisons":
                    break;
            }
        }

        /// <summary>
        /// Update directory scan progress indicator.
        /// </summary>
        void _directoryEngine_OnUpdateDirectoryScan(object sender, EventParameters2 eventArgs)
        {
            switch (eventArgs.Method)
            {
                case "DirList":
                    string phase = _processingNew ? "Source" : "Target";
                    string messageFormat = "Scanning {0}: {1} %, Bytes So Far = {2}, Estimated total bytes = {3}";
                    string numberFormat = "#,##0";
                    if (_processingNew)
                    {
                        ActionUpdateProgress(_currentActivityProgress, eventArgs.Increment);
                        _copyViewer.StatusMessage = String.Format(messageFormat, phase, _currentActivityProgress.Value, _SoFarBytes.ToString(numberFormat), _TotalBytes.ToString(numberFormat));
                        _copyViewer.CurrentActivityProgress = _currentActivityProgress;
                    }
                    else
                    {
                        ActionUpdateProgress(_overallProgress, eventArgs.Increment);
                        _copyViewer.StatusMessage = String.Format(messageFormat, phase, _overallProgress.Value, _SoFarBytes.ToString(numberFormat), _TotalBytes.ToString(numberFormat));
                        _copyViewer.OverallProgress = _overallProgress;
                    }
                    break;
                case "Changes":
                    break;
                case "Comparisons":
                    break;
            }
        }

        /// <summary>
        /// End directory scan progress indicator.
        /// </summary>
        void _directoryEngine_OnEndOfDirectoryScan(object sender, EventParameters2 eventArgs)
        {
            switch (eventArgs.Method)
            {
                case "DirList":
                    string message = string.Empty;
                    if (_directoryEngine.Interrupt.Reason == "Cancel")
                    {
                        message = "Directory scan cancelled.";
                    }
                    else
                    {
                        message = "Directory scan complete.";
                    }
                    if (_processingNew)
                    {
                        ActionEndOfProgress(_currentActivityProgress);
                        _currentActivityProgress.Value = _currentActivityProgress.Maximum;
                        _copyViewer.CurrentActivityProgress = _currentActivityProgress;
                    }
                    else
                    {
                        ActionEndOfProgress(_overallProgress);
                        _overallProgress.Value = _overallProgress.Maximum;
                        _copyViewer.OverallProgress = _overallProgress;
                    }
                    _copyViewer.StatusMessage = message;
                    _copyViewer.CursorMode = CursorModes.Normal;
                    break;
                case "Changes":
                    break;
                case "Comparisons":
                    break;
            }
        }

        #region Ultimately these will become synchronize engine progress event handlers.
        void _SynchronizeEngine_OnBeginSynchronize(SynchronizeEngine.BeginSynchronizeEventArgs e)
        {
            _TotalBytes = e.maximum;
            _SoFarBytes = 0;
            _copyViewer.CursorMode = CursorModes.Wait;
            _copyViewer.StatusMessage = "Synchronizing ...";
            ActionBeginProgress(_overallProgress);
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _SynchronizeEngine_OnUpdateSynchronize(SynchronizeEngine.UpdateSynchronizeEventArgs e) //(long increment, string method, string fileSpec)
        {
            ActionUpdateProgress(_overallProgress, e.increment);
            _copyViewer.StatusMessage = String.Format("{0}: {1} %, Bytes So Far = {2}, Total Bytes = {3}", e.message, _overallProgress.Value, _SoFarBytes.ToString("#,##0"), _TotalBytes.ToString("#,##0"));
            _copyViewer.OverallProgress = _overallProgress;
        }

        void _SynchronizeEngine_OnEndOfSynchronize(SynchronizeEngine.EndOfSynchronizeEventArgs e)
        {
            ActionEndOfProgress(_overallProgress);
            _overallProgress.Value = _overallProgress.Maximum;
            _copyViewer.OverallProgress = _overallProgress;
            _copyViewer.StatusMessage = e.message;
            _copyViewer.CursorMode = CursorModes.Normal;
        }
        #endregion
        #endregion

        #region Progress Event Helper Methods.
        private void ActionBeginProgress(ProgressLink progressLink)
        {
            progressLink.Minimum = 0;
            progressLink.Maximum = 100;
            progressLink.Value = 0;
        }

        private void ActionUpdateProgress(ProgressLink progressLink, long increment)
        {
            _SoFarBytes += increment;
            if (_TotalBytes < _SoFarBytes)
            {
                _TotalBytes = _SoFarBytes * 2;
            }
            long percent = 0;
            if (_TotalBytes > 0)
            {
                percent = _SoFarBytes * 100;
                percent /= _TotalBytes;
            }
            progressLink.Value = (int)percent;
        }

        private void ActionEndOfProgress(ProgressLink progressLink)
        {
            _TotalBytes = _SoFarBytes;
            _SoFarBytes = 0;
        }
        #endregion
        #endregion

        #region Private Methods.
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
            _copyViewer.NewBaseDir = userSetting.NewBaseDir;
            _copyViewer.OldBaseDir = userSetting.OldBaseDir;
            _copyViewer.CopyRule = userSetting.CopyRule;
            _copyViewer.MonitoredTypesOnly = userSetting.MonitoredTypesOnly;
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
                NewBaseDir = _copyViewer.NewBaseDir,
                OldBaseDir = _copyViewer.OldBaseDir,
                CopyRule = _copyViewer.CopyRule,
                MonitoredTypesOnly = _copyViewer.MonitoredTypesOnly,
            };
            _profileManager.UserSettings.Persist(userSetting);
            LoadParameters();
        }

        /// <summary>
        /// Refresh ComboBox at the begining or when a new entry has been added.
        /// </summary>
        private void RefreshCombo()
        {
            _copyViewer.PreviousSelections_IndexChanged_Event_On = false;
            _copyViewer.PreviousSelections_DataSource = _profileManager.UserSettings.Values.ToList();
            _copyViewer.PreviousSelections_DisplayMember = "KeyShrunk";
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
            }
            _copyViewer.PreviousSelections_SelectedString = userSetting.KeyShrunk;
            if (_copyViewer.PreviousSelections_Count == 0)
            {
                _copyViewer.PreviousSelections_Text = string.Empty;
            }
            _copyViewer.PreviousSelections_IndexChanged_Event_On = true;
        }

        /// <summary>
        /// Set current key.
        /// </summary>
        private void SetCurrentKey()
        {
            if (_copyViewer.NewBaseDir.Trim().Length > 0)
            {
                if (_copyViewer.OldBaseDir.Trim().Length > 0)
                {
                    _currentKey = "[" + _copyViewer.NewBaseDir + "]-[" + _copyViewer.OldBaseDir + "]";
                }
                else
                {
                    _currentKey = "[" + _copyViewer.NewBaseDir + "]-[?]";
                }
            }
            else
            {
                if (_copyViewer.OldBaseDir.Trim().Length > 0)
                {
                    _currentKey = "[?]-[" + _copyViewer.OldBaseDir + "]";
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
            _copyViewer.ClearErrors();
            int errorCount = 0;
            if (!Directory.Exists(_copyViewer.NewBaseDir))
            {
                errorCount++;
                _copyViewer.SetFieldError("NewBaseDir", "Copy from directory not found");
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