using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Log8;
using ProfileData.DataLayer.Profile;
using Search8.DataLayer.Profile;
using R = Routines8.Routines;

namespace Search8.Models
{
    /// <summary>
    /// Directory search object.
    /// </summary>
    /// <remarks>
    /// Search all selected file types contained
    /// within the specified top level directory path,
    /// and report all hits with surrounding context
    /// information.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    class SearchEngine
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
        private void SignalBeginProgress(int pnMaximum, string psMethod, string psMessage)
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
        private void SignalUpdateProgress(int pnIncrement, string psMethod, string psMessage)
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
        private void SignalEndOfProgress(string psMethod, string psMessage)
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
        private void SignalCriteriaPass(string psContext)
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
        private string _action = string.Empty;
        private string _path = string.Empty;
        private string _prevPath = string.Empty;
        private bool _pathChanged = false;
        private string _filePattern = string.Empty;
        private string _searchCriteria = string.Empty;
        private StringCollection _extensions;
        private StringCollection _options;
        private StringCollection _directoryExclusions;
        private bool _regex = false;
        private bool _methodBlocks = false;
        private bool _allTypes = false;
        #endregion
        #region Other members.
        private Dir _dir;
        private List<DirectoryEntry> _directoryListing;
        private long _dirFilesEstimate = 0;
        private Scanner _scanner = null;
        private Logger _log = null;
        private List<string> _methodBlocksList = new List<string>();
        private enum LanguageEnum
        {
            Cpp = 0,
            CSharp = 1,
            Java = 2,
            Other = 3
        }
        private LanguageEnum _methodBlockLanguage = LanguageEnum.Other;
        private int _methodBlockThreshold = 0;
        #endregion
        #region Alternative Method Block Detection Code.
        protected string _sourceCode = string.Empty;
        protected List<BlockVector> _blockVectors = new List<BlockVector>();
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
        /// Method Blocks.
        /// </summary>
        public bool MethodBlocks
        {
            get
            {
                return _methodBlocks;
            }
            set
            {
                _methodBlocks = value;
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
        public SearchEngine()
        {
            Load();
        }
        #endregion

        #region Event Handlers.
        /// <summary>
        /// Start directory scan progress indicator.
        /// </summary>
        void _dir_EventBeginProgress(object poSender, EventParameters2 poEventArgs)
        {
            try
            {
                SignalBeginProgress((int)_dirFilesEstimate, "DirList", "Scanning directories ...");
            }
            catch (Exception ex)
            {
                _dirFilesEstimate = 0;
            }
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
                _dir.EventBeginProgress += new Search8.Models.EventDelegate(_dir_EventBeginProgress);
                _dir.EventUpdateProgress += new Search8.Models.EventDelegate(_dir_EventUpdateProgress);
                _dir.EventEndOfProgress += new Search8.Models.EventDelegate(_dir_EventEndOfProgress);
                _directoryListing = _dir.DirList(_path, ref _dirFilesEstimate);
            }
            Process();
            Save();
        }
        #endregion

        #region Private members.
        /// <summary>
        /// Load system and working profile variables.
        /// </summary>
        private void Load()
        {
            _dir = new Dir();
            _path = Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath;
            _filePattern = Administrator.ProfileManager.UserSettings.SelectedItem.FilePattern;
            _searchCriteria = Administrator.ProfileManager.UserSettings.SelectedItem.Criteria;
            _extensions = Administrator.ProfileManager.UserSettings.SelectedItem.Extensions;
            _options = Administrator.ProfileManager.UserSettings.SelectedItem.Options;
            _directoryExclusions = Administrator.ProfileManager.UserSettings.SelectedItem.DirectoryExclusions;
            _dirFilesEstimate = Administrator.ProfileManager.UserSettings.SelectedItem.DirFilesEstimate;
            _regex = Administrator.ProfileManager.UserSettings.SelectedItem.RegexCriteria;
            _methodBlocks = Administrator.ProfileManager.UserSettings.SelectedItem.MethodBlocks;
            _allTypes = Administrator.ProfileManager.UserSettings.SelectedItem.AllTypes;
        }

        /// <summary>
        /// Save working profile variables.
        /// </summary>
        private void Save()
        {
            Administrator.ProfileManager.UserSettings.SelectedItem.SearchPath = _path;
            Administrator.ProfileManager.UserSettings.SelectedItem.FilePattern = _filePattern;
            Administrator.ProfileManager.UserSettings.SelectedItem.Criteria = _searchCriteria;
            Administrator.ProfileManager.UserSettings.SelectedItem.Extensions = _extensions;
            Administrator.ProfileManager.UserSettings.SelectedItem.Options = _options;
            Administrator.ProfileManager.UserSettings.SelectedItem.DirectoryExclusions = _directoryExclusions;
            Administrator.ProfileManager.UserSettings.SelectedItem.DirFilesEstimate = _dirFilesEstimate;
            Administrator.ProfileManager.UserSettings.SelectedItem.RegexCriteria = _regex;
            Administrator.ProfileManager.UserSettings.SelectedItem.MethodBlocks = _methodBlocks;
            Administrator.ProfileManager.UserSettings.SelectedItem.AllTypes = _allTypes;
            Administrator.ProfileManager.UserSettings.Save();
            _scanner.SearchCriteriaDocument.Save(Administrator.ProfileManager.SystemProfile.CurrentCriteria);
        }

        /// <summary>
        /// Process all directory entries and report any lines of text which meet
        /// the search criteria.
        /// </summary>
        private void Process()
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
                        _log.Prefix = "ACE";
                        _log.Title = "Advanced Search Engine " + Administrator.ProfileManager.ApplicationProfile.Version;
                        FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                        _log.Begin(String.Format("{0}Search_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
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
                            if (MethodBlocks)
                            {
                                anyHits |= EvaluateMethodBlocks(fileSpec);
                            }
                            else
                            {
                                anyHits |= EvaluateLines(fileSpec);
                            }
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
                        _log.Prefix = "ACE";
                        _log.Title = "Advanced Search Engine " + Administrator.ProfileManager.ApplicationProfile.Version;
                        FileHelper.PathCheck(Administrator.ProfileManager.SystemProfile.LogPath);
                        _log.Begin(String.Format("{0}Search_{1}.log", Administrator.ProfileManager.SystemProfile.LogPath, DateTime.Now.ToString("yyyyMMdd@HHmmss")));
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
        private bool Like(string field, string pattern)
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
        /// Search the specified file using the specified criteria.
        /// </summary>
        private bool EvaluateLines(string fileSpec)
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

        #region Method Blocks.
        /// <summary>
        /// Search the specified file using the specified criteria.
        /// Use method blocks { } instead of ines so anything found within
        /// the method which matches the criteria is counted as a hit.
        /// </summary>
        private bool EvaluateMethodBlocks(string fileSpec)
        {
            List<BlockVector> methodBlocks = ReadMethodBlocks(fileSpec);
            string header = string.Empty;
            long count = 0;
            bool first = true;
            bool pass = false;
            bool hits = false;
            try
            {
                foreach (BlockVector block in methodBlocks)
                {
                    count++;
                    if (_regex)
                    {
                        Regex regex = new Regex(_searchCriteria, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        pass = regex.IsMatch(block.Content);
                    }
                    else
                    {
                        pass = _scanner.Evaluate(block.Content);
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
                        _log.WriteLn();
                        header = "[" + block.StartLinekNumber.ToString("00000") + "] - Found in block :";
                        SignalCriteriaPass(header);
                        _log.WriteLn(header);
                        _log.WriteLn();
                        EvaluateLinesWithinBlock(block);
                    }
                    if (_action == "Cancel")
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
            return hits;
        }

        /// <summary>
        /// Search the specified block using the specified criteria.
        /// </summary>
        private bool EvaluateLinesWithinBlock(BlockVector block)
        {
            string[] lines = block.Content.Split('\n');
            string header = string.Empty;
            bool first = true;
            bool pass = false;
            bool hits = false;
            int lineNumber = block.StartLinekNumber;
            try
            {
                foreach (string line in lines)
                {
                    if (_regex)
                    {
                        Regex regex = new Regex(_searchCriteria, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        pass = regex.IsMatch(line);
                    }
                    else
                    {
                        pass = _scanner.EvaluateAny(line);
                    }
                    if (pass)
                    {
                        hits = true;
                        header = "(" + lineNumber.ToString("00000") + ") - " + line;
                        SignalCriteriaPass(header);
                        _log.Write(header);
                    } else {
                        header = "          " + line;
                        SignalCriteriaPass(header);
                        _log.Write(header);
                    }
                    if (_action == "Cancel")
                    {
                        break;
                    }
                    lineNumber++;
                }
            }
            catch
            {
            }
            return hits;
        }

        /// <summary>
        /// Read all method blocks from the specified file into a list of strings.
        /// </summary>
        protected List<BlockVector> ReadMethodBlocks(string fileSpec)
        {
            string ext = System.IO.Path.GetExtension(fileSpec);
            switch (ext)
            {
                case ".c":
                case ".h":
                case ".cpp":
                    _methodBlockLanguage = LanguageEnum.Cpp;
                    _methodBlockThreshold = 0;
                    break;
                case ".cs":
                    _methodBlockLanguage = LanguageEnum.CSharp;
                    _methodBlockThreshold = 0;
                    break;
                case ".js":
                    _methodBlockLanguage = LanguageEnum.Other;
                    _methodBlockThreshold = 0;
                    break;
                case ".java":
                case ".kt":
                    _methodBlockLanguage = LanguageEnum.Java;
                    _methodBlockThreshold = 0;
                    break;
                defaut:
                    _methodBlockLanguage = LanguageEnum.Other;
                    _methodBlockThreshold = 0;
                    break;
            }
            _blockVectors = new List<BlockVector>();
            try
            {
                if (File.Exists(fileSpec))
                {
                    StringBuilder sb = new StringBuilder();
                    string sourceCode = FileHelper.ReadFile(fileSpec) + Environment.NewLine;
                    if (_methodBlockLanguage == LanguageEnum.CSharp)
                    {
                        if (sourceCode.Contains("namespace"))
                        {
                            _methodBlockThreshold = 1;
                        }
                        else
                        {
                            _methodBlockThreshold = 0;
                        }
                    }
                    SplitMethodBlocks(sourceCode);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return _blockVectors;
        }

        /// <summary>
        /// Split all method blocks in the specified sourceCode into a list of string blocks.
        /// </summary>
        protected List<BlockVector> SplitMethodBlocks(string sourceCode)
        {
            _sourceCode = sourceCode;
            int position = 0;
            int level = 0;
            string inner = string.Empty;
            switch (_methodBlockLanguage)
            {
                case LanguageEnum.Cpp:
                    inner = ExpandJavaMethodBlocks(position, ref level);
                    break;
                case LanguageEnum.CSharp:
                    inner = ExpandCSharpMethodBlocks(position, ref level);
                    break;
                case LanguageEnum.Java:
                    inner = ExpandJavaMethodBlocks(position, ref level);
                    break;
                case LanguageEnum.Other:
                    inner = ExpandJavaMethodBlocks(position, ref level);
                    break;
                defaut:
                    inner = ExpandJavaMethodBlocks(position, ref level);
                    break;
            }
            int startAt = 0;
            int lineNumber = 1;
            int blockNumber = 0;
            BlockVector blockVector = new BlockVector();
            for (int ptr = 0; ptr < _blockVectors.Count; ptr++)
            {
                blockNumber++;
                blockVector = _blockVectors[ptr];
                blockVector.BlockNumber = blockNumber;
                blockVector.StartPosition = startAt;
                int endAt =  _blockVectors[ptr].EndPosition;
                int length = endAt - startAt;
                string segment = _sourceCode.Substring(startAt, length);
                string[] parts = segment.Split('\n');
                blockVector.StartLinekNumber = lineNumber;
                lineNumber += parts.Length;
                blockVector.Content = segment;
                startAt = endAt + 1;
            }
            if (startAt <= _sourceCode.Length)
            {
                blockNumber++;
                blockVector = new BlockVector();
                blockVector.BlockNumber = blockNumber;
                blockVector.StartLinekNumber = lineNumber;
                blockVector.StartPosition = startAt;
                blockVector.EndPosition = _sourceCode.Length - 1;
                string segment = _sourceCode.Substring(startAt);
                blockVector.Content = segment;
                _blockVectors.Add(blockVector);
            }
            return _blockVectors;
        }

        /// <remarks>
        /// The normal Router() method has been brought inline into this ExpandMethodBlocks() method
        /// as the router complexity is not as great as usual.
        /// </remarks>
        protected string ExpandJavaMethodBlocks(int position, ref int level)
        {
            int quoteCount = 0; //Quote count is zero at the start of every level.
            string prevCode = string.Empty;
            string code = string.Empty;
            string inner = string.Empty;
            bool goback = false;
            int startPos = position;
            int prevPos = position;
            int length = _sourceCode.Length;
            StringBuilder prevBlockCommentMarker = new StringBuilder();
            int prevBlockCommentPos = -1;
            int prevNewLinePos = 0;
            BlockVector blockVector = new BlockVector();
            int lineEndingLength = 1;
            if (length > 0)
            {
                do
                {
                    //Begin router code.
                    goback = false;
                    prevCode = code;
                    code = _sourceCode.Substring(position, 1);
                    switch (code)
                    {
                        case "/":
                            prevBlockCommentMarker.Append(code);
                            break;
                        case "*":
                            prevBlockCommentMarker.Append(code);
                            if (prevBlockCommentMarker.ToString() == "/**")
                            {
                                prevBlockCommentPos = prevNewLinePos;
                            }
                            break;
                        case "\n":
                            if (prevCode == "\r")
                            {
                                //Windows line ending.
                                lineEndingLength = 2;
                            }
                            else
                            {
                                //Unix line ending.
                                lineEndingLength = 1;
                            }
                            // -1 is based on unix line endings being 1 characters long.
                            // -2 is based on Windows line endings being 2 characters long.
                            prevNewLinePos = position;
                            prevBlockCommentMarker = new StringBuilder();
                            break;
                        case "\"":
                            quoteCount++;
                            break;
                        case "{":
                            if (level == 0)
                            {
                                //Block from start of file up to line containing class declaration.
                                blockVector = new BlockVector();
                                blockVector.EndPosition = GetStartOfBlockPosition(prevBlockCommentPos, prevNewLinePos);
                                _blockVectors.Add(blockVector);
                                prevBlockCommentPos = -1;
                            }
                            if (level == 1)
                            {
                                //Block from start of class declaration to start of line containing start of first method. 
                                blockVector = new BlockVector();
                                blockVector.EndPosition = GetStartOfBlockPosition(prevBlockCommentPos, prevNewLinePos);
                                _blockVectors.Add(blockVector);
                                prevBlockCommentPos = -1;
                            }
                            if (quoteCount % 2 == 0)
                            {
                                level++;
                                inner = ExpandJavaMethodBlocks(position + 1, ref level);
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
                                blockVector = new BlockVector();
                                blockVector.EndPosition = position + lineEndingLength;
                                _blockVectors.Add(blockVector);
                            }
                            if (level == 1)
                            {
                                //This is the end of all method blocks inside the class.
                                blockVector = new BlockVector();
                                blockVector.EndPosition = position + lineEndingLength;
                                _blockVectors.Add(blockVector);
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

        /// <remarks>
        /// The normal Router() method has been brought inline into this ExpandMethodBlocks() method
        /// as the router complexity is not as great as usual.
        /// </remarks>
        protected string ExpandCSharpMethodBlocks(int position, ref int level)
        {
            int quoteCount = 0; //Quote count is zero at the start of every level.
            string prevCode = string.Empty;
            string code = string.Empty;
            string inner = string.Empty;
            bool goback = false;
            int startPos = position;
            int prevPos = position;
            int length = _sourceCode.Length;
            StringBuilder prevBlockCommentMarker = new StringBuilder();
            int prevBlockCommentPos = -1;
            int prevPrevNewLinePos = 0;
            int prevNewLinePos = 0;
            BlockVector blockVector = new BlockVector();
            int lineEndingLength = 1;
            if (length > 0)
            {
                do
                {
                    //Begin router code.
                    goback = false;
                    prevCode = code;
                    code = _sourceCode.Substring(position, 1);
                    switch (code)
                    {
                        case "/":
                            prevBlockCommentMarker.Append(code);
                            if (prevBlockCommentMarker.ToString() == "///")
                            {
                                if (prevBlockCommentPos == -1)
                                {
                                    prevBlockCommentPos = prevNewLinePos;
                                }
                            }
                            break;
                        case "\n":
                            if (prevCode == "\r")
                            {
                                //Windows line ending.
                                lineEndingLength = 2;
                            }
                            else
                            {
                                //Unix line ending.
                                lineEndingLength = 1;
                            }
                            // -1 is based on unix line endings being 1 characters long.
                            // -2 is based on Windows line endings being 2 characters long.
                            prevPrevNewLinePos = prevNewLinePos;
                            prevNewLinePos = position;
                            prevBlockCommentMarker = new StringBuilder();
                            break;
                        case "\"":
                            quoteCount++;
                            break;
                        case "{":
                            if (level == _methodBlockThreshold - 1)
                            {
                            }
                            if (level == _methodBlockThreshold)
                            {
                                //Block from start of file up to line containing class declaration.
                                blockVector = new BlockVector();
                                blockVector.EndPosition = GetStartOfBlockPosition(prevBlockCommentPos, prevPrevNewLinePos);
                                _blockVectors.Add(blockVector);
                                prevBlockCommentPos = -1;
                            }
                            if (level == _methodBlockThreshold + 1)
                            {
                                //Block from start of class declaration to start of line containing start of first method. 
                                blockVector = new BlockVector();
                                blockVector.EndPosition = GetStartOfBlockPosition(prevBlockCommentPos, prevPrevNewLinePos);
                                _blockVectors.Add(blockVector);
                                prevBlockCommentPos = -1;
                            }
                            if (quoteCount % 2 == 0)
                            {
                                level++;
                                inner = ExpandCSharpMethodBlocks(position + 1, ref level);
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
                            if (level == _methodBlockThreshold - 1)
                            {
                            }
                            if (level == _methodBlockThreshold)
                            {
                                //This is the end of the class block.
                                blockVector = new BlockVector();
                                blockVector.EndPosition = position + lineEndingLength;
                                _blockVectors.Add(blockVector);
                            }
                            if (level == _methodBlockThreshold + 1)
                            {
                                //This is the end of all method blocks inside the class.
                                blockVector = new BlockVector();
                                blockVector.EndPosition = position + lineEndingLength;
                                _blockVectors.Add(blockVector);
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

        private int GetStartOfBlockPosition(int prevBlockCommentPos, int prevNewLinePos)
        {
            if (prevBlockCommentPos != -1)
            {
                return prevBlockCommentPos;
            }
            else
            {
                return prevNewLinePos;
            }
        }
        #endregion

        /// <summary>
        /// Read the specified file and return its contents as a string.
        /// </summary>
        private string ReadFile(string fileSpec)
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