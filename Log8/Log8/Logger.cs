using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using R = Routines8.Routines;

namespace Log8
{
    public class Logger : ILogger
    {
        #region Constants.
        private const string SEVERITY_CODES = "IWES";
        private const string DASH = "-";
        private const string ZERO = "0";
        #endregion

        #region Enumerations.
        public enum LogMode
        {
            Create = 0,
            Append = 1
        }
        #endregion

        #region Memeber Variables.
        #region Property variables.
        private string _Title;
		private FileStream _FileStream;
		private StreamWriter _StreamWriter;
		private string _Spec;
		private DateTime _Start;
		private DateTime _Finish;
		private TimeSpan _Elapsed;
        #endregion

        #region Property variables for message.
        private bool _Active;
		private string _Prefix;
		private string _Serial;
		private string _Severity;
		private string _HighestSeverity;
		private string _Message;
		private int _Number;
		private string _Source;
        #endregion
        #endregion

		#region Properties.
        /// <summary>
        /// Writing to log file is active or not.
        /// </summary>
        public bool Active
        {
            get
            {
                return _Active;
            }
        }

        /// <summary>
        /// Log file specification where log is written.
        /// </summary>
        public string Spec
        {
            get
            {
                return _Spec;
            }
        }

        /// <summary>
        /// Log title which appears at the top of the log file.
        /// </summary>
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        /// <summary>
        /// Three letter prefix which is pre-pended to every log message.
        /// This should uniquely identify the system issuing the message.
        /// </summary>
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                _Prefix = value.Trim();
                _Prefix = R.RPad(_Prefix, 3, " ");
                _Prefix = _Prefix.ToUpper();
            }
        }

        /// <summary>
        /// Highest message severity issued so far.
        /// </summary>
        /// <remarks>
        /// Message severities are:
        /// "I" - Informational, not an error.
        /// "W" - Warning, draws attention to something unusual but not an error.
        /// "E" - Error, requires some kind of action to fix, maybe just a rerun.
        /// "S" - Severe, major operating system or server error beyond our control.
        /// </remarks>
        public string HighestSeverity
        {
            get
            {
                return _HighestSeverity;
            }
        }

        /// <summary>
        /// Runtime environment error message number.
        /// </summary>
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
            }
        }

        /// <summary>
        /// Runtime environment error message source.
        /// </summary>
        public string Source
        {
            get
            {
                return _Source;
            }
        }
        #endregion

		#region Constructors.
        public Logger() : this(DateTime.Now)
        {
        }

        public Logger(DateTime completeTaskStartTime)
        {
            _Start = completeTaskStartTime;
            _StreamWriter = null;
            _Title = string.Empty;
            _Prefix = "LOG";
            _HighestSeverity = SEVERITY_CODES.Substring(0, 1);
            Clear();
        }

        public Logger(string fileSpec) : this()
        {
            Begin(fileSpec);
        }

        public Logger(string fileSpec, DateTime completeTaskStartTime) : this(completeTaskStartTime)
        {
            Begin(fileSpec);
        }

        public Logger(string fileSpec, string prefix, string title) : this()
        {
            _Prefix = prefix;
            _Title = title;
            Begin(fileSpec);
        }

        public Logger(string fileSpec, string prefix, string title, DateTime completeTaskStartTime) : this(completeTaskStartTime)
        {
            _Prefix = prefix;
            _Title = title;
            Begin(fileSpec);
        }
        #endregion

		#region public Methods.
        /// <summary>
        /// Begin logging to the log file.
        /// </summary>
        /// <remarks>
        /// The default mode is to create a new log file.
        /// </remarks>
        public void Begin(string fileSpec)
        {
            Begin(fileSpec, LogMode.Create);
        }

        /// <summary>
        /// Begin logging to the log file.
        /// </summary>
        /// <remarks>
        /// Mode "Create" creates a new log file.
        /// Mode "Append" appends to an existing log file.
        /// </remarks>
        public void Begin(string fileSpec, LogMode mode)
        {
            string filePath = string.Empty;
            string fileName = string.Empty;
            _Spec = fileSpec.Trim();
            try
            {
                if (mode == LogMode.Create)
                {
                    _FileStream = null;
                    _StreamWriter = null;
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileSpec);
                    }
                    _StreamWriter = File.CreateText(fileSpec);
                    if (_Title != string.Empty)
                    {
                        WriteLn(_Title);
                        WriteLn(R.Replicate(DASH, _Title.Length));
                    }
                }
                else
                {
                    filePath = R.FileStem(fileSpec);
                    fileName = R.FileFullName(fileSpec);
                    if (File.Exists(fileName))
                    {
                        _FileStream = new FileStream(fileSpec, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        _StreamWriter = new StreamWriter(_FileStream);
                    }
                    else
                    {
                        _StreamWriter = File.CreateText(fileSpec);
                        if (_Title != string.Empty)
                        {
                            WriteLn(_Title);
                            WriteLn(R.Replicate(DASH, _Title.Length));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Clear error information.
        /// </summary>
        public void Clear()
        {
            _Active = false;
            _Number = 0;
            _Source = string.Empty;
            _Message = string.Empty;
            _Serial = R.Replicate(ZERO, 3);
            _Severity = SEVERITY_CODES.Substring(0, 1);
        }

        /// <summary>
        /// Write a message to the log file and raise/throw an error.
        /// </summary>
        /// <remarks>
        /// Message severities are:
        /// "I" - Informational, not an error.
        /// "W" - Warning, draws attention to something unusual but not an error.
        /// "E" - Error, requires some kind of action to fix, maybe just a rerun.
        /// "S" - Severe, major operating system or server error beyond our control.
        /// </remarks>
        public void WriteErr(string serial, string severity, string message, int number, string source)
        {
            _Active = true;
            WriteMsg(serial, severity, message);
            //TODO: Throw object with these details: _Number, _Source, _Message.
        }

        /// <summary>
        /// Write a message to the log file.
        /// </summary>
        /// <remarks>
        /// Message severities are:
        /// "I" - Informational, not an error.
        /// "W" - Warning, draws attention to something unusual but not an error.
        /// "E" - Error, requires some kind of action to fix, maybe just a rerun.
        /// "S" - Severe, major operating system or server error beyond our control.
        /// </remarks>
        public void WriteMsg(string serial, string severity, string message)
        {
            string line;
            MonitorHighestSeverity(severity);
            _Serial = serial;
            _Severity = severity;
            _Message = message;
            line = _Prefix + serial + severity;
            if (message != string.Empty)
            {
                line = line + " - " + message;
            }
            WriteLn(line);
        }

        /// <summary>
        /// Write timed message to log file in standard fashion.
        /// </summary>
        /// <remarks>
        /// Message severities are:
        /// "I" - Informational, not an error.
        /// "W" - Warning, draws attention to something unusual but not an error.
        /// "E" - Error, requires some kind of action to fix, maybe just a rerun.
        /// "S" - Severe, major operating system or server error beyond our control.
        /// </remarks>
        public void WriteTimedMsg(string serial, string severity, string message)
        {
            message = R.FmtDate(DateTime.Now, "hh:nn:ss") + " " + message;
            WriteMsg(serial, severity, message);
        }

        /// <summary>
        /// Write a blank line to the log file.
        /// </summary>
        public void WriteLn()
        {
            WriteLn(string.Empty);
        }

        /// <summary>
        /// Write a line to the log file.
        /// </summary>
        public void WriteLn(string line)
        {
            try
            {
                _StreamWriter.WriteLine(line);
                _StreamWriter.Flush();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Write a text string to the log file.
        /// </summary>
        public void Write(string text)
        {
            try
            {
                _StreamWriter.Write(text);
                _StreamWriter.Flush();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Write final outcome messages to the log file.
        /// </summary>
        public void Outcome()
        {
            string prefix;
            prefix = _Prefix;
            _Prefix = "JRN";
            WriteLn();
            switch (_HighestSeverity)
            {
                case "I":
                    WriteMsg("010", "I", "Completed successfully");
                    break;
                case "W":
                    WriteMsg("020", "W", "Completed with warnings");
                    break;
                case "E":
                    WriteMsg("030", "E", "Completed with errors");
                    break;
                case "S":
                    WriteMsg("040", "S", "Completed with severe errors");
                    break;
                default:
                    WriteMsg("050", "U", "Completed with unknown errors");
                    break;
            }
            _Prefix = prefix;
        }

        /// <summary>
        /// Terminate the log file.
        /// </summary>
        /// <remarks>
        /// The log file viewer program will not be triggered.
        /// </remarks>
        public void Terminate()
        {
            Terminate(string.Empty);
        }

        /// <summary>
        /// Terminate the log file.
        /// </summary>
        /// <remarks>
        /// If suYF18ied the viewer program will be automatically triggered to view the log file.
        /// </remarks>
        public void Terminate(string viewerProgram)
        {
            CalculateRunStatistics();
            WriteLn();
            WriteLn("JRN060I - Start   : " + R.FmtDate(_Start, "dd/mm/yyyy hh:nn:ss"));
            WriteLn("JRN070I - End     : " + R.FmtDate(_Finish, "dd/mm/yyyy hh:nn:ss"));
            WriteLn("JRN080I - Elapsed : " + "           " + R.FmtTimeSpan(_Elapsed, "hh:nn:ss"));
            try
            {
                _StreamWriter.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                _StreamWriter = null;
            }
            if (viewerProgram != string.Empty)
            {
                View(viewerProgram);
            }
        }

        /// <summary>
        /// Terminate the log file without writing any timing messages.
        /// </summary>
        /// <remarks>
        /// The log file viewer program will not be triggered.
        /// </remarks>
        public void Finish()
        {
            Finish(string.Empty);
        }

        /// <summary>
        /// Terminate the log file without writing any timing messages.
        /// </summary>
        /// <remarks>
        /// If suYF18ied the viewer program will be automatically triggered to view the log file.
        /// </remarks>
        public void Finish(string viewerProgram)
        {
            CalculateRunStatistics();
            try
            {
                _StreamWriter.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                _StreamWriter = null;
            }
            if (viewerProgram != string.Empty)
            {
                View(viewerProgram);
            }
        }
        #endregion

		#region Private Methods.
        /// <summary>
        /// Keep track of the highest severity so far encountered.
        /// </summary>
        private void MonitorHighestSeverity(string severity)
        {
            int pos1 = 0;
            int pos2 = 0;
            severity = R.RPad(severity.Trim().ToUpper(), 1, " ");
            pos1 = SEVERITY_CODES.IndexOf(severity);
            if (pos1 > -1)
            {
                pos2 = SEVERITY_CODES.IndexOf(_HighestSeverity);
                if (pos2 > -1)
                {
                    if (pos1 > pos2)
                    {
                        _HighestSeverity = severity;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate run statistics.
        /// </summary>
        private void CalculateRunStatistics()
        {
            _Finish = DateTime.Now;
            _Start = _Start.Subtract(new TimeSpan(0, 0, 0, 0, _Start.Millisecond));
            _Finish = _Finish.Subtract(new TimeSpan(0, 0, 0, 0, _Finish.Millisecond));
            _Elapsed = _Finish.Subtract(_Start);
            if (_Elapsed.Milliseconds >= 500)
            {
                _Elapsed = _Elapsed.Subtract(new TimeSpan(0, 0, 0, 0, _Elapsed.Milliseconds));
                _Elapsed = _Elapsed.Add(new TimeSpan(0, 0, 1));
            }
        }

        /// <summary>
        /// View the log file.
        /// </summary>
        private void View(string viewerProgram)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = viewerProgram;
            proc.StartInfo.Arguments = _Spec;
            proc.EnableRaisingEvents = false;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            proc.Start();
        }
        #endregion
    }
}
