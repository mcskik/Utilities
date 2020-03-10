using System.Collections.Generic;

namespace Git8.Models
{
    /// <summary>
    /// Parser class.
    /// </summary>
    /// <remarks>
    /// This class contains all the methods to fields out of the command response.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class Parser
    {
        private const string GIT_STATUS = @"git status";
        private const string NEW_FILE = @"new file:";
        private const string MODIFIED = @"modified:";
        private const string DELETED = @"deleted:";

        public static List<string> ParseModifieddFiles(string[] outputLines)
        {
            List<string> fileSpecs = new List<string>();
            bool isStatusCommand = false;
            string line = string.Empty;
            foreach (string outputLine in outputLines)
            {
                line = outputLine.Trim();
                if (line.Length > 0)
                {
                    if (line.Contains(GIT_STATUS))
                    {
                        isStatusCommand = true;
                        break;
                    }
                }
            }
            if (isStatusCommand)
            {
                foreach (string outputLine in outputLines)
                {
                    line = outputLine.Trim();
                    line = line.Replace("\t", string.Empty);
                    if (line.Length > 0)
                    {
                        if (line.StartsWith(NEW_FILE) || line.StartsWith(MODIFIED) || line.StartsWith(DELETED))
                        {
                            line = line.Substring(NEW_FILE.Length);
                            line = line.Trim();
                            fileSpecs.Add(line);
                        }
                    }
                }
            }
            fileSpecs.Reverse();
            return fileSpecs;
        }
    }
}