using ProfileData.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace Same8.Models
{
    /// <summary>
    /// Directory listing object.
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
        protected string action = string.Empty;
        private bool mbIgnoreFileExtension = false;
        protected Comparisons<Comparison> comparisons;
        protected DirEntries directoryListing;
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

        public bool IgnoreFileExtension
        {
            get
            {
                return mbIgnoreFileExtension;
            }
            set
            {
                mbIgnoreFileExtension = value;
            }
        }

        /// <summary>
        /// Comparisons collection.
        /// </summary>
        public Comparisons<Comparison> Comparisons
        {
            get
            {
                return comparisons;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
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
                                    entry.StdFileNameOnly = entry.StdFile;
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
                                string fullExt = Path.GetExtension(spec);
                                string ext = fullExt;
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
                                    entry.StdFileNameOnly = RemoveExt(entry.StdFile, fullExt);
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

        private string RemoveExt(string spec, string fullExt)
        {
            fullExt = fullExt.Trim();
            string stem = spec;
            if (fullExt.Length > 0)
            {
                stem = spec.Replace(fullExt, string.Empty);
            }
            return stem;
        }

        /// <summary>
        /// Compare directory level information from the two specified directory listings.
        /// </summary>
        public virtual void Compare(ref List<DirectoryEntry> entriesBef, ref List<DirectoryEntry> entriesAft)
        {
            if (IgnoreFileExtension)
            {
                CompareIgnoreFileExtensions(ref entriesBef, ref entriesAft);
            }
            else
            {
                CompareWithFileExtensions(ref entriesBef, ref entriesAft);
            }
        }

        /// <summary>
        /// Compare directory level information from the two specified directory listings.
        /// </summary>
        public virtual void CompareWithFileExtensions(ref List<DirectoryEntry> entriesBef, ref List<DirectoryEntry> entriesAft)
        {
            SignalBeginProgress("Compare");
            try
            {
                //Identify entries which are common to both before and after directory listings.
                //This is accomplished using a LINQ query fashioned to implement an inner join.
                List<DirectoryEntry> innerEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                           join a in entriesAft
                                                                           on new { b.StdDir, b.StdFile } equals new { a.StdDir, a.StdFile }
                                                                           select new DirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.FuzzDir, a.FuzzFile, a.StdSize, a.StdDate, a.StdDate, b.StdDate, a.StdFile, b.StdFile, a.StdType, "BOTH")).ToList();

                #region Sample Code.
                //Update using a lambda function which is executed for every item found by the where clause.
                //Func<DirEntry, string, bool> update = (x, y) => { x.CtlComparison = y; return true; };
                //List<DirEntry> updatedEntries = (List<DirEntry>)(from de in innerEntries
                //where update(de, "BOTH")
                //select de).ToList();

                //Update using lambda only.
                //innerEntries.ForEach(e => e.CtlComparison = "BOTH");
                #endregion

                //Identify entries which have been deleted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (before side) of the join.
                List<DirectoryEntry> deletedEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                             join a in entriesAft
                                                                             on new { b.StdDir, b.StdFile } equals new { a.StdDir, a.StdFile }
                                                                             into changes
                                                                             from c in changes.DefaultIfEmpty(new DirectoryEntry())
                                                                             where c.CtlComparison == "EMPTY"
                                                                             select new DirectoryEntry(b.StdHlq, b.StdDir, b.StdFile, b.FuzzDir, b.FuzzFile, b.StdSize, b.StdDate, DateTime.MinValue, b.StdDate, string.Empty, b.StdFile, b.StdType, "DELETED")).ToList();

                //Identify entries which have been inserted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (after side) of the join.
                List<DirectoryEntry> insertedEntries = (List<DirectoryEntry>)(from a in entriesAft
                                                                              join b in entriesBef
                                                                              on new { a.StdDir, a.StdFile } equals new { b.StdDir, b.StdFile }
                                                                              into changes
                                                                              from c in changes.DefaultIfEmpty(new DirectoryEntry())
                                                                              where c.CtlComparison == "EMPTY"
                                                                              select new DirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.FuzzDir, a.FuzzFile, a.StdSize, a.StdDate, a.StdDate, DateTime.MinValue, a.StdFile, string.Empty, a.StdType, "INSERTED")).ToList();

                //Reconstruct the before list.
                entriesBef = new List<DirectoryEntry>();
                entriesBef.AddRange(innerEntries);
                entriesBef.AddRange(deletedEntries);

                //Reconstruct the after list.
                entriesAft = new List<DirectoryEntry>();
                entriesAft.AddRange(innerEntries);
                entriesAft.AddRange(insertedEntries);
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
        /// Compare directory level information from the two specified directory listings.
        /// </summary>
        public virtual void CompareIgnoreFileExtensions(ref List<DirectoryEntry> entriesBef, ref List<DirectoryEntry> entriesAft)
        {
            SignalBeginProgress("Compare");
            try
            {
                //Identify entries which are common to both before and after directory listings.
                //This is accomplished using a LINQ query fashioned to implement an inner join.
                List<DirectoryEntry> innerEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                           join a in entriesAft
                                                                           on new { b.StdDir, b.StdFileNameOnly } equals new { a.StdDir, a.StdFileNameOnly }
                                                                           select new DirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.FuzzDir, a.FuzzFile, a.StdSize, a.StdDate, a.StdDate, b.StdDate, a.StdFile, b.StdFile, a.StdType, "BOTH")).ToList();

                #region Sample Code.
                //Update using a lambda function which is executed for every item found by the where clause.
                //Func<DirEntry, string, bool> update = (x, y) => { x.CtlComparison = y; return true; };
                //List<DirEntry> updatedEntries = (List<DirEntry>)(from de in innerEntries
                //where update(de, "BOTH")
                //select de).ToList();

                //Update using lambda only.
                //innerEntries.ForEach(e => e.CtlComparison = "BOTH");
                #endregion

                //Identify entries which have been deleted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (before side) of the join.
                List<DirectoryEntry> deletedEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                             join a in entriesAft
                                                                             on new { b.StdDir, b.StdFileNameOnly } equals new { a.StdDir, a.StdFileNameOnly }
                                                                             into changes
                                                                             from c in changes.DefaultIfEmpty(new DirectoryEntry())
                                                                             where c.CtlComparison == "EMPTY"
                                                                             select new DirectoryEntry(b.StdHlq, b.StdDir, b.StdFile, b.FuzzDir, b.FuzzFile, b.StdSize, b.StdDate, DateTime.MinValue, b.StdDate, string.Empty, b.StdFile, b.StdType, "DELETED")).ToList();

                //Identify entries which have been inserted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (after side) of the join.
                List<DirectoryEntry> insertedEntries = (List<DirectoryEntry>)(from a in entriesAft
                                                                              join b in entriesBef
                                                                              on new { a.StdDir, a.StdFileNameOnly } equals new { b.StdDir, b.StdFileNameOnly }
                                                                              into changes
                                                                              from c in changes.DefaultIfEmpty(new DirectoryEntry())
                                                                              where c.CtlComparison == "EMPTY"
                                                                              select new DirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.FuzzDir, a.FuzzFile, a.StdSize, a.StdDate, a.StdDate, DateTime.MinValue, a.StdFile, string.Empty, a.StdType, "INSERTED")).ToList();

                //Reconstruct the before list.
                entriesBef = new List<DirectoryEntry>();
                entriesBef.AddRange(innerEntries);
                entriesBef.AddRange(deletedEntries);

                //Reconstruct the after list.
                entriesAft = new List<DirectoryEntry>();
                entriesAft.AddRange(innerEntries);
                entriesAft.AddRange(insertedEntries);
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
        /// Store details of changes to the specified change list.
        /// </summary>
        public void Changes(List<DirectoryEntry> entriesBef, List<DirectoryEntry> entriesAft, ref List<DirectoryEntry> entriesChg, ref long estimate)
        {
            estimate = 0;
            try
            {
                List<DirectoryEntry> deletedEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                             where b.CtlComparison == "DELETED"
                                                                             select b).ToList();
                SignalUpdateProgress("Changes");

                List<DirectoryEntry> innerEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                                           where b.CtlComparison == "BOTH"
                                                                           select b).ToList();
                SignalUpdateProgress("Changes");

                List<DirectoryEntry> insertedEntries = (List<DirectoryEntry>)(from a in entriesAft
                                                                              where a.CtlComparison == "INSERTED"
                                                                              select a).ToList();
                SignalUpdateProgress("Changes");

                entriesChg = new List<DirectoryEntry>();
                entriesChg.AddRange(deletedEntries);
                entriesChg.AddRange(innerEntries);
                entriesChg.AddRange(insertedEntries);
                SignalUpdateProgress("Changes");
            }
            catch (OleDbException oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Errors[0].Message);
            }
            finally
            {
            }
            SignalEndOfProgress("Changes");

            //Generate comparisons collection.
            GenerateComparisons(entriesChg, ref estimate);
        }
        #endregion

        #region Protected methods.
        /// <summary>
        /// Generate a collection of comparisons for use by external classes.
        /// </summary>
        protected void GenerateComparisons(List<DirectoryEntry> changedEntries, ref long estimate)
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            comparisons = new Comparisons<Comparison>();
            SignalBeginProgress("Comparisons");
            estimate = 0;
            try
            {
                List<DirectoryEntry> changes = (List<DirectoryEntry>)(from c in changedEntries
                                                                      where c.StdType != "dir"
                                                                      orderby c.StdDir, c.StdFile, c.StdType
                                                                      select c).ToList();
                foreach (DirectoryEntry change in changes)
                {
                    Comparison oComparison = new Comparison();
                    oComparison.Action = string.Empty;
                    oComparison.Outcome = change.CtlComparison;
                    string sFileType = change.StdType;
                    string newFileSpec = string.Empty;
                    string oldFileSpec = string.Empty;
                    if (sFileType == "dir")
                    {
                        newFileSpec = change.StdDir + change.NewFile;
                        oldFileSpec = change.StdDir + change.OldFile;
                    }
                    else
                    {
                        newFileSpec = change.StdDir + cSeparator.ToString() + change.NewFile;
                        oldFileSpec = change.StdDir + cSeparator.ToString() + change.OldFile;
                    }
                    oComparison.NewDate = change.NewDate;
                    oComparison.OldDate = change.OldDate;
                    switch (oComparison.Outcome)
                    {
                        case "DELETED":
                            oComparison.NewType = string.Empty;
                            oComparison.NewEntry = string.Empty;
                            oComparison.OldType = sFileType;
                            oComparison.OldEntry = oldFileSpec;
                            break;
                        case "INSERTED":
                            oComparison.NewType = sFileType;
                            oComparison.NewEntry = newFileSpec;
                            oComparison.OldType = string.Empty;
                            oComparison.OldEntry = string.Empty;
                            break;
                        case "BOTH":
                            oComparison.NewType = sFileType;
                            oComparison.NewEntry = newFileSpec;
                            oComparison.OldType = sFileType;
                            oComparison.OldEntry = oldFileSpec;
                            break;
                    }
                    comparisons.Add(oComparison);
                    estimate += 1;
                    SignalUpdateProgress("Comparisons");
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
            SignalEndOfProgress("Comparisons");
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
        protected string mEventData;
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