using Copy9.DataLayer.Profile;
using Copy9.Models;
using Copy9.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Copy9.Presenters
{
    public class Presenter
    {
        #region Constants.
        const string FILE_FILTER = "All files (*.*)|*.*|Text files (*.txt)|*.txt";
        const string COMPARISONS_FILENAME = @"[!Comparisons!].csv";
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
        private Actor _actor;
        private DirectoryEngine _directoryEngine; // Don't think this is used now will remove later.
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
        internal Presenter(ICopyEngine copyEngine, ICopyViewer copyViewer, DirectoryEngineL directoryEngine, SynchronizeEngine synchronizeEngine, IProfileManager profileManager)
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
            _copyViewer.OnSynchronizePlusPlusAction += new SynchronizePlusPlusActionHandler(_copyViewer_OnSynchronizePlusPlusAction);
            _copyViewer.OnSynchronizePlusPlusAllAction += new SynchronizePlusPlusAllActionHandler(_copyViewer_OnSynchronizePlusPlusAllAction);
            _copyViewer.OnResetAction += new ResetActionHandler(_copyViewer_OnResetAction);
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
        public delegate void SynchronizePlusPlusActionHandler(EventArgs e);
        public delegate void SynchronizePlusPlusAllActionHandler(EventArgs e);
        public delegate void ResetActionHandler(EventArgs e);
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

        //void _copyViewer_OnCopyAction(EventArgs e)
        //{
        //    _copyViewer.EnableControls(false);
        //    _overallProgress = new ProgressLink();
        //    _copyViewer.OverallProgress = _overallProgress;
        //    _currentActivityProgress = new ProgressLink();
        //    _copyViewer.CurrentActivityProgress = _currentActivityProgress;

        //    //Validate all fields.
        //    int errorCount = ValidateFields();
        //    if (errorCount > 0)
        //    {
        //        return;
        //    }

        //    //Save Parameters.
        //    _profileManager.UserSettings.Select(_currentKey);
        //    _profileManager.Interrupt.Reason = "OK";
        //    SaveParameters();
        //    RefreshCombo();

        //    //Copy.
        //    _copyEngine = new CopyEngine();
        //    _copyEngine.OnBeginCount += new CopyEngine.BeginCountHandler(_copyEngine_OnBeginCount);
        //    _copyEngine.OnUpdateCount += new CopyEngine.UpdateCountHandler(_copyEngine_OnUpdateCount);
        //    _copyEngine.OnEndOfCount += new CopyEngine.EndOfCountHandler(_copyEngine_OnEndOfCount);
        //    _copyEngine.OnBeginCopy += new CopyEngine.BeginCopyHandler(_copyEngine_OnBeginCopy);
        //    _copyEngine.OnUpdateCopy += new CopyEngine.UpdateCopyHandler(_copyEngine_OnUpdateCopy);
        //    _copyEngine.OnEndOfCopy += new CopyEngine.EndOfCopyHandler(_copyEngine_OnEndOfCopy);
        //    _copyEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
        //    _copyEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
        //    _copyEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
        //    _copyEngine.Interrupt = _profileManager.Interrupt;

        //    try
        //    {
        //        _copyEngine.XCopy(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
        //        Administrator.View();
        //    }
        //    catch (ParameterException pe)
        //    {
        //        _copyViewer.SetFieldError(pe.Parameter, pe.Message);
        //        //_copyViewer.DisplayMessageBox(ae.Message, _copyViewer.Caption, MsgButtons.OK, MsgIcon.Error);
        //    }
        //    finally
        //    {
        //        _copyViewer.EnableControls(true);
        //    }
        //}

        //TODO: Changed this to Bank+ which is the same as Synch Copy (No deletes) but using the image file the same way that Synch+ does.
        //TODO: Do somehting about validation.
        void _copyViewer_OnCopyAction(EventArgs e)
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _actor.Interrupt = _profileManager.Interrupt;
            _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;

            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!_actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                _actor.TargetFiler.CreateDirectory(targetDirectory);
            }
            _actor.TargetDirectoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            // Instead of scanning the target directory.
            // Load the previously synchronized snapshot to compare against.
            if (File.Exists(getSnapshotFileSpec(targetDirectory)))
            {
                _OldDirectoryListing = new DirectoryEntries();
                _OldDirectoryListing.Load(getSnapshotFileSpec(targetDirectory), DirectoryEntries.InterpretationEnum.Target, _actor.TargetConnector);
            }
            else
            {
                _OldDirectoryListing = _actor.TargetDirectoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            }
            _actor.TargetDirectoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _actor.TargetDirectoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _actor.TargetDirectoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine(_actor);
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                //The crucial difference is that allowDelete is set to false which prevents any deletion of files or removal of directories.
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
            _copyEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
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

        //TODO: Changed.
        //TODO: Do somehting about validation.
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _actor.Interrupt = _profileManager.Interrupt;
            _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!_actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                _actor.TargetFiler.CreateDirectory(targetDirectory);
            }
            _actor.TargetDirectoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            _OldDirectoryListing = _actor.TargetDirectoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            _actor.TargetDirectoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _actor.TargetDirectoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _actor.TargetDirectoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine(new Actor());
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                //The crucial difference is that allowDelete is set to false which prevents any deletion of files or removal of directories.
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

        //TODO: Changed.
        //TODO: Do somehting about validation.
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _actor.Interrupt = _profileManager.Interrupt;
            _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!_actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                _actor.TargetFiler.CreateDirectory(targetDirectory);
            }
            _actor.TargetDirectoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            _OldDirectoryListing = _actor.TargetDirectoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            _actor.TargetDirectoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _actor.TargetDirectoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _actor.TargetDirectoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine(_actor);
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;
            try
            {
                _synchronizeEngine.Process(_comparisons, startAllDateTime, false);
                //After synchronization the target directory is synchronized with the source directory.
                if (_NewDirectoryListing != null)
                {
                    _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                }
                if (_actor.SourceConnector.IsGoogleDrive)
                {
                    // Comparisons fie has already been deleted.
                }
                if (_actor.TargetConnector.IsGoogleDrive)
                {
                    //Download previously discovered comparisons file from Google Drive if there are any.
                    Comparisons<Comparison> previousComparisons = DownloadPreviousComparisons();
                    //Append newly discovered comparisons onto end of previously discovered comparisons.
                    _comparisons = AppendComparisons(previousComparisons, _comparisons);
                    //Upload comparisons file to Google Drive.
                    UploadLatestComparisons();
                }
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

        //TODO: Changed.
        //TODO: Do somehting about validation.
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _actor.Interrupt = _profileManager.Interrupt;
            _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (!_actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                _actor.TargetFiler.CreateDirectory(targetDirectory);
            }
            _actor.TargetDirectoryEngine.TargetHlq = targetDirectory;
            mnOldFilesEstimate = mnNewFilesEstimate;
            // Instead of scanning the target directory.
            // Load the previously synchronized snapshot to compare against.
            if (File.Exists(getSnapshotFileSpec(targetDirectory)))
            {
                _OldDirectoryListing = new DirectoryEntries();
                _OldDirectoryListing.Load(getSnapshotFileSpec(targetDirectory), DirectoryEntries.InterpretationEnum.Target, _actor.TargetConnector);
            }
            else
            {
                _OldDirectoryListing = _actor.TargetDirectoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
            }
            _actor.TargetDirectoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            _actor.TargetDirectoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            _comparisons = _actor.TargetDirectoryEngine.Comparisons;
            _copyEngine.Interrupt = _profileManager.Interrupt;

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _synchronizeEngine = new SynchronizeEngine(_actor);
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _synchronizeEngine.Process(_comparisons, startAllDateTime, false);
                //After synchronization the target directory is synchronized with the source directory.
                if (_NewDirectoryListing != null)
                {
                    _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                }
                if (_actor.SourceConnector.IsGoogleDrive)
                {
                    // Comparisons fie has already been deleted.
                }
                if (_actor.TargetConnector.IsGoogleDrive)
                {
                    //Download previously discovered comparisons file from Google Drive if there are any.
                    Comparisons<Comparison> previousComparisons = DownloadPreviousComparisons();
                    //Append newly discovered comparisons onto end of previously discovered comparisons.
                    _comparisons = AppendComparisons(previousComparisons, _comparisons);
                    //Upload comparisons file to Google Drive.
                    UploadLatestComparisons();
                }
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

        //TODO: Completely New.
        //RODO: Download comparisons after they have been uploaded.
        //TODO: Do somehting about validation.
        void _copyViewer_OnSynchronizePlusPlusAction(EventArgs e)
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            DateTime startAllDateTime = Administrator.Now;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            if (_actor.SourceConnector.IsGoogleDrive && _actor.TargetConnector.IsGoogleDrive)
            {
                //TODO: Implement this later, for now just terminate.
                _copyViewer.DisplayMessageBox("Use one of the other synch options", _copyViewer.Caption, MsgButtons.OK, Presenter.MsgIcon.Exclamation);
                return;
            }
            else if (_actor.SourceConnector.IsGoogleDrive)
            {
                //Download comparisons file from Google Drive.
                _comparisons = DownloadPreviousComparisons();
            }
            else if (_actor.TargetConnector.IsGoogleDrive)
            {
                //Generate newly discovered comparisons.
                _actor.Interrupt = _profileManager.Interrupt;
                _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
                _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
                _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
                _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
                _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
                _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
                _processingNew = true;
                _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
                _TotalBytes = mnNewFilesEstimate;
                //Delete log files older that the specified number of days.
                DeleteOldLogFiles();
                _processingNew = false;
                targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
                if (!_actor.TargetFiler.DirectoryExists(targetDirectory))
                {
                    _actor.TargetFiler.CreateDirectory(targetDirectory);
                }
                _actor.TargetDirectoryEngine.TargetHlq = targetDirectory;
                mnOldFilesEstimate = mnNewFilesEstimate;
                // Instead of scanning the target directory.
                // Load the previously synchronized snapshot to compare against.
                if (File.Exists(getSnapshotFileSpec(targetDirectory)))
                {
                    _OldDirectoryListing = new DirectoryEntries();
                    _OldDirectoryListing.Load(getSnapshotFileSpec(targetDirectory), DirectoryEntries.InterpretationEnum.Target, _actor.TargetConnector);
                }
                else
                {
                    _OldDirectoryListing = _actor.TargetDirectoryEngine.DirList(targetDirectory, ref mnOldFilesEstimate);
                }
                _actor.TargetDirectoryEngine.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
                _actor.TargetDirectoryEngine.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
                _comparisons = _actor.TargetDirectoryEngine.Comparisons;
            }
            else
            {
                //TODO: Implement this later, for now just terminate.
                _copyViewer.DisplayMessageBox("Use one of the other synch options", _copyViewer.Caption, MsgButtons.OK, Presenter.MsgIcon.Exclamation);
                return;
            }

            //Use the Synchronize engine to perform the synchronize.
            //TODO: Move the directory engine calls inside the Synchronize engine.
            _copyEngine.Interrupt = _profileManager.Interrupt;
            _synchronizeEngine = new SynchronizeEngine(_actor);
            _synchronizeEngine.OnBeginSynchronize += new SynchronizeEngine.BeginSynchronizeHandler(_SynchronizeEngine_OnBeginSynchronize);
            _synchronizeEngine.OnUpdateSynchronize += new SynchronizeEngine.UpdateSynchronizeHandler(_SynchronizeEngine_OnUpdateSynchronize);
            _synchronizeEngine.OnEndOfSynchronize += new SynchronizeEngine.EndOfSynchronizeHandler(_SynchronizeEngine_OnEndOfSynchronize);
            _synchronizeEngine.CopyRule = _profileManager.UserSettings.SelectedItem.CopyRule;
            _synchronizeEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _synchronizeEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _synchronizeEngine.Interrupt = _profileManager.Interrupt;

            try
            {
                _synchronizeEngine.Process(_comparisons, startAllDateTime, true);
                if (_actor.SourceConnector.IsGoogleDrive)
                {
                    // Comparisons fie has already been deleted.
                }
                if (_actor.TargetConnector.IsGoogleDrive)
                {
                    //After synchronization the target directory is synchronized with the source directory.
                    if (_NewDirectoryListing != null)
                    {
                        _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                    }
                    //Download previously discovered comparisons file from Google Drive if there are any.
                    Comparisons<Comparison> previousComparisons = DownloadPreviousComparisons();
                    //Append newly discovered comparisons onto end of previously discovered comparisons.
                    _comparisons = AppendComparisons(previousComparisons, _comparisons);
                    //Upload comparisons file to Google Drive.
                    UploadLatestComparisons();
                }
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

        //TODO: Completely New.
        //RODO: Download comparisons after they have been uploaded.
        //TODO: Do somehting about validation.
        void _copyViewer_OnResetAction(EventArgs e)
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
                //return;
            }

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            //New actor class chooses how to handle whichever file systems the source and target are using.
            _actor = new Actor(_profileManager.UserSettings.SelectedItem.NewBaseDir, _profileManager.UserSettings.SelectedItem.OldBaseDir);
            RefreshCombo();

            //Use directory engine to scan both directories to determine the differences.
            DateTime startAllDateTime = Administrator.Now;
            _actor.Interrupt = _profileManager.Interrupt;
            _actor.SourceDirectoryEngine.EventBeginProgress += new Copy9.EventDelegate(_directoryEngine_OnBeginDirectoryScan);
            _actor.SourceDirectoryEngine.EventUpdateProgress += new Copy9.EventDelegate(_directoryEngine_OnUpdateDirectoryScan);
            _actor.SourceDirectoryEngine.EventEndOfProgress += new Copy9.EventDelegate(_directoryEngine_OnEndOfDirectoryScan);
            _actor.SourceDirectoryEngine.MonitoredTypesOnly = _profileManager.UserSettings.SelectedItem.MonitoredTypesOnly;
            _actor.SourceDirectoryEngine.DateThreshold = _profileManager.UserSettings.SelectedItem.DateThreshold;
            _actor.SourceDirectoryEngine.Interrupt = _profileManager.Interrupt;
            _processingNew = true;
            _NewDirectoryListing = _actor.SourceDirectoryEngine.DirList(_profileManager.UserSettings.SelectedItem.NewBaseDir, ref mnNewFilesEstimate);
            _TotalBytes = mnNewFilesEstimate;
            //Delete log files older that the specified number of days.
            DeleteOldLogFiles();
            _processingNew = false;
            string targetDirectory = _profileManager.UserSettings.SelectedItem.OldBaseDir;
            mnOldFilesEstimate = mnNewFilesEstimate;
            try
            {
                //After synchronization the target directory would be synchronized with the source directory.
                if (_NewDirectoryListing != null)
                {
                    _NewDirectoryListing.Save(getSnapshotFileSpec(targetDirectory));
                }
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

        private Comparisons<Comparison> DownloadPreviousComparisons()
        {
            string newBaseDir = _actor.SourceConnector.BaseDir;
            string oldBaseDir = _actor.TargetConnector.BaseDir;
            Comparisons<Comparison> comparisons = new Comparisons<Comparison>();
            if (_actor.SourceConnector.IsGoogleDrive && _actor.TargetConnector.IsGoogleDrive)
            {
            }
            else if (_actor.SourceConnector.IsGoogleDrive)
            {
                if (_actor.SourceFiler.DirectoryExists(newBaseDir))
                {
                    string sourceComparisonsFileSpec = newBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                    if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                    {
                        if (!_actor.TargetConnector.IsGoogleDrive)
                        {
                            if (!oldBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            {
                                oldBaseDir += Path.DirectorySeparatorChar;
                            }
                            ProfileData.DataLayer.Profile.FileHelper.PathCheck(oldBaseDir);
                        }
                        if (_actor.TargetFiler.DirectoryExists(oldBaseDir))
                        {
                            string targetComparisonsFileSpec = oldBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                            if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                            {
                                _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                            }
                            _actor.CopyFile(sourceComparisonsFileSpec, targetComparisonsFileSpec);
                            ComparisonsHelper helper = new ComparisonsHelper();
                            comparisons = helper.Load(targetComparisonsFileSpec, _actor);
                            //Delete both source and target comparisons file(s) immediately after it has been downloaded and loaded into memory.
                            if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                            {
                                _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                            }
                            if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                            {
                                _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                            }
                        }
                    }
                }
            }
            else if (_actor.TargetConnector.IsGoogleDrive)
            {
                if (_actor.TargetFiler.DirectoryExists(oldBaseDir))
                {
                    string targetComparisonsFileSpec = oldBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                    if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                    {
                        if (_actor.SourceFiler.DirectoryExists(newBaseDir))
                        {
                            string sourceComparisonsFileSpec = newBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                            if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                            {
                                _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                            }
                            Actor reverseActor = new Actor(_profileManager.UserSettings.SelectedItem.OldBaseDir, _profileManager.UserSettings.SelectedItem.NewBaseDir);
                            reverseActor.CopyFile(targetComparisonsFileSpec, sourceComparisonsFileSpec);
                            ComparisonsHelper helper = new ComparisonsHelper();
                            comparisons = helper.Load(sourceComparisonsFileSpec, _actor);
                            //Delete both source and target comparisons file(s) immediately after it has been downloaded and loaded into memory.
                            if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                            {
                                _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                            }
                            if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                            {
                                _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                            }
                        }
                    }
                }
            }
            else
            {
            }
            return comparisons;
        }

        private void UploadLatestComparisons()
        {
            string newBaseDir = _actor.SourceConnector.BaseDir;
            string oldBaseDir = _actor.TargetConnector.BaseDir;
            if (_actor.SourceConnector.IsGoogleDrive && _actor.TargetConnector.IsGoogleDrive)
            {
            }
            else if (_actor.SourceConnector.IsGoogleDrive)
            {
                if (_actor.TargetFiler.DirectoryExists(oldBaseDir))
                {
                    string targetComparisonsFileSpec = oldBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                    if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                    {
                        _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                    }
                    ComparisonsHelper helper = new ComparisonsHelper();
                    helper.Comparisons = _comparisons;
                    helper.Save(targetComparisonsFileSpec);
                    if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                    {
                        string sourceComparisonsFileSpec = newBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                        if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                        {
                            _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                        }
                        Actor reverseActor = new Actor(_profileManager.UserSettings.SelectedItem.OldBaseDir, _profileManager.UserSettings.SelectedItem.NewBaseDir);
                        reverseActor.CopyFile(targetComparisonsFileSpec, sourceComparisonsFileSpec);
                        //Delete target comparisons file immediately after it has been uploaded.
                        if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                        {
                            _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                        }
                    }
                }
            }
            else if (_actor.TargetConnector.IsGoogleDrive)
            {
                if (_actor.SourceFiler.DirectoryExists(newBaseDir))
                {
                    string sourceComparisonsFileSpec = newBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                    if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                    {
                        _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                    }
                    ComparisonsHelper helper = new ComparisonsHelper();
                    helper.Comparisons = _comparisons;
                    helper.Save(sourceComparisonsFileSpec);
                    if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                    {
                        string targetComparisonsFileSpec = oldBaseDir + Path.DirectorySeparatorChar + COMPARISONS_FILENAME;
                        if (_actor.TargetFiler.FileExists(targetComparisonsFileSpec))
                        {
                            _actor.TargetFiler.DeleteFile(targetComparisonsFileSpec);
                        }
                        _actor.CopyFile(sourceComparisonsFileSpec, targetComparisonsFileSpec);
                        //Delete source comparisons file immediately after it has been uploaded.
                        if (_actor.SourceFiler.FileExists(sourceComparisonsFileSpec))
                        {
                            _actor.SourceFiler.DeleteFile(sourceComparisonsFileSpec);
                        }
                    }
                }
            }
            else
            {
            }
        }

        private Comparisons<Comparison> AppendComparisons(Comparisons<Comparison> previousComparisons, Comparisons<Comparison> newComparisons)
        {
            Comparisons<Comparison> comparisons = new Comparisons<Comparison>();
            foreach (Comparison comparison in previousComparisons)
            {
                comparisons.Add(comparison);
            }
            //Only filter the new comparisons.
            //so that the current copy rule is only applied to the new comparisons and not to any comparisons done previously possibly with a different copy rule.
            ComparisonsHelper helper = new ComparisonsHelper();
            newComparisons = helper.Filter(newComparisons, true);
            foreach (Comparison comparison in newComparisons)
            {
                comparisons.Add(comparison);
            }
            return comparisons;
        }

        void _copyViewer_OnSynchronizePlusPlusAllAction(EventArgs e)
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
                _copyViewer_OnSynchronizePlusPlusAction(e);
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
                    if (_actor.Interrupt.Reason == "Cancel")
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
            _copyViewer.DateThreshold = userSetting.DateThreshold;
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
                DateThreshold = _copyViewer.DateThreshold
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
            if (_copyViewer.NewBaseDir.StartsWith("!") || _copyViewer.NewBaseDir.StartsWith("!"))
            {
            }
            else
            {
                if (!Directory.Exists(_copyViewer.NewBaseDir))
                {
                    errorCount++;
                    _copyViewer.SetFieldError("NewBaseDir", "Copy from directory not found");
                }
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