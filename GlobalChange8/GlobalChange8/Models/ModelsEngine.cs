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
    /// Model generation engine (Models).
    /// </summary>
    /// <remarks>
    /// Generate model classes.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ModelsEngine
    {
        const string SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        const char SPACE = ' ';
        const string DELIMITER_SPACE = @"SPACE";
        const string DELIMITER_SPACES = @"SPACES";
        const string DELIMITER_SPECIAL = @"SPECIAL";
        const string DELIMITER_ANY = @"ANY";
        const string DELIMITER_EMPTY_STRING = @"";

        const string MODEL_CLASS_SUFFIX = "ModelState";
        protected bool _ignoreUntil = false;

        protected string _action = string.Empty;
        protected string _path = string.Empty;
        protected string _mode = string.Empty;
        protected Logger _log = null;

        public string SourceModelsDirectory { get; set; }
        public string TargetModelsDirectory { get; set; }

        protected SortedDictionary<string, string> _apiToModelClassNameMapping;

        private int classFileCount = 0;
        private int classFileLineCount = 0;
        private int classFileLineChangeCount = 0;
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
        public ModelsEngine()
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
            foreach (XElement directory in doc.Descendants("Directories").Elements())
            {
                string directoryFieldName = directory.Name.LocalName;
                switch (directoryFieldName)
                {
                    case "SourceModels":
                        SourceModelsDirectory = (string)directory;
                        break;
                    case "TargetModels":
                        TargetModelsDirectory = (string)directory;
                        break;
                }
            }
            _apiToModelClassNameMapping = new SortedDictionary<string, string>();
        }

        /// <summary>
        /// Run the selected clone (Currently equal to the selected RunMode).
        /// </summary>
        public void Run()
        {
            CloneApiDirectory();
            EditModelsDirectory();
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

        private void CloneApiDirectory()
        {
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Clone API Models) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Clone_API_Models_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string sourceDirectory = SourceModelsDirectory;
            string targetDirectory = TargetModelsDirectory;
            XCopyModels(sourceDirectory, targetDirectory);
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Copy files from a complete directory tree to another directory.
        /// </summary>
        private void XCopyModels(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XCopyModels");
            XCopyModels(diSource, diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XCopyModels");
        }

        private void XCopyModels(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
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
                            string sourceFileName = fi.Name;
                            string sourceClassName = fi.Name;
                            string ext = fi.Extension;
                            int pos = sourceClassName.LastIndexOf(ext);
                            if (pos != -1)
                            {
                                sourceClassName = sourceClassName.Substring(0, pos);
                            }
                            string targetClassName = sourceClassName + MODEL_CLASS_SUFFIX;
                            string targetFileName = targetClassName + ext;
                            _apiToModelClassNameMapping.Add(sourceClassName, targetClassName);
                            try
                            {
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
                                XCopyModels(di, targetDirectoryInfo);
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
                _log.WriteTimedMsg("000", "I", String.Format(@"XCopyModels cancelled"));
                return;
            }
        }

        /// <summary>
        /// Global change/edit target models directory.
        /// </summary>
        private void EditModelsDirectory()
        {
            classFileCount = 0;
            classFileLineCount = 0;
            classFileLineChangeCount = 0;
            lineChangeCount = 0;
            char cSeparator = System.IO.Path.DirectorySeparatorChar;
            _log = new Logger();
            _log.Prefix = "GCE";
            _log.Title = "Global Change/Clone Engine (Edit Models) " + Administrator.ProfileManager.ApplicationProfile.Version;
            FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
            _log.Begin(String.Format("{0}Global_Change_Edit_Models_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
            string targetDirectory = TargetModelsDirectory;
            XEditModels(targetDirectory);
            classFileLineChangeCount = lineChangeCount;
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", String.Format(@"Model Files   = {0}", classFileCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Model Lines   = {0}", classFileLineCount));
            _log.WriteTimedMsg("000", "I", String.Format(@"Model Changes = {0}", classFileLineChangeCount));
            _log.Outcome();
            _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
        }

        /// <summary>
        /// Global change/edit target models directory.
        /// </summary>
        private void XEditModels(string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "Begin XEditModels");
            XEditModels(diTarget);
            _log.WriteLn();
            _log.WriteTimedMsg("000", "I", "End XEditModels");
        }

        private void XEditModels(DirectoryInfo targetDirectory)
        {
            if (DirectoryExclusionsHelper.AllowDirectory(targetDirectory.FullName))
            {
                _log.WriteLn();
                _log.WriteTimedMsg("000", "I", String.Format(@"Global Change Models Directory : ""{0}""", targetDirectory.ToString()));
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
                                EditModelsFile(fileSpec);
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
                                XEditModels(di);
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
        /// Edit Models file.
        /// </summary>
        /// <remarks>
        /// Apply global changes to one model file.
        /// </remarks>
        protected bool EditModelsFile(string fileSpec)
        {
            classFileCount++;
            _ignoreUntil = true;
            StringBuilder contents = new StringBuilder();
            List<string> memberVariableLines = new List<string>();
            bool memberVariableOnNextLine = false;
            bool memberVariablesOpened = false;
            bool memberVariablesClosed = false;
            string line = string.Empty;
            bool first = true;
            bool hit = false;
            bool changed = false;
            FileInfo fileInfo = null;
            string record = string.Empty;
            long count = 0;
            string currentClassName = System.IO.Path.GetFileNameWithoutExtension(fileSpec);
            try
            {
                if (File.Exists(fileSpec))
                {
                    string modelClassName = System.IO.Path.GetFileNameWithoutExtension(fileSpec);
                    fileInfo = R.GetFileInfo(fileSpec);
                    StreamReader sr = new StreamReader(fileSpec);
                    if (sr.Peek() >= 0)
                    {
                        memberVariableOnNextLine = false;
                        memberVariablesOpened = false;
                        memberVariablesClosed = false;
                        do
                        {
                            count++;
                            classFileLineCount++;
                            record = sr.ReadLine();
                            line = record;
                            if (memberVariableOnNextLine)
                            {
                                memberVariableLines.Add(line);
                                line = line.Replace(" Boolean ", " boolean ");
                                line = line.Replace(" Integer ", " int ");
                                line = line.Replace(" Int ", " int ");
                                line = line.Replace(" Long ", " long ");
                                line = line.Replace(" = null", string.Empty);
                                memberVariableOnNextLine = false;
                            }
                            if (memberVariablesOpened && !memberVariablesClosed)
                            {
                                if (line.Trim().Length == 0)
                                {
                                    memberVariablesClosed = true;
                                    Generator generator = null;
                                    EntityMetaBlock block = null;
                                    try
                                    {
                                        generator = new Generator(modelClassName, memberVariableLines, _apiToModelClassNameMapping);
                                        block = generator.ImportMemberVariables();
                                    }
                                    catch (Exception e1)
                                    {
                                    }
                                    try
                                    {
                                        List<string> defaultConstructor = generator.GenerateDefaultConstructor(block);
                                        foreach (string ln in defaultConstructor)
                                        {
                                            contents.AppendLine(ln);
                                        }
                                    }
                                    catch (Exception e2)
                                    {
                                    }
                                    try
                                    {
                                        List<string> apiObjectConstructor = generator.GenerateApiObjectConstructor(block);
                                        foreach (string ln in apiObjectConstructor)
                                        {
                                            contents.AppendLine(ln);
                                        }
                                    }
                                    catch (Exception e3)
                                    {
                                    }
                                    try
                                    {
                                        List<string> parcelConstructor = generator.GenerateParameterisedParcelConstructor(block);
                                        foreach (string ln in parcelConstructor)
                                        {
                                            contents.AppendLine(ln);
                                        }
                                    }
                                    catch (Exception e4)
                                    {
                                    }
                                }
                            }
                            if (line.Contains("@Expose"))
                            {
                                memberVariablesOpened = true;
                                if (!memberVariablesClosed)
                                {
                                    memberVariableOnNextLine = true;
                                }
                                continue;
                            }
                            else if (line.Contains("Expose;"))
                            {
                                continue;
                            }
                            else if (line.Contains("@SerializedName"))
                            {
                                continue;
                            }
                            else if (line.Contains("SerializedName;"))
                            {
                                continue;
                            }
                            else if (line.Contains("package com.sample.example"))
                            {
                                continue;
                            }
                            if (_ignoreUntil)
                            {
                                if (line.Trim().Length == 0)
                                {
                                    continue;
                                }
                                else if (line.Trim().StartsWith("/*"))
                                {
                                    continue;
                                }
                                else if (line.Trim().StartsWith("*"))
                                {
                                    continue;
                                }
                                else if (line.Trim().StartsWith("*/"))
                                {
                                    continue;
                                }
                            }
                            if (line.Contains("public class"))
                            {
                                // Ignore all lines until the package is found.
                                _ignoreUntil = false;
                                // Insert package line.
                                string packageLine = "package com.example.kotlinmodelsexperiment.common.models.data;";
                                ConditionalAppendLine(ref contents, packageLine);
                                ConditionalAppendLine(ref contents, string.Empty);
                                // Insert comment block at start of class.
                                string[] parts = line.Split(' ');
                                if (parts.Length > 2)
                                {
                                    string className = parts[2];
                                    string comment1 = "/**";
                                    string comment2 = String.Format(" * {0} model state.", CommentFromClassName(className));
                                    string comment3 = " */";
                                    ConditionalAppendLine(ref contents, comment1);
                                    ConditionalAppendLine(ref contents, comment2);
                                    ConditionalAppendLine(ref contents, comment3);
                                    hit = true;
                                    changed = true;
                                    RecordInsert(hit, ref first, fileSpec, count, record, comment1);
                                    RecordInsert(hit, ref first, fileSpec, count, record, comment2);
                                    RecordInsert(hit, ref first, fileSpec, count, record, comment3);
                                }
                                ConditionalAppendLine(ref contents, "@Parcel");
                                //Replace all the API class names with the Model class names.
                                foreach (KeyValuePair<string, string> entry in _apiToModelClassNameMapping)
                                {
                                    //ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
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
                                ConditionalAppendLine(ref contents, line);
                                continue;
                            }
                            //Replace all the API class names with the Model class names.
                            foreach (KeyValuePair<string, string> entry in _apiToModelClassNameMapping)
                            {
                                //ChangeOldClassNameToNewClassName(string.Empty, entry, string.Empty, ref line, ref changed, ref first, fileSpec, count, ref record);
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

        protected void ConditionalAppendLine(ref StringBuilder contents, string line)
        {
            if (!_ignoreUntil)
            {
                contents.AppendLine(line);
            }
        }

        protected string CommentFromClassName(string className)
        {
            StringBuilder sb = new StringBuilder();
            for (int pos = 0; pos < className.Length; pos++)
            {
                string letter = className.Substring(pos, 1);
                if (letter == letter.ToUpper())
                {
                    sb.Append(" ");
                    sb.Append(letter.ToLower());
                }
                else
                {
                    sb.Append(letter);
                }
            }
            string output = sb.ToString().Trim();
            string result = output.Substring(0, 1).ToUpper() + output.Substring(1);
            return result;
        }

        /// <summary>
        /// Since multiple changes are being made in different places for different "modes", the recording of a hit has been moved into its own method.
        /// </summary>
        protected void RecordInsert(bool hit, ref bool first, string fileSpec, long count, string record, string line)
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
                header = "Insert  : (" + count.ToString("00000") + ") - " + line;
                _log.WriteLn(header);
            }
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