using Copy8.DataLayer.Profile;
using Copy8.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Copy8
{
    /// <summary>
    /// Directory entries for use in LINQ queries.
    /// </summary>
    public class DirectoryEntries : List<DirectoryEntry>
    {
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public enum InterpretationEnum
        {
            Source,
            Target
        }

        public int ExportedRowCount { get; set; }
        public int ImportedRowCount { get; set; }

        public DirectoryEntries()
        {
        }

        public DirectoryEntries(List<DirectoryEntry> entries)
        {
            this.AddRange(entries);
        }

        public void Load(string fileSpec, InterpretationEnum interpretation)
        {
            ImportedRowCount = 0;
            this.Clear();
            StreamReader sr = null;
            string line = string.Empty;
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteMsg("I", String.Format(@"Importing from file  ""{0}""", fileSpec));
            try
            {
                sr = new StreamReader(fileSpec);
                if (sr.Peek() > -1)
                {
                    line = sr.ReadLine();
                    do
                    {
                        line = sr.ReadLine();
                        ImportedRowCount++;
                        List<string> values = CsvSplitter.Split(line);
                        if (values.Count > 1)
                        {
                            DirectoryEntry entry = new DirectoryEntry();
                            entry.StdHlq = values[0];
                            entry.StdDir = values[1];
                            entry.StdFile = values[2];
                            int size = 0;
                            int.TryParse(values[3], out size);
                            entry.StdSize = size;
                            DateTime stdDateTime = DateTime.MinValue;
                            DateTime.TryParse(values[4], out stdDateTime);
                            entry.StdDate = stdDateTime;
                            DateTime sourceDateTime = DateTime.MinValue;
                            DateTime.TryParse(values[5], out sourceDateTime);
                            entry.SourceDate = sourceDateTime;
                            DateTime targetDateTime = DateTime.MinValue;
                            DateTime.TryParse(values[6], out targetDateTime);
                            entry.TargetDate = targetDateTime;
                            entry.SourceHlq = values[7];
                            entry.SourceFile = values[8];
                            entry.TargetHlq = values[9];
                            entry.TargetFile = values[10];
                            entry.StdType = values[11];
                            entry.CtlComparison = values[12];
                            // When using a snapshot taken from the source to represent a projected snapshot that should exist at the target after synchronization
                            // then we must edit the StdHlq to make it appear to have come from the target so that the comparison will not think everything has changed.
                            if (interpretation == InterpretationEnum.Target)
                            {
                                entry.StdHlq = entry.TargetHlq;
                            }
                            this.Add(entry);
                        }
                        else
                        {
                            Administrator.Tracer.WriteTimedMsg("W", String.Format(@"Logical error in method ""{0}"" : line number ""{1}"", number of data fields is insufficient, line ignored", MethodBase.GetCurrentMethod().Name, ImportedRowCount));
                        }
                    } while (sr.Peek() > -1);
                }
            }
            catch (Exception ex)
            {
                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Exception in method ""{0}"" : {1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            }
            finally
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format("Imported Rows: {0}", ImportedRowCount.ToString().PadLeft(9)));
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public void Save(string fileSpec)
        {
            StreamWriter sw = null;
            try
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", String.Format(@"Exporting to file ""{0}""", fileSpec));
                ExportedRowCount = 0;
                if (this.Count > 0)
                {
                    ProfileData.Models.Helpers.FileHelper.PathCheck(fileSpec);
                    ProfileData.Models.Helpers.FileHelper.Delete(fileSpec);
                    sw = File.CreateText(fileSpec);
                    string line = string.Empty;
                    line = "StdHlq,StdDir,StdFile,StdSize,StdDate,SourceDate,TargetDate,SourceHlq,SourceFile,TargetHlq,TargetFile,StdType,CtlComparison";
                    sw.WriteLine(line);
                    foreach (var entry in this)
                    {
                        ExportedRowCount++;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format(@"""{0}"",", entry.StdHlq));
                        sb.Append(String.Format(@"""{0}"",", entry.StdDir));
                        sb.Append(String.Format(@"""{0}"",", entry.StdFile));
                        sb.Append(String.Format(@"""{0}"",", entry.StdSize));
                        sb.Append(String.Format(@"""{0}"",", entry.StdDate.ToString(DATE_TIME_FORMAT)));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceDate.ToString(DATE_TIME_FORMAT)));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetDate.ToString(DATE_TIME_FORMAT)));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceHlq));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceFile));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetHlq));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetFile));
                        sb.Append(String.Format(@"""{0}"",", entry.StdType));
                        sb.Append(String.Format(@"""{0}"",", entry.CtlComparison));
                        line = sb.ToString();
                        sw.WriteLine(line.ToString());
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Exception in method ""{0}"" : {1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            }
            finally
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format("Exported Rows: {0}", ExportedRowCount.ToString().PadLeft(9)));
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }

    /// <summary>
    /// Directory entry for use in LINQ queries.
    /// </summary>
    public class DirectoryEntry
    {
        public const string COMPARISON_EMPTY = "EMPTY";

        public string StdHlq { get; set; }
        public string StdDir { get; set; }
        public string StdFile { get; set; }
        public long StdSize { get; set; }
        public DateTime StdDate { get; set; }
        public DateTime SourceDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string SourceHlq { get; set; }
        public string SourceFile { get; set; }
        public string TargetHlq { get; set; }
        public string TargetFile { get; set; }
        public string StdType { get; set; }
        public string CtlComparison { get; set; }

        public DirectoryEntry()
        {
            StdHlq = string.Empty;
            StdDir = string.Empty;
            StdFile = string.Empty;
            StdSize = 0;
            StdDate = DateTime.Now;
            SourceDate = DateTime.Now;
            TargetDate = DateTime.Now;
            SourceHlq = string.Empty;
            SourceFile = string.Empty;
            TargetHlq = string.Empty;
            TargetFile = string.Empty;
            StdType = string.Empty;
            CtlComparison = COMPARISON_EMPTY;
        }

        public DirectoryEntry(string stdHlq, string stdDir, string stdFile, long stdSize, DateTime stdDate, DateTime sourceDate, DateTime targetDate, string sourceHlq, string sourceFile, string targetHlq, string targetFile, string stdType, string ctlComparison)
        {
            StdHlq = stdHlq;
            StdDir = stdDir;
            StdFile = stdFile;
            StdSize = stdSize;
            StdDate = stdDate;
            SourceDate = sourceDate;
            TargetDate = targetDate;
            SourceHlq = sourceHlq;
            SourceFile = sourceFile;
            TargetHlq = targetHlq;
            TargetFile = targetFile;
            StdType = stdType;
            CtlComparison = ctlComparison;
        }
    }
}