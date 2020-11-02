using Copy9.DataLayer.Profile;
using Copy9.Models;
using System;
using System.IO;

namespace Copy9
{
    /// <summary>
    /// Synchronize engine controller class.
    /// </summary>
    /// <remarks>
    /// Synchronize engine controller class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class SynchronizeEngine
    {
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        #region Member variables.
        private string newFile = string.Empty;
        private string oldFile = string.Empty;
        private UserSetting.CopyRuleEnum copyRule = UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer;
        private bool monitoredTypesOnly = false;
        private DateTime dateThreshold = DateTime.MinValue;
        private bool identical = false;
        private DateTime completeTaskStartTime;
        private Interrupt interrupt;
        private long totalBytes = 0;
        private bool changeOfDirectory = false;
        private long sameCount;
        private long deleteCount;
        private long removeCount;
        private long createCount;
        private long insertCount;
        private long updateCount;
        private long copyCount;
        #endregion

        #region Properties.
        public Actor Actor { get; set; }

        /// <summary>
        /// Copy rule used.
        /// </summary>
        public UserSetting.CopyRuleEnum CopyRule
        {
            get
            {
                return copyRule;
            }
            set
            {
                copyRule = value;
            }
        }

        /// <summary>
        /// Only process known monitored types.
        /// </summary>
        public bool MonitoredTypesOnly
        {
            get
            {
                return monitoredTypesOnly;
            }
            set
            {
                monitoredTypesOnly = value;
            }
        }

        /// <summary>
        /// Only process if file DateTime is greater than or equal to threshold DateTime.
        /// </summary>
        public DateTime DateThreshold
        {
            get
            {
                return dateThreshold;
            }
            set
            {
                dateThreshold = value;
            }
        }

        /// <summary>
        /// Complete Task Start Time.
        /// </summary>
        public DateTime CompleteTaskStartTime
        {
            get
            {
                return completeTaskStartTime;
            }
        }

        /// <summary>
        /// True if both files are identical.
        /// </summary>
        public bool Identical
        {
            get
            {
                return identical;
            }
        }

        /// <summary>
        /// User interrupt object.
        /// </summary>
        public Interrupt Interrupt
        {
            get
            {
                return interrupt;
            }
            set
            {
                interrupt = value;
            }
        }
        #endregion

        #region Custom Event Arguments.
        public class BeginSynchronizeEventArgs : EventArgs
        {
            public readonly long maximum;
            public readonly string message;
            public BeginSynchronizeEventArgs(long maximum, string message)
            {
                this.maximum = maximum;
                this.message = message;
            }
        }
        public class UpdateSynchronizeEventArgs : EventArgs
        {
            public readonly long increment;
            public readonly string message;
            public UpdateSynchronizeEventArgs(long increment, string message)
            {
                this.increment = increment;
                this.message = message;
            }
        }
        public class EndOfSynchronizeEventArgs : EventArgs
        {
            public readonly long totalLength;
            public readonly string message;
            public EndOfSynchronizeEventArgs(long totalLength, string message)
            {
                this.totalLength = totalLength;
                this.message = message;
            }
        }
        #endregion

        #region Delegates.
        public delegate void BeginDirectoryScanHandler(BeginSynchronizeEventArgs e);
        public delegate void UpdateDirectoryScanHandler(UpdateSynchronizeEventArgs e);
        public delegate void EndOfDirectoryScanHandler(EndOfSynchronizeEventArgs e);
        public delegate void BeginSynchronizeHandler(BeginSynchronizeEventArgs e);
        public delegate void UpdateSynchronizeHandler(UpdateSynchronizeEventArgs e);
        public delegate void EndOfSynchronizeHandler(EndOfSynchronizeEventArgs e);
        public delegate void BeginDirectoryScanTwoHandler(BeginSynchronizeEventArgs e);
        public delegate void UpdateDirectoryScanTwoHandler(UpdateSynchronizeEventArgs e);
        public delegate void EndOfDirectoryScanTwoHandler(EndOfSynchronizeEventArgs e);
        #endregion

        #region Event Declarations.
        public event BeginDirectoryScanHandler OnBeginDirectoryScan;
        public event UpdateDirectoryScanHandler OnUpdateDirectoryScan;
        public event EndOfDirectoryScanHandler OnEndOfDirectoryScan;
        public event BeginSynchronizeHandler OnBeginSynchronize;
        public event UpdateSynchronizeHandler OnUpdateSynchronize;
        public event EndOfSynchronizeHandler OnEndOfSynchronize;
        public event BeginDirectoryScanTwoHandler OnBeginDirectoryScanTwo;
        public event UpdateDirectoryScanTwoHandler OnUpdateDirectoryScanTwo;
        public event EndOfDirectoryScanTwoHandler OnEndOfDirectoryScanTwo;
        #endregion

        #region Event raising helper methods.
        /// <summary>
        /// Trigger begin directory scan event.
        /// </summary>
        private void SignalBeginDirectoryScan(long maximum)
        {
            if (OnBeginDirectoryScan != null)
            {
                OnBeginDirectoryScan(new BeginSynchronizeEventArgs(maximum, string.Empty));
            }
        }

        /// <summary>
        /// Trigger update directory scan event.
        /// </summary>
        private void SignalUpdateDirectoryScan(long increment)
        {
            if (OnUpdateDirectoryScan != null)
            {
                OnUpdateDirectoryScan(new UpdateSynchronizeEventArgs(increment, string.Empty));
            }
        }

        /// <summary>
        /// Trigger end of directory scan event.
        /// </summary>
        private void SignalEndOfDirectoryScan(string message)
        {
            if (OnEndOfDirectoryScan != null)
            {
                OnEndOfDirectoryScan(new EndOfSynchronizeEventArgs(this.totalBytes, message));
            }
        }

        /// <summary>
        /// Trigger begin synchronize event.
        /// </summary>
        private void SignalBeginSynchronize(long maximum)
        {
            if (OnBeginSynchronize != null)
            {
                OnBeginSynchronize(new BeginSynchronizeEventArgs(maximum, string.Empty));
            }
        }

        /// <summary>
        /// Trigger update synchronize event.
        /// </summary>
        private void SignalUpdateSynchronize(long increment, string message)
        {
            if (OnUpdateSynchronize != null)
            {
                OnUpdateSynchronize(new UpdateSynchronizeEventArgs(increment, message));
            }
        }

        /// <summary>
        /// Trigger end of synchronize event.
        /// </summary>
        private void SignalEndOfSynchronize(string message)
        {
            if (OnEndOfSynchronize != null)
            {
                OnEndOfSynchronize(new EndOfSynchronizeEventArgs(this.totalBytes, message));
            }
        }

        /// <summary>
        /// Trigger begin directory scan two event.
        /// </summary>
        private void SignalBeginDirectoryScanTwo(long maximum)
        {
            if (OnBeginDirectoryScanTwo != null)
            {
                OnBeginDirectoryScanTwo(new BeginSynchronizeEventArgs(maximum, string.Empty));
            }
        }

        /// <summary>
        /// Trigger update directory scan two event.
        /// </summary>
        private void SignalUpdateDirectoryScanTwo(long increment)
        {
            if (OnUpdateDirectoryScanTwo != null)
            {
                OnUpdateDirectoryScanTwo(new UpdateSynchronizeEventArgs(increment, string.Empty));
            }
        }

        /// <summary>
        /// Trigger end of directory scan two event.
        /// </summary>
        private void SignalEndOfDirectoryScanTwo(string message)
        {
            if (OnEndOfDirectoryScanTwo != null)
            {
                OnEndOfDirectoryScanTwo(new EndOfSynchronizeEventArgs(this.totalBytes, message));
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SynchronizeEngine(Actor actor)
        {
            Actor = actor;
            completeTaskStartTime = DateTime.Now;
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Process all comparison entries to determine if the files need to be synchronized.
        /// </summary>
        /// <remarks>
        /// Normal synchronize which aims to make the target identical to the source
        /// so it will delete files and remove directories.
        /// </remarks>
        public void Process(Comparisons<Comparison> comparisons, DateTime startAllDateTime, bool ignoreLogs)
        {
            Process(comparisons, startAllDateTime, true, ignoreLogs);
        }

        /// <summary>
        /// Process all comparison entries to determine if the files need to be synchronized.
        /// </summary>
        /// <remarks>
        /// The normal synchronize has allowDelete set to true.
        /// If allowDelete is set to false, then the process will create directories plus insert and update files
        /// but it will not remove directories or delete files so it acts like a copy over, so that if you also
        /// select "replace only with newer" then it will keep the latest versions of everything.
        /// </remarks>
        public void Process(Comparisons<Comparison> comparisons, DateTime startAllDateTime, bool allowDelete, bool ignoreLogs)
        {
            sameCount = 0;
            deleteCount = 0;
            removeCount = 0;
            createCount = 0;
            insertCount = 0;
            updateCount = 0;
            copyCount = 0;
            string prevSourceDirectory = string.Empty;
            string prevSourceFileName = string.Empty;
            string prevTargetDirectory = string.Empty;
            string prevTargetFileName = string.Empty;
            string prevCommand = string.Empty;
            bool identical = true;
            try
            {
                char separator = Path.DirectorySeparatorChar;
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", "Synchronize Source Dir " + Administrator.ProfileManager.UserSettings.SelectedItem.NewBaseDir + separator);
                Administrator.Tracer.WriteMsg("I", "Synchronize Target Dir " + Administrator.ProfileManager.UserSettings.SelectedItem.OldBaseDir + separator);
                Administrator.Tracer.StartAllDateTime = startAllDateTime;
                if (comparisons.Count == 0)
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteMsg("I", "Nothing to compare");
                }
                long totalChangedBytes = CalculateTotalChangedBytes(comparisons);
                SignalBeginSynchronize(totalChangedBytes);
                for (int count = 0; count < comparisons.Count; count++)
                {
                    Comparison comparison = comparisons[count];
                    if (ignoreLogs)
                    {
                        if (comparison.SourceType == "log")
                        {
                            continue;
                        }
                    }
                    string sourceDirectory = string.Empty;
                    string sourceFileName = string.Empty;
                    string targetDirectory = string.Empty;
                    string targetFileName = string.Empty;
                    if (comparison.SourceType == "dir")
                    {
                        sourceDirectory = comparison.SourceEntry;
                        sourceFileName = FileHelper.FileFullName(comparison.SourceEntry);
                        targetDirectory = comparison.TargetEntry;
                        targetFileName = FileHelper.FileFullName(comparison.TargetEntry);
                    }
                    else
                    {
                        sourceDirectory = FileHelper.FileStem(comparison.SourceEntry);
                        sourceFileName = FileHelper.FileFullName(comparison.SourceEntry);
                        targetDirectory = FileHelper.FileStem(comparison.TargetEntry);
                        targetFileName = FileHelper.FileFullName(comparison.TargetEntry);
                    }
                    if (sourceDirectory != prevSourceDirectory)
                    {
                        this.changeOfDirectory = true;
                        prevSourceDirectory = sourceDirectory;
                    }
                    if (targetDirectory != prevTargetDirectory)
                    {
                        this.changeOfDirectory = true;
                        prevTargetDirectory = targetDirectory;
                    }
                    if (comparison.Command != prevCommand)
                    {
                        this.changeOfDirectory = true;
                        prevCommand = comparison.Command;
                    }
                    switch (comparison.Command)
                    {
                        case "Same":
                            // Same directory entries only, no need to do anything.
                            bool comparisonsSameFooter = true;
                            comparisonsSameFooter &= comparison.SortOrder == "Same";
                            comparisonsSameFooter &= comparison.Action == "Same";
                            comparisonsSameFooter &= comparison.Command == "Same";
                            comparisonsSameFooter &= comparison.DisplayCommand == "Same";
                            comparisonsSameFooter &= comparison.Outcome == "Same";
                            comparisonsSameFooter &= comparison.SourceType == "Same";
                            if (comparisonsSameFooter)
                            {
                                // If from reinflated comparisons CSV file then count is taken from this value.
                                // This is just the count that appears in the summary at the end.
                                // nothing gets done for any rows marked as "Same".
                                sameCount = comparison.SourceSize;
                            }
                            else
                            {
                                // Locally each comparison marked as "Same" adds to the count.
                                sameCount++;
                            }
                            break;
                        case "Delete":
                            if (allowDelete)
                            {
                                deleteCount++;
                                identical = false;
                                printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                DeleteFile(comparison);
                                SignalUpdateSynchronize(comparison.TargetSize, comparison.DisplayCommand);
                            }
                            break;
                        case "Remove":
                            if (allowDelete)
                            {
                                removeCount++;
                                identical = false;
                                printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                RemoveDirectory(comparison);
                                SignalUpdateSynchronize(comparison.TargetSize, comparison.DisplayCommand);
                            }
                            break;
                        case "Create":
                            createCount++;
                            identical = false;
                            printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                            CreateDirectory(comparison);
                            SignalUpdateSynchronize(comparison.SourceSize, comparison.DisplayCommand);
                            break;
                        case "Insert":
                            if (CompareDateTimeStamps(comparison.SourceDate, DateThreshold) >= 0)
                            {
                                insertCount++;
                                copyCount++;
                                identical = false;
                                printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                CopyFileUnconditional(comparison);
                            }
                            SignalUpdateSynchronize(comparison.SourceSize, comparison.DisplayCommand);
                            break;
                        case "Update":
                            bool doCopy = false;
                            if (CompareDateTimeStamps(comparison.SourceDate, DateThreshold) < 0)
                            {
                                doCopy = false;
                            }
                            else if (copyRule == UserSetting.CopyRuleEnum.SkipMatches)
                            {
                                doCopy = false;
                                if (Administrator.ProfileManager.SystemProfile.Verbose)
                                {
                                    printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped", sourceFileName));
                                }
                            }
                            else if (copyRule == UserSetting.CopyRuleEnum.ReplaceMatches)
                            {
                                doCopy = true;
                            }
                            else if (copyRule == UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer)
                            {
                                if (CompareDateTimeStamps(comparison.SourceDate, comparison.TargetDate) > 0)
                                {
                                    doCopy = true;
                                }
                                else
                                {
                                    doCopy = false;
                                    if (Administrator.ProfileManager.SystemProfile.Verbose)
                                    {
                                        printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is not newer", sourceFileName));
                                    }
                                }
                            }
                            else if (copyRule == UserSetting.CopyRuleEnum.ReplacePreservingNewest)
                            {
                                if (CompareDateTimeStamps(comparison.SourceDate, comparison.TargetDate) >= 0)
                                {
                                    doCopy = true;
                                }
                                else
                                {
                                    doCopy = false;
                                    if (Administrator.ProfileManager.SystemProfile.Verbose)
                                    {
                                        printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is older", sourceFileName));
                                    }
                                }
                            }
                            if (doCopy)
                            {
                                updateCount++;
                                copyCount++;
                                identical = false;
                                printDirectoryChange(changeOfDirectory, comparison.Command, sourceDirectory, targetDirectory);
                                CopyFileUnconditional(comparison);
                                SignalUpdateSynchronize(comparison.SourceSize, comparison.DisplayCommand);
                            }
                            else
                            {
                                sameCount++;
                                SignalUpdateSynchronize(comparison.SourceSize, comparison.DisplayCommand);
                            }
                            break;
                    }
                    if (Interrupt.Reason == "Cancel")
                    {
                        break;
                    }
                }
                string message = string.Empty;
                string suffix = string.Empty;
                if (Interrupt.Reason == "Cancel")
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteMsg("W", "Synchronization process cancelled");
                }
                else
                {
                    if (identical)
                    {
                        Administrator.Tracer.WriteLine();
                        Administrator.Tracer.WriteMsg("I", "No Changes Detected");
                    }
                    else
                    {
                        Administrator.Tracer.WriteLine();
                        Administrator.Tracer.WriteMsg("I", "Changes Detected");
                        Administrator.Tracer.WriteLine();
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Same   Count = {0}", sameCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Delete Count = {0}", deleteCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Remove Count = {0}", removeCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Create Count = {0}", createCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Insert Count = {0}", insertCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Update Count = {0}", updateCount));
                        Administrator.Tracer.WriteMsg("I", String.Format(@"Copy   Count = {0}", copyCount));
                    }
                }
                SignalEndOfSynchronize(message);
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Calculate the total number of changed bytes.
        /// </summary>
        public long CalculateTotalChangedBytes(Comparisons<Comparison> comparisons)
        {
            long averageBytesPerCommand = CalculateAverageBytesPerCommand(comparisons);
            AssignAverageBytesToDirectoryCommands(comparisons, averageBytesPerCommand);
            long totalBytes = 0;
            foreach (Comparison comparison in comparisons)
            {
                switch (comparison.Command)
                {
                    case "Delete":
                    case "Remove":
                    case "Create":
                    case "Insert":
                        totalBytes += comparison.SourceSize;
                        break;
                    case "Update":
                        if (DoCopy(comparison))
                        {
                            totalBytes += comparison.SourceSize;
                        }
                        break;
                    default:
                        break;
                }
            }
            return totalBytes;
        }

        /// <summary>
        /// Calculate the average number of bytes per command.
        /// 
        /// Directory create and remove commands have zero byte lengths which means that if there are a lot of directory commands there will be
        /// no apparent progress while those commands are executed.  To overcome this problem we will calculate the average byte size of all the
        /// non-zero delete / insert / update commands then assign this avergae bytes size to all the directory create and remove commands.
        /// For the progress display the total bytes will be calculated including the artificially assigned values which will inflate the total a bit
        /// but the progress display will hopefully give a better indication of progress.
        /// </summary>
        public long CalculateAverageBytesPerCommand(Comparisons<Comparison> comparisons)
        {
            int commandCount = 0;
            long totalBytes = 0;
            foreach (Comparison comparison in comparisons)
            {
                switch (comparison.Command)
                {
                    case "Delete":
                        commandCount++;
                        totalBytes += Math.Max(comparison.SourceSize, 1);
                        break;
                    case "Insert":
                        commandCount++;
                        totalBytes += Math.Max(comparison.SourceSize, 1);
                        break;
                    case "Update":
                        if (DoCopy(comparison))
                        {
                            commandCount++;
                            totalBytes += Math.Max(comparison.SourceSize, 1);
                        }
                        break;
                    default:
                        break;
                }
            }
            long averageBytestPerCommand = 0;
            if (commandCount > 0)
            {
                averageBytestPerCommand = totalBytes / commandCount;
            }
            return averageBytestPerCommand;
        }

        /// <summary>
        /// Assign average bytes per command to every create and remove directory command.
        /// </summary>
        public void AssignAverageBytesToDirectoryCommands(Comparisons<Comparison> comparisons, long averageBytesPerCommand)
        {
            foreach (Comparison comparison in comparisons)
            {
                switch (comparison.Command)
                {
                    case "Remove":
                    case "Create":
                        comparison.SourceSize = averageBytesPerCommand;
                        comparison.TargetSize = averageBytesPerCommand;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Decide whether to do the copy for update comparisons.
        /// </summary>
        public bool DoCopy(Comparison comparison)
        {
            bool doCopy = false;
            if (comparison.Command == "Update")
            {
                if (copyRule == UserSetting.CopyRuleEnum.SkipMatches)
                {
                    doCopy = false;
                }
                else if (copyRule == UserSetting.CopyRuleEnum.ReplaceMatches)
                {
                    doCopy = true;
                }
                else if (copyRule == UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer)
                {
                    if (CompareDateTimeStamps(comparison.SourceDate, comparison.TargetDate) > 0)
                    {
                        doCopy = true;
                    }
                    else
                    {
                        doCopy = false;
                    }
                }
                else if (copyRule == UserSetting.CopyRuleEnum.ReplacePreservingNewest)
                {
                    if (CompareDateTimeStamps(comparison.SourceDate, comparison.TargetDate) >= 0)
                    {
                        doCopy = true;
                    }
                    else
                    {
                        doCopy = false;
                    }
                }
            }
            return doCopy;
        }

        private void printDirectoryChange(bool changeOfDirectory, string currentCommand, string sourceDirectory, string targetDirectory)
        {
            if (this.changeOfDirectory)
            {
                this.changeOfDirectory = false;
                if (currentCommand == "Insert" || currentCommand == "Update")
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} From : ""{1}""", currentCommand, sourceDirectory));
                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} To   : ""{1}""", currentCommand, targetDirectory));
                }
                if (currentCommand == "Delete")
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} From : ""{1}""", currentCommand, targetDirectory));
                }
                if (currentCommand == "Remove")
                {
                    string targetStem = FileHelper.FileStem(targetDirectory);
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} From : ""{1}""", currentCommand, targetStem));
                }
                if (currentCommand == "Create")
                {
                    string targetStem = FileHelper.FileStem(targetDirectory);
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} In : ""{1}""", currentCommand, targetStem));
                }
            }
        }

        private void CreateDirectory(Comparison comparison)
        {
            CreateDirectory(comparison.TargetEntry);
        }

        private void CreateDirectory(string targetDirectory)
        {
            if (!Actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                if (Interrupt.Reason != "Cancel")
                {
                    try
                    {
                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Creating  : ""{0}""", targetDirectory));
                        Actor.TargetFiler.CreateDirectory(targetDirectory);
                    }
                    catch (Exception ex)
                    {
                        Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Error while creating : ""{0}""", targetDirectory));
                        Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                    }
                }
            }
        }

        private void RemoveDirectory(Comparison comparison)
        {
            RemoveDirectory(comparison.TargetEntry);
        }

        private void RemoveDirectory(string targetDirectory)
        {
            if (Actor.TargetFiler.DirectoryExists(targetDirectory))
            {
                if (Interrupt.Reason != "Cancel")
                {
                    try
                    {
                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Removing    : ""{0}""", targetDirectory));
                        Actor.TargetFiler.RemoveDirectory(targetDirectory);
                    }
                    catch (Exception ex)
                    {
                        Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Error while removing : ""{0}""", targetDirectory));
                        Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                    }
                }
            }
        }

        private void CopyFileConditional(Comparison comparison)
        {
            string sourceFileSpec = comparison.SourceEntry;
            string targetFileSpec = comparison.TargetEntry;
            string sourceFileName = Actor.SourceFiler.GetFileName(sourceFileSpec);
            string targetDirectory = Actor.TargetFiler.GetDirectoryName(targetFileSpec);
            CopyFileConditional(sourceFileSpec, targetFileSpec, sourceFileName, targetDirectory);
        }

        private void CopyFileConditional(string sourceFileSpec, string targetFileSpec, string sourceFileName, string targetDirectory)
        {
            Actor.TargetFiler.PathCheck(targetFileSpec);
            try
            {
                bool doCopy = false;
                if (Actor.TargetFiler.FileExists(targetFileSpec))
                {
                    if (copyRule == UserSetting.CopyRuleEnum.SkipMatches)
                    {
                        doCopy = false;
                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped", sourceFileName));
                    }
                    else if (copyRule == UserSetting.CopyRuleEnum.ReplaceMatches)
                    {
                        doCopy = true;
                    }
                    else if (copyRule == UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer)
                    {
                        if (CompareDateTimeStamps(Actor.SourceFiler.FileModifiedDateTime(sourceFileSpec), Actor.TargetFiler.FileModifiedDateTime(targetFileSpec)) > 0)
                        {
                            doCopy = true;
                        }
                        else
                        {
                            doCopy = false;
                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is not newer", sourceFileName));
                        }
                    }
                    else if (copyRule == UserSetting.CopyRuleEnum.ReplacePreservingNewest)
                    {
                        if (CompareDateTimeStamps(Actor.SourceFiler.FileModifiedDateTime(sourceFileSpec), Actor.TargetFiler.FileModifiedDateTime(targetFileSpec)) >= 0)
                        {
                            doCopy = true;
                        }
                        else
                        {
                            doCopy = false;
                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is older", sourceFileName));
                        }
                    }
                }
                else
                {
                    doCopy = true;
                }
                if (doCopy)
                {
                    try
                    {
                        Actor.CopyFile(sourceFileSpec, targetFileSpec);
                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Copied", sourceFileName));
                    }
                    catch (Exception ex)
                    {
                        Administrator.Tracer.WriteLine();
                        Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Error while copying : {1}{2}", sourceFileName, Environment.NewLine, ex.Message));
                        Administrator.Tracer.WriteLine();
                    }
                }
            }
            catch (Exception ex2)
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Target File (Name too long) Error while copying : {1}{2}", sourceFileName, Environment.NewLine, ex2.Message));
                Administrator.Tracer.WriteLine();
            }
        }

        private void CopyFileUnconditional(Comparison comparison)
        {
            string sourceFileSpec = comparison.SourceEntry;
            string targetFileSpec = comparison.TargetEntry;
            string sourceFileName = Actor.SourceFiler.GetFileName(sourceFileSpec);
            string targetDirectory = Actor.TargetFiler.GetDirectoryName(targetFileSpec);
            CopyFileUnconditional(sourceFileSpec, targetFileSpec, sourceFileName, targetDirectory);
        }

        private void CopyFileUnconditional(string sourceFileSpec, string targetFileSpec, string sourceFileName, string targetDirectory)
        {
            Actor.TargetFiler.PathCheck(targetFileSpec);
            try
            {
                Actor.CopyFile(sourceFileSpec, targetFileSpec);
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Copied", sourceFileName));
            }
            catch (Exception ex)
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Error while copying : {1}{2}", sourceFileName, Environment.NewLine, ex.Message));
                Administrator.Tracer.WriteLine();
            }
        }

        private void DeleteFile(Comparison comparison)
        {
            string targetFileSpec = comparison.TargetEntry;
            DeleteFile(targetFileSpec);
        }

        private void DeleteFile(string targetFileSpec)
        {
            if (Actor.TargetFiler.FileExists(targetFileSpec))
            {
                if (Interrupt.Reason != "Cancel")
                {
                    try
                    {
                        Actor.TargetFiler.DeleteFile(targetFileSpec);
                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Deleted", targetFileSpec));
                    }
                    catch (Exception ex)
                    {
                        Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Error while deleting : ""{0}""", targetFileSpec));
                        Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                    }
                }
            }
        }

        //TODO: Plan to add a synchronize method which does the directory scans as well, later.
        /// <summary>
        /// Synchronize files from a complete directory tree to another directory.
        /// </summary>
        /// <remarks>
        /// The CopyRule determines which action to take when the source and target files match.
        /// 
        /// The CopyRule(s) are:
        /// SkipMatches - Don't replace any matching target files.
        /// ReplaceMatches - Replace all matching target files unconditionally
        /// ReplaceOnlyIfNewer - Only replace target files if the matching source file has a newer last modified DateTime stamp.
        /// ReplacePreservingNewest - Replace target files if the matching source file has either the same or a newer last modified DateTime stamp.
        /// </remarks>
        private void Synchronize(string sourceDirectory, string targetDirectory, UserSetting.CopyRuleEnum copyRule)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "Begin Synchronize");
            SignalBeginSynchronize(99);
            //XCopy(diSource, diTarget, copyRule);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "End Synchronize");
            SignalEndOfSynchronize(string.Empty);
        }

        private int CompareDateTimeStamps(DateTime sourceDateTime, DateTime targetDateTime)
        {
            string sourceDateTimeText = sourceDateTime.ToString(DATE_TIME_FORMAT);
            string targetDateTimeText = targetDateTime.ToString(DATE_TIME_FORMAT);
            int comparison = sourceDateTimeText.CompareTo(targetDateTimeText);
            return comparison;
        }
        #endregion
    }
}