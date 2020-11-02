using Copy9.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Copy9.Models
{
    /// <summary>
    /// Command launcher base class.
    /// </summary>
    /// <remarks>
    /// This runs a program from the command line redirecting and trapping the standard inputs and outputs.
    /// The main reason for writing this class was to explore the feasibility of capturing success, error, or exception information
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class CommandLauncher
    {
        public delegate void OutputEventHandler(object sender, CommandEventArgs e);
        public event OutputEventHandler StdOutReceived;
        public event OutputEventHandler StdErrReceived;
        private List<String> _outputLines;
        private StringBuilder _outputAll;
        private StringBuilder _outputStdOut;
        private StringBuilder _outputStdErr;
        private Process _proc;
        public string CommandLine { get; set; }
        public int ExitCode { get; set; }
        public string OutcomeMessage { get; set; }
        public List<string> OutputLines
        {
            get
            {
                return _outputLines;
            }
        }
        public string OutputAll
        {
            get
            {
                return _outputAll.ToString();
            }
        }
        public string OutputStdOut
        {
            get
            {
                return _outputStdOut.ToString();
            }
        }
        public string OutputStdErr
        {
            get
            {
                return _outputStdErr.ToString();
            }
        }

        public CommandLauncher()
        {
        }

        public void Run(string commandLine)
        {
            CommandLine = commandLine;
            try
            {
                _outputLines = new List<string>();
                _outputAll = new StringBuilder();
                _outputStdOut = new StringBuilder();
                _outputStdErr = new StringBuilder();
                _proc = new Process();
                //This method runs the command prompt with "/C" with the specified command line arguments.
                //"/C" instructs the command prompt to terminate when the command line has finished executing.
                _proc.StartInfo.FileName = Administrator.ProfileManager.SystemProfile.CMD;
                _proc.StartInfo.Arguments = "/C " + CommandLine;
                _proc.EnableRaisingEvents = true;
                _proc.StartInfo.CreateNoWindow = true;
                _proc.StartInfo.UseShellExecute = false;
                _proc.StartInfo.RedirectStandardInput = true;
                _proc.StartInfo.RedirectStandardOutput = true;
                _proc.StartInfo.RedirectStandardError = true;
                _proc.Exited += new EventHandler(_proc_Exited);
                _proc.Start();
                _proc.OutputDataReceived += _proc_StdOutReceived;
                _proc.ErrorDataReceived += _proc_StdErrReceived;
                _proc.BeginOutputReadLine();
                try
                {
                    _proc.BeginErrorReadLine();
                }
                catch (Exception exi)
                {
                    string message = FormatError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Process failed during start", exi);
                }
                _proc.WaitForExit();
                //The method below inputs commnads via standard input.
                //Unfortunately there is no easy way to know when any batch file is finished executing.
                //Exec(_jobDetails.ProgramArgs);
                //Close(true);
            }
            catch (Exception ex)
            {
                string message = FormatError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Process failed during start", ex);
                ExitCode = 999;
                OutcomeMessage = message;
            }
        }

        /// <summary>
        /// Propogate event when standard output data has been received.
        /// </summary>
        private void _proc_StdOutReceived(object sender, DataReceivedEventArgs output)
        {
            if (output.Data == null)
            {
                return;
            }
            else
            {
                _outputLines.Add(output.Data);
                _outputAll.AppendLine(output.Data);
                _outputStdOut.AppendLine(output.Data);
                if (StdOutReceived != null)
                {
                    CommandEventArgs e = new CommandEventArgs();
                    e.Output = output.Data;
                    StdOutReceived(this, e);
                }
            }
        }

        /// <summary>
        /// Propogate event when standard error data has been received.
        /// </summary>
        private void _proc_StdErrReceived(object sender, DataReceivedEventArgs output)
        {
            if (output.Data == null)
            {
                return;
            }
            else
            {
                _outputLines.Add(output.Data);
                _outputAll.AppendLine(output.Data);
                _outputStdErr.AppendLine(output.Data);
                if (StdErrReceived != null)
                {
                    CommandEventArgs e = new CommandEventArgs();
                    e.Output = output.Data;
                    StdErrReceived(this, e);
                }
            }
        }

        /// <summary>
        /// This event handler is called when the process exits.
        /// </summary>
        private void _proc_Exited(object sender, EventArgs e)
        {
            try
            {
                if (_proc != null)
                {
                    ExitCode = _proc.ExitCode;
                    if (_proc.ExitCode == 0)
                    {
                        OutcomeMessage = String.Format(@"Success: {0}", _outputAll.ToString());
                    }
                    else
                    {
                        OutcomeMessage = String.Format(@"Error: {0}", _outputAll.ToString());
                    }
                    _proc.Close();
                }
                else
                {
                    ExitCode = 999;
                    OutcomeMessage = "Process is null";
                }
            }
            catch (Exception ex)
            {
                string message = FormatError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Process failed during exit", ex);
                ExitCode = 999;
                OutcomeMessage = message;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Execute command.
        /// </summary>
        /// <remarks>
        /// This method executes a command by essentially typing the command into a command shell.
        /// This is not being used because it is difficult to tell when the command has finished executing.
        /// </remarks>
        private void Exec(string command)
        {
            string check = command.ToLower();
            if (!check.StartsWith("call"))
            {
                command = "Call " + command;
            }
            _proc.StandardInput.WriteLine(command);
            _proc.StandardInput.WriteLine("Exit");
        }

        /// <summary>
        /// Close command prompt.
        /// </summary>
        private void Close(bool waitForExit)
        {
            if (_proc != null)
            {
                _proc.StandardInput.WriteLine("exit");
                if (waitForExit)
                {
                    _proc.WaitForExit();
                }
                _proc.Close();
            }
        }

        protected string FormatError(string objectName, string methodName, string errorMessage, Exception ex)
        {
            string message = String.Format("[Object:] {1} [Method:] {2} [Error:] {3}{0}{0}[Exception:]{0}{4}{0}[StackTrace:]{0}{5}{0}{0}", Environment.NewLine, objectName, methodName, errorMessage, ex.Message, ex.StackTrace);
            return message;
        }
    }

    /// <summary>
    /// Used to pass StdOut and StdErr output data.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        public string Output { get; internal set; }
    }
}