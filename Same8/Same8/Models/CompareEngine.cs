using Log8;
using Same8.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;

namespace Same8.Models
{
    /// <summary>
    /// Directory comparison engine.
    /// </summary>
    /// <remarks>
    /// To compare two directory trees.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    class CompareEngine
    {
        #region Events.
        public event EventDelegate EventBeginProgress;
        public event EventDelegate EventUpdateProgress;
        public event EventDelegate EventEndOfProgress;
        public event EventDelegate EventCriteriaPass;

        /// <summary>
        /// Trigger begin progress event.
        /// </summary>
        /// <param name="pnMaximum">Estimated or actual maximum for progress bar.</param>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        private void SignalBeginProgress(int pnMaximum, string psMethod, string psMessage)
        {
            if (EventBeginProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(pnMaximum, psMethod, psMessage);
                EventBeginProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger update progress event.
        /// </summary>
        /// <param name="pnIncrement">How much to increment progress bar by.</param>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        private void SignalUpdateProgress(int pnIncrement, string psMethod, string psMessage)
        {
            if (EventUpdateProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(pnIncrement, psMethod, psMessage);
                EventUpdateProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger end of progress event.
        /// </summary>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        private void SignalEndOfProgress(string psMethod, string psMessage)
        {
            if (EventEndOfProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(0, psMethod, psMessage);
                EventEndOfProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger criteria pass event.
        /// </summary>
        /// <param name="psContext">Found data in context to display.</param>
        private void SignalCriteriaPass(string psContext)
        {
            if (EventCriteriaPass != null)
            {
                EventParameters oEventParameters = new EventParameters(0, string.Empty, psContext);
                EventCriteriaPass(this, oEventParameters);
            }
        }
        #endregion

        #region Constants.
        const string PROVIDER = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        #endregion

        #region Member variables.
        #region Property members.
        private string msAction = string.Empty;
        private string msNewPath = string.Empty;
        private string msOldPath = string.Empty;
        private Comparisons<Comparison> mcComparisons;
        private bool mbIdentical = false;
        #endregion
        #region Other members.
        private ProfileManager _profileManager;
        private DifLauncher moDifLuncher;
        private Dir moDir;
        private DirFuzzy moDirFuzzy;
        private long mnNewFilesEstimate = 0;
        private long mnOldFilesEstimate = 0;
        private long mnChgFilesEstimate = 0;
        private bool mbProcessingNew = false;
        Logger moLog = null;
        List<DirectoryEntry> _OldDirectoryListing;
        List<DirectoryEntry> _NewDirectoryListing;
        List<DirectoryEntry> _ChangedDirectoryListing;
        #endregion
        #endregion

        #region Properties.
        /// <summary>
        /// Action.
        /// </summary>
        public string Action
        {
            get
            {
                return msAction;
            }
            set
            {
                msAction = value;
                if (moDir != null)
                {
                    moDir.Action = msAction;
                }
            }
        }

        /// <summary>
        /// New path.
        /// </summary>
        public string NewPath
        {
            get
            {
                return msNewPath;
            }
            set
            {
                msNewPath = value;
            }
        }

        /// <summary>
        /// Old path.
        /// </summary>
        public string OldPath
        {
            get
            {
                return msOldPath;
            }
            set
            {
                msOldPath = value;
            }
        }

        /// <summary>
        /// Comparisons collection.
        /// </summary>
        public Comparisons<Comparison> Comparisons
        {
            get
            {
                return mcComparisons;
            }
        }

        /// <summary>
        /// Whether both directory trees are identical or not.
        /// </summary>
        public bool Identical
        {
            get
            {
                return mbIdentical;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompareEngine()
        {
            Load();
        }
        #endregion

        #region Event Handlers.
        /// <summary>
        /// Start directory scan progress indicator.
        /// </summary>
        void moDir_EventBeginProgress(object poSender, EventParameters2 poEventArgs)
        {
            long nEstimate = mbProcessingNew ? mnNewFilesEstimate : mnOldFilesEstimate;
            SignalBeginProgress((int)nEstimate, "DirList", "Scanning directories ...");
        }

        /// <summary>
        /// Update directory scan progress indicator.
        /// </summary>
        void moDir_EventUpdateProgress(object poSender, EventParameters2 poEventArgs)
        {
            SignalUpdateProgress(1, "DirList", "Scanning directories ...");
        }

        /// <summary>
        /// End directory scan progress indicator.
        /// </summary>
        void moDir_EventEndOfProgress(object poSender, EventParameters2 poEventArgs)
        {
            string sMessage = string.Empty;
            if (msAction == "Cancel")
            {
                sMessage = "Directory scan cancelled.";
            }
            else
            {
                sMessage = "Directory scan complete.";
            }
            SignalEndOfProgress("DirList", sMessage);
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Normal compare.
        /// </summary>
        public void Compare()
        {
            Save();
            moDir = new Dir();
            moDir.Action = msAction;
            moDir.EventBeginProgress += new Models.EventDelegate(moDir_EventBeginProgress);
            moDir.EventUpdateProgress += new Models.EventDelegate(moDir_EventUpdateProgress);
            moDir.EventEndOfProgress += new Models.EventDelegate(moDir_EventEndOfProgress);
            mbProcessingNew = true;
            _NewDirectoryListing = moDir.DirList(msNewPath, ref mnNewFilesEstimate);
            mbProcessingNew = false;
            _OldDirectoryListing = moDir.DirList(msOldPath, ref mnOldFilesEstimate);
            moDir.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            moDir.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            mcComparisons = moDir.Comparisons;
            Process();
            Save();
        }

        /// <summary>
        /// Fuzzy compare.
        /// </summary>
        public void FuzzyCompare()
        {
            Save();
            moDirFuzzy = new DirFuzzy();
            moDirFuzzy.Action = msAction;
            moDirFuzzy.EventBeginProgress += new Models.EventDelegate(moDir_EventBeginProgress);
            moDirFuzzy.EventUpdateProgress += new Models.EventDelegate(moDir_EventUpdateProgress);
            moDirFuzzy.EventEndOfProgress += new Models.EventDelegate(moDir_EventEndOfProgress);
            mbProcessingNew = true;
            _NewDirectoryListing = moDirFuzzy.DirList(msNewPath, ref mnNewFilesEstimate);
            mbProcessingNew = false;
            _OldDirectoryListing = moDirFuzzy.DirList(msOldPath, ref mnOldFilesEstimate);
            moDirFuzzy.FuzzyMatch(_OldDirectoryListing, _NewDirectoryListing);
            moDirFuzzy.Compare(ref _OldDirectoryListing, ref _NewDirectoryListing);
            moDirFuzzy.Changes(_OldDirectoryListing, _NewDirectoryListing, ref _ChangedDirectoryListing, ref mnChgFilesEstimate);
            mcComparisons = moDirFuzzy.Comparisons;
            Process();
            Save();
        }

        /// <summary>
        /// Compare individual files using "Diff" application.
        /// </summary>
        public void Diff(string psNewFile, string psOldFile)
        {
            _profileManager = new ProfileManager();
            moDifLuncher = new DifLauncher(_profileManager);
            moDifLuncher.NewBaseDir = NewPath;
            moDifLuncher.OldBaseDir = OldPath;
            moDifLuncher.NewFile = psNewFile;
            moDifLuncher.OldFile = psOldFile;
            moDifLuncher.Compare();
        }

        /// <summary>
        /// View the specified file.
        /// </summary>
        public void View(string psFileSpec)
        {
            System.Diagnostics.Process oProc = new System.Diagnostics.Process();
            oProc.StartInfo.FileName = _profileManager.SystemProfile.ViewerWindows;
            oProc.StartInfo.Arguments = psFileSpec;
            oProc.EnableRaisingEvents = false;
            oProc.StartInfo.UseShellExecute = true;
            oProc.StartInfo.CreateNoWindow = false;
            oProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            oProc.Start();
        }

        /// <summary>
        /// Launch Meld to compare the two specified files.
        /// </summary>
        public void MeldCompare(string fileSpecNew, string fileSpecOld)
        {
            System.Diagnostics.Process oProc = new System.Diagnostics.Process();
            oProc.StartInfo.FileName = _profileManager.SystemProfile.MeldProgram;
            oProc.StartInfo.Arguments = String.Format(@"{0} {1}",fileSpecNew,fileSpecOld);
            oProc.EnableRaisingEvents = false;
            oProc.StartInfo.UseShellExecute = true;
            oProc.StartInfo.CreateNoWindow = false;
            oProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            oProc.Start();
        }
        #endregion

        #region Private members.
        /// <summary>
        /// Load system and working profile variables.
        /// </summary>
        private void Load()
        {
            _profileManager = new ProfileManager();
            moDir = new Dir();
            UserSetting userSetting = new UserSetting();
            if (_profileManager.UserSettings.SelectedItem != null)
            {
                userSetting = _profileManager.UserSettings.SelectedItem;
            }
            msNewPath = userSetting.NewPath;
            msOldPath = userSetting.OldPath;
            mnNewFilesEstimate = userSetting.NewFilesEstimate;
            mnOldFilesEstimate = userSetting.OldFilesEstimate;
            mnChgFilesEstimate = userSetting.ChgFilesEstimate;
        }

        /// <summary>
        /// Save working profile variables.
        /// </summary>
        private void Save()
        {
            _profileManager.UserSettings.SelectedItem.NewPath = msNewPath;
            _profileManager.UserSettings.SelectedItem.OldPath = msOldPath;
            _profileManager.UserSettings.SelectedItem.NewFilesEstimate = mnNewFilesEstimate;
            _profileManager.UserSettings.SelectedItem.OldFilesEstimate = mnOldFilesEstimate;
            _profileManager.UserSettings.SelectedItem.ChgFilesEstimate = mnChgFilesEstimate;
            _profileManager.UserSettings.Save();
        }

        /// <summary>
        /// Process all comparison entries to determine if the file contents are the same or not.
        /// </summary>
        private void Process()
        {
            int nNewMaxLen = 0;
            int nOldMaxLen = 0;
            try
            {
                if (mcComparisons.Count > 0)
                {
                    //Determine longest new and old entry lengths for report.
                    SignalBeginProgress(mcComparisons.Count, "Files", "Longest lengths...");
                    for (int nCount = 0; nCount < mcComparisons.Count; nCount++)
                    {
                        Comparison oComparison = mcComparisons[nCount];
                        nNewMaxLen = Math.Max(nNewMaxLen, oComparison.NewEntry.Length);
                        nOldMaxLen = Math.Max(nOldMaxLen, oComparison.OldEntry.Length);
                        string sFileSpec = oComparison.NewEntry != string.Empty ? oComparison.NewEntry : oComparison.OldEntry;
                        SignalUpdateProgress(1, "Files", sFileSpec);
                        if (msAction == "Cancel")
                        {
                            break;
                        }
                    }
                    SignalEndOfProgress("Files", string.Empty);

                    //Create log file.
                    char cSeparator = System.IO.Path.DirectorySeparatorChar;
                    moLog = new Logger();
                    moLog.Prefix = "SAM";
                    moLog.Title = String.Format("SAME COMPARISON PROCESSOR {0}", _profileManager.ApplicationProfile.Version);
                    string logFileSpec = String.Format("{0}{1}Same_Comparison_{2}.log", _profileManager.SystemProfile.LogPath, cSeparator.ToString(), DateTime.Now.ToString("yyyyMMdd@HHmmss"));
                    FileHelper.PathCheck(logFileSpec);
                    moLog.Begin(logFileSpec);
                    moLog.WriteLn();
                    moLog.WriteMsg("010", "I", "Comparison" + " " + msNewPath.PadRight(nNewMaxLen) + " " + msOldPath.PadRight(nOldMaxLen));
                    moLog.WriteMsg("020", "I", new string('=', 10) + " " + new string('=', nNewMaxLen) + " " + new string('=', nOldMaxLen));
                    SignalBeginProgress(mcComparisons.Count, "Files", "Comparing...");
                    mbIdentical = true;
                    for (int nCount = 0; nCount < mcComparisons.Count; nCount++)
                    {
                        Comparison oComparison = mcComparisons[nCount];
                        switch (oComparison.Outcome)
                        {
                            case "DELETED":
                                oComparison.Outcome = "Old";
                                Report(oComparison, nNewMaxLen, nOldMaxLen);
                                mbIdentical = false;
                                break;
                            case "INSERTED":
                                oComparison.Outcome = "New";
                                Report(oComparison, nNewMaxLen, nOldMaxLen);
                                mbIdentical = false;
                                break;
                            case "BOTH":
                                if (oComparison.NewType == "dir" && oComparison.OldType == "dir")
                                {
                                    oComparison.Outcome = "Dir";
                                    Report(oComparison, nNewMaxLen, nOldMaxLen);
                                }
                                else
                                {
                                    string sNewEntry = ReadFile(msNewPath + oComparison.NewEntry);
                                    string sOldEntry = ReadFile(msOldPath + oComparison.OldEntry);
                                    if (sNewEntry.CompareTo(sOldEntry) == 0)
                                    {
                                        oComparison.Outcome = "Same";
                                        Report(oComparison, nNewMaxLen, nOldMaxLen);
                                    }
                                    else
                                    {
                                        if (sNewEntry.EndsWith(Environment.NewLine) && !sOldEntry.EndsWith(Environment.NewLine))
                                        {
                                            int pos = sNewEntry.LastIndexOf(Environment.NewLine);
                                            sNewEntry = sNewEntry.Substring(0, pos);
                                        }
                                        if (!sNewEntry.EndsWith(Environment.NewLine) && sOldEntry.EndsWith(Environment.NewLine))
                                        {
                                            int pos = sOldEntry.LastIndexOf(Environment.NewLine);
                                            sOldEntry = sOldEntry.Substring(0, pos);
                                        }
                                        if (sNewEntry.CompareTo(sOldEntry) == 0)
                                        {
                                            oComparison.Outcome = "Same+";
                                            Report(oComparison, nNewMaxLen, nOldMaxLen);
                                        }
                                        else
                                        {
                                            oComparison.Outcome = "Diff";
                                            Report(oComparison, nNewMaxLen, nOldMaxLen);
                                            mbIdentical = false;
                                        }
                                    }
                                }
                                break;
                        }
                        string sFileSpec = oComparison.NewEntry != string.Empty ? oComparison.NewEntry : oComparison.OldEntry;
                        SignalUpdateProgress(1, "Files", sFileSpec);
                        if (msAction == "Cancel")
                        {
                            break;
                        }
                    }
                    string sMessage = string.Empty;
                    string sSuffix = string.Empty;
                    if (msAction == "Cancel")
                    {
                        moLog.WriteLn();
                        moLog.WriteMsg("040", "W", "Same comparison cancelled.");
                    }
                    else
                    {
                        if (mbIdentical)
                        {
                            moLog.WriteLn();
                            moLog.WriteMsg("050", "I", "Trees are identical.");
                        }
                        else
                        {
                            moLog.WriteLn();
                            moLog.WriteMsg("060", "W", "Differences found.");
                        }
                    }
                    SignalEndOfProgress("Files", sMessage);
                    //Close and free log file.
                    moLog.Outcome();
                    moLog.Terminate(_profileManager.SystemProfile.ViewerWindows);
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
        }

        public string ReCheck(string fileSpecNew, string fileSpecOld)
        {
            string outcome = string.Empty;
            string newEntry = ReadFile(fileSpecNew);
            string oldEntry = ReadFile(fileSpecOld);
            if (newEntry.CompareTo(oldEntry) == 0)
            {
                outcome = "Same";
            }
            else
            {
                if (newEntry.EndsWith(Environment.NewLine) && !oldEntry.EndsWith(Environment.NewLine))
                {
                    int pos = newEntry.LastIndexOf(Environment.NewLine);
                    newEntry = newEntry.Substring(0, pos);
                }
                if (!newEntry.EndsWith(Environment.NewLine) && oldEntry.EndsWith(Environment.NewLine))
                {
                    int pos = oldEntry.LastIndexOf(Environment.NewLine);
                    oldEntry = oldEntry.Substring(0, pos);
                }
                if (newEntry.CompareTo(oldEntry) == 0)
                {
                    outcome = "Same+";
                }
                else
                {
                    outcome = "Diff";
                }
            }
            return outcome;
        }

        /// <summary>
        /// Report one comparison entry.
        /// </summary>
        /// <param name="poComparison"></param>
        private void Report(Comparison poComparison, int pnNewMax, int pnOldMax)
        {
            string sMessage = poComparison.Outcome.PadRight(10);
            sMessage += new string(' ', 1);
            sMessage += poComparison.NewEntry.PadRight(pnNewMax);
            sMessage += new string(' ', 1);
            sMessage += poComparison.OldEntry.PadRight(pnOldMax);
            moLog.WriteMsg("030", "I", sMessage);
        }

        /// <summary>
        /// Read the specified file and return its contents as a string.
        /// </summary>
        /// <param name="psFile">Text file specification.</param>
        /// <returns>Entire file contents as a string.</returns>
        private string ReadFile(string psFile)
        {
            StreamReader oSr;
            string sContents = string.Empty;
            try
            {
                if (File.Exists(psFile))
                {
                    oSr = new StreamReader(psFile);
                    sContents = oSr.ReadToEnd();
                    oSr.Close();
                }
                else
                {
                    sContents = string.Empty;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return sContents;
        }
        #endregion

        #region Delegates.
        public delegate void EventDelegate(object poSender, EventParameters poEventArgs);
        #endregion

        #region Event data.
        /// <summary>
        /// Class that defines data for event handling.
        /// </summary>
        /// <remarks>
        /// This class is used by two different sets of events
        /// which is a bit naughty but that is why the names
        /// are a bit generic.
        /// </remarks>
        public class EventParameters : EventArgs
        {
            #region Member Variables.
            private int mnNumber;
            private string msMethod;
            private string msText;
            #endregion

            #region Properties.
            public int Number
            {
                get
                {
                    return mnNumber;
                }
            }
            public string Method
            {
                get
                {
                    return msMethod;
                }
            }
            public string Text
            {
                get
                {
                    return msText;
                }
            }
            #endregion

            #region Constructors.
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pnNumber">Numeric value.</param>
            /// <param name="psText">Text value.</param>
            public EventParameters(int pnNumber, string psMethod, string psText)
            {
                mnNumber = pnNumber;
                msMethod = psMethod;
                msText = psText;
            }
            #endregion
        }
        #endregion
    }
}