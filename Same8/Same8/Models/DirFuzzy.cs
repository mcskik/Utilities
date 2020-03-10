using System;
using System.Collections.Generic;
using System.Linq;

namespace Same8.Models
{
    /// <summary>
    /// Directory listing with fuzzy matching class.
    /// </summary>
    /// <remarks>
    /// Uses fuzzy matching instead of exact matching.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class DirFuzzy : Dir
    {
        public Matcher Matcher { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DirFuzzy()
            : base()
        {
            Matcher = new Matcher();
        }

        /// <summary>
        /// Prepare data prior to fuzzy match.
        /// </summary>
        public void FuzzyMatch(List<DirectoryEntry> entriesBef, List<DirectoryEntry> entriesAft)
        {
            Matcher = new Matcher();
            List<string> oldRows = new List<string>();
            List<string> newRows = new List<string>();
            FuzzyLoadDir(entriesBef, ref oldRows);
            FuzzyLoadDir(entriesAft, ref newRows);
            Matcher.OldRows = oldRows;
            Matcher.NewRows = newRows;
            Matcher.FuzzyMatch();
            FuzzyUnloadDir(Matcher.OldMatches, ref entriesBef);
            FuzzyUnloadDir(Matcher.NewMatches, ref entriesAft);
        }

        /// <summary>
        /// Prepare data prior to fuzzy match.
        /// </summary>
        private void FuzzyLoadDir(List<DirectoryEntry> entries, ref List<string> rows)
        {
            string compositeKey = string.Empty;
            rows = new List<string>();
            foreach (DirectoryEntry entry in entries)
            {
                if (entry.StdType != "dir")
                {
                    entry.FuzzDir = entry.StdDir;
                    entry.FuzzFile = entry.StdFile;
                    compositeKey = String.Format("{0}", entry.FuzzFile);
                    rows.Add(compositeKey);
                }
            }
            rows.Sort();
        }

        /// <summary>
        /// Unload fuzzy match results prior to comparison.
        /// </summary>
        public void FuzzyUnloadDir(MatchingPairs matchingPairs, ref List<DirectoryEntry> entries)
        {
            foreach (DirectoryEntry entry in entries)
            {
                if (matchingPairs.ContainsKey(entry.StdFile))
                {
                    MatchingPair matchingPair = matchingPairs[entry.StdFile];
                    if (matchingPair.Matched)
                    {
                        entry.FuzzFile = matchingPair.NewKey;
                    }
                }
            }
        }

        /// <summary>
        /// Compare directory level information from the two specified directory listings.
        /// </summary>
        public override void Compare(ref List<DirectoryEntry> entriesBef, ref List<DirectoryEntry> entriesAft)
        {
            SignalBeginProgress("Compare");
            try
            {
                //Identify entries which are common to both before and after directory listings.
                //This is accomplished using a LINQ query fashioned to implement an inner join.
                List<DirectoryEntry> innerEntries = (List<DirectoryEntry>)(from b in entriesBef
                                                               join a in entriesAft
                                                               on new { b.StdDir, b.FuzzFile } equals new { a.StdDir, a.FuzzFile }
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
                                                                 on new { b.StdDir, b.FuzzFile } equals new { a.StdDir, a.FuzzFile }
                                                                 into changes
                                                                 from c in changes.DefaultIfEmpty(new DirectoryEntry())
                                                                 where c.CtlComparison == "EMPTY"
                                                                 select new DirectoryEntry(b.StdHlq, b.StdDir, b.StdFile, b.FuzzDir, b.FuzzFile, b.StdSize, b.StdDate, DateTime.MinValue, b.StdDate, string.Empty, b.StdFile, b.StdType, "DELETED")).ToList();

                //Identify entries which have been inserted.
                //This is accomplished using a LINQ query fashioned to implement a left join
                //then selecting entries which only exist on the left side (after side) of the join.
                List<DirectoryEntry> insertedEntries = (List<DirectoryEntry>)(from a in entriesAft
                                                                  join b in entriesBef
                                                                  on new { a.StdDir, a.FuzzFile } equals new { b.StdDir, b.FuzzFile }
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
    }
}