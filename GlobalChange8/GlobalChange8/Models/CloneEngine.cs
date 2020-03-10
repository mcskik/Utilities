using GlobalChange8.DataLayer.Profile;
using Log8;
using ProfileData.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using R = Routines8.Routines;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Global change engine (Clone).
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and apply all the custom changes required for mode = "Clone".
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class CloneEngine
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

        public string CloneJava { get; set; }
        public string EditJava { get; set; }
        public string CloneJavaAndroidTests { get; set; }
        public string EditJavaAndroidTests { get; set; }
        public string CloneJavaUnitTests { get; set; }
        public string EditJavaUnitTests { get; set; }
        public string CloneLayout { get; set; }
        public string EditLayout { get; set; }
        public string SourceJavaDirectory { get; set; }
        public string TargetJavaDirectory { get; set; }
        public string SourceAndroidTestsJavaDirectory { get; set; }
        public string TargetAndroidTestsJavaDirectory { get; set; }
        public string SourceUnitTestsJavaDirectory { get; set; }
        public string TargetUnitTestsJavaDirectory { get; set; }
        public string SourceLayoutDirectory { get; set; }
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
        public CloneEngine()
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
                    case "CloneJava":
                        CloneJava = (string)flag;
                        break;
                    case "EditJava":
                        EditJava = (string)flag;
                        break;
                    case "CloneJavaAndroidTests":
                        CloneJavaAndroidTests = (string)flag;
                        break;
                    case "EditJavaAndroidTests":
                        EditJavaAndroidTests = (string)flag;
                        break;
                    case "CloneJavaUnitTests":
                        CloneJavaUnitTests = (string)flag;
                        break;
                    case "EditJavaUnitTests":
                        EditJavaUnitTests = (string)flag;
                        break;
                    case "CloneLayout":
                        CloneLayout = (string)flag;
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
                    case "SourceJava":
                        SourceJavaDirectory = (string)directory;
                        break;
                    case "TargetJava":
                        TargetJavaDirectory = (string)directory;
                        break;
                    case "SourceAndroidTestsJava":
                        SourceAndroidTestsJavaDirectory = (string)directory;
                        break;
                    case "TargetAndroidTestsJava":
                        TargetAndroidTestsJavaDirectory = (string)directory;
                        break;
                    case "SourceUnitTestsJava":
                        SourceUnitTestsJavaDirectory = (string)directory;
                        break;
                    case "TargetUnitTestsJava":
                        TargetUnitTestsJavaDirectory = (string)directory;
                        break;
                    case "SourceLayout":
                        SourceLayoutDirectory = (string)directory;
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
            if (IsAffirmative(CloneJava))
            {
                CloneJavaDirectory();
            }
            if (IsAffirmative(CloneJavaAndroidTests))
            {
                CloneJavaAndroidTestsDirectory();
            }
            if (IsAffirmative(CloneJavaUnitTests))
            {
                CloneJavaUnitTestsDirectory();
            }
            ScanLayoutDirectory();
            if (IsAffirmative(CloneLayout))
            {
                CloneLayoutDirectory();
            }
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
            if (IsAffirmative(CloneJava) || IsAffirmative(EditJava))
            {
                GenerateAndroidManifestEntriesForNewActivities();
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

        private void CloneJavaDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Clone Java) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_Java_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceJavaDirectory;
            string targetDirectory = TargetJavaDirectory;
            XCopyJava(sourceDirectory, targetDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        private void CloneJavaAndroidTestsDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Clone Java Android Tests) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_Java_AndroidTests_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceAndroidTestsJavaDirectory;
            string targetDirectory = TargetAndroidTestsJavaDirectory;
            XCopyJava(sourceDirectory, targetDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        private void CloneJavaUnitTestsDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Clone Java Unit Tests) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_Java_UnitTests_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceUnitTestsJavaDirectory;
            string targetDirectory = TargetUnitTestsJavaDirectory;
            XCopyJava(sourceDirectory, targetDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Copy files from a complete directory tree to another directory.
        /// </summary>
        private void XCopyJava(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XCopyJava");
            XCopyJava(diSource, diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XCopyJava");
        }

        private void XCopyJava(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                if (!Directory.Exists(targetDirectory.FullName))
                {
                    Directory.CreateDirectory(targetDirectory.FullName);
                }
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", String.Format(@"Copy From : ""{0}""", sourceDirectory.ToString()));
                _log.WriteTimedMsg("000", "I", String.Format(@"Copy To   : ""{0}""", targetDirectory.ToString()));
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (true)
                    {
                        if (_action != "Cancel")
                        {
                            string sourceFileName = string.Empty;
                            string targetFileName = string.Empty;
                            try
                            {
                                // Most classes have to start with the prefix.
                                foreach (KeyValuePair<string, string> entry in _classNamePrefixes)
                                {
                                    string findPrefix = entry.Key;
                                    string replacementPrefix = entry.Value;
                                    string findClass = System.IO.Path.GetFileNameWithoutExtension(fi.Name);
                                    string replacementClass = string.Empty;
                                    if (findClass.StartsWith(findPrefix))
                                    {
                                        replacementClass = findClass.Replace(findPrefix, replacementPrefix);
                                        if (!_classNameMappings.ContainsKey(findClass))
                                        {
                                            _classNameMappings.Add(findClass, replacementClass);
                                        }
                                    }
                                }
                                // A few selected classes can contain the pattern.
                                foreach (KeyValuePair<string, string> entry in _classNamePatterns)
                                {
                                    string findPattern = entry.Key;
                                    string replacementPattern = entry.Value;
                                    string findClass = System.IO.Path.GetFileNameWithoutExtension(fi.Name);
                                    string replacementClass = string.Empty;
                                    if (findClass.Contains(findPattern))
                                    {
                                        replacementClass = findClass.Replace(findPattern, replacementPattern);
                                        if (!_classNameMappings.ContainsKey(findClass))
                                        {
                                            _classNameMappings.Add(findClass, replacementClass);
                                        }
                                    }
                                }
                                sourceFileName = fi.Name;
                                targetFileName = fi.Name;
                                foreach (KeyValuePair<string, string> entry in _classNameMappings)
                                {
                                    string findClass = entry.Key;
                                    string replacementClass = entry.Value;
                                    if (sourceFileName.StartsWith(findClass))
                                    {
                                        targetFileName = sourceFileName.Replace(findClass, replacementClass);
                                        if (targetFileName != sourceFileName)
                                        {
                                            break;
                                        }
                                    }
                                }
                                string targetFile = System.IO.Path.Combine(targetDirectory.ToString(), targetFileName);
                                FileInfo targetFileInfo = new FileInfo(targetFile);
                                if (true)
                                {
                                    try
                                    {
                                        if (targetFileInfo.Exists)
                                        {
                                            targetFileInfo.IsReadOnly = false;
                                        }
                                        fi.CopyTo(System.IO.Path.Combine(targetDirectory.ToString(), targetFileName), true);
                                        targetFileInfo = new FileInfo(targetFile);
                                        if (targetFileInfo.Exists)
                                        {
                                            targetFileInfo.IsReadOnly = false;
                                        }
                                        _log.WriteTimedMsg("000", "I", String.Format(@"{0} - Copied", targetFileName));
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.WriteLn();
                                        _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Error : {1}{2}", targetFileName, Environment.NewLine, ex.Message));
                                        _log.WriteLn();
                                    }
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
                }
                foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
                {
                    if (_action != "Cancel")
                    {
                        try
                        {
                            if (DirectoryExclusionsHelper.AllowDirectory(di.FullName))
                            {
                                DirectoryInfo targetDirectoryInfo = targetDirectory.CreateSubdirectory(di.Name);
                                XCopyJava(di, targetDirectoryInfo);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XCopyJava cancelled"));
                return;
            }
        }

        /// <summary>
        /// Scan all layout directory entries and capture view model class names, layout file names, and deduce data binding class names.
        /// </summary>
        private void ScanLayoutDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Scan Layouts) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Scan_Layouts_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceLayoutDirectory;
            XScanLayouts(sourceDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Scan layout files from a complete directory tree.
        /// </summary>
        private void XScanLayouts(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XScanLayouts");
            XScanLayouts(diSource);
            _log.WriteLn();
            _log.WriteLn("Class Name Mapping:");
            foreach (KeyValuePair<string, string> entry in _classNameMappings)
            {
                _log.WriteLn();
                _log.WriteLn(entry.Key);
                _log.WriteLn(entry.Value);
            }
            _log.WriteLn();
            _log.WriteLn("Layout Name Mapping:");
            foreach (KeyValuePair<string, string> entry in _layoutNameMappings)
            {
                _log.WriteLn();
                _log.WriteLn(entry.Key);
                _log.WriteLn(entry.Value);
            }
            _log.WriteLn();
            _log.WriteLn("Layout ViewModel Mapping:");
            foreach (KeyValuePair<string, string> entry in _cloneCopyLayoutViewModelMapping)
            {
                _log.WriteLn();
                _log.WriteLn(entry.Key);
                _log.WriteLn(BindingClassNameFromLayoutName(entry.Key));
                _log.WriteLn(entry.Value);
            }
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XScanLayouts");
        }

        private void XScanLayouts(DirectoryInfo sourceDirectory)
        {
            DirectoryInfo targetDirectory = new DirectoryInfo(TargetLayoutDirectory);
            if (!Directory.Exists(targetDirectory.FullName))
            {
                Directory.CreateDirectory(targetDirectory.FullName);
            }
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", String.Format(@"Scan Layout Directory : ""{0}""", sourceDirectory.ToString()));
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileName = string.Empty;
                        try
                        {
                            foreach (KeyValuePair<string, string> entry in _layoutNamePatterns)
                            {
                                string findPattern = entry.Key;
                                string replacementPattern = entry.Value;
                                string findLayout = System.IO.Path.GetFileNameWithoutExtension(fi.Name);
                                string replacementLayout = string.Empty;
                                if (findLayout.Contains(findPattern))
                                {
                                    replacementLayout = findLayout.Replace(findPattern, replacementPattern);
                                    if (!_layoutNameMappings.ContainsKey(findLayout))
                                    {
                                        _layoutNameMappings.Add(findLayout, replacementLayout);
                                    }
                                    string findBinding = BindingClassNameFromLayoutName(findLayout);
                                    string replacementBinding = BindingClassNameFromLayoutName(replacementLayout);
                                    if (!_classNameMappings.ContainsKey(findBinding))
                                    {
                                        _classNameMappings.Add(findBinding, replacementBinding);
                                    }
                                }
                            }
                            sourceFileName = fi.Name;
                            string fileSpec = System.IO.Path.Combine(sourceDirectory.ToString(), sourceFileName);
                            ScanLayoutFile(fileSpec);
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
                                XScanLayouts(di);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XScanLayouts cancelled"));
                return;
            }
        }

        /// <summary>
        /// Scan layout file.
        /// </summary>
        /// <remarks>
        /// Search layout files for view model names and tools context Activities and Fragments.
        /// </remarks>
        protected bool ScanLayoutFile(string fileSpec)
        {
            StringBuilder contents = new StringBuilder();
            string line = string.Empty;
            bool first = true;
            bool hit = false;
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
                            record = sr.ReadLine();
                            line = record;
                            if (fileSpec.Contains(@"layout") && fileSpec.Contains(@".xml"))
                            {
                                if (line.EndsWith(@"Activity"""))
                                {
                                    RecordFound(hit, ref first, fileSpec, count, line);
                                }
                                else if (line.EndsWith(@"Fragment"""))
                                {
                                    RecordFound(hit, ref first, fileSpec, count, line);
                                }
                                else if (line.EndsWith(@"ViewModel"""))
                                {
                                    // Extract view model class name.
                                    line = line.Trim();
                                    string[] parts = line.Split('.');
                                    if (parts.Length > 0)
                                    {
                                        hit = true;
                                        string viewModel = parts[parts.Length - 1];
                                        if (viewModel.EndsWith(@""""))
                                        {
                                            viewModel = viewModel.Substring(0, viewModel.Length - 1);
                                        }
                                        string layoutFileName = System.IO.Path.GetFileName(fileSpec);
                                        string layoutName = System.IO.Path.GetFileNameWithoutExtension(fileSpec);
                                        string bindingClassName = BindingClassNameFromLayoutName(layoutName);
                                        _cloneCopyLayoutViewModelMapping.Add(layoutName, viewModel);
                                    }
                                    RecordFound(hit, ref first, fileSpec, count, line);
                                }
                            }
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
        /// Clone all layout directory entries.
        /// </summary>
        private void CloneLayoutDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Clone Layouts) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_Layouts_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceLayoutDirectory;
            XCopyLauouts(sourceDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Clone layout files from a complete directory tree.
        /// </summary>
        private void XCopyLauouts(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XCopyLayouts");
            XCopyLayouts(diSource);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XCopyLayouts");
        }

        private void XCopyLayouts(DirectoryInfo sourceDirectory)
        {
            DirectoryInfo targetDirectory = new DirectoryInfo(TargetLayoutDirectory);
            if (!Directory.Exists(targetDirectory.FullName))
            {
                Directory.CreateDirectory(targetDirectory.FullName);
            }
            if (DirectoryExclusionsHelper.AllowDirectory(sourceDirectory.FullName))
            {
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", String.Format(@"Copy Layout Directory : ""{0}""", sourceDirectory.ToString()));
                foreach (FileInfo fi in sourceDirectory.GetFiles())
                {
                    if (_action != "Cancel")
                    {
                        string sourceFileName = string.Empty;
                        string targetFileName = string.Empty;
                        try
                        {
                            sourceFileName = fi.Name;
                            targetFileName = fi.Name;
                            foreach (KeyValuePair<string, string> entry in _layoutNameMappings)
                            {
                                string findLayout = entry.Key;
                                string replacementLayout = entry.Value;
                                if (sourceFileName.Contains(findLayout))
                                {
                                    targetFileName = sourceFileName.Replace(findLayout, replacementLayout);
                                    if (targetFileName != sourceFileName)
                                    {
                                        string targetFile = System.IO.Path.Combine(targetDirectory.ToString(), targetFileName);
                                        try
                                        {
                                            FileInfo targetFileInfo = new FileInfo(targetFile);
                                            if (targetFileInfo.Exists)
                                            {
                                                targetFileInfo.IsReadOnly = false;
                                            }
                                            fi.CopyTo(System.IO.Path.Combine(targetDirectory.ToString(), targetFileName), true);
                                            targetFileInfo = new FileInfo(targetFile);
                                            if (targetFileInfo.Exists)
                                            {
                                                targetFileInfo.IsReadOnly = false;
                                            }
                                            _log.WriteTimedMsg("000", "I", String.Format(@"{0} - Layout Copied", targetFileName));
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.WriteLn();
                                            _log.WriteTimedMsg("000", "E", String.Format(@"{0} - Layout Copy Error : {1}{2}", targetFileName, Environment.NewLine, ex.Message));
                                            _log.WriteLn();
                                        }
                                        break;
                                    }
                                }
                            }
                            //_log.WriteTimedMsg("000", "I", String.Format(@"{0} - Scanned", sourceFileName));
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
                                XCopyLayouts(di);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XCopyLayouts cancelled"));
                return;
            }
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
                                ChangeOldClassNameToNewClassName(string.Empty, entry, ".", ref line, ref changed, ref first, fileSpec, count, ref record);
                                ChangeOldClassNameToNewClassName(string.Empty, entry, " ", ref line, ref changed, ref first, fileSpec, count, ref record);
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
                                //string fileSpec = System.IO.Path.Combine(targetDirectory.ToString(), targetFileName);
                                string fileSpec = fi.FullName;
                                //Only change media layout files.
                                foreach (KeyValuePair<string, string> entry in _layoutNameMappings)
                                {
                                    if (fileSpec.Contains(entry.Value))
                                    {
                                        EditLayoutFile(fileSpec);
                                        break;
                                    }
                                }
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

        private void GenerateAndroidManifestEntriesForNewActivities()
        {
            int activityCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Generate Manifest Entries) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Generate_Manifest_Entries_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            _log.WriteLn();
            foreach (KeyValuePair<string, string> entry in _newActivitiesMapping)
            {
                string className = entry.Value;
                if (className.EndsWith("Activity"))
                {
                    activityCount++;
                    string package = entry.Key;
                    _log.WriteLn(@"        <activity");
                    _log.WriteLn(String.Format(@"            android:name=""{0}""", package));
                    _log.WriteLn(@"            android:screenOrientation=""portrait""");
                    _log.WriteLn(@"            android:theme=""@style/AppThemeNoActionBar""");
                    _log.WriteLn(@"            />");
                }
            }
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Activity Count = {0}", activityCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
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