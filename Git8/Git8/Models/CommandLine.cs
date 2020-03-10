using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git8.Models
{
    public static class CommandLine
    {
        private const string GIT_INSTALLED_DIRECTORY = @"C:\Program Files (x86)\Git\bin\";
        private const string GIT_EXECUTABLE = GIT_INSTALLED_DIRECTORY + @"git.exe";
        private const string GIT_REPOSITORY_PATH = @"C:\_BK\FA\";
        private static StringBuilder _Sb;

        /// <summary>
        /// Run console command in C#
        /// </summary>
        public static string Execute(string executable,
            string arguments,
            bool standardOutput = true,
            bool standardError = true,
            bool throwOnError = true)
        {
            // This will be out return string
            string standardOutputString = string.Empty;
            string standardErrorString = string.Empty;

            // Use process
            Process process;
            try
            {
                // Setup our process with the executable and it's arguments
                process = new Process();
                process.StartInfo = new ProcessStartInfo(executable, arguments);

                // To get IO streams set use shell to false
                process.StartInfo.UseShellExecute = false;

                process.StartInfo.WorkingDirectory = GIT_REPOSITORY_PATH;

                // If we want to return the output then redirect standard output
                if (standardOutput) process.StartInfo.RedirectStandardOutput = true;

                // If we std error or to throw on error then redirect error
                if (standardError || throwOnError) process.StartInfo.RedirectStandardError = true;

                // Run the process
                process.Start();

                // Get the standard error
                if (standardError || throwOnError) standardErrorString = process.StandardError.ReadToEnd();

                // If we want to throw on error and there is an error
                if (throwOnError && !string.IsNullOrEmpty(standardErrorString))
                    throw new Exception(
                        string.Format("Error in ConsoleCommand while executing {0} with arguments {1}.",
                        executable, arguments, Environment.NewLine, standardErrorString));

                // If we want to return the output then get it
                if (standardOutput) standardOutputString = process.StandardOutput.ReadToEnd();

                // If we want standard error then append it to our output string
                if (standardError) standardOutputString += standardErrorString;

                // Wait for the process to finish
                process.WaitForExit();
            }
            catch (Exception e)
            {
                // Encapsulate and throw
                throw new Exception(
                    string.Format("Error in ConsoleCommand while executing {0} with arguments {1}.", executable, arguments), e);
            }

            // Return the output string
            return standardOutputString;
        }
    }
}