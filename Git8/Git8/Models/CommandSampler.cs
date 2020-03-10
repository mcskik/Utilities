using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Git8.Models
{
    /// <summary>
    /// This class is not used but it contains a number of different samples of how to run cmd.exe and git.exe
    /// in its own process while trapping stdin, stdout, and stderr.
    /// This class is being kept so that the unused code can be removed from the Commander class to clean it up.
    /// </summary>
    public class CommandSampler
    {
        private const string GIT_INSTALLED_DIRECTORY = @"C:\Users\K\AppData\Local\Programs\Git\bin\";
        private const string GIT_EXECUTABLE = GIT_INSTALLED_DIRECTORY + @"git.exe";
        private const string GIT_REPOSITORY_PATH = @"C:\_Bitbucket\FA";
        private StringBuilder _Sb;

        public CommandSampler()
        {
        }

        public void Run(string command)
        {
            //RunGitCommand(GIT_REPOSITORY_PATH, command);
            string output = RunGitCommand(command);
            //string output = RunCommand(command);
            //output = output.Replace("\n", "\r");
            //rtxOutput.Text = output;
            //rtxOutput.Text = _Sb.ToString();
            //rtxOutput.Text = CommandLine.Execute(GIT_EXECUTABLE, command);
        }

        public void RunGitCommand(string repositoryPath, string gitArguments)
        {
            _Sb = new StringBuilder();
            if (null == GIT_EXECUTABLE || String.IsNullOrWhiteSpace(GIT_EXECUTABLE))
            {
                throw new FileNotFoundException("Unable to find git.exe on your system PATH.");
            }
            // gitArguments contains the command to run (e.g. "clone -- git@repo:projectName c:\repositories\repo_a8c0dd321f")
            var startInfo = new ProcessStartInfo(GIT_EXECUTABLE, gitArguments)
            {
                WorkingDirectory = (null != repositoryPath && Directory.Exists(repositoryPath)) ? repositoryPath : String.Empty,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };
            using (var p = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = startInfo
            })
            {
                p.OutputDataReceived += (sender, args) => LogDebug(args.Data);
                p.ErrorDataReceived += (sender, args) => LogDebug(args.Data);
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }

        private void LogDebug(string data)
        {
            _Sb.AppendLine(data);
        }

        /// <summary>
        /// !!! This is the one being used at present !!!
        /// </summary>
        private string RunGitCommand(string command)
        {
            bool needsInput = false;
            List<string> commandsWithInput = new List<string>();
            commandsWithInput.Add("pull");
            commandsWithInput.Add("push");
            foreach (var cmd in commandsWithInput)
            {
                if (command.Contains(cmd))
                {
                    needsInput = true;
                }
            }
            StringBuilder output = new StringBuilder();
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            //Can't use CreateNoWindow if the command needs input like a password as we need the window to open to allow input of the password.
            gitInfo.CreateNoWindow = !needsInput;
            gitInfo.UseShellExecute = false;
            //gitInfo.RedirectStandardInput = true; // Can't get this redirection to work so that I can automate inputting the password.
            gitInfo.RedirectStandardOutput = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.FileName = GIT_EXECUTABLE;
            Process gitProcess = new Process();
            gitInfo.Arguments = command;
            gitInfo.WorkingDirectory = GIT_REPOSITORY_PATH;
            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();
            while (!gitProcess.StandardOutput.EndOfStream)
            {
                string line = gitProcess.StandardOutput.ReadLine();
                output.AppendLine(line);
            }
            while (!gitProcess.StandardError.EndOfStream)
            {
                output.AppendLine(gitProcess.StandardError.ReadLine());
            }
            gitProcess.WaitForExit();
            gitProcess.Close();
            return output.ToString();
        }

        private string RunCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe");
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            //redirect all standard inout to program
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            //create the process with above infor and start it
            Process plinkProcess = new Process();
            plinkProcess.StartInfo = psi;
            plinkProcess.Start();
            //link the streams to standard inout of process
            StreamWriter inputWriter = plinkProcess.StandardInput;
            StreamReader outputReader = plinkProcess.StandardOutput;
            StreamReader errorReader = plinkProcess.StandardError;
            //send command to cmd prompt and wait for command to execute with thread sleep
            inputWriter.WriteLine(command + Environment.NewLine);
            Thread.Sleep(2000);
            // flush the input stream before sending exit command to end process for any unwanted characters
            inputWriter.Flush();
            inputWriter.WriteLine("exit\r\n");
            // read till end the stream into string
            string stdOut = outputReader.ReadToEnd();
            string stdErr = outputReader.ReadToEnd();
            string output = stdOut + Environment.NewLine + stdErr;
            //remove the part of string which is not needed
            //MessageBox.Show(output);
            return output;
        }

        private void test_cmd()
        {
            string strOutput;
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe");
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            //redirect all standard inout to program
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            //create the process with above infor and start it
            Process plinkProcess = new Process();
            plinkProcess.StartInfo = psi;
            plinkProcess.Start();
            //link the streams to standard inout of process
            StreamWriter inputWriter = plinkProcess.StandardInput;
            StreamReader outputReader = plinkProcess.StandardOutput;
            StreamReader errorReader = plinkProcess.StandardError;
            //send command to cmd prompt and wait for command to execute with thread sleep
            inputWriter.WriteLine("dir\r\n");
            Thread.Sleep(2000);
            // flush the input stream before sending exit command to end process for any unwanted characters
            inputWriter.Flush();
            inputWriter.WriteLine("exit\r\n");
            // read till end the stream into string
            strOutput = outputReader.ReadToEnd();
            //remove the part of string which is not needed
            //MessageBox.Show(strOutput);
        }
    }
}