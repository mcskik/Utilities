using GlobalChange8.DataLayer.Profile;
using Log8;
using ProfileData.DataLayer.Profile;
using Routines8;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using R = Routines8.Routines;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Global change droid (ReplaceDroid).
    /// </summary>
    /// <remarks>
    /// Apply all gobal changes required for global search and replace mode = "Replace*".
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ReplaceDroid
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
        public string EditAll { get; set; }
        public string ApplyChanges { get; set; }
        public string SourceHlq { get; set; }

        protected SortedDictionary<string, EditRule> _editRules;

        private int _editRuleCount = 0;
        private int _foldersSearchedCount = 0;
        private int _filesSearchedCount = 0;
        private int _linesSearchedCount = 0;
        private int _foldersChangedCount = 0;
        private int _filesChangedCount = 0;
        private int _filesSavedCount = 0;
        private int _linesChangedCount = 0;

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
        public ReplaceDroid()
        {
            Load();
        }

        /// <summary>
        /// Run the selected repro (Currently equal to the selected RunMode).
        /// </summary>
        public void LoadConfiguration(string runMode)
        {
            RunMode = runMode;
            LoadConfigurationSwitches(runMode);
        }

        /// <summary>
        /// Load Configuration Switches.
        /// </summary>
        public void LoadConfigurationSwitches(string runMode)
        {
            string fileSpec = String.Format(@"{0}{1}.xml", Administrator.ProfileManager.SystemProfile.CloneConfigurationPath, runMode);
            XDocument doc = XDocument.Load(fileSpec);
            foreach (XElement flag in doc.Descendants("Switches").Elements())
            {
                string flagName = flag.Name.LocalName;
                switch (flagName)
                {
                    case "EditAll":
                        EditAll = (string)flag;
                        break;
                    case "ApplyChanges":
                        ApplyChanges = (string)flag;
                        break;
                    case "SourceHlq":
                        SourceHlq = (string)flag;
                        break;
                }
            }
        }

        /// <summary>
        /// Load Configuration Edit Rules.
        /// </summary>
        public void LoadConfigurationEditRules()
        {
            string fileSpec = String.Format(@"{0}{1}.xml", Administrator.ProfileManager.SystemProfile.CloneConfigurationPath, RunMode);
            XDocument doc = XDocument.Load(fileSpec);
            foreach (XElement step in doc.Descendants("Step"))
            {
                //Edit Rules.
                _editRules = new SortedDictionary<string, EditRule>();
                _editRuleCount = 0;
                foreach (XElement edits in step.Descendants("Edits"))
                {
                    foreach (XElement rule in edits.Descendants("Edit"))
                    {
                        string active = (string)rule.Attribute("Active");
                        if (IsAffirmative(active))
                        {
                            string find = (string)rule.Attribute("Find");
                            string replacement = (string)rule.Attribute("Replacement");
                            if (find.Length > 0 && replacement.Length > 0)
                            {
                                if (!_editRules.ContainsKey(find))
                                {
                                    _editRuleCount++;
                                    var editRUle = new EditRule(find, replacement, _editRuleCount);
                                    _editRules.Add(editRUle.Order, editRUle);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string EditLine(SortedDictionary<string, EditRule> editRules, string line)
        {
            foreach (KeyValuePair<string, EditRule> rule in editRules)
            {
                string find = rule.Value.Find;
                string replacement = rule.Value.Replacement;
                line = line.Replace(find, replacement);
            }
            return line;
        }

        private void PrintEditRules(SortedDictionary<string, EditRule> editRules)
        {
            foreach (KeyValuePair<string, EditRule> rule in editRules)
            {
                string find = rule.Value.Find;
                string replacement = rule.Value.Replacement;
                _log.WriteTimedMsg("000", "I", String.Format(@"Find: ""{0}"", Replacement: ""{1}""", find, replacement));
            }
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
        /// </summary>
        public void Run()
        {
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

        private void PrintAllRules()
        {
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Edit Rules:");
            _log.WriteLn();
            PrintEditRules(_editRules);
        }

        /// <summary>
        /// Global change/edit all files in complete source directory tree.
        /// </summary>
        private void EditAllEntries()
        {
            LoadConfigurationEditRules();
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _foldersSearchedCount = 0;
            _filesSearchedCount = 0;
            _linesSearchedCount = 0;
            _foldersChangedCount = 0;
            _filesChangedCount = 0;
            _filesSavedCount = 0;
            _linesChangedCount = 0;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Replace Droid (Edit All) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            PrintAllRules();
            XEditAll(SourceHlq);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Folders Searched = {0}", _foldersSearchedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Searched = {0}", _filesSearchedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Lines   Searched = {0}", _linesSearchedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Folders Changed  = {0}", _foldersChangedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Changed  = {0}", _filesChangedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Saved    = {0}", _filesSavedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Lines   Changed  = {0}", _linesChangedCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Edit all editable (text) files from a complete directory tree.
        /// </summary>
        /// <remarks>
        /// At present the following edit rules will be used on all text files: _editRules.
        /// We are planning to add _editShortLeaderRules and _editShortTrailerRules to edit very short strings where there is an increased risk of unintentionally editing the wrong things.
        /// For these rules we will prefix or suffix a list of special characters to either end to reduce the incidence of incorrect changes.
        /// We will probably use the special characters defined in this constant: SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        /// </remarks>
        private void XEditAll(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XEditAll");
            XEditAll(diSource);
            if (_linesChangedCount == 0)
            {
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", "Nothing Changed");
            }
            else
            {
                if (_filesSavedCount == 0)
                {
                    _log.WriteLn();
                    _log.WriteTimedMsg("000", "I", "No Changes Saved");
                }
                else
                {
                    _log.WriteLn();
                    _log.WriteTimedMsg("000", "I", "All Changes Saved");
                }
            }
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XEditAll");
        }

        private void XEditAll(DirectoryInfo sourceDirectory)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                _foldersSearchedCount++;
                //_log.WriteLn();
                //_log.WriteTimedMsg("000", "I", String.Format(@"Global Change All Folders : {0}", sourceDirectory.ToString()));
                int currentFilesChanged = _filesChangedCount;
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileName = string.Empty;
                        try
                        {
                            sourceFileName = fi.Name;
                            try
                            {
                                string fileSpec = fi.FullName;
                                if (IsTextFile(fileSpec))
                                {
                                    int currentLinesChanged = _linesChangedCount;
                                    EditTextFile(fileSpec);
                                    if (_linesChangedCount > currentLinesChanged)
                                    {
                                        _filesChangedCount++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.WriteLn();
                                _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Error : {1}{2}", sourceFileName, Environment.NewLine, ex.Message));
                                _log.WriteLn();
                            }
                        }
                        catch (Exception ex2)
                        {
                            _log.WriteLn();
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Source File (Name too long) Error : {1}{2}", sourceFileName, Environment.NewLine, ex2.Message));
                            _log.WriteLn();
                        }
                    }
                }
                if (_filesChangedCount > currentFilesChanged)
                {
                    _foldersChangedCount++;
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (_action != "Cancel")
                    {
                        try
                        {
                            if (DirectoryExclusionsHelper.AllowDirectory(di.FullName))
                            {
                                XEditAll(di);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteLn();
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Source Directory (Name too long) Error : {1}{2}", di.Name, Environment.NewLine, ex.Message));
                            _log.WriteLn();
                        }
                    }
                }
            }
            //Check if user has chosen to cancel run.
            if (_action == "Cancel")
            {
                _log.WriteTimedMsg("000", "I", String.Format(@"XEditAll cancelled"));
                return;
            }
        }

        protected bool IsTextFile(string fileSpec)
        {
            bool isTextType = false;
            //TODO: These text types need to come from a config XML file and not be hard coded in here.
            List<string> textTypes = new List<string>();
            textTypes.Add("txt");
            textTypes.Add("htm");
            textTypes.Add("html");
            textTypes.Add("json");
            textTypes.Add("cs");
            textTypes.Add("java");
            textTypes.Add("kt");
            textTypes.Add("xml");
            string fileType = Routines.FileExt(fileSpec);
            if (textTypes.Contains(fileType))
            {
                isTextType = true;
            }
            return isTextType;
        }

        /// <summary>
        /// Edit Text file.
        /// </summary>
        /// <remarks>
        /// Apply global changes to one Text file.
        /// </remarks>
        protected bool EditTextFile(string fileSpec)
        {
            _filesSearchedCount++;
            StringBuilder contents = new StringBuilder();
            string line = string.Empty;
            bool first = true;
            bool hit = false;
            bool changed = false;
            FileInfo fileInfo = null;
            string record = string.Empty;
            long count = 0;
            try
            {
                if (File.Exists(fileSpec))
                {
                    fileInfo = R.GetFileInfo(fileSpec);
                    StreamReader sr = new StreamReader(fileSpec);
                    if (sr.Peek() >= 0)
                    {
                        do
                        {
                            count++;
                            _linesSearchedCount++;
                            record = sr.ReadLine();
                            line = record;
                            foreach (KeyValuePair<string, EditRule> entry in _editRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //TODO: Add _editShortLeaderRules.
                            //TODO: Add _editShortTrailerRules.
                            contents.AppendLine(line);
                            if (_action == "Cancel")
                            {
                                break;
                            }
                        } while (sr.Peek() >= 0);
                    }
                    sr.Close();
                    if (changed)
                    {
                        if (IsAffirmative(ApplyChanges))
                        {
                            FileHelper.WriteFile(fileSpec, contents.ToString());
                            _filesSavedCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return hit;
        }

        private string EditLine(string prefix, KeyValuePair<string, EditRule> entry, string suffix, ref string line, ref bool changed, ref bool first, string fileSpec, long count, ref string record)
        {
            bool hit = false;
            string find = prefix + entry.Value.Find + suffix;
            string replacement = prefix + entry.Value.Replacement + suffix;
            if (line.Contains(find) && !line.Contains(replacement))
            {
                hit = true;
                changed = true;
                _linesChangedCount++;
                line = line.Replace(find, replacement);
                RecordChange(hit, ref first, fileSpec, count, record, line);
                record = line;
            }
            return line;
        }

        /// <summary>
        /// Since multiple changes are being made in different places for different "modes", the recording of a hit has been moved into its own method.
        /// </summary>
        protected void RecordChange(bool hit, ref bool first, string fileSpec, long count, string record, string line)
        {
            string header = string.Empty;
            if (hit)
            {
                if (first)
                {
                    header = "\r\n" + fileSpec + "\r\n";
                    header += string.Empty.PadRight(fileSpec.Length, '-');
                    _log.WriteLn(header);
                    first = false;
                }
                header = "Found   : (" + count.ToString("00000") + ") - " + record;
                _log.WriteLn(header);
                header = "Replace : (" + count.ToString("00000") + ") - " + line;
                _log.WriteLn(header);
            }
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