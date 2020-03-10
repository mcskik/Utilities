using ProfileData.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Directory listing object. (Using LINQ with no database access.)
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Dir
    {
        #region Events.
        public event EventDelegate EventBeginProgress;
        public event EventDelegate EventUpdateProgress;
        public event EventDelegate EventEndOfProgress;

        /// <summary>
        /// Trigger begin progress event.
        /// </summary>
        /// <param name="psMethod">Current method to which this applies.</param>
        public void SignalBeginProgress(string psMethod)
        {
            if (EventBeginProgress != null)
            {
                EventParameters2 oEventParameters = new EventParameters2(psMethod);
                EventBeginProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger update progress event.
        /// </summary>
        /// <param name="psMethod">Current method to which this applies.</param>
        public void SignalUpdateProgress(string psMethod)
        {
            if (EventUpdateProgress != null)
            {
                EventParameters2 oEventParameters = new EventParameters2(psMethod);
                EventUpdateProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger end of progress event.
        /// </summary>
        /// <param name="psMethod">Current method to which this applies.</param>
        public void SignalEndOfProgress(string psMethod)
        {
            if (EventEndOfProgress != null)
            {
                EventParameters2 oEventParameters = new EventParameters2(psMethod);
                EventEndOfProgress(this, oEventParameters);
            }
        }
        #endregion

        #region Member variables.
        private string action = string.Empty;
        private DirEntries directoryListing;
        #endregion

        #region Properties.
        /// <summary>
        /// Action.
        /// </summary>
        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// Monitored types are now read from an XML file which is much better than the previous hard coding which it replaces.
        /// </remarks>
        public Dir()
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Scan directory information into memory resident directory listing object.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        /// <returns>List of directory entries.</returns>
        public DirEntries DirList(string qualifier, ref long estimate)
        {
            List<string> dirs = new List<string>();
            string directory = string.Empty;
            long count;
            long entries;
            SignalBeginProgress("DirList");
            try
            {
                this.directoryListing = new DirEntries();
                //Build directory and file list.
                dirs = new List<string>();
                qualifier = qualifier.Trim();
                qualifier = qualifier.Strip(StringExtender.StripMode.Trailing, Path.DirectorySeparatorChar.ToString());
                dirs.Add(qualifier);
                estimate = 1;
                count = 1;
                entries = dirs.Count;
                while (count <= entries)
                {
                    directory = dirs[(int)count - 1].ToString();
                    DirLevel(qualifier, directory, ref dirs, ref estimate);
                    count++;
                    entries = dirs.Count;
                    if (action == "Cancel")
                    {
                        break;
                    }
                }
            }
            catch (OleDbException oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Errors[0].Message);
            }
            finally
            {
                SignalEndOfProgress("DirList");
            }
            return directoryListing;
        }

        /// <summary>
        /// Append directory level information into the specified DataTable.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="dirs">Work in progress collection of directories.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        public void DirLevel(string qualifier, string directory, ref List<string> dirs, ref long estimate)
        {
            string spec;
            string path;
            string name;
            path = directory;
            if (directory.Length > qualifier.Length)
            {
                directory = directory.Substring(qualifier.Length);
            }
            else
            {
                directory = string.Empty;
            }
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Refresh();
                if (DirectoryExclusionsHelper.AllowDirectory(path))
                {
                    DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                    for (int row = 0; row < directoryInfoArray.Length; row++)
                    {
                        try
                        {
                            name = directoryInfoArray[row].Name;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (DirectoryExclusionsHelper.AllowDirectory(spec))
                            {
                                if (directoryInfoArray[row].Exists)
                                {
                                    dirs.Add(spec);
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = 0;
                                    entry.StdDate = System.DateTime.Parse("01/01/2000");
                                    entry.StdType = "dir";
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList");
                                }
                            }
                        }
                        catch (OleDbException oExceptionA)
                        {
                            System.Diagnostics.Debug.WriteLine(oExceptionA.Errors[0].Message);
                        }
                        if (action == "Cancel")
                        {
                            break;
                        }
                    }
                    FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                    for (int row = 0; row < fileInfoArray.Length; row++)
                    {
                        try
                        {
                            name = fileInfoArray[row].Name;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (fileInfoArray[row].Exists)
                            {
                                string ext = Path.GetExtension(spec);
                                if (ext.StartsWith("."))
                                {
                                    if (ext.Length > 1)
                                    {
                                        ext = ext.Substring(1);
                                    }
                                    else
                                    {
                                        ext = string.Empty;
                                    }
                                }
                                if (MonitoredTypesHelper.AllowFile(spec, true))
                                {
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = fileInfoArray[row].Length;
                                    entry.StdDate = fileInfoArray[row].LastWriteTime;
                                    entry.StdType = ext;
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList");
                                }
                            }
                        }
                        catch (OleDbException exceptionB)
                        {
                            System.Diagnostics.Debug.WriteLine(exceptionB.Errors[0].Message);
                        }
                        if (action == "Cancel")
                        {
                            break;
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException oExceptionNF)
            {
                System.Diagnostics.Debug.WriteLine(oExceptionNF.Message);
            }
            finally
            {
            }
        }
        #endregion
    }

    #region Delegates.
    public delegate void EventDelegate(object poSender, EventParameters2 poEventArgs);
    #endregion

    #region Event data.
    /// <summary>
    /// Class that defines data for event handling.
    /// </summary>
    public class EventParameters2 : EventArgs
    {
        #region Member Variables.
        private string mEventData;
        #endregion

        #region Properties.
        public string EventData
        {
            get
            {
                return mEventData;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psEventData">Event data.</param>
        public EventParameters2(string psEventData)
        {
            mEventData = psEventData;
        }
        #endregion
    }
    #endregion
}