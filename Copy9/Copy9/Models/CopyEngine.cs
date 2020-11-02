using Copy9.DataLayer.Profile;
using System;
using System.IO;

namespace Copy9
{
    /// <summary>
    /// Copy engine controller class.
    /// </summary>
    /// <remarks>
    /// Copy engine controller class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class CopyEngine : ICopyEngine
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
        #endregion

        #region Properties.
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
        public class BeginCopyEventArgs : EventArgs
        {
            public BeginCopyEventArgs()
            {
            }
        }
        public class UpdateCopyEventArgs : EventArgs
        {
            public readonly long increment;
            public UpdateCopyEventArgs(long increment)
            {
                this.increment = increment;
            }
        }
        public class EndOfCopyEventArgs : EventArgs
        {
            public EndOfCopyEventArgs()
            {
            }
            public readonly long totalLength;
            public EndOfCopyEventArgs(long totalLength)
            {
                this.totalLength = totalLength;
            }
        }
        #endregion

        #region Delegates.
        public delegate void BeginCountHandler(BeginCopyEventArgs e);
        public delegate void UpdateCountHandler(UpdateCopyEventArgs e);
        public delegate void EndOfCountHandler(EndOfCopyEventArgs e);
        public delegate void BeginCopyHandler(BeginCopyEventArgs e);
        public delegate void UpdateCopyHandler(UpdateCopyEventArgs e);
        public delegate void EndOfCopyHandler(EndOfCopyEventArgs e);
        public delegate void BeginDeleteHandler(BeginCopyEventArgs e);
        public delegate void UpdateDeleteHandler(UpdateCopyEventArgs e);
        public delegate void EndOfDeleteHandler(EndOfCopyEventArgs e);
        #endregion

        #region Event Declarations.
        public event BeginCountHandler OnBeginCount;
        public event UpdateCountHandler OnUpdateCount;
        public event EndOfCountHandler OnEndOfCount;
        public event BeginCopyHandler OnBeginCopy;
        public event UpdateCopyHandler OnUpdateCopy;
        public event EndOfCopyHandler OnEndOfCopy;
        public event BeginDeleteHandler OnBeginDelete;
        public event UpdateDeleteHandler OnUpdateDelete;
        public event EndOfDeleteHandler OnEndOfDelete;
        #endregion

        #region Event raising helper methods.
        /// <summary>
        /// Trigger begin count event.
        /// </summary>
        private void SignalBeginCount()
        {
            if (OnBeginCount != null)
            {
                OnBeginCount(new BeginCopyEventArgs());
            }
        }

        /// <summary>
        /// Trigger update count event.
        /// </summary>
        private void SignalUpdateCount(long increment)
        {
            if (OnUpdateCount != null)
            {
                OnUpdateCount(new UpdateCopyEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of count event.
        /// </summary>
        private void SignalEndOfCount()
        {
            if (OnEndOfCount != null)
            {
                OnEndOfCount(new EndOfCopyEventArgs(totalBytes));
            }
        }

        /// <summary>
        /// Trigger begin copy event.
        /// </summary>
        private void SignalBeginCopy()
        {
            if (OnBeginCopy != null)
            {
                OnBeginCopy(new BeginCopyEventArgs());
            }
        }

        /// <summary>
        /// Trigger update copy event.
        /// </summary>
        private void SignalUpdateCopy(long increment)
        {
            if (OnUpdateCopy != null)
            {
                OnUpdateCopy(new UpdateCopyEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of copy event.
        /// </summary>
        private void SignalEndOfCopy()
        {
            if (OnEndOfCopy != null)
            {
                OnEndOfCopy(new EndOfCopyEventArgs(totalBytes));
            }
        }

        /// <summary>
        /// Trigger begin delete event.
        /// </summary>
        private void SignalBeginDelete()
        {
            if (OnBeginDelete != null)
            {
                OnBeginDelete(new BeginCopyEventArgs());
            }
        }

        /// <summary>
        /// Trigger update delete event.
        /// </summary>
        private void SignalUpdateDelete(long increment)
        {
            if (OnUpdateDelete != null)
            {
                OnUpdateDelete(new UpdateCopyEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of delete event.
        /// </summary>
        private void SignalEndOfDelete()
        {
            if (OnEndOfDelete != null)
            {
                OnEndOfDelete(new EndOfCopyEventArgs(totalBytes));
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CopyEngine()
        {
            completeTaskStartTime = DateTime.Now;
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Copy files from a complete directory tree to another directory.
        /// </summary>
        public void XCopy(string sourceDirectory, string targetDirectory)
        {
            if (Directory.Exists(sourceDirectory))
            {
                XCount(sourceDirectory);
                XCopy(sourceDirectory, targetDirectory, CopyRule);
            }
            else
            {
                throw new ParameterException("NewBaseDirectory", "Copy From directory not found.");
            }
        }

        /// <summary>
        /// Delete all files and sub directories from a complete directory tree.
        /// </summary>
        public void XDelete(string sourceDirectory)
        {
            if (Directory.Exists(sourceDirectory))
            {
                XCount(sourceDirectory);
                XDelete(sourceDirectory, CopyRule);
            }
            else
            {
                throw new ParameterException("NewBaseDirectory", "Copy From directory not found.");
            }
        }

        /// <summary>
        /// Count the total number of bytes in the source directory and all sub directories of it.
        /// </summary>
        private void XCount(string sourceDirectory)
        {
            SignalBeginCount();
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Counting total bytes in : ""{0}""", sourceDirectory.ToString()));
            totalBytes = 0;
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            XCount(diSource);
            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Total bytes = {0}", totalBytes.ToString("#,##0")));
            SignalEndOfCount();
        }

        private void XCount(DirectoryInfo sourceDirectory)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (MonitoredTypesHelper.AllowFile(fi.FullName, monitoredTypesOnly))
                    {
                        if (Interrupt.Reason != "Cancel")
                        {
                            totalBytes += fi.Length;
                            SignalUpdateCount(fi.Length);
                        }
                    }
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (Interrupt.Reason != "Cancel")
                    {
                        XCount(di);
                    }
                }
            }
            //Check if user has chosen to cancel run.
            if (Interrupt.Reason == "Cancel")
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Count cancelled"));
            }
        }

        /// <summary>
        /// Copy files from a complete directory tree to another directory.
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
        private void XCopy(string sourceDirectory, string targetDirectory, UserSetting.CopyRuleEnum copyRule)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "Begin XCopy");
            SignalBeginCopy();
            XCopy(diSource, diTarget, copyRule);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "End XCopy");
            SignalEndOfCopy();
        }

        private void XCopy(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, UserSetting.CopyRuleEnum copyRule)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                if (!Directory.Exists(targetDirectory.FullName))
                {
                    Directory.CreateDirectory(targetDirectory.FullName);
                }
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Copy From : ""{0}""", sourceDirectory.ToString()));
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Copy To   : ""{0}""", targetDirectory.ToString()));
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (MonitoredTypesHelper.AllowFile(fi.FullName, monitoredTypesOnly))
                    {
                        if (Interrupt.Reason != "Cancel")
                        {
                            try
                            {
                                string targetFile = Path.Combine(targetDirectory.ToString(), fi.Name);
                                FileInfo targetFileInfo = new FileInfo(targetFile);
                                bool doCopy = false;
                                if (targetFileInfo.Exists)
                                {
                                    if (copyRule == UserSetting.CopyRuleEnum.SkipMatches)
                                    {
                                        doCopy = false;
                                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped", fi.Name));
                                    }
                                    else if (copyRule == UserSetting.CopyRuleEnum.ReplaceMatches)
                                    {
                                        doCopy = true;
                                    }
                                    else if (copyRule == UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer)
                                    {
                                        if (CompareDateTimeStamps(fi.LastWriteTime, targetFileInfo.LastWriteTime) > 0)
                                        {
                                            doCopy = true;
                                        }
                                        else
                                        {
                                            doCopy = false;
                                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is not newer", fi.Name));
                                        }
                                    }
                                    else if (copyRule == UserSetting.CopyRuleEnum.ReplacePreservingNewest)
                                    {
                                        if (CompareDateTimeStamps(fi.LastWriteTime, targetFileInfo.LastWriteTime) >= 0)
                                        {
                                            doCopy = true;
                                        }
                                        else
                                        {
                                            doCopy = false;
                                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Skipped - Source file is older", fi.Name));
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
                                        if (targetFileInfo.Exists)
                                        {
                                            targetFileInfo.IsReadOnly = false;
                                        }
                                        fi.CopyTo(Path.Combine(targetDirectory.ToString(), fi.Name), true);
                                        targetFileInfo = new FileInfo(targetFile);
                                        if (targetFileInfo.Exists)
                                        {
                                            targetFileInfo.IsReadOnly = false;
                                        }
                                        Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Copied", fi.Name));
                                    }
                                    catch (Exception ex)
                                    {
                                        Administrator.Tracer.WriteLine();
                                        Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Error : {1}{2}", fi.Name, Environment.NewLine, ex.Message));
                                        Administrator.Tracer.WriteLine();
                                    }
                                }
                                SignalUpdateCopy(fi.Length);
                            }
                            catch (Exception ex2)
                            {
                                Administrator.Tracer.WriteLine();
                                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Target File (Name too long) Error : {1}{2}", fi.Name, Environment.NewLine, ex2.Message));
                                Administrator.Tracer.WriteLine();
                            }
                        }
                    }
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (Interrupt.Reason != "Cancel")
                    {
                        try
                        {
                            if (DirectoryExclusionsHelper.AllowDirectory(di.FullName))
                            {
                                DirectoryInfo targetDirectoryInfo = targetDirectory.CreateSubdirectory(di.Name);
                                XCopy(di, targetDirectoryInfo, copyRule);
                            }
                        }
                        catch (Exception ex)
                        {
                            Administrator.Tracer.WriteLine();
                            Administrator.Tracer.WriteTimedMsg("E", String.Format(@"{0} - Target Directory (Name too long) Error : {1}{2}", di.Name, Environment.NewLine, ex.Message));
                            Administrator.Tracer.WriteLine();
                        }
                    }
                }
            }
            //Check if user has chosen to cancel run.
            if (Interrupt.Reason == "Cancel")
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Copy cancelled"));
                return;
            }
        }

        /// <summary>
        /// Delete all files and sub directories from a complete directory tree.
        /// </summary>
        private void XDelete(string sourceDirectory, UserSetting.CopyRuleEnum copyRule)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "Begin XDelete");
            SignalBeginDelete();
            XDelete(diSource, copyRule);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "End XDelete");
            SignalEndOfDelete();
        }

        private void XDelete(DirectoryInfo sourceDirectory, UserSetting.CopyRuleEnum copyRule)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Delete From : ""{0}""", sourceDirectory.ToString()));
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (Interrupt.Reason != "Cancel")
                    {
                        XDelete(di, copyRule);
                    }
                }
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (MonitoredTypesHelper.AllowFile(fi.FullName, monitoredTypesOnly))
                    {
                        if (fi.Exists)
                        {
                            long fileLength = fi.Length;
                            if (Interrupt.Reason != "Cancel")
                            {
                                try
                                {
                                    fi.IsReadOnly = false;
                                    fi.Delete();
                                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} - Deleted", fi.Name));
                                }
                                catch (Exception ex)
                                {
                                    Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Error while deleting : ""{0}""", fi.Name));
                                    Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                                }
                            }
                            SignalUpdateDelete(fileLength);
                        }
                    }
                }
                if (Directory.Exists(sourceDirectory.FullName))
                {
                    if (Interrupt.Reason != "Cancel")
                    {
                        try
                        {
                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Removing : ""{0}""", sourceDirectory.FullName));
                            sourceDirectory.Delete();
                        }
                        catch (Exception ex)
                        {
                            Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Error while removing : ""{0}""", sourceDirectory.FullName));
                            Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                        }
                    }
                }
            }
            //Check if user has chosen to cancel run.
            if (Interrupt.Reason == "Cancel")
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Delete cancelled"));
                return;
            }
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