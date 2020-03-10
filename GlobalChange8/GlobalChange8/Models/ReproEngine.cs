using GlobalChange8.DataLayer.Profile;
using Log8;
using ProfileData.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Global change engine (Repro).
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and apply all the custom changes required for mode = "Clone".
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ReproEngine
    {
        const string SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        const char SPACE = ' ';
        const string DELIMITER_SPACE = @"SPACE";
        const string DELIMITER_SPACES = @"SPACES";
        const string DELIMITER_SPECIAL = @"SPECIAL";
        const string DELIMITER_ANY = @"ANY";
        const string DELIMITER_EMPTY_STRING = @"";

        protected string _action = string.Empty;
        protected string _path = string.Empty;
        protected string _mode = string.Empty;
        protected Logger _log = null;

        public string CloneAll { get; set; }
        public string EditAll { get; set; }
        public string UseTargetSample { get; set; }
        public string RuleBaseHlq { get; set; }
        public string SourceHlq { get; set; }
        public string TargetHlq { get; set; }
        public string TargetSampleHlq { get; set; }
        public string CurrentPath { get; set; }

        protected SortedDictionary<string, string> _directoryRules;
        protected SortedDictionary<string, string> _fileRules;
        protected SortedDictionary<string, string> _editRules;

        private int _directoryRuleCount = 0;
        private int _fileRuleCount = 0;
        private int _directoriesCopiedCount = 0;
        private int _filesCopiedCount = 0;
        private int _filesChangedCount = 0;
        private int _linesChangedCount = 0;

        AdvancedDirectoryEntries _TargetSampleDirectoryListing;
        private long _TargetSampleTotalBytes = 0;

        /// <summary>
        /// Action.
        /// </summary>
        public string Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        /// <summary>
        /// Path.
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        /// <summary>
        /// Mode.
        /// </summary>
        public string Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReproEngine()
        {
            Load();
        }

        /// <summary>
        /// Run the selected repro (Currently equal to the selected RunMode).
        /// </summary>
        public void LoadConfiguration(string runMode)
        {
            string fileSpec = String.Format(@"{0}{1}.xml", Administrator.ProfileManager.SystemProfile.CloneConfigurationPath, runMode);
            XDocument doc = XDocument.Load(fileSpec);
            foreach (XElement flag in doc.Descendants("Switches").Elements())
            {
                string flagName = flag.Name.LocalName;
                switch (flagName)
                {
                    case "CloneAll":
                        CloneAll = (string)flag;
                        break;
                    case "EditAll":
                        EditAll = (string)flag;
                        break;
                    case "UseTargetSample":
                        UseTargetSample = (string)flag;
                        break;
                    case "RuleBaseHlq":
                        //This is used if the source directory that you are copying from is different to the one used when the rules were originally created.
                        RuleBaseHlq = (string)flag;
                        break;
                    case "SourceHlq":
                        SourceHlq = (string)flag;
                        break;
                    case "TargetHlq":
                        TargetHlq = (string)flag;
                        break;
                    case "TargetSampleHlq":
                        TargetSampleHlq = (string)flag;
                        break;
                }
            }
            foreach (XElement step in doc.Descendants("Step"))
            {
                //Directory Rules.
                _directoryRuleCount = 0;
                _directoryRules = new SortedDictionary<string, string>();
                _directoryRules.Add(SourceHlq, TargetHlq);
                foreach (XElement directories in step.Descendants("Directories"))
                {
                    foreach (XElement rule in directories.Descendants("Copy"))
                    {
                        string active = (string)rule.Attribute("Active");
                        if (IsAffirmative(active))
                        {
                            string type = (string)rule.Attribute("Type");
                            string section = (string)rule.Attribute("Section");
                            string source = (string)rule.Attribute("Source");
                            //This is used if the source directory that you are copying from is different to the one used when the rules were originally created.
                            source = source.Replace(RuleBaseHlq, SourceHlq);
                            string target = (string)rule.Attribute("Target");
                            target = DeriveRuleTarget(source, target);
                            if (!_directoryRules.ContainsKey(source))
                            {
                                _directoryRuleCount++;
                                _directoryRules.Add(source, target);
                            }
                        }
                    }
                }
                //File Rules.
                _fileRules = new SortedDictionary<string, string>();
                _fileRuleCount = 0;
                foreach (XElement files in step.Descendants("Files"))
                {
                    foreach (XElement rule in files.Descendants("Copy"))
                    {
                        string active = (string)rule.Attribute("Active");
                        if (IsAffirmative(active))
                        {
                            string type = (string)rule.Attribute("Type");
                            string section = (string)rule.Attribute("Section");
                            string source = (string)rule.Attribute("Source");
                            //This is used if the source directory that you are copying from is different to the one used when the rules were originally created.
                            source = source.Replace(RuleBaseHlq, SourceHlq);
                            string target = (string)rule.Attribute("Target");
                            target = DeriveRuleTarget(source, target);
                            if (!_fileRules.ContainsKey(source))
                            {
                                _fileRuleCount++;
                                _fileRules.Add(source, target);
                            }
                        }
                    }
                }
            }
            if (IsAffirmative(UseTargetSample))
            {
                LoadTargetSampleData();
            }
        }

        /// <summary>
        /// Load target sample data by scanning an existing target directory to obtain information about existing target directories and files.
        /// </summary>
        public void LoadTargetSampleData()
        {
            AdvancedDirectoryEngine directoryEngine = new AdvancedDirectoryEngine();
            directoryEngine.Interrupt = new Interrupt("OK");
            _TargetSampleDirectoryListing = directoryEngine.DirList(TargetSampleHlq, ref _TargetSampleTotalBytes);
            long totalBytes = _TargetSampleTotalBytes;
        }

        private string ExtractSymbolicTarget(string targetEntry)
        {
            //TODO: Replace hard coding with something more generic.
            CurrentPath = targetEntry;
            string symbolicTarget = "DEFAULT";
            if (targetEntry.Contains(@"\main\"))
            {
                symbolicTarget = "MAIN";
            }
            else if (targetEntry.Contains(@"\test\"))
            {
                symbolicTarget = "TEST";
            }
            else if (targetEntry.Contains(@"\aaa\"))
            {
                symbolicTarget = "AAA";
            }
            else if (targetEntry.Contains(@"\aaaTest\"))
            {
                symbolicTarget = "AAATEST";
            }
            else if (targetEntry.Contains(@"\bbb\"))
            {
                symbolicTarget = "BBB";
            }
            else if (targetEntry.Contains(@"\bbbTest\"))
            {
                symbolicTarget = "BBBTEST";
            }
            return symbolicTarget.PadRight(7);
        }

        private string DeriveRuleTarget(string source, string target)
        {
            string ruleTarget = target;
            List<string> symbolicTargets = new List<string>();
            symbolicTargets.Add("main");
            symbolicTargets.Add("aaa");
            symbolicTargets.Add("aaaTest");
            symbolicTargets.Add("bbb");
            symbolicTargets.Add("bbbTest");
            if (symbolicTargets.Contains(target))
            {
                string symbolicSource = "main";
                string symbolicTarget = target;
                if (target == "main")
                {
                    symbolicSource = "aaa";
                }
                else if (target == "test")
                {
                    symbolicSource = "aaaTest";
                }
                else if (target == "aaa")
                {
                    symbolicSource = "main";
                }
                else if (target == "aaaTest")
                {
                    symbolicSource = "test";
                }
                else if (target == "bbb")
                {
                    symbolicSource = "main";
                }
                else if (target == "bbbTest")
                {
                    symbolicSource = "test";
                }
                ruleTarget = source.Replace(SourceHlq, TargetHlq);
                ruleTarget = ruleTarget.Replace(@"\" + symbolicSource + @"\", @"\" + symbolicTarget + @"\");
            }
            return ruleTarget;
        }

        private string DeriveFullTargetDirectory(string sourceDirectory)
        {
            CurrentPath = sourceDirectory;
            string targetDirectory = sourceDirectory.Replace(SourceHlq, TargetHlq);
            KeyValuePair<string, string> rulePair = new KeyValuePair<string, string>(SourceHlq, TargetHlq);
            rulePair = FindMostSpecificLongestDirectoryRuleMatch();
            string sourceRule = rulePair.Key;
            string targetRule = rulePair.Value;
            if (sourceDirectory.Length >= sourceRule.Length)
            {
                targetDirectory = sourceDirectory.Replace(sourceRule, targetRule);
            }
            return targetDirectory;
        }

        private string DeriveFullTargetFile(string sourceFileSpec)
        {
            CurrentPath = sourceFileSpec;
            string targetFileSpec = sourceFileSpec.Replace(SourceHlq, TargetHlq);
            KeyValuePair<string, string> rulePair = new KeyValuePair<string, string>(SourceHlq, TargetHlq);
            rulePair = FindMostSpecificLongestFileRuleMatch();
            string sourceRule = rulePair.Key;
            string targetRule = rulePair.Value;
            if (sourceFileSpec.Length >= sourceRule.Length)
            {
                targetFileSpec = sourceFileSpec.Replace(sourceRule, targetRule);
            }
            return targetFileSpec;
        }

        private KeyValuePair<string, string> FindMostSpecificLongestDirectoryRuleMatch()
        {
            KeyValuePair<string, string> rulePair = new KeyValuePair<string, string>(SourceHlq, TargetHlq);
            int longestMatchLength = 0;
            int matchLength = 0;
            foreach (KeyValuePair<string, string> rule in _directoryRules)
            {
                string sourceRule = rule.Key;
                string targetRule = rule.Value;
                if (CurrentPath.StartsWith(sourceRule))
                {
                    matchLength = sourceRule.Length;
                    if (matchLength > longestMatchLength)
                    {
                        longestMatchLength = matchLength;
                        rulePair = rule;
                    }
                }
            }
            return rulePair;
        }

        private KeyValuePair<string, string> FindMostSpecificLongestFileRuleMatch()
        {
            KeyValuePair<string, string> rulePair = new KeyValuePair<string, string>(SourceHlq, TargetHlq);
            int longestMatchLength = 0;
            int matchLength = 0;
            foreach (KeyValuePair<string, string> rule in _directoryRules)
            {
                string sourceRule = rule.Key;
                string targetRule = rule.Value;
                if (CurrentPath.StartsWith(sourceRule))
                {
                    matchLength = sourceRule.Length;
                    if (matchLength > longestMatchLength)
                    {
                        longestMatchLength = matchLength;
                        rulePair = rule;
                    }
                }
            }
            AdvancedDirectoryEntry fileMatch = null;
            AdvancedDirectoryEntry mainMatch = null;
            AdvancedDirectoryEntry aaaMatch = null;
            AdvancedDirectoryEntry bbbMatch = null;
            foreach (var entry in _TargetSampleDirectoryListing)
            {
                if (entry.StdType != "dir")
                {
                    if (CurrentPath.Contains(entry.FolderGroupMatchFileSpec))
                    {
                        if (entry.FolderGroup == "MAIN" || entry.FolderGroup == "TEST")
                        {
                            mainMatch = entry;
                            break;
                        }
                        else if (entry.FolderGroup == "AAA" || entry.FolderGroup == "AAATEST" || entry.FolderGroup == "TESTAAA")
                        {
                            aaaMatch = entry;
                            break;
                        }
                        else if (entry.FolderGroup == "BBB" || entry.FolderGroup == "BBBTEST" || entry.FolderGroup == "TESTBBB")
                        {
                            bbbMatch = entry;
                            break;
                        }
                    }
                }
            }
            if (mainMatch != null)
            {
                fileMatch = mainMatch;
            }
            else if (aaaMatch != null)
            {
                fileMatch = aaaMatch;
            }
            if (fileMatch != null)
            {
                string source = CurrentPath;
                string target = fileMatch.FolderGroupFullFileSpec.Replace(TargetSampleHlq, TargetHlq);
                rulePair = new KeyValuePair<string, string>(source, target);
            }
            else
            {
                foreach (KeyValuePair<string, string> rule in _fileRules)
                {
                    string sourceRule = rule.Key;
                    string targetRule = rule.Value;
                    if (CurrentPath.StartsWith(sourceRule))
                    {
                        matchLength = sourceRule.Length;
                        if (matchLength > longestMatchLength)
                        {
                            longestMatchLength = matchLength;
                            rulePair = rule;
                        }
                    }
                }
            }
            return rulePair;
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
        /// </summary>
        public void Run()
        {
            if (IsAffirmative(CloneAll))
            {
                CloneAllEntries();
            }
            if (IsAffirmative(EditAll))
            {
                EditAllEntries();
            }
            Save();
        }

        protected bool IsAffirmative(string booleanText)
        {
            bool affirmative = false;
            booleanText = booleanText.ToLower();
            string positive = ":yes:y:true:t:";
            if (positive.Contains(String.Format(@":{0}:", booleanText)))
            {
                affirmative = true;
            }
            return affirmative;
        }

        /// <summary>
        /// Clone all directory entries from a complete directory tree.
        /// </summary>
        private void CloneAllEntries()
        {
            _directoriesCopiedCount = 0;
            _filesCopiedCount = 0;
            _filesChangedCount = 0;
            _linesChangedCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Repro Engine (Clone All) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceHlq;
            XCopyAll(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Directories Copied = {0}", _directoriesCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files Copied       = {0}", _filesCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files Changed      = {0}", _filesChangedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Lines Changed      = {0}", _linesChangedCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Clone all directories and files from a complete directory tree.
        /// </summary>
        private void XCopyAll(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XCopyAll");
            XCopyAll(diSource);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XCopyAll");
        }

        private void XCopyAll(DirectoryInfo sourceDirectory)
        {
            string targetDir = DeriveFullTargetDirectory(sourceDirectory.FullName);
            DirectoryInfo targetDirectory = new DirectoryInfo(targetDir);
            if (!Directory.Exists(targetDirectory.FullName))
            {
                Directory.CreateDirectory(targetDirectory.FullName);
            }
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                _log.WriteLn();
                string symbolicTargetDirectory = ExtractSymbolicTarget(targetDir);
                _log.WriteTimedMsg("000", "I", String.Format(@"{0} : Copy Directory To : ""{1}""", symbolicTargetDirectory, targetDirectory.ToString()));
                _directoriesCopiedCount++;
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileSpec = fi.FullName;
                        string sourceFileName = fi.Name;
                        string targetFileSpec = DeriveFullTargetFile(sourceFileSpec);
                        try
                        {
                            try
                            {
                                if (File.Exists(targetFileSpec))
                                {
                                    FileInfo targetFileInfo = new FileInfo(targetFileSpec);
                                    if (targetFileInfo.Exists)
                                    {
                                        targetFileInfo.IsReadOnly = false;
                                    }
                                }
                                FileHelper.PathCheck(targetFileSpec);
                                fi.CopyTo(targetFileSpec, true);
                                if (File.Exists(targetFileSpec))
                                {
                                    FileInfo targetFileInfo = new FileInfo(targetFileSpec);
                                    if (targetFileInfo.Exists)
                                    {
                                        targetFileInfo.IsReadOnly = false;
                                    }
                                }
                                string symbolicTargetFile = ExtractSymbolicTarget(targetFileSpec);
                                _log.WriteTimedMsg("000", "I", String.Format(@"{0} : File Copied To : {1}", symbolicTargetFile, targetFileSpec));
                                _filesCopiedCount++;
                            }
                            catch (Exception ex)
                            {
                                _log.WriteLn();
                                _log.WriteTimedMsg("000", "E", String.Format(@"{0} - File Copy Error : {1}{2}", targetFileSpec, Environment.NewLine, ex.Message));
                                _log.WriteLn();
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteLn();
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Error : {1}{2}", sourceFileName, Environment.NewLine, ex.Message));
                            _log.WriteLn();
                        }
                    }
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (_action != "Cancel")
                    {
                        try
                        {
                            if (DirectoryExclusionsHelper.AllowDirectory(di.FullName))
                            {
                                XCopyAll(di);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteLn();
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Target Directory (Name too long) Error : {1}{2}", di.Name, Environment.NewLine, ex.Message));
                            _log.WriteLn();
                        }
                    }
                }
            }
            //Check if user has chosen to cancel run.
            if (_action == "Cancel")
            {
                _log.WriteTimedMsg("000", "I", String.Format(@"XCopyAll cancelled"));
                return;
            }
        }

        /// <summary>
        /// Global change/edit all files in complete target directory tree.
        /// </summary>
        private void EditAllEntries()
        {
            //TODO: This is not needed yet, but will do later.
            // _filesChangedCount++;
            // _linesChangedCount++;
        }

        /// <summary>
        /// Return a count of words in the input string.
        /// </summary>
        /// <param name="textString">Input string.</param>
        /// <param name="textDelimiter">Used to determine where words begin and end.</param>
        /// <returns>Count of words in the input string.</returns>
        public int WordCount(string textString, string textDelimiter)
        {
            string special = string.Empty;
            string delimiter = string.Empty;
            string text = string.Empty;
            string word = string.Empty;
            string letter = string.Empty;
            int pos = 0;
            int len = 0;
            int count = 0;
            special = SPECIAL;
            text = textString.Trim() + new string(SPACE, 2);
            if (text.CompareTo(new string(SPACE, 2)) <= 0)
            {
                return 0;
            }
            switch (textDelimiter.ToUpper())
            {
                case DELIMITER_SPACE:
                    delimiter = new string(SPACE, 1);
                    break;
                case DELIMITER_SPACES:
                    delimiter = new string(SPACE, 1);
                    break;
                case DELIMITER_SPECIAL:
                    delimiter = special;
                    break;
                case DELIMITER_ANY:
                    delimiter = special;
                    break;
                case DELIMITER_EMPTY_STRING:
                    delimiter = special;
                    break;
                default:
                    delimiter = textDelimiter;
                    break;
            }
            count = 0;
            pos = 0;
            len = text.Length;
            letter = text.Substring(pos, 1);
            do
            {
                word = string.Empty;
                while (pos < len - 1 && delimiter.IndexOf(letter) != -1)
                {
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                while (pos < len - 1 && delimiter.IndexOf(letter) == -1)
                {
                    word = word + letter;
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                if (word.CompareTo(string.Empty) > 0)
                {
                    count++;
                }
            } while (word != string.Empty);
            return count;
        }

        /// <summary>
        /// Return a count of the words in the input string and store all words found
        /// in consecutive locations of the output word array.
        /// </summary>
        /// <param name="textString">Input string.</param>
        /// <param name="wordArray">Output word array.</param>
        /// <param name="textDelimiter">Used to determine where words begin and end.</param>
        /// <returns>Count of words in the input string.</returns>
        public int WordArray(string textString, ref string[] wordArray, string textDelimiter)
        {
            string special = string.Empty;
            string delimiter = string.Empty;
            string text = string.Empty;
            string word = string.Empty;
            string letter = string.Empty;
            int pos = 0;
            int len = 0;
            int count = 0;
            special = SPECIAL;
            text = textString.Trim() + new string(SPACE, 2);
            if (text.CompareTo(new string(SPACE, 2)) <= 0)
            {
                return 0;
            }
            switch (textDelimiter.ToUpper())
            {
                case DELIMITER_SPACE:
                    delimiter = new string(SPACE, 1);
                    break;
                case DELIMITER_SPACES:
                    delimiter = new string(SPACE, 1);
                    break;
                case DELIMITER_SPECIAL:
                    delimiter = special;
                    break;
                case DELIMITER_ANY:
                    delimiter = special;
                    break;
                case DELIMITER_EMPTY_STRING:
                    delimiter = special;
                    break;
                default:
                    delimiter = textDelimiter;
                    break;
            }
            count = 0;
            pos = 0;
            len = text.Length;
            letter = text.Substring(pos, 1);
            do
            {
                word = string.Empty;
                while (pos < len - 1 && delimiter.IndexOf(letter) != -1)
                {
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                while (pos < len - 1 && delimiter.IndexOf(letter) == -1)
                {
                    word = word + letter;
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                if (word.CompareTo(string.Empty) > 0)
                {
                    count++;
                    wordArray[count - 1] = word;
                }
            } while (word != string.Empty);
            return count;
        }

        /// <summary>
        /// Load system and working profile variables.
        /// </summary>
        protected void Load()
        {
            _path = Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath;
            _mode = Administrator.ProfileManager.UserSettings.SelectedItem.Mode;
        }

        /// <summary>
        /// Save working profile variables.
        /// </summary>
        protected void Save()
        {
            Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath = _path;
            Administrator.ProfileManager.UserSettings.SelectedItem.Mode = _mode;
            Administrator.ProfileManager.UserSettings.Save();
        }
    }
}