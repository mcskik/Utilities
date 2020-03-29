using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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
        private bool _allTypes = false;
        #endregion
        #region Other members.
        private Dir _dir;
        private List<DirectoryEntry> _directoryListing;
        private long _dirFilesEstimate = 0;
        private Scanner _scanner = null;
        private Logger _log = null;
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
                            anyHits |= EvaluateLines(fileSpec);
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