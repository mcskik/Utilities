using Log8;
using ProfileData.DataLayer.Profile;
using GlobalChange8.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using R = Routines8.Routines;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Global change engine (Search Replace Multiple).
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and apply all the custom changes required for mode = "Clone".
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class SearchReplaceMultipleEngine
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

        public string EditJava { get; set; }
        public string EditJavaAndroidTests { get; set; }
        public string EditJavaUnitTests { get; set; }
        public string EditLayout { get; set; }
        public string TargetJavaDirectory { get; set; }
        public string TargetAndroidTestsJavaDirectory { get; set; }
        public string TargetUnitTestsJavaDirectory { get; set; }
        public string TargetLayoutDirectory { get; set; }

        protected SortedDictionary<string, string> _classNamePrefixes;
        protected SortedDictionary<string, string> _classNamePatterns;
        protected SortedDictionary<string, string> _variableNameExacts;
        protected SortedDictionary<string, string> _variableNamePrefixes;
        protected SortedDictionary<string, string> _variableNamePatterns;
        protected SortedDictionary<string, string> _classNameMappings;
        protected SortedDictionary<string, string> _packageNameMappings;
        protected SortedDictionary<string, string> _layoutNamePatterns;
        protected SortedDictionary<string, string> _layoutNameMappings;
        protected SortedDictionary<string, string> _cloneCopyLayoutViewModelMapping;
        protected SortedDictionary<string, string> _newActivitiesMapping;

        private int classFileCount = 0;
        private int classFileLineCount = 0;
        private int classFileLineChangeCount = 0;
        private int layoutFileCount = 0;
        private int layoutFileLineCount = 0;
        private int layoutFileLineChangeCount = 0;
        private int lineChangeCount = 0;

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
        public SearchReplaceMultipleEngine()
        {
            Load();
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
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
                    case "EditJava":
                        EditJava = (string)flag;
                        break;
                    case "EditJavaAndroidTests":
                        EditJavaAndroidTests = (string)flag;
                        break;
                    case "EditJavaUnitTests":
                        EditJavaUnitTests = (string)flag;
                        break;
                    case "EditLayout":
                        EditLayout = (string)flag;
                        break;
                }
            }
            foreach (XElement directory in doc.Descendants("Directories").Elements())
            {
                string directoryFieldName = directory.Name.LocalName;
                switch (directoryFieldName)
                {
                    case "TargetJava":
                        TargetJavaDirectory = (string)directory;
                        break;
                    case "TargetAndroidTestsJava":
                        TargetAndroidTestsJavaDirectory = (string)directory;
                        break;
                    case "TargetUnitTestsJava":
                        TargetUnitTestsJavaDirectory = (string)directory;
                        break;
                    case "TargetLayout":
                        TargetLayoutDirectory = (string)directory;
                        break;
                }
            }
            LoadMappingValues(doc, "ClassNamePrefixes", ref _classNamePrefixes);
            LoadMappingValues(doc, "ClassNamePatterns", ref _classNamePatterns);
            LoadMappingValues(doc, "VariableNameExacts", ref _variableNameExacts);
            LoadMappingValues(doc, "VariableNamePrefixes", ref _variableNamePrefixes);
            LoadMappingValues(doc, "VariableNamePatterns", ref _variableNamePatterns);
            LoadMappingValues(doc, "ClassNameMappings", ref _classNameMappings);
            LoadMappingValues(doc, "PackageNameMappings", ref _packageNameMappings);
            LoadMappingValues(doc, "LayoutNamePatterns", ref _layoutNamePatterns);
            LoadMappingValues(doc, "LayoutNameMappings", ref _layoutNameMappings);
            _cloneCopyLayoutViewModelMapping = new SortedDictionary<string, string>();
            _newActivitiesMapping = new SortedDictionary<string, string>();
        }

        protected void LoadMappingValues(XDocument doc, string mappingSetName, ref SortedDictionary<string, string> mappingDictionary)
        {
            mappingDictionary = new SortedDictionary<string, string>();
            foreach (XElement element in doc.Descendants(mappingSetName).Elements())
            {
                string find = (string)element.Attribute("Find");
                string replacement = (string)element.Attribute("Replacement");
                mappingDictionary.Add(find, replacement);
            }
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
        /// </summary>
        public void Run()
        {
            if (IsAffirmative(EditJava))
            {
                EditTargetJavaDirectory();
            }
            if (IsAffirmative(EditJavaAndroidTests))
            {
                EditTargetJavaAndroidTestsDirectory();
            }
            if (IsAffirmative(EditJavaUnitTests))
            {
                EditTargetJavaUnitTestsDirectory();
            }
            if (IsAffirmative(EditLayout))
            {
                EditTargetLayoutDirectory();
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
        /// Global change/edit target Java directory.
        /// </summary>
        private void EditTargetJavaDirectory()
        {
            classFileCount = 0;
            classFileLineCount = 0;
            classFileLineChangeCount = 0;
            lineChangeCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Edit Java) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_Java_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string targetDirectory = TargetJavaDirectory;
            XEditJava(targetDirectory);
            classFileLineChangeCount = lineChangeCount;
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Files   = {0}", classFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Lines   = {0}", classFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Changes = {0}", classFileLineChangeCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Global change/edit target Java android tests directory.
        /// </summary>
        private void EditTargetJavaAndroidTestsDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Edit Java Android Tests) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_Java_AndroidTests_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string targetDirectory = TargetAndroidTestsJavaDirectory;
            XEditJava(targetDirectory);
            int lineChangeCountIncrease = lineChangeCount - classFileLineChangeCount;
            classFileLineChangeCount = lineChangeCount;
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Files   = {0}", classFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Lines   = {0}", classFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Changes = {0}", classFileLineChangeCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Added Changes = {0}", lineChangeCountIncrease));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Global change/edit target Java unit tests directory.
        /// </summary>
        private void EditTargetJavaUnitTestsDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Edit Java Unit Tests) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_Java_UnitTests_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string targetDirectory = TargetUnitTestsJavaDirectory;
            XEditJava(targetDirectory);
            int lineChangeCountIncrease = lineChangeCount - classFileLineChangeCount;
            classFileLineChangeCount = lineChangeCount;
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Files   = {0}", classFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Lines   = {0}", classFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Changes = {0}", classFileLineChangeCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Added Changes = {0}", lineChangeCountIncrease));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Global change/edit target Java directory.
        /// </summary>
        private void XEditJava(string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XEditJava");
            XEditJava(diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XEditJava");
        }

        private void XEditJava(DirectoryInfo targetDirectory)
        {
            try
            {
                if (DirectoryExclusionsHelper.AllowDirectory(targetDirectory.FullName))
                {
                    _log.WriteLn();
                    _log.WriteTimedMsg("000", "I", String.Format(@"Global Change Java Directory : ""{0}""", targetDirectory.ToString()));
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
                                    //string fileSpec = System.IO.Path.Combine(targetDirectory.ToString(), targetFileName);
                                    string fileSpec = fi.FullName;
                                    EditJavaFile(fileSpec);
                                    //_log.WriteTimedMsg("000", "I", String.Format(@"{0} - Scanned", sourceFileName));
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
                                    XEditJava(di);
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
                    _log.WriteTimedMsg("000", "I", String.Format(@"XEditJava cancelled"));
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.WriteTimedMsg("000", "I", String.Format(@"XEditJava exception: " + ex.Message));
                return;
            }
        }

        /// <summary>
        /// Edit Java file.
        /// </summary>
        /// <remarks>
        /// Apply global changes to one Java file.
        /// </remarks>
        protected bool EditJavaFile(string fileSpec)
        {
            classFileCount++;
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
                            classFileLineCount++;
                            record = sr.ReadLine();
                            line = record;
                            if (line.Contains("MediaApproved"))
                            {
                                string temp = string.Empty;
                            }
                            //Replace all the old package names with the new package names.
                            foreach (KeyValuePair<string, string> entry in _packageNameMappings)
                            {
                                ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //Replace all the old class names with the new class names.
                            foreach (KeyValuePair<string, string> entry in _classNameMappings)
                            {
                                // Originally added to support Java.
                                ChangeOldClassNameToNewClassName(".", entry, ";", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName("<", entry, ">", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName("<", entry, ",", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, ",", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, ">", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, "<", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName("(", entry, ")", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName("(", entry, " ", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, "(", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, ")", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, " ", ref line, ref changed, ref first, fileSpec, count, ref record);
                                // Added to support Kotlin.
                                ChangeOldClassNameToNewClassName(" ", entry, "?", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(" ", entry, ":", ref line, ref changed, ref first, fileSpec, count, ref record);
                                // Originally added to support Java.
                                ChangeOldClassNameToNewClassName(string.Empty, entry, ".", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(string.Empty, entry, " ", ref line, ref changed, ref first, fileSpec, count, ref record);
                                // Added to support Kotlin.
                                ChangeOldClassNameToNewClassName(" ", entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //Replace all the old class variables with the new class variables.
                            foreach (KeyValuePair<string, string> entry in _classNameMappings)
                            {
                                ChangeOldClassVariableToNewClassVariable(entry, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //Replace all the old exact variable names with the new exact variable names.
                            foreach (KeyValuePair<string, string> entry in _variableNameExacts)
                            {
                                ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //Replace all the old layout names with the new layout names.
                            foreach (KeyValuePair<string, string> entry in _layoutNameMappings)
                            {
                                ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            if (!activityInfoCaptured)
                            {
                                //Capture package name after it ha been changed to the new package name.
                                if (line.StartsWith("package"))
                                {
                                    string package = line.Replace("package", string.Empty);
                                    package = package.Replace(";", string.Empty);
                                    package = package.Trim();
                                    if (package.StartsWith("com.sample.example"))
                                    {
                                        package = package.Replace("com.sample.example", string.Empty);
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

        /// <summary>
        /// Global change/edit target layout directory.
        /// </summary>
        private void EditTargetLayoutDirectory()
        {
            classFileLineChangeCount = lineChangeCount;
            lineChangeCount = 0;
            layoutFileCount = 0;
            layoutFileLineCount = 0;
            layoutFileLineChangeCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Edit Layouts) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_Layouts_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string targetDirectory = TargetLayoutDirectory;
            XEditLayouts(targetDirectory);
            layoutFileLineChangeCount = lineChangeCount;
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Files    = {0}", classFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Lines    = {0}", classFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Class Changes  = {0}", classFileLineChangeCount));
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Layout Files   = {0}", layoutFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Layout Lines   = {0}", layoutFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Layout Changes = {0}", layoutFileLineChangeCount));
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Total Files    = {0}", classFileCount + layoutFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Total Lines    = {0}", classFileLineCount + layoutFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Total Changes  = {0}", classFileLineChangeCount + layoutFileLineChangeCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Global change target layout directory.
        /// </summary>
        private void XEditLayouts(string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XEditLayouts");
            XEditLayouts(diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XEditLayouts");
        }

        private void XEditLayouts(DirectoryInfo targetDirectory)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(targetDirectory.FullName))
            {
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", String.Format(@"Global Change Layout Directory : ""{0}""", targetDirectory.ToString()));
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
                                EditLayoutFile(fileSpec);
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
                                XEditLayouts(di);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XEditLayouts cancelled"));
                return;
            }
        }

        /// <summary>
        /// Edit Layout file.
        /// </summary>
        /// <remarks>
        /// Apply global changes to one Layout file.
        /// </remarks>
        protected bool EditLayoutFile(string fileSpec)
        {
            layoutFileCount++;
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
                            layoutFileLineCount++;
                            record = sr.ReadLine();
                            line = record;
                            //Replace all the old package names with the new package names.
                            foreach (KeyValuePair<string, string> entry in _packageNameMappings)
                            {
                                ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
                            }
                            //Replace all the old class names with the new class names.
                            //Mostly ViewModels but could also be Activities or Fragments for tool contexts.
                            foreach (KeyValuePair<string, string> entry in _classNameMappings)
                            {
                                ChangeOldClassNameToNewClassName(".", entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
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

        private string ChangeOldClassNameToNewClassName(string prefix, KeyValuePair<string, string> entry, string suffix, ref string line, ref bool changed, ref bool first, string fileSpec, long count, ref string record)
        {
            bool hit = false;
            string findClass = prefix + entry.Key + suffix;
            string replacementClass = prefix + entry.Value + suffix;
            if (line.Contains(findClass) && !line.Contains(replacementClass))
            {
                hit = true;
                changed = true;
                lineChangeCount++;
                line = line.Replace(findClass, replacementClass);
                RecordChange(hit, ref first, fileSpec, count, record, line);
                record = line;
            }
            return line;
        }

        private string ChangeOldClassVariableToNewClassVariable(KeyValuePair<string, string> entry, ref string line, ref bool changed, ref bool first, string fileSpec, long count, ref string record)
        {
            bool hit = false;
            string findClass = entry.Key;
            string replacementClass = entry.Value;
            if (line.Contains(replacementClass))
            {
                int pos = line.IndexOf(replacementClass);
                if (pos != -1)
                {
                    bool proceed = false;
                    pos += replacementClass.Length;
                    if (pos < line.Length)
                    {
                        string followedBySpace = line.Substring(pos, 1);
                        if (followedBySpace == SPACE.ToString())
                        {
                            proceed = true;
                        }
                    }
                    if (proceed)
                    {
                        string remainder = line.Substring(pos);
                        remainder = remainder.Trim();
                        string smallWord = string.Empty;
                        string[] wordArray = new string[50];
                        int wordCount = 0;
                        wordCount = WordArray(remainder, ref wordArray, SPECIAL);
                        if (wordCount > 0)
                        {
                            string variableName = wordArray[0];
                            //Replace all the old variable name prefixes with the new variable name prefixes.
                            foreach (KeyValuePair<string, string> prefixEntry in _variableNamePrefixes)
                            {
                                string findPrefix = prefixEntry.Key;
                                string findVariable = variableName;
                                string replacementPrefix = prefixEntry.Value;
                                string replacementVariable = string.Empty;
                                if (variableName.StartsWith(findPrefix))
                                {
                                    replacementVariable = findVariable.Replace(findPrefix, replacementPrefix);
                                    if (!_variableNameExacts.ContainsKey(findVariable))
                                    {
                                        _variableNameExacts.Add(findVariable, replacementVariable);
                                    }
                                    //hit = true;
                                    //changed = true;
                                    //lineChangeCount++;
                                    //line = line.Replace(findPrefix, replacementPrefix);
                                    //RecordChange(hit, ref first, fileSpec, count, record, line);
                                    //record = line;
                                }
                            }
                            //Replace all the old variable name patterns with the new variable name patterns.
                            foreach (KeyValuePair<string, string> patternEntry in _variableNamePatterns)
                            {
                                string findPattern = patternEntry.Key;
                                string findVariable = variableName;
                                string replacementPattern = patternEntry.Value;
                                string replacementVariable = string.Empty;
                                if (variableName.Contains(findPattern))
                                {
                                    replacementVariable = findVariable.Replace(findPattern, replacementPattern);
                                    if (!_variableNameExacts.ContainsKey(findVariable))
                                    {
                                        _variableNameExacts.Add(findVariable, replacementVariable);
                                    }
                                    //hit = true;
                                    //changed = true;
                                    //lineChangeCount++;
                                    //line = line.Replace(findPrefix, replacementPrefix);
                                    //RecordChange(hit, ref first, fileSpec, count, record, line);
                                    //record = line;
                                }
                            }
                        }
                    }
                }
            }
            return line;
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

        protected string BindingClassNameFromLayoutName(string layoutName)
        {
            StringBuilder sb = new StringBuilder();
            bool makeUpper = true;
            for (int pos = 0; pos < layoutName.Length; pos++)
            {
                string letter = layoutName.Substring(pos, 1);
                if (letter == "_")
                {
                    makeUpper = true;
                }
                else
                {
                    if (makeUpper)
                    {
                        sb.Append(letter.ToUpper());
                    }
                    else
                    {
                        sb.Append(letter.ToLower());
                    }
                    makeUpper = false;
                }
            }
            sb.Append("Binding");
            return sb.ToString();
        }

        /// <summary>
        /// Since multiple changes are being made in different places for different "modes", the recording of a hit has been moved into its own method.
        /// </summary>
        protected void RecordFound(bool hit, ref bool first, string fileSpec, long count, string line)
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
                header = "Found   : (" + count.ToString("00000") + ") - " + line;
                _log.WriteLn(header);
            }
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