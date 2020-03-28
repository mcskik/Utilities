using GlobalChange8.DataLayer.Profile;
using Log8;
using ProfileData.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

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

        public string RunMode { get; set; }
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

        private int _directoryRulesCount = 0;
        private int _directoryRulesSkippedCount = 0;
        private int _fileRulesCount = 0;
        private int _fileRulesSkippedCount = 0;
        private int _directoriesCopiedCount = 0;
        private int _filesCopiedCount = 0;
        private int _filesChangedCount = 0;
        private int _linesChangedCount = 0;

        protected AdvancedDirectoryEntries _targetSampleDirectoryListing;
        protected long _targetSampleTotalBytes = 0;

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
            _directoryRulesCount = 0;
            _directoryRulesSkippedCount = 0;
            _fileRulesCount = 0;
            _fileRulesSkippedCount = 0;
            _directoriesCopiedCount = 0;
            _filesCopiedCount = 0;
            _linesChangedCount = 0;
            Load();
        }

        /// <summary>
        /// Run the selected repro (Currently equal to the selected RunMode).
        /// </summary>
        public void LoadConfiguration(string runMode)
        {
            RunMode = runMode;
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
                foreach (XElement clone in step.Descendants("Clone"))
                {
                    //Directory Rules.
                    _directoryRulesCount = 0;
                    _directoryRules = new SortedDictionary<string, string>();
                    _directoryRules.Add(SourceHlq, TargetHlq);
                    foreach (XElement directories in clone.Descendants("Directories"))
                    {
                        foreach (XElement copy in directories.Descendants("Copy"))
                        {
                            string active = (string)copy.Attribute("Active");
                            if (IsAffirmative(active))
                            {
                                string type = (string)copy.Attribute("Type");
                                string section = (string)copy.Attribute("Section");
                                string source = (string)copy.Attribute("Source");
                                //This is used if the source directory that you are copying from is different to the one used when the rules were originally created.
                                source = source.Replace(RuleBaseHlq, SourceHlq);
                                string target = (string)copy.Attribute("Target");
                                target = DeriveRuleTarget(source, target);
                                if (!_directoryRules.ContainsKey(source))
                                {
                                    _directoryRulesCount++;
                                    _directoryRules.Add(source, target);
                                }
                            }
                            else
                            {
                                _directoryRulesSkippedCount++;
                            }
                        }
                    }
                    //File Rules.
                    _fileRules = new SortedDictionary<string, string>();
                    _fileRulesCount = 0;
                    foreach (XElement files in clone.Descendants("Files"))
                    {
                        foreach (XElement copy in files.Descendants("Copy"))
                        {
                            string active = (string)copy.Attribute("Active");
                            if (IsAffirmative(active))
                            {
                                string type = (string)copy.Attribute("Type");
                                string section = (string)copy.Attribute("Section");
                                string source = (string)copy.Attribute("Source");
                                //This is used if the source directory that you are copying from is different to the one used when the rules were originally created.
                                source = source.Replace(RuleBaseHlq, SourceHlq);
                                string target = (string)copy.Attribute("Target");
                                target = DeriveRuleTarget(source, target);
                                if (!_fileRules.ContainsKey(source))
                                {
                                    _fileRulesCount++;
                                    _fileRules.Add(source, target);
                                }
                            }
                            else
                            {
                                _fileRulesSkippedCount++;
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
        /// Load target sample data which can be used to ensure that existing target directories and files take precedence for updates to existing directories and files.
        /// Directory and file rules will be used for any directories or files which are not decided by the presence of existing directories and files.
        /// This essantially means that any new directories or files use the directory and file rules.
        /// </summary>
        protected void LoadTargetSampleData()
        {
            AdvancedDirectoryEngine directoryEngine = new AdvancedDirectoryEngine();
            directoryEngine.Interrupt = new Interrupt("OK");
            _targetSampleTotalBytes = 0;
            _targetSampleDirectoryListing = directoryEngine.DirList(TargetSampleHlq, ref _targetSampleTotalBytes);
            long totalBytes = _targetSampleTotalBytes;
        }

        private string ExtractSymbolicTarget(string targetPath)
        {
            //TODO: Need to remove hard coding at some point.
            CurrentPath = targetPath;
            string symbolicTarget = "default";
            if (targetPath.Contains(@"\main\"))
            {
                symbolicTarget = "main";
            }
            else if (targetPath.Contains(@"\test\"))
            {
                symbolicTarget = "test";
            }
            else if (targetPath.Contains(@"\aaa\"))
            {
                symbolicTarget = "aaa";
            }
            else if (targetPath.Contains(@"\aaaTest\"))
            {
                symbolicTarget = "aaaTest";
            }
            else if (targetPath.Contains(@"\bbb\"))
            {
                symbolicTarget = "bbb";
            }
            else if (targetPath.Contains(@"\bbbTest\"))
            {
                symbolicTarget = "bbbTest";
            }
            return symbolicTarget.ToUpper().PadRight(7);
        }

        private string DeriveRuleTarget(string source, string target)
        {
            //TODO: Eventually remove hard coded values with something more generic.
            string ruleTarget = target;
            var symbolicTargets = new List<string>();
            symbolicTargets.Add("main");
            symbolicTargets.Add("test");
            symbolicTargets.Add("aaa");
            symbolicTargets.Add("aaaTest");
            symbolicTargets.Add("bbb");
            symbolicTargets.Add("bbbTest");
            if (symbolicTargets.Contains(target))
            {
                string symbolicSource = "main";
                string symbolicTarget = target;
                ruleTarget = source.Replace(SourceHlq, TargetHlq);
                if (target == "main")
                {
                    //Going back to old structure.
                    symbolicSource = "aaa";
                }
                else if (target == "test")
                {
                    //Going back to old structure.
                    symbolicSource = "aaaTest";
                }
                else if (target == "aaa")
                {
                    //Going to new structure.
                    symbolicSource = "main";
                }
                else if (target == "aaaTest")
                {
                    //Going to new structure.
                    symbolicSource = "test";
                }
                else if (target == "bbb")
                {
                    //Going from dummy section in old structure.
                    symbolicSource = "main";
                }
                else if (target == "bbbTest")
                {
                    //Going from dummy section in old structure.
                    symbolicSource = "test";
                }
                ruleTarget = ruleTarget.Replace(@"\" + symbolicSource + @"\", @"\" + symbolicTarget + @"\");
            }
            return ruleTarget;
        }

        private string DeriveFullTargetDirectory(string sourceDirectory)
        {
            //Only search directory rules.
            CurrentPath = sourceDirectory;
            string targetDirectory = sourceDirectory.Replace(SourceHlq, TargetHlq);
            KeyValuePair<string, string> rulePair = FindMostSpecificLongestDirectoryRuleMatch();
            string sourceRule = rulePair.Key;
            string targetRule = rulePair.Value;
            if (targetDirectory.Length >= sourceRule.Length)
            {
                targetDirectory = targetDirectory.Replace(sourceRule, targetRule);
            }
            return targetDirectory;
        }

        private string DeriveFullTargetFile(string sourceFileSpec)
        {
            //Deliberately search both directory and file rules since all files will not necessarily have a file rule whereas all files will have a directory rule even if it is the default one.
            CurrentPath = sourceFileSpec;
            string targetFileSpec = sourceFileSpec.Replace(SourceHlq, TargetHlq);
            KeyValuePair<string, string> rulePair = FindMostSpecificLongestRuleMatch();
            string sourceRule = rulePair.Key;
            string targetRule = rulePair.Value;
            if (targetFileSpec.Length >= sourceRule.Length)
            {
                targetFileSpec = targetFileSpec.Replace(sourceRule, targetRule);
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

        private KeyValuePair<string, string> FindMostSpecificLongestRuleMatch()
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
            foreach (var entry in _targetSampleDirectoryListing)
            {
                var existingFileMatches = (from existingFile in _targetSampleDirectoryListing
                                           where CurrentPath.Contains(existingFile.FolderGroupMatchFileSpec) && existingFile.StdType != "dir"
                                           select existingFile).ToList();
                if (existingFileMatches.Count() > 0)
                {
                    foreach (var existingFile in existingFileMatches)
                    {
                        if (existingFile.FolderGroup == "MAIN" || existingFile.FolderGroup == "TEST")
                        {
                            mainMatch = existingFile;
                        }
                        else if (existingFile.FolderGroup == "BBB" || existingFile.FolderGroup == "BBBTEST")
                        {
                            //Ignore BBB matches.
                        }
                        else if (existingFile.FolderGroup == "AAA" || existingFile.FolderGroup == "AAATEST")
                        {
                            aaaMatch = existingFile;
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
            _log.Title = "Global Change/Repro Engine (Clone All Entries) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceHlq;
            XCopyAll(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Directory Rules         = {0}", _directoryRulesCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"File Rules              = {0}", _fileRulesCount));
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Directory Rules Skipped = {0}", _directoryRulesSkippedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"File Rules Skipped      = {0}", _fileRulesSkippedCount));
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Directories Copied      = {0}", _directoriesCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files Copied            = {0}", _filesCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Lines Changed           = {0}", _linesChangedCount));
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
            CurrentPath = sourceDirectory.FullName;
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
                        CurrentPath = sourceFileSpec;
                        string sourceFileName = fi.Name;
                        string targetFileSpec = DeriveFullTargetFile(sourceFileSpec);
                        if (targetFileSpec != sourceFileSpec)
                        {
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
                                    string symbolicTargetFileSpec = ExtractSymbolicTarget(targetFileSpec);
                                    _log.WriteTimedMsg("000", "I", String.Format(@"{0} : File Copied To : {1}", symbolicTargetFileSpec, targetFileSpec));
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
                                _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Error : {1}{2}", sourceFileSpec, Environment.NewLine, ex.Message));
                                _log.WriteLn();
                            }
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
