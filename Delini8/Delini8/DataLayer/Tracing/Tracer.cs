using Delini8.DataLayer.Profile;
using System;
using System.IO;

namespace Delini8.DataLayer.Tracing
{
    /// <summary>
    /// Tracer class.
    /// </summary>
    /// <remarks>
    /// File based tracing/logging class which writes to a file instead of an SqlContext.Pipe.
    /// A common interface is used by the Tracer and SqlTracer objects.
    /// This makes it possible to switch between the file based tracer
    /// and the SqlContext.Pipe based tracer with minimal change.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class Tracer : ITracer
    {
        #region Constants.
        private const string SEVERITY_CODES = "IWES";
        private const char DASH = '-';
        public enum StripMode
        {
            Both = 1,
            Leading = 2,
            Trailing = 3
        }
        #endregion

        #region Memeber Variables.
        private StreamWriter _streamWriter;
        private DateTime _startAll;
        private DateTime _finishAll;
        private TimeSpan _elapsedAll;
        private DateTime _startTask;
        private DateTime _finishTask;
        private TimeSpan _elapsedTask;
        private DateTime _mostRecentDateTime;
        private bool _useSeverity = false;
        private bool _useSerial = false;
        #endregion

        #region Properties.
        public DateTime StartAllDateTime
        {
            get
            {
                return _startAll;
            }
            set
            {
                _startAll = value;
            }
        }
        public string LogPath { get; set; }
        public string LogName { get; set; }
        public string LogFileSpec { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        /// <remarks>
        /// Message severities are:
        /// "I" - Informational, not an error.
        /// "W" - Warning, draws attention to something unusual but not an error.
        /// "E" - Error, requires some kind of action to fix, maybe just a rerun.
        /// "S" - Severe, major operating system or server error beyond our control.
        /// </remarks>
        public string HighestSeverity { get; set; }
        public DateTime StartedAt
        {
            get
            {
                return _startAll;
            }
        }
        #endregion

        #region Constructors.
        public Tracer(string logPath, string logName)
            : this(logPath, logName, Administrator.Now, string.Empty, string.Empty, false, false)
        {
        }

        public Tracer(string logPath, string logName, string title)
            : this(logPath, logName, Administrator.Now, string.Empty, title, false, false)
        {
        }

        public Tracer(string logPath, string logName, string prefix, string title)
            : this(logPath, logName, Administrator.Now, prefix, title, false, false)
        {
        }

        public Tracer(string logPath, string logName, string prefix, string title, bool useSeverity)
            : this(logPath, logName, Administrator.Now, prefix, title, useSeverity, false)
        {
        }

        public Tracer(string logPath, string logName, DateTime startDateTime, string prefix, string title, bool useSeverity, bool useSerial)
        {
            LogPath = logPath;
            LogName = logName;
            _startAll = startDateTime;
            LogFileSpec = String.Format(@"{0}{1}_{2}.log", LogPath, LogName, _startAll.ToString("yyyyMMdd@HHmmss"));
            Prefix = prefix;
            Title = title;
            HighestSeverity = SEVERITY_CODES.Substring(0, 1);
            _useSerial = useSerial;
            _useSeverity = useSeverity;
            Begin();
        }
        #endregion

        #region Public Methods.
        public void Begin()
        {
            PathCheck(LogFileSpec);
            if (File.Exists(LogFileSpec))
            {
                File.Delete(LogFileSpec);
            }
            _streamWriter = File.CreateText(LogFileSpec);
            if (Title != string.Empty)
            {
                WriteLine(Title);
                WriteLine(new string(DASH, Title.Length));
                StartOfDayMessage();
            }
            _mostRecentDateTime = Administrator.Now;
        }

        public void BeginTask()
        {
            _startTask = Administrator.Now;
        }

        public void FinishTask(string message)
        {
            FinishTask(string.Empty, string.Empty, message);
        }

        public void FinishTask(string severity, string message)
        {
            FinishTask(string.Empty, severity, message);
        }

        public void FinishTask(string serial, string severity, string message)
        {
            _finishTask = Administrator.Now;
            _elapsedTask = _finishTask.Subtract(_startTask);
            message = String.Format("Start: {0} Stop: {1} Elapsed: {2} {3}", _startTask.ToString("HH:mm:ss:fffffff"), _finishTask.ToString("HH:mm:ss:fffffff"), _elapsedTask.ToString().PadRight(16, ' '), message);
            WriteMsg(serial, severity, message);
        }

        public void WriteMsg(string message)
        {
            WriteMsg(string.Empty, string.Empty, message);
        }

        public void WriteMsg(string severity, string message)
        {
            WriteMsg(string.Empty, severity, message);
        }

        public void WriteMsg(string serial, string severity, string message)
        {
            CheckDay();
            MonitorHighestSeverity(severity);
            string messagePrefix = String.Format("{0}{1}({2})", Prefix, serial, severity);
            string line = string.Empty;
            if (messagePrefix.Trim().Length > 0)
            {
                line = String.Format("{0} - {1}", messagePrefix, message);
            }
            else
            {
                line = message;
            }
            WriteLine(line);
        }

        public void WriteTimedMsg(string message)
        {
            WriteTimedMsg(string.Empty, string.Empty, message);
        }

        public void WriteTimedMsg(string severity, string message)
        {
            WriteTimedMsg(string.Empty, severity, message);
        }

        public void WriteTimedMsg(string serial, string severity, string message)
        {
            message = String.Format("{0} {1}", Administrator.Now.ToString("HH:mm:ss"), message);
            WriteMsg(serial, severity, message);
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(string line)
        {
            _streamWriter.WriteLine(line);
            _streamWriter.Flush();
        }

        private void CheckDay()
        {
            DateTime today = DateTime.Today;
            DateTime mostRecentDate = new DateTime(_mostRecentDateTime.Year, _mostRecentDateTime.Month, _mostRecentDateTime.Day);
            if (today > mostRecentDate)
            {
                StartOfDayMessage();
            }
            _mostRecentDateTime = Administrator.Now;
        }

        private void StartOfDayMessage()
        {
            string message = String.Format("Start of new day: {0}", DateTime.Today.ToString("dd/MM/yyyy"));
            WriteLine();
            WriteLine(message);
            WriteLine(new string(DASH, message.Length));
        }

        public void Outcome()
        {
            string serial = string.Empty;
            WriteLine();
            switch (HighestSeverity)
            {
                case "I":
                    if (_useSerial) serial = "910";
                    WriteMsg(serial, "I", "Completed successfully");
                    break;
                case "W":
                    if (_useSerial) serial = "920";
                    WriteMsg(serial, "W", "Completed with warnings");
                    break;
                case "E":
                    if (_useSerial) serial = "930";
                    WriteMsg(serial, "E", "Completed with errors");
                    break;
                case "S":
                    if (_useSerial) serial = "940";
                    WriteMsg(serial, "S", "Completed with severe errors");
                    break;
                default:
                    if (_useSerial) serial = "950";
                    WriteMsg(serial, "U", "Completed with unknown errors");
                    break;
            }
        }

        public void Finish()
        {
            string serial = string.Empty;
            string severity = string.Empty;
            _finishAll = Administrator.Now;
            _elapsedAll = CalculateElapsedSeconds(ref _startAll, ref _finishAll);
            WriteLine();
            if (_useSeverity) severity = "I";
            if (_useSerial) serial = "960";
            WriteMsg(serial, severity, "Start   : " + _startAll.ToString("dd/MM/yyyy HH:mm:ss"));
            if (_useSerial) serial = "970";
            WriteMsg(serial, severity, "End     : " + _finishAll.ToString("dd/MM/yyyy HH:mm:ss"));
            if (_useSerial) serial = "980";
            WriteMsg(serial, severity, "Elapsed : " + "           " + _elapsedAll.ToString());
            WriteLine();
        }
        #endregion

        #region Private Methods.
        private void MonitorHighestSeverity(string severity)
        {
            int pos1 = 0;
            int pos2 = 0;
            severity = severity.Trim().ToUpper();
            if (severity.Trim().Length == 0)
            {
                severity = "I";
            }
            pos1 = SEVERITY_CODES.IndexOf(severity);
            if (pos1 > -1)
            {
                pos2 = SEVERITY_CODES.IndexOf(HighestSeverity);
                if (pos2 > -1)
                {
                    if (pos1 > pos2)
                    {
                        HighestSeverity = severity;
                    }
                }
            }
        }

        private TimeSpan CalculateElapsedSeconds(ref DateTime start, ref DateTime finish)
        {
            start = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0);
            finish = new DateTime(finish.Year, finish.Month, finish.Day, finish.Hour, finish.Minute, finish.Second, 0);
            TimeSpan elapsed = finish.Subtract(start);
            if (elapsed.Milliseconds != 0)
            {
                elapsed = new TimeSpan(elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds, 0);
            }
            return elapsed;
        }

        private string Strip(string input, StripMode mode, string character)
        {
            int ptr = 0;
            string output = string.Empty;
            if (input.Length == 0)
            {
                return input;
            }
            switch (mode)
            {
                case StripMode.Both:
                    break;
                case StripMode.Leading:
                    break;
                case StripMode.Trailing:
                    break;
                default:
                    return input;
            }
            if (character.Length != 1)
            {
                return input;
            }
            if (mode == StripMode.Leading || mode == StripMode.Both)
            {
                for (ptr = 0; ptr < input.Length; ptr++)
                {
                    if (input.Substring(ptr, 1) != character)
                    {
                        output = input.Substring(ptr);
                        break;
                    }
                }
                input = output;
            }
            if (mode == StripMode.Trailing || mode == StripMode.Both)
            {
                for (ptr = input.Length - 1; ptr >= 0; ptr--)
                {
                    if (input.Substring(ptr, 1) != character)
                    {
                        output = input.Substring(0, ptr + 1);
                        break;
                    }
                }
            }
            return output;
        }

        private void PathCheck(string fileSpec)
        {
            string separator = Path.DirectorySeparatorChar.ToString();
            string path = string.Empty;
            int pos = 0;
            do
            {
                pos = fileSpec.IndexOf(separator, pos + 1);
                if (pos > -1)
                {
                    path = fileSpec.Substring(0, pos);
                    if (!Directory.Exists(path) && !path.EndsWith(":"))
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch
                        {
                        }
                    }
                }
            } while (pos >= 0);
        }
        #endregion
    }
}