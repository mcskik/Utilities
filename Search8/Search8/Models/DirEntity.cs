using System;
using System.Collections.Generic;

namespace Search8.Models
{
    /// <summary>
    /// Directory entities for use in LINQ queries.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class DirEntities : List<DirEntity>
    {
        public DirEntities()
        {
        }
    }

    /// <summary>
    /// Directory entity for use in LINQ queries.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class DirEntity
    {
        public string StdHlq { get; set; }
        public string StdDir { get; set; }
        public string StdFile { get; set; }
        public long StdSize { get; set; }
        public DateTime StdDate { get; set; }
        public string StdType { get; set; }
        public string CtlComparison { get; set; }

        public DirEntity()
        {
        }
    }
}