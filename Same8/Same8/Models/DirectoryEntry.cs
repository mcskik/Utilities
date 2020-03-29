using System;
using System.Collections.Generic;

namespace Same8.Models
{
    /// <summary>
    /// Directory entries for use in LINQ queries.
    /// </summary>
    public class DirEntries : List<DirectoryEntry>
    {
        public DirEntries()
        {
        }
    }

    /// <summary>
    /// Directory entry for use in LINQ queries.
    /// </summary>
    public class DirectoryEntry
    {
        #region Properties.
        public string StdHlq { get; set; }
        public string StdDir { get; set; }
        public string StdFile { get; set; }
        public string StdFileNameOnly { get; set; }
        public string FuzzDir { get; set; }
        public string FuzzFile { get; set; }
        public long StdSize { get; set; }
        public DateTime StdDate { get; set; }
        public DateTime NewDate { get; set; }
        public DateTime OldDate { get; set; }
        public string NewFile { get; set; }
        public string OldFile { get; set; }
        public string StdType { get; set; }
        public string CtlComparison { get; set; }
        #endregion

        #region Constructors.
        public DirectoryEntry()
        {
            StdHlq = string.Empty;
            StdDir = string.Empty;
            StdFile = string.Empty;
            StdFileNameOnly = string.Empty;
            FuzzDir = string.Empty;
            FuzzFile = string.Empty;
            StdSize = 0;
            StdDate = DateTime.Now;
            NewDate = DateTime.Now;
            OldDate = DateTime.Now;
            StdType = string.Empty;
            CtlComparison = "EMPTY";
        }

        public DirectoryEntry(string stdHlq, string stdDir, string stdFile, string fuzzDir, string fuzzFile, long stdSize, DateTime stdDate, DateTime newDate, DateTime oldDate, string newFile, string oldFile, string stdType, string ctlComparison)
        {
            StdHlq = stdHlq;
            StdDir = stdDir;
            StdFile = stdFile;
            StdFileNameOnly = RemoveExt(stdFile, stdType);
            FuzzDir = fuzzDir;
            FuzzFile = fuzzFile;
            StdSize = stdSize;
            StdDate = stdDate;
            NewDate = newDate;
            OldDate = oldDate;
            NewFile = newFile;
            OldFile = oldFile;
            StdType = stdType;
            CtlComparison = ctlComparison;
        }
        #endregion

        #region Private Methods.
        private string RemoveExt(string spec, string ext)
        {
            ext = ext.Trim();
            string fullExt = ext;
            if (ext.Length > 0)
            {
                fullExt = "." + ext;
            }
            string stem = spec;
            if (fullExt.Length > 0)
            {
                stem = spec.Replace(fullExt, string.Empty);
            }
            return stem;
        }
        #endregion
    }
}