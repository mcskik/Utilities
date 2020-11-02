using Copy9.DataLayer.Profile;
using Copy9.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Copy9
{
    /// <summary>
    /// Comparisons helper class.
    /// </summary>
    /// <remarks>
    /// Used to save and load comparisons to and from a csv file.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ComparisonsHelper
    {
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public enum InterpretationEnum
        {
            Source,
            Target
        }

        public int ExportedRowCount { get; set; }
        public int ImportedRowCount { get; set; }
        public Comparisons<Comparison> Comparisons { get; set; }

        public Comparisons<Comparison> Load(string fileSpec, Actor actor)
        {
            ImportedRowCount = 0;
            Comparisons = new Comparisons<Comparison>();
            Comparisons.Clear();
            StreamReader sr = null;
            string line = string.Empty;
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteMsg("I", String.Format(@"Importing comparisons from file  ""{0}""", fileSpec));
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
                            Comparison entry = new Comparison();
                            entry.SortOrder = values[0];
                            entry.Action = values[1];
                            entry.Command = values[2];
                            entry.DisplayCommand = values[3];
                            entry.Outcome = values[4];
                            entry.SourceType = values[5];
                            entry.SourceEntry = values[6];
                            DateTime dateTime = DateTime.MinValue;
                            DateTime.TryParse(values[7], out dateTime);
                            entry.SourceDate = dateTime;
                            int size = 0;
                            int.TryParse(values[8], out size);
                            entry.SourceSize = size;
                            entry.TargetType = values[9];
                            entry.TargetEntry = values[10];
                            dateTime = DateTime.MinValue;
                            DateTime.TryParse(values[11], out dateTime);
                            entry.TargetDate = dateTime;
                            size = 0;
                            int.TryParse(values[12], out size);
                            entry.TargetSize = size;
                            // When using a comparisons snapshot taken from a previously saved action
                            // we must edit the sourceEntry and targetEntry to make them appear to have come from the action you are now performing.
                            //Edit the source entry prefix to match the source connector prefix.
                            string sourceEntry = entry.SourceEntry;
                            sourceEntry = actor.SourceConnector.HlqPrefix + sourceEntry.Substring(1);
                            entry.SourceEntry = sourceEntry;
                            //Edit the target entry prefix to match the target connector prefix.
                            string targetEntry = entry.TargetEntry;
                            targetEntry = actor.TargetConnector.HlqPrefix + targetEntry.Substring(1);
                            entry.TargetEntry = targetEntry;
                            Comparisons.Add(entry);
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
            return Comparisons;
        }

        public Comparisons<Comparison> Filter(Comparisons<Comparison> inputComparisons, bool allowDelete)
        {
            Comparisons<Comparison> outputComparisons = new Comparisons<Comparison>();
            //TODO: Read from somewhere.
            //UserSetting.CopyRuleEnum copyRule = UserSetting.CopyRuleEnum.ReplaceOnlyWithNewer;
            UserSetting.CopyRuleEnum copyRule = Administrator.ProfileManager.UserSettings.SelectedItem.CopyRule;
            long sameCount = 0;
            int deleteCount = 0;
            int removeCount = 0;
            int createCount = 0;
            int insertCount = 0;
            int updateCount = 0;
            int copyCount = 0;
            bool identical = true;
            try
            {
                char separator = Path.DirectorySeparatorChar;
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", "Filtering");
                if (inputComparisons.Count == 0)
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteMsg("I", "Nothing to filter");
                }
                for (int count = 0; count < inputComparisons.Count; count++)
                {
                    Comparison comparison = inputComparisons[count];
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
                            outputComparisons.Add(comparison);
                            break;
                        case "Delete":
                            if (allowDelete)
                            {
                                deleteCount++;
                                identical = false;
                                outputComparisons.Add(comparison);
                            }
                            break;
                        case "Remove":
                            if (allowDelete)
                            {
                                removeCount++;
                                identical = false;
                                outputComparisons.Add(comparison);
                            }
                            break;
                        case "Create":
                            createCount++;
                            identical = false;
                            outputComparisons.Add(comparison);
                            break;
                        case "Insert":
                            insertCount++;
                            copyCount++;
                            identical = false;
                            outputComparisons.Add(comparison);
                            break;
                        case "Update":
                            bool doCopy = false;
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
                            if (doCopy)
                            {
                                updateCount++;
                                copyCount++;
                                identical = false;
                                outputComparisons.Add(comparison);
                            }
                            else
                            {
                                sameCount++;
                            }
                            break;
                    }
                }
                if (identical)
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteMsg("I", "No Changes To Make");
                }
                else
                {
                    Administrator.Tracer.WriteLine();
                    Administrator.Tracer.WriteMsg("I", "Changes Will Be Made");
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
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
            return outputComparisons;
        }

        private int CompareDateTimeStamps(DateTime sourceDateTime, DateTime targetDateTime)
        {
            string sourceDateTimeText = sourceDateTime.ToString(DATE_TIME_FORMAT);
            string targetDateTimeText = targetDateTime.ToString(DATE_TIME_FORMAT);
            int comparison = sourceDateTimeText.CompareTo(targetDateTimeText);
            return comparison;
        }

        public void Squash()
        {
            Dictionary<string, Comparison> uniqueComparisons = new Dictionary<string, Comparison>();
            Comparisons<Comparison> reversedComparisons = new Comparisons<Comparison>();
            StringBuilder sb = new StringBuilder();
            StringBuilder keySb = new StringBuilder();
            int inputActionRowCount = 0;
            int outputActionRowCount = 0;
            int sameRowCount = 0;
            try
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", String.Format(@"Squashing comparisons"));
                inputActionRowCount = 0;
                outputActionRowCount = 0;
                sameRowCount = 0;
                if (Comparisons.Count > 0)
                {
                    string line = string.Empty;
                    string key = string.Empty;
                    for (int index = Comparisons.Count - 1; index >= 0; index--)
                    {
                        Comparison entry = Comparisons[index];
                        if (entry.SourceType == "log")
                        {
                            continue;
                        }
                        if (entry.Command == "Same")
                        {
                            sameRowCount++;
                        }
                        else
                        {
                            inputActionRowCount++;
                            sb = new StringBuilder();
                            sb.Append(String.Format(@"""{0}"",", entry.SortOrder));
                            sb.Append(String.Format(@"""{0}"",", entry.Action));
                            sb.Append(String.Format(@"""{0}"",", entry.Command));
                            sb.Append(String.Format(@"""{0}"",", entry.DisplayCommand));
                            sb.Append(String.Format(@"""{0}"",", entry.Outcome));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceType));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceEntry));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceDate.ToString(DATE_TIME_FORMAT)));
                            sb.Append(String.Format(@"{0},", entry.SourceSize));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetType));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetEntry));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetDate.ToString(DATE_TIME_FORMAT)));
                            sb.Append(String.Format(@"{0},", entry.TargetSize));
                            line = sb.ToString();
                            //Only the last action on a given key has to be carried forward.
                            //The entry DateTime or Size, or the Action don't matter, the net effect is the last action.
                            keySb = new StringBuilder();
                            string equivalentCommand = entry.Command;
                            if (equivalentCommand == "Insert")
                            {
                                equivalentCommand = "Copy";
                            }
                            else if (equivalentCommand == "Update")
                            {
                                equivalentCommand = "Copy";
                            }
                            keySb.Append(String.Format(@"""{0}"",", equivalentCommand));
                            keySb.Append(String.Format(@"""{0}"",", entry.SourceType));
                            keySb.Append(String.Format(@"""{0}"",", entry.SourceEntry));
                            keySb.Append(String.Format(@"""{0}"",", entry.TargetType));
                            keySb.Append(String.Format(@"""{0}"",", entry.TargetEntry));
                            key = keySb.ToString();
                            if (!uniqueComparisons.ContainsKey(key))
                            {
                                outputActionRowCount++;
                                uniqueComparisons.Add(key, entry);
                                reversedComparisons.Add(entry);
                            }
                        }
                    }
                    inputActionRowCount++;
                    Comparison e = new Comparison();
                    e.SortOrder = "Same";
                    e.Action = "Same";
                    e.Command = "Same";
                    e.DisplayCommand = "Same";
                    e.Outcome = "Same";
                    e.SourceType = "Same";
                    e.SourceEntry = "Same";
                    e.SourceDate = DateTime.MinValue;
                    e.SourceSize = sameRowCount;
                    e.TargetType = "Same";
                    e.TargetEntry = "Same";
                    e.TargetDate = DateTime.MinValue;
                    e.TargetSize = sameRowCount;
                    sb = new StringBuilder();
                    sb.Append(String.Format(@"""{0}"",", e.SortOrder));
                    sb.Append(String.Format(@"""{0}"",", e.Action));
                    sb.Append(String.Format(@"""{0}"",", e.Command));
                    sb.Append(String.Format(@"""{0}"",", e.DisplayCommand));
                    sb.Append(String.Format(@"""{0}"",", e.Outcome));
                    sb.Append(String.Format(@"""{0}"",", e.SourceType));
                    sb.Append(String.Format(@"""{0}"",", e.SourceEntry));
                    sb.Append(String.Format(@"""{0}"",", e.SourceDate.ToString(DATE_TIME_FORMAT)));
                    sb.Append(String.Format(@"{0},", e.SourceSize));
                    sb.Append(String.Format(@"""{0}"",", e.TargetType));
                    sb.Append(String.Format(@"""{0}"",", e.TargetEntry));
                    sb.Append(String.Format(@"""{0}"",", e.TargetDate.ToString(DATE_TIME_FORMAT)));
                    sb.Append(String.Format(@"{0},", e.TargetSize));
                    line = sb.ToString();
                    //Only the last action on a given key has to be carried forward.
                    //The entry DateTime or Size, or the Action don't matter, the net effect is the last action.
                    keySb = new StringBuilder();
                    keySb.Append(String.Format(@"""{0}"",", e.Command));
                    keySb.Append(String.Format(@"""{0}"",", e.SourceType));
                    keySb.Append(String.Format(@"""{0}"",", e.SourceEntry));
                    keySb.Append(String.Format(@"""{0}"",", e.TargetType));
                    keySb.Append(String.Format(@"""{0}"",", e.TargetEntry));
                    key = keySb.ToString();
                    if (!uniqueComparisons.ContainsKey(key))
                    {
                        outputActionRowCount++;
                        uniqueComparisons.Add(key, e);
                        reversedComparisons.Add(e);
                    }
                    Comparisons = Reverse(reversedComparisons);
                }
            }
            catch (Exception ex)
            {
                Administrator.Tracer.WriteTimedMsg("E", String.Format(@"Exception in method ""{0}"" : {1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            }
            finally
            {
                Administrator.Tracer.WriteTimedMsg("I", String.Format("Input  Rows: {0}", inputActionRowCount.ToString().PadLeft(9)));
                Administrator.Tracer.WriteTimedMsg("I", String.Format("Output Rows: {0}", outputActionRowCount.ToString().PadLeft(9)));
            }
        }

        public Comparisons<Comparison> Reverse(Comparisons<Comparison> inputComparisons)
        {
            Comparisons<Comparison> outputComparisons = new Comparisons<Comparison>();
            for (int index = inputComparisons.Count - 1; index >= 0; index--)
            {
                Comparison entry = inputComparisons[index];
                outputComparisons.Add(entry);
            }
            return outputComparisons;
        }

        public void Save(string fileSpec)
        {
            //Filter is now done during the append so that only the new comparisons are filtered and not any previous comparisons.
            //Comparisons = Filter(Comparisons, true);
            Squash();
            StringBuilder sb = new StringBuilder();
            StreamWriter sw = null;
            try
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", String.Format(@"Exporting to comparisons file ""{0}""", fileSpec));
                ExportedRowCount = 0;
                if (Comparisons.Count > 0)
                {
                    ProfileData.Models.Helpers.FileHelper.PathCheck(fileSpec);
                    ProfileData.Models.Helpers.FileHelper.Delete(fileSpec);
                    sw = File.CreateText(fileSpec);
                    string line = string.Empty;
                    line = "SortOrder,Action,Command,DisplayCommand,Outcome,SourceType,SourceEntry,SourceDate,SourceSize,TargetType,TargetEntry,TargetDate,TargetSize";
                    sw.WriteLine(line);
                    for (int index = 0; index < Comparisons.Count; index++)
                    {
                        Comparison entry = Comparisons[index];
                        ExportedRowCount++;
                        sb = new StringBuilder();
                        sb.Append(String.Format(@"""{0}"",", entry.SortOrder));
                        sb.Append(String.Format(@"""{0}"",", entry.Action));
                        sb.Append(String.Format(@"""{0}"",", entry.Command));
                        sb.Append(String.Format(@"""{0}"",", entry.DisplayCommand));
                        sb.Append(String.Format(@"""{0}"",", entry.Outcome));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceType));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceEntry));
                        sb.Append(String.Format(@"""{0}"",", entry.SourceDate.ToString(DATE_TIME_FORMAT)));
                        sb.Append(String.Format(@"{0},", entry.SourceSize));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetType));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetEntry));
                        sb.Append(String.Format(@"""{0}"",", entry.TargetDate.ToString(DATE_TIME_FORMAT)));
                        sb.Append(String.Format(@"{0},", entry.TargetSize));
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

        public void Save_Old_NotUsed(string fileSpec)
        {
            StringBuilder sb = new StringBuilder();
            StreamWriter sw = null;
            try
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteMsg("I", String.Format(@"Exporting to comparisons file ""{0}""", fileSpec));
                ExportedRowCount = 0;
                int sameCount = 0;
                if (Comparisons.Count > 0)
                {
                    ProfileData.Models.Helpers.FileHelper.PathCheck(fileSpec);
                    ProfileData.Models.Helpers.FileHelper.Delete(fileSpec);
                    sw = File.CreateText(fileSpec);
                    string line = string.Empty;
                    line = "SortOrder,Action,Command,DisplayCommand,Outcome,SourceType,SourceEntry,SourceDate,SourceSize,TargetType,TargetEntry,TargetDate,TargetSize";
                    sw.WriteLine(line);
                    for (int index = 0; index < Comparisons.Count; index++)
                    {
                        Comparison entry = Comparisons[index];
                        if (entry.Command == "Same")
                        {
                            sameCount++;
                        }
                        else
                        {
                            ExportedRowCount++;
                            sb = new StringBuilder();
                            sb.Append(String.Format(@"""{0}"",", entry.SortOrder));
                            sb.Append(String.Format(@"""{0}"",", entry.Action));
                            sb.Append(String.Format(@"""{0}"",", entry.Command));
                            sb.Append(String.Format(@"""{0}"",", entry.DisplayCommand));
                            sb.Append(String.Format(@"""{0}"",", entry.Outcome));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceType));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceEntry));
                            sb.Append(String.Format(@"""{0}"",", entry.SourceDate.ToString(DATE_TIME_FORMAT)));
                            sb.Append(String.Format(@"{0},", entry.SourceSize));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetType));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetEntry));
                            sb.Append(String.Format(@"""{0}"",", entry.TargetDate.ToString(DATE_TIME_FORMAT)));
                            sb.Append(String.Format(@"{0},", entry.TargetSize));
                            line = sb.ToString();
                            sw.WriteLine(line.ToString());
                        }
                    }
                    ExportedRowCount++;
                    sb = new StringBuilder();
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", DateTime.MinValue.ToString(DATE_TIME_FORMAT)));
                    sb.Append(String.Format(@"{0},", sameCount));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", "Same"));
                    sb.Append(String.Format(@"""{0}"",", DateTime.MinValue.ToString(DATE_TIME_FORMAT)));
                    sb.Append(String.Format(@"{0},", sameCount));
                    line = sb.ToString();
                    sw.WriteLine(line.ToString());
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
}