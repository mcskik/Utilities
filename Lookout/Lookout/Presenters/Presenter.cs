using Lookout.DataLayer.Profile;
using Lookout.Models;
using Lookout.Views;
using ProfileData.DataLayer.Profile;
using ProfileData.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Lookout.Presenters
{
    public class Presenter
    {
        #region Constants.
        const string FILE_FILTER = "APK files (*.apk)|*.apk";
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
        //private ILookoutEngine _inspectionEngine;
        private ILookoutMover _LookoutMover;
        private ILookoutViewer _lookoutViewer;
        private IProfileManager _profileManager;
        private string _currentKey = string.Empty;
        private ProgressLink _currentActivityProgress;
        private ProgressLink _overallProgress;
        private long _TotalMailItems = 0;
        private long _SoFarMailItems = 0;
        private long _TotalScanItems = 0;
        private long _SoFarScanItems = 0;
        private long _FilesEstimate = 0;
        private string ApkContentsContainerFolder;
        private string ApkContentsFolder;
        #endregion

        #region Properties.
        private string TargetOutlookFolder
        {
            get
            {
                string initialPath = Directory.Exists(_lookoutViewer.TargetOutlookFolder) ? _lookoutViewer.TargetOutlookFolder : string.Empty;
                string apkFolder = initialPath.Length > 0 ? initialPath : _profileManager.SystemProfile.HomePath;
                return apkFolder;
            }
        }
        #endregion

        #region Constructors.
        internal Presenter(ILookoutMover lookoutMover, ILookoutViewer lookoutViewer, IProfileManager profileManager)
        {
            _LookoutMover = lookoutMover;
            _lookoutViewer = lookoutViewer;
            _lookoutViewer.OnPreviousSelectionSelectedIndexChanged += new PreviousSelectionSelectedIndexChangedHandler(_lookoutViewer_OnPreviousSelectionSelectedIndexChanged);
            _lookoutViewer.OnRemovePreviousSelection += new RemovePreviousSelectionHandler(_lookoutViewer_OnRemovePreviousSelection);
            _lookoutViewer.OnChooseApkFolder += new ChooseTargetFolderHandler(_lookoutViewer_OnChooseApkFolder);
            _lookoutViewer.OnMoveAction += new MoveActionHandler(_lookoutViewer_OnMoveAction);
            _lookoutViewer.OnCancelAction += new CancelActionHandler(_lookoutViewer_OnCancelAction);
            _profileManager = profileManager;
            _currentActivityProgress = new ProgressLink();
            _overallProgress = new ProgressLink();
            _lookoutViewer.EnableControls(true);
            LoadParameters();
            RefreshCombo();
        }
        #endregion

        #region Delegates.
        public delegate void PreviousSelectionSelectedIndexChangedHandler(EventArgs e);
        public delegate void RemovePreviousSelectionHandler(EventArgs e);
        public delegate void ChooseSourceFolderHandler(EventArgs e);
        public delegate void ChooseTargetFolderHandler(EventArgs e);
        public delegate void CopySourceFolderHandler(EventArgs e);
        public delegate void CopyTargetFolderHandler(EventArgs e);
        public delegate void CopyActionHandler(EventArgs e);
        public delegate void ChooseReflateFolderHandler(EventArgs e);
        public delegate void ChooseTargetFileHandler(EventArgs e);
        public delegate void DeleteActionHandler(EventArgs e);
        public delegate void MoveActionHandler(EventArgs e);
        public delegate void CancelActionHandler(EventArgs e);
        #endregion

        #region Event Handlers.
        #region Main Form Event Handlers.
        void _lookoutViewer_OnPreviousSelectionSelectedIndexChanged(EventArgs e)
        {
            _lookoutViewer.ClearErrors();
            UserSetting oSetting = _lookoutViewer.PreviousSelections_SelectedItem;
            _profileManager.UserSettings.Select(oSetting.Key);
            LoadParameters();
        }

        void _lookoutViewer_OnRemovePreviousSelection(EventArgs e)
        {
            _lookoutViewer.ClearErrors();
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
                _profileManager.UserSettings.Delete(_profileManager.UserSettings.SelectedItem.Key);
            }
            LoadParameters();
            RefreshCombo();
        }

        void _lookoutViewer_OnChooseApkFolder(EventArgs e)
        {
            _lookoutViewer.ClearErrors();
            _lookoutViewer.TargetOutlookFolder = _lookoutViewer.BrowseFolder(TargetOutlookFolder, _lookoutViewer.TargetOutlookFolder);
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

        void _lookoutViewer_OnCancelAction(EventArgs e)
        {
            _lookoutViewer.EnableControls(true);
            _LookoutMover.Interrupt.Reason = "Cancel";
        }

        void _lookoutViewer_OnMoveAction(EventArgs e)
        {
            _lookoutViewer.EnableControls(false);
            _overallProgress = new ProgressLink();
            _lookoutViewer.OverallProgress = _overallProgress;
            _currentActivityProgress = new ProgressLink();
            _lookoutViewer.CurrentActivityProgress = _currentActivityProgress;

            //Validate all fields.
            //int errorCount = ValidateFields();
            //if (errorCount > 0)
            //{
            //    return;
            //}

            //Save Parameters.
            _profileManager.UserSettings.Select(_currentKey);
            _profileManager.Interrupt.Reason = "OK";
            SaveParameters();
            RefreshCombo();

            //Move.
            _LookoutMover = new LookoutMover();
            _LookoutMover.OnBeginMove += new LookoutMover.BeginMoveHandler(_LookoutMover_OnBeginMove);
            _LookoutMover.OnUpdateMove += new LookoutMover.UpdateMoveHandler(_LookoutMover_OnUpdateMove);
            _LookoutMover.OnEndOfMove += new LookoutMover.EndOfMoveHandler(_LookoutMover_OnEndOfMove);
            _LookoutMover.OnBeginScan += new LookoutMover.BeginScanHandler(_LookoutMover_OnBeginScan);
            _LookoutMover.OnUpdateScan += new LookoutMover.UpdateScanHandler(_LookoutMover_OnUpdateScan);
            _LookoutMover.OnEndOfScan += new LookoutMover.EndOfScanHandler(_LookoutMover_OnEndOfScan);
            _LookoutMover.Interrupt = _profileManager.Interrupt;

            try
            {
                _LookoutMover.MoveMail();
                Administrator.View();
            }
            catch (ParameterException pe)
            {
                _lookoutViewer.SetFieldError(pe.Parameter, pe.Message);
            }
            finally
            {
                _lookoutViewer.EnableControls(true);
            }
        }
        #endregion

        #region Progress Event Handlers.
        void _LookoutMover_OnBeginScan(LookoutMover.BeginMoveEventArgs e)
        {
            _lookoutViewer.CursorMode = CursorModes.Wait;
            _TotalScanItems = e.maximum;
            _SoFarScanItems = 0;
            ActionBeginScanItemsProgress(_currentActivityProgress);
        }

        void _LookoutMover_OnUpdateScan(LookoutMover.UpdateMoveEventArgs e)
        {
            ActionUpdateScanItemsProgress(_currentActivityProgress, e.increment);
        }

        void _LookoutMover_OnEndOfScan(LookoutMover.EndOfMoveEventArgs e)
        {
            ActionEndOfScanItemsProgress(_currentActivityProgress);
        }

        void _LookoutMover_OnBeginMove(LookoutMover.BeginMoveEventArgs e)
        {
            _lookoutViewer.CursorMode = CursorModes.Wait;
            _lookoutViewer.StatusMessage = "Moving ...";
            _TotalMailItems = e.maximum;
            _SoFarMailItems = 0;
            ActionBeginMailItemsProgress(_overallProgress);
            _TotalScanItems = 100;
            _SoFarScanItems = 0;
            ActionBeginScanItemsProgress(_currentActivityProgress);
        }

        void _LookoutMover_OnUpdateMove(LookoutMover.UpdateMoveEventArgs e)
        {
            ActionUpdateMailItemsProgress(_overallProgress, e.increment);
            _lookoutViewer.StatusMessage = String.Format("Moving: {0} %, Items So Far = {1}, Total Items = {2}", _overallProgress.Value, _SoFarMailItems.ToString("#,##0"), _TotalMailItems.ToString("#,##0"));
        }

        void _LookoutMover_OnEndOfMove(LookoutMover.EndOfMoveEventArgs e)
        {
            ActionEndOfMailItemsProgress(_overallProgress);
            ActionEndOfScanItemsProgress(_currentActivityProgress);
            _lookoutViewer.StatusMessage = "End of Move";
            _lookoutViewer.CursorMode = CursorModes.Normal;
        }
        #endregion

        #region Progress Event Helper Methods.
        private void ActionBeginMailItemsProgress(ProgressLink progressLink)
        {
            progressLink.Minimum = 0;
            progressLink.Maximum = 100;
            progressLink.Value = 0;
            _lookoutViewer.OverallProgress = progressLink;
        }

        private void ActionUpdateMailItemsProgress(ProgressLink progressLink, long increment)
        {
            _SoFarMailItems += increment;
            if (_TotalMailItems < _SoFarMailItems)
            {
                _TotalMailItems = _SoFarMailItems * 2;
            }
            long percent = 0;
            if (_TotalMailItems > 0)
            {
                percent = _SoFarMailItems * 100;
                percent /= _TotalMailItems;
            }
            progressLink.Value = (int)percent;
            _lookoutViewer.OverallProgress = progressLink;
        }

        private void ActionEndOfMailItemsProgress(ProgressLink progressLink)
        {
            _TotalMailItems = _SoFarMailItems;
            _SoFarMailItems = 0;
            progressLink.Minimum = 0;
            progressLink.Maximum = 100;
            progressLink.Value = progressLink.Maximum;
            _lookoutViewer.OverallProgress = progressLink;
        }

        private void ActionBeginScanItemsProgress(ProgressLink progressLink)
        {
            progressLink.Minimum = 0;
            progressLink.Maximum = 100;
            progressLink.Value = 0;
            _lookoutViewer.CurrentActivityProgress = progressLink;
        }

        private void ActionUpdateScanItemsProgress(ProgressLink progressLink, long increment)
        {
            _SoFarScanItems += increment;
            if (_TotalScanItems < _SoFarScanItems)
            {
                _TotalScanItems = _SoFarScanItems * 2;
            }
            long percent = 0;
            if (_TotalScanItems > 0)
            {
                percent = _SoFarScanItems * 100;
                percent /= _TotalScanItems;
            }
            progressLink.Value = (int)percent;
            _lookoutViewer.CurrentActivityProgress = progressLink;
        }

        private void ActionEndOfScanItemsProgress(ProgressLink progressLink)
        {
            _TotalScanItems = _SoFarScanItems;
            _SoFarScanItems = 0;
            progressLink.Minimum = 0;
            progressLink.Maximum = 100;
            progressLink.Value = progressLink.Maximum;
            _lookoutViewer.CurrentActivityProgress = progressLink;
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
            _lookoutViewer.TargetOutlookFolder = userSetting.Folder;
            string criteriaName = userSetting.Key;
            _lookoutViewer.CriteriaName = criteriaName;
            _lookoutViewer.SenderCriteria = userSetting.SenderCriteria;
            _lookoutViewer.SubjectCriteria = userSetting.SubjectCriteria;
            _lookoutViewer.BodyCriteria = userSetting.BodyCriteria;
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
                Folder = _lookoutViewer.TargetOutlookFolder,
                SenderCriteria = _lookoutViewer.SenderCriteria,
                SubjectCriteria = _lookoutViewer.SubjectCriteria,
                BodyCriteria = _lookoutViewer.BodyCriteria
            };
            _profileManager.UserSettings.Persist(userSetting);
            LoadParameters();
        }

        /// <summary>
        /// Refresh ComboBox at the begining or when a new entry has been added.
        /// </summary>
        private void RefreshCombo()
        {
            _lookoutViewer.PreviousSelections_IndexChanged_Event_On = false;
            List<UserSetting> sortedList =
                       (from setting in _profileManager.UserSettings.Values.ToList()
                        orderby setting.Key
                        select setting).ToList();
            _lookoutViewer.PreviousSelections_DataSource = sortedList;
            _lookoutViewer.PreviousSelections_DisplayMember = "Key";
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
            }
            _lookoutViewer.PreviousSelections_SelectedString = userSetting.Key;
            if (_lookoutViewer.PreviousSelections_Count == 0)
            {
                _lookoutViewer.PreviousSelections_Text = string.Empty;
            }
            _lookoutViewer.PreviousSelections_IndexChanged_Event_On = true;
        }

        /// <summary>
        /// Set current key.
        /// </summary>
        private void SetCurrentKey()
        {
            if (_lookoutViewer.CriteriaName.Trim().Length > 0)
            {
                _currentKey = _lookoutViewer.CriteriaName;
            }
            else
            {
                _currentKey = string.Empty;
            }
        }

        /// <summary>
        /// Validate all fields.
        /// </summary>
        private int ValidateFields()
        {
            _lookoutViewer.ClearErrors();
            int errorCount = 0;
            LookoutMover mover = new LookoutMover();
            if (!mover.Exists(_lookoutViewer.TargetOutlookFolder))
            {
                errorCount++;
                _lookoutViewer.SetFieldError("TargetOutlookFolder", "Target outlook folder not found");
            }
            if (_lookoutViewer.CriteriaName.Trim().Length == 0)
            {
                errorCount++;
                _lookoutViewer.SetFieldError("CriteriaName", "Criteria name must be entered and must be unique");
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