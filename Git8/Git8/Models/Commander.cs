using Git8.DataLayer.Profile;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Git8.Models
{
    /// <summary>
    /// Commander class.
    /// </summary>
    /// <remarks>
    /// This class contains all the code to run a git command in its own process while trapping stdin, stdout, and stderr.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class Commander
    {
        /// <summary>
        /// Run the specified git command.
        /// </summary>
        public static string RunGitCommand(string command)
        {
            if (command.StartsWith("git") || command.StartsWith("GIT"))
            {
                if (command.Length > 3)
                {
                    command = command.Substring(3).Trim();
                }
            }
            bool needsInput = NeedsInput(command);
            StringBuilder output = new StringBuilder();
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            //Can't use CreateNoWindow if the command needs input like a password as we need the window to open to allow input of the password.
            gitInfo.CreateNoWindow = !needsInput;
            gitInfo.UseShellExecute = false;
            //gitInfo.RedirectStandardInput = true; // Can't get this redirection to work so that I can automate inputting the password.
            gitInfo.RedirectStandardOutput = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.FileName = Administrator.ProfileManager.SystemProfile.Git;
            Process gitProcess = new Process();
            gitInfo.Arguments = command;
            gitInfo.WorkingDirectory = Administrator.ProfileManager.RepositorySettings.SelectedItem.LocalPath;
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

        /// <summary>
        /// Determine whether the specified git command needs to allow input from stdin.
        /// This is necessary because we can't use CreateNoWindow if the command needs input like a password
        /// as we need the window to open to allow input of the password.
        /// </summary>
        private static bool NeedsInput(string command)
        {
            bool needsInput = false;
            List<string> commandsWithInput = Administrator.ProfileManager.SystemProfile.CommandsWithInput;
            foreach (var cmd in commandsWithInput)
            {
                if (command.Contains(cmd))
                {
                    needsInput = true;
                }
            }
            return needsInput;
        }
    }
}