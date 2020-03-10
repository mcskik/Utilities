using System;
using System.Collections.Generic;

namespace GlobalChange8.Models
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
        public string StdHlq { get; set; }
        public string StdDir { get; set; }
        public string StdFile { get; set; }
        public long StdSize { get; set; }
        public DateTime StdDate { get; set; }
        public string StdType { get; set; }
        public string CtlComparison { get; set; }

        public DirectoryEntry()
        {
        }
    }
}