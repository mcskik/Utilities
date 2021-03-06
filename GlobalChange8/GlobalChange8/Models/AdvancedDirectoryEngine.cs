using ProfileData.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Directory listing object.
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class AdvancedDirectoryEngine
    {
        public event AdvancedEventDelegate EventBeginProgress;
        public event AdvancedEventDelegate EventUpdateProgress;
        public event AdvancedEventDelegate EventEndOfProgress;

        /// <summary>
        /// Trigger begin progress event.
        /// </summary>
        /// <param name="method">Current method to which this applies.</param>
        public void SignalBeginProgress(string method)
        {
            if (EventBeginProgress != null)
            {
                AdvancedEventParameters2 eventParameters = new AdvancedEventParameters2(method, 0);
                EventBeginProgress(this, eventParameters);
            }
        }

        /// <summary>
        /// Trigger update progress event.
        /// </summary>
        /// <param name="method">Current method to which this applies.</param>
        public void SignalUpdateProgress(string method, long increment)
        {
            if (EventUpdateProgress != null)
            {
                AdvancedEventParameters2 eventParameters = new AdvancedEventParameters2(method, increment);
                EventUpdateProgress(this, eventParameters);
            }
        }

        /// <summary>
        /// Trigger end of progress event.
        /// </summary>
        /// <param name="method">Current method to which this applies.</param>
        public void SignalEndOfProgress(string method)
        {
            if (EventEndOfProgress != null)
            {
                AdvancedEventParameters2 eventParameters = new AdvancedEventParameters2(method, 0);
                EventEndOfProgress(this, eventParameters);
            }
        }

        private Interrupt interrupt;
        private bool monitoredTypesOnly = false;
        protected Comparisons<Comparison> comparisons;
        protected AdvancedDirectoryEntries directoryListing;
        private string targetHlq;

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
        /// Comparisons collection.
        /// </summary>
        public Comparisons<Comparison> Comparisons
        {
            get
            {
                return comparisons;
            }
        }

        /// <summary>
        /// Target High Level Qualifier.
        /// </summary>
        public string TargetHlq
        {
            get
            {
                return targetHlq;
            }
            set
            {
                targetHlq = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AdvancedDirectoryEngine()
        {
        }

        /// <summary>
        /// Scan directory information into memory resident directory listing object.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        /// <returns>List of directory entries.</returns>
        public AdvancedDirectoryEntries DirList(string qualifier, ref long estimate)
        {
            List<string> dirs = new List<string>();
            string directory = string.Empty;
            long count;
            long entries;
            SignalBeginProgress("DirList");
            try
            {
                this.directoryListing = new AdvancedDirectoryEntries();
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
                    if (Interrupt.Reason == "Cancel")
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
                                    AdvancedDirectoryEntry entry = new AdvancedDirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = 0;
                                    entry.StdDate = System.DateTime.Parse("01/01/2000");
                                    entry.SourceDate = entry.StdDate;
                                    entry.TargetDate = entry.StdDate;
                                    entry.SourceFile = entry.StdFile;
                                    entry.TargetFile = entry.StdFile;
                                    entry.StdType = "dir";
                                    string matchAfter = string.Empty;
                                    entry.FolderGroup = GetFolderGroup(spec, ref matchAfter);
                                    string matchSpec = GetMatchPart(spec, matchAfter);
                                    entry.FolderGroupMatchPath = matchSpec;
                                    entry.FolderGroupMatchStem = matchSpec;
                                    entry.FolderGroupMatchFileSpec = matchSpec;
                                    entry.FolderGroupFullPath = spec;
                                    entry.FolderGroupFullStem = spec;
                                    entry.FolderGroupFullFileSpec = spec;
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList", entry.StdSize);
                                }
                            }
                        }
                        catch (OleDbException oExceptionA)
                        {
                            System.Diagnostics.Debug.WriteLine(oExceptionA.Errors[0].Message);
                        }
                        if (Interrupt.Reason == "Cancel")
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
                                if (MonitoredTypesHelper.AllowFile(spec, monitoredTypesOnly))
                                {
                                    AdvancedDirectoryEntry entry = new AdvancedDirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = fileInfoArray[row].Length;
                                    entry.StdDate = fileInfoArray[row].LastWriteTime;
                                    entry.SourceDate = entry.StdDate;
                                    entry.TargetDate = entry.StdDate;
                                    entry.SourceFile = entry.StdFile;
                                    entry.TargetFile = entry.StdFile;
                                    entry.StdType = ext;
                                    string matchAfter = string.Empty;
                                    entry.FolderGroup = GetFolderGroup(spec, ref matchAfter);
                                    entry.FolderGroupMatchPath = GetMatchPart(fileInfoArray[row].DirectoryName, matchAfter);
                                    string matchSpec = GetMatchPart(spec, matchAfter);
                                    string matchStem = RemoveExt(matchSpec, fullExt);
                                    entry.FolderGroupMatchStem = matchStem;
                                    entry.FolderGroupMatchFileSpec = matchSpec;
                                    entry.FolderGroupFullPath = fileInfoArray[row].DirectoryName;
                                    entry.FolderGroupFullStem = RemoveExt(spec, fullExt);
                                    entry.FolderGroupFullFileSpec = spec;
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList", entry.StdSize);
                                }
                            }
                        }
                        catch (OleDbException exceptionB)
                        {
                            System.Diagnostics.Debug.WriteLine(exceptionB.Errors[0].Message);
                        }
                        if (Interrupt.Reason == "Cancel")
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

        private string GetFolderGroup(string path, ref string matchAfter)
        {
            string folderGroup = string.Empty;
            matchAfter = string.Empty;
            Dictionary<string, string> folderGroups = new Dictionary<string, string>();
            folderGroups.Add("MAIN", @"\main\");
            folderGroups.Add("TEST", @"\test\");
            folderGroups.Add("AAA", @"\aaa\");
            folderGroups.Add("AAATEST", @"\aaaTest\");
            folderGroups.Add("TESTAAA", @"\testAaa\");
            folderGroups.Add("BBB", @"\bbb\");
            folderGroups.Add("BBBTEST", @"\bbbTest\");
            folderGroups.Add("TESTBBB", @"\testBbb\");
            foreach (KeyValuePair<string, string> group in folderGroups)
            {
                string groupName = group.Key;
                string groupPath = group.Value;
                if (path.Contains(groupPath))
                {
                    folderGroup = groupName;
                    matchAfter = groupPath;
                    break;
                }
            }
            return folderGroup;
        }

        private string GetMatchPart(string full, string matchAfter)
        {
            string matchPart = full;
            if (full.Length > 0 && matchAfter.Length > 0)
            {
                int pos = full.IndexOf(matchAfter);
                if (pos != -1)
                {
                    pos += matchAfter.Length;
                    if (pos < full.Length)
                    {
                        matchPart = full.Substring(pos);
                    }
                }
            }
            return matchPart;
        }

        private string RemoveExt(string fileSpec, string fullExt)
        {
            string stem = fileSpec;
            if (fileSpec.Length > 0 && fullExt.Length > 0)
            {
                stem = fileSpec.Replace(fullExt, string.Empty);
            }
            return stem;
        }

        /// <summary>
        /// Compare directory level information from the two specified directory listings.
        /// </summary>
        public virtual void Compare(ref AdvancedDirectoryEntries entriesBef, ref AdvancedDirectoryEntries entriesAft)
        {
            SignalBeginProgress("Changes");
            try
            {
                //Identify entries which are common to both before and after directory listings.
                //This is accomplished using a LINQ query fashioned to implement an inner join.
                List<AdvancedDirectoryEntry> innerEntries = (List<AdvancedDirectoryEntry>)(from b in entriesBef
                                                                                           join a in entriesAft
                                                                                           on new { b.StdDir, b.StdFile } equals new { a.StdDir, a.StdFile }
                                                                                           select new AdvancedDirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.StdSize, a.StdDate, a.StdDate, b.StdDate, a.StdHlq, a.StdFile, b.StdHlq, b.StdFile, a.StdType, "BOTH")).ToList();

                //Identify entries which have been deleted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (before side) of the join.
                List<AdvancedDirectoryEntry> deletedEntries = (List<AdvancedDirectoryEntry>)(from b in entriesBef
                                                                                             join a in entriesAft
                                                                                             on new { b.StdDir, b.StdFile } equals new { a.StdDir, a.StdFile }
                                                                                             into changes
                                                                                             from c in changes.DefaultIfEmpty(new AdvancedDirectoryEntry())
                                                                                             where c.CtlComparison == "EMPTY"
                                                                                             select new AdvancedDirectoryEntry(b.StdHlq, b.StdDir, b.StdFile, b.StdSize, b.StdDate, DateTime.MinValue, b.StdDate, this.targetHlq, b.StdFile, b.StdHlq, b.StdFile, b.StdType, "DELETED")).ToList();

                //Identify entries which have been inserted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (after side) of the join.
                List<AdvancedDirectoryEntry> insertedEntries = (List<AdvancedDirectoryEntry>)(from a in entriesAft
                                                                                              join b in entriesBef
                                                                                              on new { a.StdDir, a.StdFile } equals new { b.StdDir, b.StdFile }
                                                                                              into changes
                                                                                              from c in changes.DefaultIfEmpty(new AdvancedDirectoryEntry())
                                                                                              where c.CtlComparison == "EMPTY"
                                                                                              select new AdvancedDirectoryEntry(a.StdHlq, a.StdDir, a.StdFile, a.StdSize, a.StdDate, a.StdDate, DateTime.MinValue, a.StdHlq, a.StdFile, this.targetHlq, a.StdFile, a.StdType, "INSERTED")).ToList();
                //Reconstruct the before list.
                entriesBef = new AdvancedDirectoryEntries();
                entriesBef.AddRange(innerEntries);
                entriesBef.AddRange(deletedEntries);

                //Reconstruct the after list.
                entriesAft = new AdvancedDirectoryEntries();
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
        public void Changes(AdvancedDirectoryEntries entriesBef, AdvancedDirectoryEntries entriesAft, ref AdvancedDirectoryEntries entriesChg, ref long estimate)
        {
            estimate = 0;
            try
            {
                AdvancedDirectoryEntries deletedEntries = new AdvancedDirectoryEntries((from b in entriesBef
                                                                                        where b.CtlComparison == "DELETED"
                                                                                        select b).ToList());
                SignalUpdateProgress("Changes", 1);

                AdvancedDirectoryEntries innerEntries = new AdvancedDirectoryEntries((from b in entriesBef
                                                                                      where b.CtlComparison == "BOTH"
                                                                                      select b).ToList());
                SignalUpdateProgress("Changes", 1);

                AdvancedDirectoryEntries insertedEntries = new AdvancedDirectoryEntries((from a in entriesAft
                                                                                         where a.CtlComparison == "INSERTED"
                                                                                         select a).ToList());
                SignalUpdateProgress("Changes", 1);

                entriesChg = new AdvancedDirectoryEntries();
                entriesChg.AddRange(deletedEntries);
                entriesChg.AddRange(innerEntries);
                entriesChg.AddRange(insertedEntries);
                SignalUpdateProgress("Changes", 1);
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

        /// <summary>
        /// Generate a collection of comparisons for use by external classes.
        /// </summary>
        protected void GenerateComparisons(List<AdvancedDirectoryEntry> changedEntries, ref long estimate)
        {
            char separator = Path.DirectorySeparatorChar;
            List<Comparison> sameDirectories = new List<Comparison>();
            List<Comparison> deletes = new List<Comparison>();
            List<Comparison> removes = new List<Comparison>();
            List<Comparison> creates = new List<Comparison>();
            List<Comparison> copies = new List<Comparison>();
            SignalBeginProgress("Comparisons");
            estimate = 0;
            try
            {
                List<AdvancedDirectoryEntry> changes = (List<AdvancedDirectoryEntry>)(from c in changedEntries
                                                                                      orderby c.StdDir, c.StdFile, c.StdType
                                                                                      select c).ToList();
                foreach (AdvancedDirectoryEntry change in changes)
                {
                    Comparison comparison = new Comparison();
                    comparison.Action = string.Empty;
                    comparison.Command = string.Empty;
                    comparison.DisplayCommand = string.Empty;
                    comparison.Outcome = change.CtlComparison;
                    string fileType = change.StdType;
                    string sourceFileSpec = string.Empty;
                    string targetFileSpec = string.Empty;
                    if (fileType == "dir")
                    {
                        switch (comparison.Outcome)
                        {
                            case "DELETED":
                                comparison.SortOrder = "B";
                                comparison.Command = "Remove";
                                comparison.DisplayCommand = "Removing";
                                break;
                            case "INSERTED":
                                comparison.SortOrder = "C";
                                comparison.Command = "Create";
                                comparison.DisplayCommand = "Creating";
                                break;
                            case "BOTH":
                                comparison.SortOrder = "C";
                                comparison.Command = "Same";
                                comparison.DisplayCommand = "Nothing";
                                break;
                        }
                        sourceFileSpec = change.SourceHlq + change.StdDir + separator.ToString() + change.SourceFile;
                        targetFileSpec = change.TargetHlq + change.StdDir + separator.ToString() + change.TargetFile;
                    }
                    else
                    {
                        switch (comparison.Outcome)
                        {
                            case "DELETED":
                                comparison.SortOrder = "A";
                                comparison.Command = "Delete";
                                comparison.DisplayCommand = "Deleting";
                                break;
                            case "INSERTED":
                                comparison.SortOrder = "D";
                                comparison.Command = "Insert";
                                comparison.DisplayCommand = "Copying";
                                break;
                            case "BOTH":
                                comparison.SortOrder = "E";
                                comparison.Command = "Update";
                                comparison.DisplayCommand = "Copying";
                                break;
                        }
                        sourceFileSpec = change.SourceHlq + change.StdDir + separator.ToString() + change.SourceFile;
                        targetFileSpec = change.TargetHlq + change.StdDir + separator.ToString() + change.TargetFile;
                    }
                    comparison.SourceDate = change.SourceDate;
                    comparison.TargetDate = change.TargetDate;
                    comparison.SourceType = fileType;
                    comparison.SourceEntry = sourceFileSpec;
                    comparison.SourceSize = change.StdSize;
                    comparison.TargetType = fileType;
                    comparison.TargetEntry = targetFileSpec;
                    comparison.TargetSize = change.StdSize;
                    switch (comparison.Command)
                    {
                        case "Same":
                            sameDirectories.Add(comparison);
                            break;
                        case "Delete":
                            deletes.Add(comparison);
                            break;
                        case "Remove":
                            removes.Add(comparison);
                            break;
                        case "Create":
                            creates.Add(comparison);
                            break;
                        case "Insert":
                            copies.Add(comparison);
                            break;
                        case "Update":
                            copies.Add(comparison);
                            break;
                    }
                    estimate += change.StdSize;
                    SignalUpdateProgress("Comparisons", 1);
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
                List<Comparison> sortedSameDirectories = (List<Comparison>)(from c in sameDirectories
                                                                            orderby c.SortOrder, c.Command, c.SourceEntry, c.TargetEntry
                                                                            select c).ToList();
                List<Comparison> sortedDeletes = (List<Comparison>)(from c in deletes
                                                                    orderby c.SortOrder, c.Action, c.SourceEntry, c.TargetEntry
                                                                    select c).ToList();
                //Have to be sorted descending so that deepest directories are removed before higher up directories.
                List<Comparison> sortedRemoves = (List<Comparison>)(from c in removes
                                                                    orderby c.SortOrder, c.Action, c.SourceEntry descending, c.TargetEntry descending
                                                                    select c).ToList();
                List<Comparison> sortedCreates = (List<Comparison>)(from c in creates
                                                                    orderby c.SortOrder, c.Action, c.SourceEntry, c.TargetEntry
                                                                    select c).ToList();
                List<Comparison> sortedCopies = (List<Comparison>)(from c in copies
                                                                   orderby c.SortOrder, c.Action, c.SourceEntry, c.TargetEntry
                                                                   select c).ToList();
                //Transfer all groups of commands into one list.
                comparisons = new Comparisons<Comparison>();
                foreach (var comparison in sortedSameDirectories)
                {
                    comparisons.Add(comparison);
                }
                foreach (var comparison in sortedDeletes)
                {
                    comparisons.Add(comparison);
                }
                foreach (var comparison in sortedRemoves)
                {
                    comparisons.Add(comparison);
                }
                foreach (var comparison in sortedCreates)
                {
                    comparisons.Add(comparison);
                }
                foreach (var comparison in sortedCopies)
                {
                    comparisons.Add(comparison);
                }
            }
            SignalEndOfProgress("Comparisons");
        }
    }

    public delegate void AdvancedEventDelegate(object sender, AdvancedEventParameters2 eventArgs);

    /// <summary>
    /// Class that defines data for event handling.
    /// </summary>
    public class AdvancedEventParameters2 : EventArgs
    {
        protected string method;
        protected long increment;

        public string Method
        {
            get
            {
                return method;
            }
        }

        public long Increment
        {
            get
            {
                return increment;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="method">Method which is currently making use of the progress events.</param>
        public AdvancedEventParameters2(string method, long increment)
        {
            this.method = method;
            this.increment = increment;
        }
    }
}
