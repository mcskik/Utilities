using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Log8;
using ProfileData.DataLayer.Profile;
using GlobalChange8.DataLayer.Profile;
using R = Routines8.Routines;
using System.Text;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Directory search object (Kotlin).
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and report all hits with surrounding context
    /// information.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class SearchKotlinEngine
    {
        #region Events.
        public event EventDelegate EventBeginProgress;
        public event EventDelegate EventUpdateProgress;
        public event EventDelegate EventEndOfProgress;
        public event EventDelegate EventCriteriaPass;

        /// <summary>
        /// Trigger begin progress event.
        /// </summary>
        /// <param name="pnMaximum">Estimated or actual maximum for progress bar.</param>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        protected void SignalBeginProgress(int pnMaximum, string psMethod, string psMessage)
        {
            if (EventBeginProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(pnMaximum, psMethod, psMessage);
                EventBeginProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger update progress event.
        /// </summary>
        /// <param name="pnIncrement">How much to increment progress bar by.</param>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        protected void SignalUpdateProgress(int pnIncrement, string psMethod, string psMessage)
        {
            if (EventUpdateProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(pnIncrement, psMethod, psMessage);
                EventUpdateProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger end of progress event.
        /// </summary>
        /// <param name="psMethod">Current method to which this applies.</param>
        /// <param name="psMessage">Message to display.</param>
        protected void SignalEndOfProgress(string psMethod, string psMessage)
        {
            if (EventEndOfProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(0, psMethod, psMessage);
                EventEndOfProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger criteria pass event.
        /// </summary>
        /// <param name="psContext">Found data in context to display.</param>
        protected void SignalCriteriaPass(string psContext)
        {
            if (EventCriteriaPass != null)
            {
                EventParameters oEventParameters = new EventParameters(0, string.Empty, psContext);
                EventCriteriaPass(this, oEventParameters);
            }
        }
        #endregion

        #region Member variables.
        #region Property members.
        protected string _action = string.Empty;
        protected string _path = string.Empty;
        protected string _prevPath = string.Empty;
        protected bool _pathChanged = false;
        protected string _filePattern = string.Empty;
        protected string _searchCriteria = string.Empty;
        protected string _find = string.Empty;
        protected string _replacement = string.Empty;
        protected string _mode = string.Empty;
        protected StringCollection _extensions;
        protected StringCollection _options;
        protected StringCollection _directoryExclusions;
        protected bool _regex = false;
        protected bool _allTypes = false;
        protected bool _ignoreUntil = false;
        protected bool _bufferUntil = false;
        protected bool _getReady = false;
        protected bool _headerCommentDone = false;
        #endregion
        #region Other members.
        protected Dir _dir;
        protected List<DirectoryEntry> _directoryListing;
        protected long _dirFilesEstimate = 0;
        protected Scanner _scanner = null;
        protected Logger _log = null;
        protected string _sourceCode = string.Empty;
        protected List<int> _endPositions = new List<int>();
        protected Dictionary<string, string> _apiClassNameMapping;
        protected List<string> _buffer = new List<string>();
        #endregion
        #endregion

        #region Properties.
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
                if (_dir != null)
                {
                    _dir.Action = _action;
                }
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
                if (_path != _prevPath)
                {
                    _pathChanged = true;
                    _prevPath = _path;
                }
                else
                {
                    _pathChanged = false;
                }
            }
        }

        /// <summary>
        /// File pattern.
        /// </summary>
        public string FilePattern
        {
            get
            {
                _filePattern = _filePattern.Replace("%", "*");
                return _filePattern;
            }
            set
            {
                _filePattern = value;
                _filePattern = _filePattern.Replace("*", "%");
            }
        }

        /// <summary>
        /// Search criteria.
        /// </summary>
        public string SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
            }
        }

        /// <summary>
        /// Find.
        /// </summary>
        public string Find
        {
            get
            {
                return _find;
            }
            set
            {
                _find = value;
            }
        }
        /// <summary>
        /// Replacement.
        /// </summary>
        public string Replacement
        {
            get
            {
                return _replacement;
            }
            set
            {
                _replacement = value;
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
        /// Extensions.
        /// </summary>
        public StringCollection Extensions
        {
            get
            {
                return _extensions;
            }
            set
            {
                _extensions = value;
            }
        }

        /// <summary>
        /// Options.
        /// </summary>
        public StringCollection Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
            }
        }

        /// <summary>
        /// Directory exclusions.
        /// </summary>
        public StringCollection DirectoryExclusions
        {
            get
            {
                return _directoryExclusions;
            }
            set
            {
                _directoryExclusions = value;
            }
        }

        /// <summary>
        /// Regex.
        /// </summary>
        public bool Regex
        {
            get
            {
                return _regex;
            }
            set
            {
                _regex = value;
            }
        }

        /// <summary>
        /// All types.
        /// </summary>
        public bool AllTypes
        {
            get
            {
                return _allTypes;
            }
            set
            {
                _allTypes = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SearchKotlinEngine()
        {
            _apiClassNameMapping = new Dictionary<string, string>();
            //TODO: Add any mapping to change class names every time.
            Load();
        }
        #endregion

        #region Event Handlers.
        /// <summary>
        /// Start directory scan progress indicator.
        /// </summary>
        void _dir_EventBeginProgress(object poSender, EventParameters2 poEventArgs)
        {
            SignalBeginProgress((int)_dirFilesEstimate, "DirList", "Scanning directories ...");
        }

        /// <summary>
        /// Update directory scan progress indicator.
        /// </summary>
        void _dir_EventUpdateProgress(object poSender, EventParameters2 poEventArgs)
        {
            SignalUpdateProgress(1, "DirList", "Scanning directories ...");
        }

        /// <summary>
        /// End directory scan progress indicator.
        /// </summary>
        void _dir_EventEndOfProgress(object poSender, EventParameters2 poEventArgs)
        {
            string message = string.Empty;
            if (_action == "Cancel")
            {
                message = "Directory scan cancelled.";
            }
            else
            {
                message = "Directory scan complete.";
            }
            SignalEndOfProgress("DirList", message);
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Search.
        /// </summary>
        public void Search()
        {
            _scanner = new Scanner(_searchCriteria);
            Save();
            if (_pathChanged)
            {
                _dir = new Dir();
                _dir.Action = _action;
                _dir.EventBeginProgress += new GlobalChange8.Models.EventDelegate(_dir_EventBeginProgress);
                _dir.EventUpdateProgress += new GlobalChange8.Models.EventDelegate(_dir_EventUpdateProgress);
                _dir.EventEndOfProgress += new GlobalChange8.Models.EventDelegate(_dir_EventEndOfProgress);
                _directoryListing = _dir.DirList(_path, ref _dirFilesEstimate);
            }
            Process();
            Save();
        }
        #endregion

        #region protected members.
        /// <summary>
        /// Load system and working profile variables.
        /// </summary>
        public void Load()
        {
            _dir = new Dir();
            _path = Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath;
            _filePattern = Administrator.ProfileManager.UserSettings.SelectedItem.FilePattern;
            _searchCriteria = Administrator.ProfileManager.UserSettings.SelectedItem.Criteria;
            _find = Administrator.ProfileManager.UserSettings.SelectedItem.Find;
            _replacement = Administrator.ProfileManager.UserSettings.SelectedItem.Replacement;
            _mode = Administrator.ProfileManager.UserSettings.SelectedItem.Mode;
            _extensions = Administrator.ProfileManager.UserSettings.SelectedItem.Extensions;
            _options = Administrator.ProfileManager.UserSettings.SelectedItem.Options;
            _directoryExclusions = Administrator.ProfileManager.UserSettings.SelectedItem.DirectoryExclusions;
            _dirFilesEstimate = Administrator.ProfileManager.UserSettings.SelectedItem.DirFilesEstimate;
            _regex = Administrator.ProfileManager.UserSettings.SelectedItem.RegexCriteria;
            _allTypes = Administrator.ProfileManager.UserSettings.SelectedItem.AllTypes;
            _scanner = new Scanner(_searchCriteria);
        }

        /// <summary>
        /// Save working profile variables.
        /// </summary>
        public void Save()
        {
            Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath = _path;
            Administrator.ProfileManager.UserSettings.SelectedItem.FilePattern = _filePattern;
            Administrator.ProfileManager.UserSettings.SelectedItem.Criteria = _searchCriteria;
            Administrator.ProfileManager.UserSettings.SelectedItem.Find = _find;
            Administrator.ProfileManager.UserSettings.SelectedItem.Replacement = _replacement;
            Administrator.ProfileManager.UserSettings.SelectedItem.Mode = _mode;
            Administrator.ProfileManager.UserSettings.SelectedItem.Extensions = _extensions;
            Administrator.ProfileManager.UserSettings.SelectedItem.Options = _options;
            Administrator.ProfileManager.UserSettings.SelectedItem.DirectoryExclusions = _directoryExclusions;
            Administrator.ProfileManager.UserSettings.SelectedItem.DirFilesEstimate = _dirFilesEstimate;
            Administrator.ProfileManager.UserSettings.SelectedItem.RegexCriteria = _regex;
            Administrator.ProfileManager.UserSettings.SelectedItem.AllTypes = _allTypes;
            Administrator.ProfileManager.UserSettings.Save();
            _scanner.SearchCriteriaDocument.Save(Administrator.ProfileManager.SystemProfile.CurrentCriteria);
        }

        /// <summary>
        /// Process all directory entries and report any lines of text which meet
        /// the search criteria.
        /// </summary>
        protected void Process()
        {
            var allFiles = new List<DirectoryEntry>();
            foreach (var directoryEntry in _directoryListing)
            {
                bool exclude = false;
                foreach (var exclusion in _directoryExclusions)
                {
                    if (directoryEntry.StdDir.Contains(exclusion))
                    {
                        exclude = true;
                    }
                    else if (directoryEntry.StdHlq.Contains(exclusion))
                    {
                        exclude = true;
                    }
                }
                if (!exclude)
                {
                    allFiles.Add(directoryEntry);
                }
            }
            var someFiles = new List<DirectoryEntry>();
            if (_allTypes)
            {
                someFiles = (from dl in allFiles
                             where dl.StdType != "dir"
                             select dl).ToList();
            }
            else
            {
                List<string> fileTypes = new List<string>();
                foreach (string sExtension in _extensions)
                {
                    fileTypes.Add(sExtension);
                }
                someFiles = (from dl in allFiles
                             where dl.StdType != "dir"
                             where fileTypes.Contains(dl.StdType)
                             select dl).ToList();
            }
            var selectedFiles = new List<DirectoryEntry>();
            if (_filePattern.Trim().Length == 0)
            {
                selectedFiles = (from dl in someFiles
                                 orderby dl.StdHlq, dl.StdDir, dl.StdFile, dl.StdType
                                 select dl).ToList();
            }
            else
            {
                selectedFiles = (from dl in someFiles
                                 where Like(dl.StdFile, _filePattern)
                                 orderby dl.StdHlq, dl.StdDir, dl.StdFile, dl.StdType
                                 select dl).ToList();
            }
            try
            {
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count > 0)
                    {
                        //Create log file.
                        char cSeparator = System.IO.Path.DirectorySeparatorChar;
                        _log = new Logger();
                        _log.Prefix = "GCE";
                        _log.Title = "Global Change Engine " + Administrator.ProfileManager.ApplicationProfile.Version;
                        FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                        _log.Begin(String.Format("{0}Global_Change_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
                        SignalBeginProgress(selectedFiles.Count, "Files", "Searching...");
                        bool anyHits = false;
                        for (int row = 0; row < selectedFiles.Count; row++)
                        {
                            DirectoryEntry entry = selectedFiles[row];
                            string stdHlq = entry.StdHlq;
                            string stdDir = entry.StdDir;
                            string stdFile = entry.StdFile;
                            string stdType = entry.StdType;
                            string fileSpec = entry.StdHlq;
                            if (stdDir != string.Empty)
                            {
                                fileSpec += stdDir;
                            }
                            fileSpec += cSeparator.ToString() + stdFile;
                            if (_mode == "Swagger")
                            {
                                //if (fileSpec.Contains(@"swagger\client\model") || fileSpec.Contains(@"swagger\client\request"))
                                if (fileSpec.Contains(@"swagger\client\model"))
                                {
                                    MarkAllMethodBlocks(fileSpec);
                                    anyHits |= GlobalChange(fileSpec);
                                }
                                else
                                {
                                    anyHits = false;
                                }
                            } else
                            {
                                anyHits |= GlobalChange(fileSpec);
                            }
                            //anyHits |= EvaluateLines(fileSpec);
                            SignalUpdateProgress(1, "Files", fileSpec);
                            if (_action == "Cancel")
                            {
                                break;
                            }
                        }
                        _log.WriteLn();
                        string message = string.Empty;
                        string suffix = string.Empty;
                        if (_action == "Cancel")
                        {
                            _log.WriteMsg("010", "W", "Search cancelled.");
                        }
                        if (anyHits)
                        {
                            if (_action == "Cancel")
                            {
                                message = "Search successful, but cancelled before completion.";
                            }
                            else
                            {
                                message = "Search successful.";
                            }
                            _log.WriteMsg("020", "I", message);
                            SignalEndOfProgress("Files", message);
                        }
                        else
                        {
                            if (_action == "Cancel")
                            {
                                message = "Search not successful, and cancelled before completion.";
                            }
                            else
                            {
                                message = "Search not successful.";
                            }
                            _log.WriteMsg("030", "W", message);
                            SignalEndOfProgress("Files", message);
                        }
                        //Close and free log file.
                        _log.Outcome();
                        _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
                    }
                    else
                    {
                        //When no rows are returned from the SQL query to even begin
                        //searching then use this log file.  This will happen if
                        //no directory files are found at all or, more likely, if
                        //nothing matches the file pattern resulting in no rows
                        //being returned.
                        char separator = System.IO.Path.DirectorySeparatorChar;
                        _log = new Logger();
                        _log.Prefix = "GCE";
                        _log.Title = "Global Change Engine " + Administrator.ProfileManager.ApplicationProfile.Version;
                        FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                        _log.Begin(String.Format("{0}Global_Change_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
                        SignalBeginProgress(selectedFiles.Count, "Files", "Searching...");
                        string message = "No entries to search.";
                        _log.WriteMsg("040", "W", message);
                        SignalEndOfProgress("Files", message);
                        _log.Outcome();
                        _log.Terminate(Administrator.ProfileManager.SystemProfile.ViewerWindows);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Function to mimic the SQL LIKE clause for LINQ queries.
        /// </summary>
        protected bool Like(string field, string pattern)
        {
            bool result = false;
            pattern = pattern.Trim();
            string contents = R.Strip(pattern, "BOTH", "%");
            if (pattern.StartsWith("%") && pattern.EndsWith("%"))
            {
                result = field.Contains(contents);
            }
            else if (pattern.StartsWith("%"))
            {
                result = field.EndsWith(contents);
            }
            else if (pattern.EndsWith("%"))
            {
                result = field.StartsWith(contents);
            }
            else
            {
                result = field == contents;
            }
            return result;
        }

        /// <summary>
        /// Read a class file and identify all methods and mark them with an identifier.
        /// </summary>
        protected void MarkAllMethodBlocks(string fileSpec)
        {
            _endPositions = new List<int>();
            FileInfo fileInfo = null;
            try
            {
                if (File.Exists(fileSpec))
                {
                    StringBuilder sb = new StringBuilder();
                    StreamReader sr1 = new StreamReader(fileSpec);
                    if (sr1.Peek() >= 0)
                    {
                        do
                        {
                            string record = sr1.ReadLine();
                            string line = record.Trim();
                            if (line.StartsWith("*") && line.Contains("JSON"))
                            {
                                //Ignore this bad line.
                                string temp = "";
                            }
                            else
                            {
                                sb.AppendLine(record);
                            }
                        } while (sr1.Peek() >= 0);
                    }
                    sr1.Close();
                    string data = sb.ToString();
                    FileHelper.WriteFile(fileSpec, data);
                    fileInfo = R.GetFileInfo(fileSpec);
                    StreamReader sr2 = new StreamReader(fileSpec);
                    string contents = sr2.ReadToEnd();
                    sr2.Close();
                    contents = MarkBlocks(contents);
                    FileHelper.WriteFile(fileSpec, contents);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        protected string MarkBlocks(string sourceCode)
        {
            const string END_OF_BLOCK = "\n[@END_OF_BLOCK@]\n";
            _sourceCode = sourceCode;
            int position = 0;
            int level = 0;
            string inner = Expand(position, ref level);
            StringBuilder sb = new StringBuilder();
            int startAt = 0;
            for (int ptr = 0; ptr < _endPositions.Count; ptr++)
            {
                int endAt = _endPositions[ptr];
                int length = endAt - startAt;
                string segment = _sourceCode.Substring(startAt, length);
                sb.Append(segment);
                //sb.Append(Environment.NewLine + END_OF_BLOCK + Environment.NewLine);
                sb.Append(END_OF_BLOCK);
                _log.WriteLn(segment);
                _log.WriteLn(END_OF_BLOCK);
                startAt = endAt + 1;
            }
            if (startAt < _sourceCode.Length)
            {
                string segment = _sourceCode.Substring(startAt);
                //segment = segment.Replace("\r\n", string.Empty);
                //segment = segment.Replace("\n", string.Empty);
                sb.Append(segment);
            }
            return sb.ToString();
        }

        /// <remarks>
        /// The normal Router() method has been brought inline into this Expand() method
        /// as the router complexity is not as great as usual.
        /// </remarks>
        protected string Expand(int position, ref int level)
        {
            int quoteCount = 0; //Quote count is zero at the start of every level.
            string code = string.Empty;
            string inner = string.Empty;
            bool goback = false;
            int startPos = position;
            int prevPos = position;
            int length = _sourceCode.Length;
            if (length > 0)
            {
                do
                {
                    //Begin router code.
                    goback = false;
                    code = _sourceCode.Substring(position, 1);
                    switch (code)
                    {
                        case "\"":
                            quoteCount++;
                            break;
                        case "{":
                            if (quoteCount % 2 == 0)
                            {
                                level++;
                                inner = Expand(position + 1, ref level);
                                //position += inner.Length + 1;
                                position += inner.Length;
                                prevPos = position;
                            }
                            break;
                        case "}":
                            inner = _sourceCode.Substring(startPos, position - startPos + 1);
                            if (quoteCount % 2 == 0)
                            {
                                level--;
                                goback = true;
                            }
                            if (level == 0)
                            {
                                //This is the end of the class block.
                                string temp = inner;
                            }
                            if (level == 1)
                            {
                                //This is the end of all method blocks inside the class.
                                _endPositions.Add(position + 1);
                            }
                            break;
                        default:
                            //Any character which is not a control character.
                            break;
                    }
                    //End router code.
                    position++;
                } while (position < length && !goback);
            }
            return inner;
        }

        /// <summary>
        /// Global change the specified file using the hard coded criteria.
        /// </summary>
        protected virtual bool GlobalChange(string fileSpec)
        {
            _headerCommentDone = false;
            StringBuilder contents = new StringBuilder();
            string line = string.Empty;
            bool first = true;
            bool hit = false;
            bool changed = false;
            bool skipUntil = true;
            bool blockStarted = true;
            FileInfo fileInfo = null;
            string record = string.Empty;
            long count = 0;
            try
            {
                if (File.Exists(fileSpec))
                {
                    fileInfo = R.GetFileInfo(fileSpec);
                    if (_mode == "Swagger")
                    {
                        _ignoreUntil = true;
                        _getReady = false;
                    }
                    StreamReader sr = new StreamReader(fileSpec);
                    if (sr.Peek() >= 0)
                    {
                        do
                        {
                            count++;
                            record = sr.ReadLine();
                            line = record;
                            if (_mode == "Swagger")
                            {
                                SwaggerModeChanges(ref hit, ref changed, ref first, fileSpec, count, record, ref line, ref contents, ref blockStarted, ref skipUntil);
                            }
                            else
                            {
                                // Replace UI find value with UI replacement value.
                                line = ReplacementValue(line, _find, _replacement, ref hit, ref changed);
                                contents.AppendLine(line);
                                RecordChange(hit, ref first, fileSpec, count, record, line);
                            }
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
                        foreach (KeyValuePair<string, string> entry in _apiClassNameMapping)
                        {
                            string find = entry.Key + ".";
                            string replacement = entry.Value + ".";
                            if (fileSpec.Contains(find))
                            {
                                fileSpec = fileSpec.Replace(find, replacement);
                                FileHelper.WriteFile(fileSpec, contents.ToString());
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return changed;
        }

        protected void SwaggerModeChanges(ref bool hit, ref bool changed, ref bool first, string fileSpec, long count, string record, ref string line, ref StringBuilder contents, ref bool blockStarted, ref bool skipUntil)
        {
            string temp = string.Empty;
            string trimmedLine = line.Trim();
            foreach (KeyValuePair<string, string> entry in _apiClassNameMapping)
            {
                string find = " " + entry.Key + " ";
                string replacement = " " + entry.Value + " ";
                if (line.Contains(find))
                {
                    line = line.Replace(find, replacement);
                }
                find = "<" + entry.Key + ">";
                replacement = "<" + entry.Value + ">";
                if (line.Contains(find))
                {
                    line = line.Replace(find, replacement);
                }
            }
            if (line.Contains("OpenAPI spec version"))
            {
                //TODO: DONE
                //This was an attempt to work with non-model files which didn't work.
                //Ignore all lines until the end of the horrible GCM JSON example at the top ends.
                //_getReady = true;
            }
            if (line.Contains("package io.swagger.client.models"))
            {
                //TODO: DONE
                //Ignore all lines until the package is found.
                _ignoreUntil = false;
                // Replace package name.
                line = ReplacementValue(line, "package io.swagger.client.models", "package com.sample.example.common.models.api", ref hit, ref changed);
                ConditionalAppendLine(ref contents, line);
                RecordChange(hit, ref first, fileSpec, count, record, line);
                skipUntil = false;
                ConditionalAppendLine(ref contents, string.Empty);
                hit = true;
                changed = true;
                RecordInsert(hit, ref first, fileSpec, count, record, string.Empty);
            }
            else if (line.Contains("import io.swagger.client.models"))
            {
                //TODO: DONE
                //Ignore all lines until the package is found.
                _ignoreUntil = false;
                // Replace import stem.
                line = ReplacementValue(line, "import io.swagger.client.models", "import com.sample.example.common.models.api", ref hit, ref changed);
                ConditionalAppendLine(ref contents, line);
                RecordChange(hit, ref first, fileSpec, count, record, line);
                skipUntil = false;
                ConditionalAppendLine(ref contents, string.Empty);
                hit = true;
                changed = true;
                RecordInsert(hit, ref first, fileSpec, count, record, string.Empty);
            }
            else if (trimmedLine == string.Empty)
            {
                //Ignore all lines until the package is found.
                _ignoreUntil = false;
                // Drop swagger import statements.
                hit = true;
                changed = true;
                RecordSkip(hit, ref first, fileSpec, count, record, line);
            }
            else if (trimmedLine == "/**")
            {
                _ignoreUntil = false;
                if (!_headerCommentDone)
                {
                    _buffer = new List<string>();
                    _bufferUntil = true;
                    RecordBuffer(hit, ref first, fileSpec, count, record, line);
                }
            }
            else if (trimmedLine.StartsWith("*"))
            {
                RecordBuffer(hit, ref first, fileSpec, count, record, line);
            }
            else if (line.Contains("/*") && line.Contains("*/"))
            {
                RecordKeep(hit, ref first, fileSpec, count, record, line);
                ConditionalAppendLine(ref contents, line);
            }
            else if (line.Contains("*/"))
            {
                if (!_headerCommentDone)
                {
                    RecordBuffer(hit, ref first, fileSpec, count, record, line);
                }
                _headerCommentDone = true;
                _bufferUntil = false;
            }
            else if (line.Contains("data class"))
            {
                //Ignore all lines until the package is found.
                _ignoreUntil = false;
                // Insert comment block at start of class.
                string[] parts = line.Split(' ');
                if (parts.Length > 2)
                {
                    string className = parts[2];
                    string classComment = String.Format(" * API{0}.", CommentFromClassName(className));
                    for (int ptr = 0; ptr < _buffer.Count; ptr++)
                    {
                        string bufferLine = _buffer[ptr];
                        bufferLine = StripType(bufferLine);
                        ConditionalAppendLine(ref contents, bufferLine);
                        if (ptr == 0)
                        {
                            ConditionalAppendLine(ref contents, classComment);
                            hit = true;
                            changed = true;
                            RecordInsert(hit, ref first, fileSpec, count, record, classComment);
                        }
                    }
                }
                ConditionalAppendLine(ref contents, line);
            }
            else if (line.Contains("[@END_OF_BLOCK@]"))
            {
                // End of ignored method block.
                blockStarted = false;
                if (skipUntil)
                {
                    skipUntil = false;
                    hit = true;
                    changed = true;
                    RecordSkip(hit, ref first, fileSpec, count, record, line);
                }
            }
            else if (skipUntil)
            {
                // Do nothing while the skipUntil flag is true.
                hit = true;
                changed = true;
                RecordSkip(hit, ref first, fileSpec, count, record, line);
            }
            else
            {
                RecordKeep(hit, ref first, fileSpec, count, record, line);
                ConditionalAppendLine(ref contents, line);
            }
        }

        private string StripType(string line)
        {
            line = line.Replace("kotlin.", string.Empty);
            return line;
        }

        protected void ConditionalAppendLine(ref StringBuilder contents)
        {
            if (!_ignoreUntil)
            {
                contents.AppendLine();
            }
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
                    SignalCriteriaPass(header);
                    _log.WriteLn(header);
                    first = false;
                }
                header = "Found   : (" + count.ToString("00000") + ") - " + line;
                SignalCriteriaPass(header);
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
                    SignalCriteriaPass(header);
                    _log.WriteLn(header);
                    first = false;
                }
                header = "Found   : (" + count.ToString("00000") + ") - " + record;
                SignalCriteriaPass(header);
                _log.WriteLn(header);
                header = "Replace : (" + count.ToString("00000") + ") - " + line;
                SignalCriteriaPass(header);
                _log.WriteLn(header);
            }
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
                    SignalCriteriaPass(header);
                    _log.WriteLn(header);
                    first = false;
                }
                header = "Found   : (" + count.ToString("00000") + ") - " + record;
                SignalCriteriaPass(header);
                _log.WriteLn(header);
                header = "Insert  : (" + count.ToString("00000") + ") - " + line;
                SignalCriteriaPass(header);
                _log.WriteLn(header);
            }
        }

        /// <summary>
        /// Since multiple changes are being made in different places for different "modes", the recording of a hit has been moved into its own method.
        /// </summary>
        protected void RecordSkip(bool hit, ref bool first, string fileSpec, long count, string record, string line)
        {
            string header = string.Empty;
            if (hit)
            {
                if (first)
                {
                    header = "\r\n" + fileSpec + "\r\n";
                    header += string.Empty.PadRight(fileSpec.Length, '-');
                    SignalCriteriaPass(header);
                    _log.WriteLn(header);
                    first = false;
                }
                header = "Skipped : (" + count.ToString("00000") + ") - " + line;
                SignalCriteriaPass(header);
                _log.WriteLn(header);
            }
        }

        /// <summary>
        /// Record buffer.
        /// </summary>
        protected void RecordBuffer(bool hit, ref bool first, string fileSpec, long count, string record, string line)
        {
            string header = string.Empty;
            if (first)
            {
                header = "\r\n" + fileSpec + "\r\n";
                header += string.Empty.PadRight(fileSpec.Length, '-');
                SignalCriteriaPass(header);
                _log.WriteLn(header);
                first = false;
            }
            header = "Buffer  : (" + count.ToString("00000") + ") - " + line;
            _buffer.Add(line);
            SignalCriteriaPass(header);
            _log.WriteLn(header);
        }

        /// <summary>
        /// Record keep.
        /// </summary>
        protected void RecordKeep(bool hit, ref bool first, string fileSpec, long count, string record, string line)
        {
            string header = string.Empty;
            if (first)
            {
                header = "\r\n" + fileSpec + "\r\n";
                header += string.Empty.PadRight(fileSpec.Length, '-');
                SignalCriteriaPass(header);
                _log.WriteLn(header);
                first = false;
            }
            header = "Keep    : (" + count.ToString("00000") + ") - " + line;
            SignalCriteriaPass(header);
            _log.WriteLn(header);
        }

        /// <summary>
        /// This method is used to perform the standard global replacement of the find string with the replacement stirng withing a single line only.
        /// </summary>
        protected string ReplacementValue(string originalLine, string findValue, string replacementValue, ref bool hit, ref bool changed)
        {
            hit = false;
            string replacementLine = originalLine;
            string trimmedLine = replacementLine.Trim();
            if (trimmedLine.Contains(findValue))
            {
                replacementLine = originalLine.Replace(findValue, replacementValue);
                if (replacementLine != originalLine)
                {
                    hit = true;
                }
            }
            changed |= hit;
            return replacementLine;
        }

        /// <summary>
        /// This method was used by ReplaceInfo8 to "Global Change" AssemblyInfo.cs files.
        /// Plan to re-introduce this method when more modes are added to the new "Global Change Engine".
        /// </summary>
        protected string ReplacementLine(string originalLine, string startsWith, string keyword, string replacementValue, string endsWith, ref bool hit, ref bool changed)
        {
            hit = false;
            string replacementLine = originalLine;
            string trimmedLine = replacementLine.Trim();
            if (trimmedLine.StartsWith(startsWith))
            {
                if (trimmedLine.Contains(keyword))
                {
                    if (trimmedLine.EndsWith(endsWith))
                    {
                        replacementLine = String.Format(@"{0}{1}{2}{3}", startsWith, keyword, replacementValue, endsWith);
                        if (replacementLine != originalLine)
                        {
                            hit = true;
                        }
                    }
                }
            }
            changed |= hit;
            return replacementLine;
        }

        /// <summary>
        /// This method was used by Search8, it is not used by the Global Change Engine.
        /// 
        /// Search the specified file using the specified criteria.
        /// </summary>
        protected bool EvaluateLines_Not_Used_Any_More(string fileSpec)
        {
            FileInfo fileInfo = null;
            string header = string.Empty;
            string rec = string.Empty;
            long count = 0;
            bool first = true;
            bool pass = false;
            bool hits = false;
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
                            rec = sr.ReadLine();
                            if (_regex)
                            {
                                Regex regex = new Regex(_searchCriteria, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                                pass = regex.IsMatch(rec);
                            }
                            else
                            {
                                pass = _scanner.Evaluate(rec);
                            }
                            if (pass)
                            {
                                hits = true;
                                if (first)
                                {
                                    header = "\r\n" + fileSpec + "\r\n";
                                    header += string.Empty.PadRight(fileSpec.Length, '-');
                                    SignalCriteriaPass(header);
                                    _log.WriteLn(header);
                                    first = false;
                                }
                                header = "(" + count.ToString("00000") + ") - " + rec;
                                SignalCriteriaPass(header);
                                _log.WriteLn(header);
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
            catch
            {
            }
            return hits;
        }

        /// <summary>
        /// Read the specified file and return its contents as a string.
        /// </summary>
        protected string ReadFile(string fileSpec)
        {
            StreamReader sr;
            string contents = string.Empty;
            try
            {
                if (File.Exists(fileSpec))
                {
                    sr = new StreamReader(fileSpec);
                    contents = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    contents = string.Empty;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return contents;
        }
        #endregion

        #region Delegates.
        public delegate void EventDelegate(object poSender, EventParameters poEventArgs);
        #endregion

        #region Event data.
        /// <summary>
        /// Class that defines data for event handling.
        /// </summary>
        /// <remarks>
        /// This class is used by two different sets of events
        /// which is a bit naughty but that is why the names
        /// are a bit generic.
        /// </remarks>
        public class EventParameters : EventArgs
        {
            #region Member Variables.
            private int _number;
            private string _method;
            private string _text;
            #endregion

            #region Properties.
            public int Number
            {
                get
                {
                    return _number;
                }
            }
            public string Method
            {
                get
                {
                    return _method;
                }
            }
            public string Text
            {
                get
                {
                    return _text;
                }
            }
            #endregion

            #region Constructors.
            public EventParameters(int number, string method, string text)
            {
                _number = number;
                _method = method;
                _text = text;
            }
            #endregion
        }
        #endregion
    }
}