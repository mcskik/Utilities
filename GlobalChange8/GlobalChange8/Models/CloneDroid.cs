using Log8;
using ProfileData.DataLayer.Profile;
using Routines8;
using GlobalChange8.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using R = Routines8.Routines;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Global change droid (CloneDroid).
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and apply all the custom changes required for mode = "CloneDroid".
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class CloneDroid
    {
        const string SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        const string GUID_LETTERS = @"0123456789abcdefABCDEF-";
        const string GUID_LETTERS_LOWER = @"0123456789abcdef-";
        const string GUID_LETTERS_UPPER = @"0123456789ABCDEF-";
        const char SPACE = ' ';
        const string DELIMITER_SPACE = @"SPACE";
        const string DELIMITER_SPACES = @"SPACES";
        const string DELIMITER_SPECIAL = @"SPECIAL";
        const string DELIMITER_ANY = @"ANY";
        const string DELIMITER_EMPTY_STRING = @"";

        public enum ProcessingModeEnum
        {
            SplitSections,
            SourceSectionAll,
            TargetSectionOnly
        }

        protected string _action = string.Empty;
        protected string _path = string.Empty;
        protected string _mode = string.Empty;
        protected Logger _log = null;

        public string RunMode { get; set; }
        public string InspectAll { get; set; } 
        public string CloneAll { get; set; }
        public string EditAll { get; set; }
        public string RegexEditAll { get; set; }
        public string PackageNameRoot { get; set; }
        public string SourceHlq { get; set; }
        public string TargetHlq { get; set; }
        public string PassOneTargetHlq { get; set; }
        public string PassTwoTargetHlq { get; set; }
        public string CurrentPath { get; set; }
        public ProcessingModeEnum ProcessingMode { get; set; }

        protected SortedDictionary<string, EditRule> _editRules;
        protected SortedDictionary<string, RegexEditRule> _regexEditRules;
        protected SortedDictionary<string, EditRule> _packageRules;
        protected SortedDictionary<string, EditRule> _folderRules;
        protected SortedDictionary<string, EditRule> _guidRules;
        protected SortedDictionary<string, EditRule> _identificationRules;
        protected SortedDictionary<string, EditRule> _classOrResourceRules;
        protected SortedDictionary<string, string> _newActivitiesMapping;

        private int _directoryRuleCount = 0;
        private int _fileRuleCount = 0;
        private int _editRuleCount = 0;
        private int _regexEditRuleCount = 0;
        private int _packageRuleCount = 0;
        private int _folderRuleCount = 0;
        private int _classOrResourceRuleCount = 0;
        private int _directoriesSourceSectionAllCopiedCount = 0;
        private int _filesSourceSectionAllCopiedCount = 0;
        private int _filesSourceSectionAllChangedCount = 0;
        private int _filesSourceSectionAllCount = 0;
        private int _linesSourceSectionAllChangedCount = 0;
        private int _linesSourceSectionAllCount = 0;

        private int _directoriesTargetSectionOnlyCopiedCount = 0;
        private int _filesTargetSectionOnlyCopiedCount = 0;
        private int _filesTargetSectionOnlyChangedCount = 0;
        private int _filesTargetSectionOnlyCount = 0;
        private int _linesTargetSectionOnlyChangedCount = 0;
        private int _linesTargetSectionOnlyCount = 0;

        private int _filesInspectedCount = 0;
        private int _filesInspectedHitCount = 0;
        private int _linesInspectedCount = 0;
        private int _linesInspectedHitCount = 0;

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
        public CloneDroid()
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
                    case "InspectAll":
                        InspectAll = (string)flag;
                        break;
                    case "CloneAll":
                        CloneAll = (string)flag;
                        break;
                    case "EditAll":
                        EditAll = (string)flag;
                        break;
                    case "RegexEditAll":
                        RegexEditAll = (string)flag;
                        break;
                    case "PackageNameRoot":
                        PackageNameRoot = (string)flag;
                        break;
                    case "SourceHlq":
                        SourceHlq = (string)flag;
                        break;
                    case "PassOneTargetHlq":
                        PassOneTargetHlq = (string)flag;
                        break;
                    case "PassTwoTargetHlq":
                        PassTwoTargetHlq = (string)flag;
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

        /// <summary>
        /// Load Configuration Regex Edit Rules.
        /// </summary>
        public void LoadConfigurationRegexEditRules()
        {
            string fileSpec = String.Format(@"{0}{1}.xml", Administrator.ProfileManager.SystemProfile.CloneConfigurationPath, RunMode);
            XDocument doc = XDocument.Load(fileSpec);
            foreach (XElement step in doc.Descendants("Step"))
            {
                //Regex Edit Rules.
                _regexEditRules = new SortedDictionary<string, RegexEditRule>();
                _regexEditRuleCount = 0;
                foreach (XElement edits in step.Descendants("RegexEdits"))
                {
                    foreach (XElement rule in edits.Descendants("RegexEdit"))
                    {
                        string active = (string)rule.Attribute("Active");
                        if (IsAffirmative(active))
                        {
                            string findPrefix = (string)rule.Element("FindPrefix");
                            string findSuffix = (string)rule.Element("FindSuffix");
                            string replacement = (string)rule.Element("Replacement");
                            if (findPrefix.Length > 0 && findSuffix.Length > 0 && replacement.Length > 0)
                            {
                                if (!_regexEditRules.ContainsKey(findPrefix))
                                {
                                    _regexEditRuleCount++;
                                    var regexEditRUle = new RegexEditRule(findPrefix, findSuffix, replacement, _regexEditRuleCount);
                                    _regexEditRules.Add(regexEditRUle.Order, regexEditRUle);
                                }
                            }
                        }
                    }
                }
            }
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

        private void EvaluatePackages(string sourceDir, string targetDir)
        {
            sourceDir = sourceDir.Replace(SourceHlq, string.Empty);
            targetDir = targetDir.Replace(SourceHlq, string.Empty);
            if (sourceDir != targetDir)
            {
                _folderRuleCount++;
                var folderRule = new EditRule(sourceDir, targetDir, _folderRuleCount);
                _folderRules.Add(folderRule.Order, folderRule);
                string sourcePackage = ExtractPackage(sourceDir);
                string targetPackage = ExtractPackage(targetDir);
                if (sourcePackage.Length > 0 && targetPackage.Length > 0)
                {
                    if (sourcePackage != targetPackage)
                    {
                        _packageRuleCount++;
                        var packageRule = new EditRule(sourcePackage, targetPackage, _packageRuleCount);
                        _packageRules.Add(packageRule.Order, packageRule);
                    }
                }
            }
        }

        private void EvaluateFiles(string sourceFileSpec, string targetFileSpec)
        {
            if (sourceFileSpec != targetFileSpec)
            {
                string sourceFileName = Routines.FileName(sourceFileSpec);
                string targetFileName = Routines.FileName(targetFileSpec);
                if (sourceFileName.Length > 0 && targetFileName.Length > 0)
                {
                    if (sourceFileName != targetFileName)
                    {
                        _classOrResourceRuleCount++;
                        var classOrResourceRule = new EditRule(sourceFileName, targetFileName, _classOrResourceRuleCount);
                        _classOrResourceRules.Add(classOrResourceRule.Order, classOrResourceRule);
                    }
                }
            }
        }

        private string ExtractPackage(string directory)
        {
            string package = string.Empty;
            int pos = directory.IndexOf("com");
            if (pos != -1)
            {
                directory = directory.Substring(pos);
                package = directory.Replace(@"\", ".");
            }
            return package;
        }

        private void PrintEditRules(SortedDictionary<string, EditRule> editRules)
        {
            if (editRules != null)
            {
                if (editRules.Count > 0)
                {
                    _log.WriteLn();
                    foreach (KeyValuePair<string, EditRule> rule in editRules)
                    {
                        string find = rule.Value.Find;
                        string replacement = rule.Value.Replacement;
                        _log.WriteTimedMsg("000", "I", String.Format(@"Find: ""{0}"", Replacement: ""{1}""", find, replacement));
                    }
                }
            }
        }

        private void PrintRegexEditRules(SortedDictionary<string, RegexEditRule> regexEditRules)
        {
            if (regexEditRules != null)
            {
                if (regexEditRules.Count > 0)
                {
                    _log.WriteLn();
                    foreach (KeyValuePair<string, RegexEditRule> rule in regexEditRules)
                    {
                        string findPrefix = rule.Value.FindPrefix;
                        string findSuffix = rule.Value.FindSuffix;
                        string replacement = rule.Value.Replacement;
                        _log.WriteTimedMsg("000", "I", String.Format(@"FindPrefix: ""{0}"", FindSuffix: ""{1}"", Replacement: ""{2}""", findPrefix, findSuffix, replacement));
                    }
                }
            }
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
        /// </summary>
        public void Run()
        {
            if (IsAffirmative(InspectAll))
            {
            }
            if (IsAffirmative(CloneAll))
            {
                ScanSourceSectionAllEntries();
                CopySourceSectionAllEntries();
                CloneNewFeatureSectionOnlyEntries();
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
        /// Scan all directory entries from a complete directory tree.
        /// </summary>
        private void ScanSourceSectionAllEntries()
        {
            //TODO: First pass should really just copy everything from SourceHlq to TargetHlq plus edit all files plus build a list of clone rules and FileName edit rules.

            //TODO: Maybe there should be three passes:

            //TODO: First Pass - Traverse full source directory and file tree and build a list of clone directory and clone files rules to add to the ones in the original XML file.

            //TODO: Second Pass - Traverse full source directory and copy everything to the SourceSectionAll target directory using the same directory and file names, but editing all files using all edit rules known at this point.
            //TODO: This directory tree and its contents can be used to find code snippets with everything changed to reflect the target section.  For example a strings.xml file with all journal changed to ledger.
            //TODO: These code snippets can be added to the original strings.xml file alongside the original journal strings.

            //TODO: Third Pass - Traverse full source directory and copy just the target section directories and files to the TargetSectionOnly directory using the full set of clone directory and clone file rules known at this time.
            //TODO: These directories and files can be copied into the original project.
            //TODO: Hopefully we have generated a few extra target section directories and files which weren't known about at the start
            //TODO: but which have come from references found in the code that was originally copied like for example API POJO classes, but renamed to match the new section.

            _packageRules = new SortedDictionary<string, EditRule>();
            _folderRules = new SortedDictionary<string, EditRule>();
            _classOrResourceRules = new SortedDictionary<string, EditRule>();
            _guidRules = new SortedDictionary<string, EditRule>();
            _identificationRules = new SortedDictionary<string, EditRule>();

            ProcessingMode = ProcessingModeEnum.SourceSectionAll;
            TargetHlq = PassOneTargetHlq;
            LoadConfigurationEditRules();
            _directoriesSourceSectionAllCopiedCount = 0;
            _filesSourceSectionAllCopiedCount = 0;
            _filesSourceSectionAllChangedCount = 0;
            _linesSourceSectionAllChangedCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Droid (Scan Source Section All) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Scan_Source_Section_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceHlq;
            XScanAll(sourceDirectory);
            PrintAllRules();
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Folders Scanned = {0}", _directoriesSourceSectionAllCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Scanned = {0}", _filesSourceSectionAllCopiedCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        private void PrintAllRules()
        {
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Edit Rules:");
            _log.WriteLn();
            PrintEditRules(_editRules);
            if (IsAffirmative(RegexEditAll))
            {
                PrintRegexEditRules(_regexEditRules);
            }
            PrintEditRules(_packageRules);
            PrintEditRules(_folderRules);
            PrintEditRules(_classOrResourceRules);
            PrintEditRules(_guidRules);
            PrintEditRules(_identificationRules);
        }

        /// <summary>
        /// Copy all directory entries from a complete directory tree.
        /// </summary>
        private void CopySourceSectionAllEntries()
        {
            ProcessingMode = ProcessingModeEnum.SourceSectionAll;
            TargetHlq = PassOneTargetHlq;
            LoadConfigurationEditRules();
            if (IsAffirmative(RegexEditAll))
            {
                LoadConfigurationRegexEditRules();
            }
            _directoriesSourceSectionAllCopiedCount = 0;
            _filesSourceSectionAllCopiedCount = 0;
            _filesSourceSectionAllChangedCount = 0;
            _linesSourceSectionAllChangedCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Droid (Copy Source Section All) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Copy_Source_Section_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            PrintAllRules();
            XCopyAll(SourceHlq);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Folders Copied = {0}", _directoriesSourceSectionAllCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Copied = {0}", _filesSourceSectionAllCopiedCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Clone only the new feature section and related directories and files from a complete directory tree.
        /// </summary>
        private void CloneNewFeatureSectionOnlyEntries()
        {
            ProcessingMode = ProcessingModeEnum.TargetSectionOnly;
            TargetHlq = PassTwoTargetHlq;
            LoadConfigurationEditRules();
            if (IsAffirmative(RegexEditAll))
            {
                LoadConfigurationRegexEditRules();
            }
            _directoriesTargetSectionOnlyCopiedCount = 0;
            _filesTargetSectionOnlyCopiedCount = 0;
            _filesTargetSectionOnlyChangedCount = 0;
            _linesTargetSectionOnlyChangedCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Droid (Clone New Feature Section Only) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_New_Feature_Section_Only_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            PrintAllRules();
            XCloneNewSection(SourceHlq);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Folders Copied = {0}", _directoriesTargetSectionOnlyCopiedCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Files   Copied = {0}", _filesTargetSectionOnlyCopiedCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Scan all directories and files from a complete directory tree.
        /// </summary>
        /// <remarks>
        /// The purpose of this is to apply the original set of edit rules to discover a full set of changed folder, package, and file name edit rules,
        /// So that a more precise set of edit rules can be applied later. For example all the file name change rules will capture precise class names and drawable resource names,
        /// which can be applied first before later applying the original edit rules which are more generic.
        /// The aim of this is to try to avoid making accidental changes to the wrong things.
        /// </remarks>
        private void XScanAll(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XScanAll");
            XScanAll(diSource);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XScanAll");
        }

        private void XScanAll(DirectoryInfo sourceDirectory)
        {
            string sourceDir = sourceDirectory.FullName;
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                string trialDir = EditLine(_editRules, sourceDir);
                EvaluatePackages(sourceDir, trialDir);
                _directoriesSourceSectionAllCopiedCount++;
                _log.WriteLn();
                string symbolicTargetDirectory = ExtractSymbolicTarget(sourceDir);
                _log.WriteTimedMsg("000", "I", String.Format(@"{0} : Scan Folder  : {1}", symbolicTargetDirectory, sourceDir));
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileSpec = fi.FullName;
                        string trialFileSpec = EditLine(_editRules, sourceFileSpec);
                        EvaluateFiles(sourceFileSpec, trialFileSpec);
                        if (IsAffirmative(InspectAll))
                        {
                            if (IsTextFile(sourceFileSpec))
                            {
                                InspectTextFile(sourceFileSpec);
                            }
                        }
                        _filesSourceSectionAllCopiedCount++;
                        string symbolicTargetFile = ExtractSymbolicTarget(sourceFileSpec);
                        _log.WriteTimedMsg("000", "I", String.Format(@"{0} : File Scanned : {1}", symbolicTargetFile, sourceFileSpec));
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
                                XScanAll(di);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XScanAll cancelled"));
                return;
            }
        }

        /// <summary>
        /// Copy all directories and files from a complete directory tree.
        /// </summary>
        /// <remarks>
        /// This just copies everything from the source HLQ to the target HLQ.
        /// The directory structure and all file contents are the same until we edit everything which happens later in another step.
        /// The purpose of this is to edit everything within the same named files to provide edited code snippets which can be manually appended to the original files.
        /// Good examples of these are strings.xml, colors.xml, and dimens.xml which may have dedicated groups of resources for each section of the app.
        /// So if there was already a "Journal" section and you were creating a "Ledger" section then this would provide groups with everything edited to match the "Ledger" section.
        /// These could then be manually copied into the original files below the original "Journal" section.
        /// </remarks>
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
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                string targetDir = sourceDirectory.FullName.Replace(SourceHlq, TargetHlq);
                DirectoryInfo targetDirectory = new DirectoryInfo(targetDir);
                if (!Directory.Exists(targetDirectory.FullName))
                {
                    Directory.CreateDirectory(targetDirectory.FullName);
                }
                _log.WriteLn();
                string symbolicTargetDirectory = ExtractSymbolicTarget(targetDir);
                _log.WriteTimedMsg("000", "I", String.Format(@"{0} : Copy Folder To : {1}", symbolicTargetDirectory, targetDir));
                _directoriesSourceSectionAllCopiedCount++;
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileName = fi.Name;
                        string targetFileName = sourceFileName;
                        string sourceFileSpec = fi.FullName;
                        string targetFileSpec = sourceFileSpec.Replace(SourceHlq, TargetHlq);
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
                                _filesSourceSectionAllCopiedCount++;
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
        /// Clone only the new feature section and related directories and files from a complete directory tree.
        /// </summary>
        /// <remarks>
        /// This clones the directories and files for the new feature section only into a separate target folder so that they can easily be seen.
        /// This will primarily clone the new feature section folders and files including layouts.
        /// However this will also pick up other folders and files elsewhere (perhaps in the common folder) which have directory or file names which are affected by the edit rules.
        /// For example there may be API POJO classes with "Journal" in their directory or file names which will be changed to "Ledger" in all the same places.
        /// So you will end up with clones of these as well.
        /// It is now up to the developer to copy all the cloned folders, files and code snippets into the original project to create the new cloned feature section to use as a basis for further development.
        /// </remarks>
        private void XCloneNewSection(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XCloneNewSection");
            XCloneNewSection(diSource);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XCloneNewSection");
        }

        /// <remarks>
        /// This is not using the "Copy" rules, it is just using a simplified approach of just editing the source directory and files using rules derived from the original edit rules.
        /// </remarks>
        private void XCloneNewSection(DirectoryInfo sourceDirectory)
        {
            string sourceDir = sourceDirectory.FullName;
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDir))
            {
                bool directoryHeaderWritten = false;
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileName = fi.Name;
                        string targetFileName = EditLine(_classOrResourceRules, sourceFileName);
                        string sourceFileSpec = fi.FullName;
                        string targetFileSpec = sourceFileSpec.Replace(SourceHlq, TargetHlq);
                        string targetFileSpecHlqChangedOnly = targetFileSpec;
                        // This is not using the "Copy" rules, it is just using a simplified approach of just editing the source directory and files using rules derived from the original edit rules.
                        targetFileSpec = EditLine(_folderRules, targetFileSpec);
                        targetFileSpec = EditLine(_classOrResourceRules, targetFileSpec);
                        if (targetFileSpec != targetFileSpecHlqChangedOnly)
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
                                    string symbolicTarget = ExtractSymbolicTarget(targetFileSpec);
                                    if (!directoryHeaderWritten)
                                    {
                                        directoryHeaderWritten = true;
                                        string targetDir = Routines.FileStem(targetFileSpec);
                                        _log.WriteLn();
                                        _log.WriteTimedMsg("000", "I", String.Format(@"{0} : Copy Folder To : {1}", symbolicTarget, targetDir));
                                        _directoriesTargetSectionOnlyCopiedCount++;
                                    }
                                    _log.WriteTimedMsg("000", "I", String.Format(@"{0} : File Copied To : {1}", symbolicTarget, targetFileSpec));
                                    _filesTargetSectionOnlyCopiedCount++;
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
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (_action != "Cancel")
                    {
                        try
                        {
                            if (DirectoryExclusionsHelper.AllowDirectory(di.FullName))
                            {
                                XCloneNewSection(di);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XCloneNewSection cancelled"));
                return;
            }
        }

        /// <summary>
        /// Global change/edit all files in complete target directory tree.
        /// </summary>
        private void EditAllEntries()
        {
            EditAllEntries(ProcessingModeEnum.SourceSectionAll);
            EditAllEntries(ProcessingModeEnum.TargetSectionOnly);
        }

        /// <summary>
        /// Global change/edit all files in complete target directory tree.
        /// </summary>
        private void EditAllEntries(ProcessingModeEnum processingMode)
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = null;
            GC.Collect();
            switch (processingMode)
            {
                case ProcessingModeEnum.SourceSectionAll:
                    TargetHlq = PassOneTargetHlq;
                    _filesSourceSectionAllCount = 0;
                    _linesSourceSectionAllCount = 0;
                    _filesSourceSectionAllChangedCount = 0;
                    _linesSourceSectionAllChangedCount = 0;
                    _log = new Logger();
                    _log.Prefix = "GCE";
                    _log.Title = "Global Change/Clone Droid (Edit Source Section All) " + Administrator.ProfileManager.ApplicationProfile.Version;
                    FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                    _log.Begin(String.Format("{0}Global_Change_Edit_Source_Section_All_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
                    PrintAllRules();
                    XEditAll(TargetHlq);
                    _log.WriteLn();
                    _log.WriteTimedMsg("000", "I", String.Format(@"Files Processed = {0}", _filesSourceSectionAllCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Lines Processed = {0}", _linesSourceSectionAllCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Files Changed   = {0}", _filesSourceSectionAllChangedCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Lines Changed   = {0}", _linesSourceSectionAllChangedCount));
                    _log.Outcome();
                    _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
                    break;
                case ProcessingModeEnum.TargetSectionOnly:
                    TargetHlq = PassTwoTargetHlq;
                    _filesTargetSectionOnlyCount = 0;
                    _linesTargetSectionOnlyCount = 0;
                    _filesTargetSectionOnlyChangedCount = 0;
                    _linesTargetSectionOnlyChangedCount = 0;
                    _log = new Logger();
                    _log.Prefix = "GCE";
                    _log.Title = "Global Change/Clone Droid (Edit Target Section Only) " + Administrator.ProfileManager.ApplicationProfile.Version;
                    FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                    _log.Begin(String.Format("{0}Global_Change_Edit_Target_Section_Only_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
                    PrintAllRules();
                    XEditAll(TargetHlq);
                    _log.WriteLn();
                    _log.WriteTimedMsg("000", "I", String.Format(@"Files Processed = {0}", _filesTargetSectionOnlyCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Lines Processed = {0}", _linesTargetSectionOnlyCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Files Changed   = {0}", _filesTargetSectionOnlyChangedCount));
                    _log.WriteTimedMsg("000", "I", String.Format(@"Lines Changed   = {0}", _linesTargetSectionOnlyChangedCount));
                    _log.Outcome();
                    _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Edit all editable (text) files from a complete directory tree.
        /// </summary>
        /// <remarks>
        /// At present the following edit rules will be used on all text files: _editRules, _packageRules, _classOrResourceRules.
        /// At present _folderRules will not be used to edit text files.
        /// We are planning to add _editShortLeaderRules and _editShortTrailerRules to edit very short strings where there is an increased risk of unintentionally editing the wrong things.
        /// For these rules we will prefix or suffix a list of special characters to either end to reduce the incidence of incorrect changes.
        /// We will probably use the special characters defined in this constant: SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        /// </remarks>
        private void XEditAll(string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XEditAll");
            XEditAll(diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XEditAll");
        }

        private void XEditAll(DirectoryInfo targetDirectory)
        {
            //_log.WriteLn("XEditAll Directory : " + targetDirectory.FullName);
            if (Directory.Exists(targetDirectory.FullName) && DirectoryExclusionsHelper.AllowDirectory(targetDirectory.FullName))
            {
                //_log.WriteLn();
                //_log.WriteTimedMsg("000", "I", String.Format(@"Global Change All Folders : {0}", targetDirectory.ToString()));
                foreach (FileInfo fi in targetDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string targetFileName = string.Empty;
                        try
                        {
                            targetFileName = fi.Name;
                            try
                            {
                                string fileSpec = fi.FullName;
                                if (IsTextFile(fileSpec))
                                {
                                    int currentLinesChanged = _linesSourceSectionAllChangedCount;
                                    EditTextFile(fileSpec);
                                    if (_linesSourceSectionAllChangedCount > currentLinesChanged)
                                    {
                                        _filesSourceSectionAllChangedCount++;
                                        _filesTargetSectionOnlyChangedCount++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.WriteLn();
                                _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Error : {1}{2}", targetFileName, Environment.NewLine, ex.Message));
                                _log.WriteLn();
                            }
                        }
                        catch (Exception ex2)
                        {
                            _log.WriteLn();
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Target File (Name too long) Error : {1}{2}", targetFileName, Environment.NewLine, ex2.Message));
                            _log.WriteLn();
                        }
                    }
                }
                foreach (DirectoryInfo di in targetDirectory.GetDirectories())
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
                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Target Directory (Name too long) Error : {1}{2}", di.Name, Environment.NewLine, ex.Message));
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

        protected bool IsInspectionTextFile(string fileSpec)
        {
            return TextTypesHelper.AllowFile(fileSpec, true);
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
            textTypes.Add("csproj");
            textTypes.Add("java");
            textTypes.Add("kt");
            textTypes.Add("sln");
            textTypes.Add("xml");
            string fileType = Routines.FileExt(fileSpec);
            if (textTypes.Contains(fileType))
            {
                isTextType = true;
            }
            return isTextType;
        }

        /// <summary>
        /// Inspect Text file.
        /// </summary>
        /// <remarks>
        /// Inspect one Text file.
        /// </remarks>
        protected bool InspectTextFile(string fileSpec)
        {
            _filesInspectedCount++;
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
                            _linesInspectedCount++;
                            record = sr.ReadLine();
                            line = record;
                            string[] guidLowerWordArray = new string[100];
                            int guidLowerWordCount = GuidWordArray(line, ref guidLowerWordArray, GUID_LETTERS_LOWER);
                            if (guidLowerWordCount > 0)
                            {
                                _linesInspectedHitCount++;
                                for(int index = 0; index < guidLowerWordCount; index++)
                                {
                                    var guidFind = guidLowerWordArray[index];
                                    Guid guidLower = Guid.NewGuid();
                                    string guidReplace = guidLower.ToString().ToLower();
                                    if (!guidFind.Contains("-"))
                                    {
                                        guidReplace = guidReplace.Replace("-", string.Empty);
                                    }
                                    EditRule guidRule = new EditRule(guidFind, guidReplace, _guidRules.Count + 1);
                                    if (!_guidRules.ContainsKey(guidFind))
                                    {
                                        _guidRules.Add(guidFind, guidRule);
                                    }
                                }
                            }
                            string[] guidUpperWordArray = new string[100];
                            int guidUpperWordCount = GuidWordArray(line, ref guidUpperWordArray, GUID_LETTERS_UPPER);
                            if (guidUpperWordCount > 0)
                            {
                                _linesInspectedHitCount++;
                                for (int index = 0; index < guidUpperWordCount; index++)
                                {
                                    var guidFind = guidUpperWordArray[index];
                                    Guid guidUpper = Guid.NewGuid();
                                    string guidReplace = guidUpper.ToString().ToUpper();
                                    if (!guidFind.Contains("-"))
                                    {
                                        guidReplace = guidReplace.Replace("-", string.Empty);
                                    }
                                    EditRule guidRule = new EditRule(guidFind, guidReplace, _guidRules.Count + 1);
                                    if (!_guidRules.ContainsKey(guidFind))
                                    {
                                        _guidRules.Add(guidFind, guidRule);
                                    }
                                }
                            }
                            contents.AppendLine(line);
                            if (_action == "Cancel")
                            {
                                break;
                            }
                        } while (sr.Peek() >= 0);
                    }
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
            }
            return hit;
        }

        /// <summary>
        /// Edit Text file.
        /// </summary>
        /// <remarks>
        /// Apply global changes to one Text file.
        /// </remarks>
        protected bool EditTextFile(string fileSpec)
        {
            _filesSourceSectionAllCount++;
            _filesTargetSectionOnlyCount++;
            StringBuilder contents = new StringBuilder();
            string line = string.Empty;
            bool first = true;
            bool hit = false;
            bool changed = false;
            FileInfo fileInfo = null;
            string record = string.Empty;
            long count = 0;
            bool activityInfoCaptured = true;
            string currentClassName = System.IO.Path.GetFileNameWithoutExtension(fileSpec);
            if (currentClassName.EndsWith("Activity"))
            {
                activityInfoCaptured = false;
            }
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
                            _linesSourceSectionAllCount++;
                            _linesTargetSectionOnlyCount++;
                            record = sr.ReadLine();
                            line = record;
                            foreach (KeyValuePair<string, EditRule> entry in _packageRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            foreach (KeyValuePair<string, EditRule> entry in _classOrResourceRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            foreach (KeyValuePair<string, EditRule> entry in _guidRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            foreach (KeyValuePair<string, EditRule> entry in _identificationRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            foreach (KeyValuePair<string, EditRule> entry in _editRules)
                            {
                                EditLine(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            if (IsAffirmative(RegexEditAll))
                            {
                                foreach (KeyValuePair<string, RegexEditRule> entry in _regexEditRules)
                                {
                                    RegexEditLine(entry, ref line, ref changed, ref first, fileSpec, count, ref record);
                                }
                            }
                            //TODO: Add _editShortLeaderRules.
                            //TODO: Add _editShortTrailerRules.
                            if (!activityInfoCaptured)
                            {
                                //Capture package name after it ha been changed to the new package name.
                                if (line.StartsWith("package"))
                                {
                                    string package = line.Replace("package", string.Empty);
                                    package = package.Replace(";", string.Empty);
                                    package = package.Trim();
                                    if (package.StartsWith(PackageNameRoot))
                                    {
                                        package = package.Replace(PackageNameRoot, string.Empty);
                                    }
                                    package = package.Trim();
                                    package += "." + currentClassName;
                                    if (!_newActivitiesMapping.ContainsKey(package))
                                    {
                                        _newActivitiesMapping.Add(package, currentClassName);
                                    }
                                    activityInfoCaptured = true;
                                }
                            }
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
                        FileHelper.WriteFile(fileSpec, contents.ToString());
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
                _linesSourceSectionAllChangedCount++;
                _linesTargetSectionOnlyChangedCount++;
                line = line.Replace(find, replacement);
                RecordChange(hit, ref first, fileSpec, count, record, line);
                record = line;
            }
            return line;
        }

        //TODO: Regex find and replace is not working (No surprise there!)
        //TODO: Therefore I am using an interim solution using my own non-regex code.
        //TODO: The interim code just looks for a prefix string and a suffix string,
        //TODO: and then replaces the text found in the middle with the replacement string.
        private string RegexEditLine(KeyValuePair<string, RegexEditRule> entry, ref string line, ref bool changed, ref bool first, string fileSpec, long count, ref string record)
        {
            bool hit = false;
            string findPrefix = entry.Value.FindPrefix;
            string findSuffix = entry.Value.FindSuffix;
            string replacement = entry.Value.Replacement;
            int pos1 = line.IndexOf(findPrefix);
            if (pos1 != -1)
            {
                pos1 += findPrefix.Length;
                int pos2 = line.IndexOf(findSuffix, pos1);
                if (pos2 != -1)
                {
                    hit = true;
                    changed = true;
                    _linesSourceSectionAllChangedCount++;
                    _linesTargetSectionOnlyChangedCount++;
                    string leader = line.Substring(0, pos1);
                    string middle = line.Substring(pos1, pos2 - pos1);
                    string trailer = line.Substring(pos2);
                    string check = leader + replacement + trailer;
                    if (check == line)
                    {
                        RecordUnchanged(hit, ref first, fileSpec, count, record, line);
                    }
                    else
                    {
                        line = leader + replacement + trailer;
                        RecordChange(hit, ref first, fileSpec, count, record, line);
                        record = line;
                    }
                }
            }
            return line;
        }

        //TODO: Regex find and replace is not working (No surprise there!)
        //TODO: Therefore I am using an interim solution using my own non-regex code.
        //TODO: The interim code just looks for a prefix string and a suffix string,
        //TODO: and then replaces the text found in the middle with the replacement string.
        private string RegexEditLine_Doesnt_Work(KeyValuePair<string, RegexEditRule> entry, ref string line, ref bool changed, ref bool first, string fileSpec, long count, ref string record)
        {
            bool hit = false;
            string find = entry.Value.FindPrefix;
            string replacement = entry.Value.Replacement;
            Regex regex = new Regex(find);
            hit = regex.IsMatch(line);
            if (hit)
            {
                changed = true;
                _linesSourceSectionAllChangedCount++;
                _linesTargetSectionOnlyChangedCount++;
                line = regex.Replace(line, replacement);
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
        /// Since multiple changes are being made in different places for different "modes", the recording of a hit has been moved into its own method.
        /// </summary>
        protected void RecordUnchanged(bool hit, ref bool first, string fileSpec, long count, string record, string line)
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
                header = "Keep    : (" + count.ToString("00000") + ") - " + line;
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
        /// Return a count of the GUID words in the input string and store all GUID words found
        /// in consecutive locations of the output word array.
        /// </summary>
        /// <param name="textString">Input string.</param>
        /// <param name="wordArray">Output GUID word array.</param>
        /// <param name="textDelimiter">Used to determine where words begin and end.</param>
        /// <returns>Count of words in the input string.</returns>
        public int GuidWordArray(string textString, ref string[] wordArray, string guidLetters)
        {
            string special = string.Empty;
            string delimiter = string.Empty;
            string text = string.Empty;
            string word = string.Empty;
            string letter = string.Empty;
            int pos = 0;
            int len = 0;
            int count = 0;
            text = textString.Trim() + new string(SPACE, 2);
            if (text.CompareTo(new string(SPACE, 2)) <= 0)
            {
                return 0;
            }
            count = 0;
            pos = 0;
            len = text.Length;
            letter = text.Substring(pos, 1);
            do
            {
                word = string.Empty;
                while (pos < len - 1 && guidLetters.IndexOf(letter) == -1)
                {
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                while (pos < len - 1 && guidLetters.IndexOf(letter) != -1)
                {
                    word = word + letter;
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                if (word.CompareTo(string.Empty) > 0)
                {
                    if (word.Length > 20 && word.Length < 50)
                    {
                        string prevLetter = word.Substring(0, 1);
                        for(int ptr = 1; ptr < word.Length; ptr++)
                        {
                            string thisLetter = word.Substring(ptr, 1);
                            if (thisLetter != prevLetter)
                            {
                                count++;
                                wordArray[count - 1] = word;
                                break;
                            }
                            prevLetter = thisLetter;
                        }
                    }
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
