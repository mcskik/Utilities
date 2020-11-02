using Copy9.DataLayer.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Copy9.Models
{
    /// <summary>
    /// Filer class (ADB sdcard).
    /// </summary>
    /// <remarks>
    /// Filer class (ADB sdcard).
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class FilerA : Filer
    {
        const string SDCARD_PREFIX = @"/sdcard/";
        private const string DIRECTORY_LIST_COMMAND_TEMPLATE = @" shell ls -l ""{0}""";
        private const string CREATE_DIRECTORY_COMMAND_TEMPLATE = @" shell mkdir ""{0}""";
        private const string REMOVE_DIRECTORY_COMMAND_TEMPLATE = @" shell rmdir ""{0}""";
        private const string UPLOAD_FILE_COMMAND_TEMPLATE = @" push ""{0}"" ""{1}""";
        private const string DOWNLOAD_FILE_COMMAND_TEMPLATE = @" pull ""{0}"" ""{1}""";
        private const string DELETE_FILE_COMMAND_TEMPLATE = @" shell rm ""{0}""";

        public Connector Connector { get; set; }

        public FilerA(Connector connector)
        {
            Connector = connector;
        }

        public override bool DirectoryExists(string targetDirectory)
        {
            targetDirectory = targetDirectory.Replace(@"\", @"/");
            targetDirectory = Connector.StripDrivePrefix(targetDirectory);
            targetDirectory = targetDirectory.Trim();
            if (!targetDirectory.StartsWith(SDCARD_PREFIX))
            {
                targetDirectory = SDCARD_PREFIX + targetDirectory;
            }
            bool exists = false;
            try
            {
                string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DIRECTORY_LIST_COMMAND_TEMPLATE, targetDirectory);
                CommandLauncher launcher = new CommandLauncher();
                launcher.Run(commandLine);
                List<string> lines = launcher.OutputLines;
                for (int row = 0; row < lines.Count; row++)
                {
                    string line = lines[row];
                    if (line.Trim().Length == 0) continue;
                    if (line.StartsWith("total"))
                    {
                        //Directory. 
                        exists = true;
                        break;
                    }
                    else if (line.StartsWith("/"))
                    {
                        //No such file or directory.
                        exists = false;
                        break;
                    }
                    else if (line.StartsWith("d"))
                    {
                        //Directory. 
                        exists = true;
                        break;
                    }
                    else if (line.ToLower().Contains("no such file or directory"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else if (line.ToLower().Contains("no such file"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else if (line.ToLower().Contains("no such directory"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else
                    {
                        //File.
                        exists = true;
                        break;
                    }
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
            return exists;
        }

        public override void CreateDirectory(string targetDirectory)
        {
            targetDirectory = targetDirectory.Replace(@"\", @"/");
            targetDirectory = Connector.StripDrivePrefix(targetDirectory);
            targetDirectory = targetDirectory.Trim();
            if (!targetDirectory.StartsWith(SDCARD_PREFIX))
            {
                targetDirectory = SDCARD_PREFIX + targetDirectory;
            }
            string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(CREATE_DIRECTORY_COMMAND_TEMPLATE, targetDirectory);
            CommandLauncher launcher = new CommandLauncher();
            launcher.Run(commandLine);
            List<string> lines = launcher.OutputLines;
            int exitCode = launcher.ExitCode;
        }

        public override void RemoveDirectory(string targetDirectory)
        {
            targetDirectory = targetDirectory.Replace(@"\", @"/");
            targetDirectory = Connector.StripDrivePrefix(targetDirectory);
            targetDirectory = targetDirectory.Trim();
            if (!targetDirectory.StartsWith(SDCARD_PREFIX))
            {
                targetDirectory = SDCARD_PREFIX + targetDirectory;
            }
            string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(REMOVE_DIRECTORY_COMMAND_TEMPLATE, targetDirectory);
            CommandLauncher launcher = new CommandLauncher();
            launcher.Run(commandLine);
            List<string> lines = launcher.OutputLines;
            int exitCode = launcher.ExitCode;
        }

        public override bool FileExists(string targetFileSpec)
        {
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = EscapeSpaces(targetFileSpec);
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            bool exists = false;
            try
            {
                string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DIRECTORY_LIST_COMMAND_TEMPLATE, targetFileSpec);
                CommandLauncher launcher = new CommandLauncher();
                launcher.Run(commandLine);
                List<string> lines = launcher.OutputLines;
                for (int row = 0; row < lines.Count; row++)
                {
                    string line = lines[row];
                    if (line.Trim().Length == 0) continue;
                    if (line.StartsWith("total")) continue;
                    string[] parts = line.Split(' ');
                    int index = parts.Length - 1;
                    string fileName = parts[index--];
                    string timeText = parts[index--];
                    string dateText = parts[index--];
                    string sizeText = parts[index--];
                    string dateTimeText = dateText + " " + timeText;
                    if (line.StartsWith("/"))
                    {
                        //No such file or directory.
                        exists = false;
                        break;
                    }
                    else if (line.StartsWith("d"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else if (line.ToLower().Contains("no such file or directory"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else if (line.ToLower().Contains("no such file"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else if (line.ToLower().Contains("no such directory"))
                    {
                        //Directory. 
                        exists = false;
                        break;
                    }
                    else
                    {
                        //File.
                        exists = true;
                    }
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
            return exists;
        }

        public override DateTime FileModifiedDateTime(string targetFileSpec)
        {
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = EscapeSpaces(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            string name = string.Empty;
            DateTime dateTime = DateTime.MinValue;
            try
            {
                string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DIRECTORY_LIST_COMMAND_TEMPLATE, targetFileSpec);
                CommandLauncher launcher = new CommandLauncher();
                launcher.Run(commandLine);
                List<string> lines = launcher.OutputLines;
                for (int row = 0; row < lines.Count; row++)
                {
                    string line = lines[row];
                    if (line.Trim().Length == 0) continue;
                    if (line.StartsWith("total")) continue;
                    string[] parts = line.Split(' ');
                    int index = parts.Length - 1;
                    string fileName = parts[index--];
                    string timeText = parts[index--];
                    string dateText = parts[index--];
                    string sizeText = parts[index--];
                    string dateTimeText = dateText + " " + timeText;
                    if (line.StartsWith("/"))
                    {
                        //No such file or directory.
                    }
                    else if (line.StartsWith("d"))
                    {
                        //Directory. 
                    }
                    else
                    {
                        //File.
                        name = fileName;
                        long size = 0;
                        long.TryParse(sizeText, out size);
                        dateTime = DateTime.MinValue;
                        DateTime.TryParse(dateTimeText, out dateTime);
                    }
                }
            }
            catch (Exception oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Message);
            }
            finally
            {
            }
            return dateTime;
        }

        public override void CopyFile(string sourceFileSpec, string targetFileSpec)
        {
            sourceFileSpec = sourceFileSpec.Replace(@"\", @"/");
            sourceFileSpec = Connector.StripDrivePrefix(sourceFileSpec);
            sourceFileSpec = sourceFileSpec.Trim();
            if (!sourceFileSpec.StartsWith(SDCARD_PREFIX))
            {
                sourceFileSpec = SDCARD_PREFIX + sourceFileSpec;
            }
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            //TODO: At present we will confine the capability to uploading and downloading to and from sdcard, not copying between two locations on the sdcard.
        }

        public override void UploadFile(string sourceFileSpec, string targetFileSpec)
        {
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(UPLOAD_FILE_COMMAND_TEMPLATE, sourceFileSpec, targetFileSpec);
            CommandLauncher launcher = new CommandLauncher();
            launcher.Run(commandLine);
            List<string> lines = launcher.OutputLines;
            int exitCode = launcher.ExitCode;
        }

        public override void DownloadFile(string sourceFileSpec, string targetFileSpec)
        {
            sourceFileSpec = sourceFileSpec.Replace(@"\", @"/");
            sourceFileSpec = Connector.StripDrivePrefix(sourceFileSpec);
            sourceFileSpec = sourceFileSpec.Trim();
            if (!sourceFileSpec.StartsWith(SDCARD_PREFIX))
            {
                sourceFileSpec = SDCARD_PREFIX + sourceFileSpec;
            }
            string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DOWNLOAD_FILE_COMMAND_TEMPLATE, sourceFileSpec, targetFileSpec);
            CommandLauncher launcher = new CommandLauncher();
            launcher.Run(commandLine);
            List<string> lines = launcher.OutputLines;
            int exitCode = launcher.ExitCode;
        }

        public override void DeleteFile(string targetFileSpec)
        {
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = EscapeSpaces(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DELETE_FILE_COMMAND_TEMPLATE, targetFileSpec);
            CommandLauncher launcher = new CommandLauncher();
            launcher.Run(commandLine);
            List<string> lines = launcher.OutputLines;
            int exitCode = launcher.ExitCode;
        }

        public override void PathCheck(string targetFileSpec)
        {
            targetFileSpec = targetFileSpec.Replace(@"\", @"/");
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            targetFileSpec = targetFileSpec.Trim();
            if (!targetFileSpec.StartsWith(SDCARD_PREFIX))
            {
                targetFileSpec = SDCARD_PREFIX + targetFileSpec;
            }
            //Do nothing at the moment as the FilerG manages OK without this.
            //The synchronize engine should generate CreateDirectory requests for all necessary directories.
        }

        public override String GetDirectoryName(string fileSpec)
        {
            fileSpec = fileSpec.Replace(@"\", @"/");
            fileSpec = Connector.StripDrivePrefix(fileSpec);
            fileSpec = fileSpec.Trim();
            if (!fileSpec.StartsWith(SDCARD_PREFIX))
            {
                fileSpec = SDCARD_PREFIX + fileSpec;
            }
            String folder = fileSpec.Trim();
            int pos = folder.LastIndexOf(@"/");
            if (pos != -1)
            {
                folder = folder.Substring(0, pos);
            }
            return folder;
        }

        public override String GetFileName(string fileSpec)
        {
            fileSpec = fileSpec.Replace(@"\", @"/");
            fileSpec = Connector.StripDrivePrefix(fileSpec);
            fileSpec = fileSpec.Trim();
            if (!fileSpec.StartsWith(SDCARD_PREFIX))
            {
                fileSpec = SDCARD_PREFIX + fileSpec;
            }
            String fileName = fileSpec.Trim();
            if (fileName.EndsWith(@"/"))
            {
                fileName = string.Empty;
            }
            else
            {
                int pos = fileName.LastIndexOf(@"/");
                if (pos != -1)
                {
                    fileName = fileName.Substring(pos + 1);
                }
            }
            return fileName;
        }

        private string EscapeSpaces(string spec)
        {
            const char SPACE = ' ';
            const char OPEN_BRACKET = '(';
            const char CLOSE_BRACKET = ')';
            StringBuilder sb = new StringBuilder();
            if (spec.Length > 0)
            {
                char[] letters = spec.ToCharArray();
                foreach (char letter in letters)
                {
                    if (letter == SPACE)
                    {
                        sb.Append(@"\").Append(letter);
                    }
                    else if (letter == OPEN_BRACKET)
                    {
                        sb.Append(@"\").Append(letter);
                    }
                    else if (letter == CLOSE_BRACKET)
                    {
                        sb.Append(@"\").Append(letter);
                    }
                    else
                    {
                        sb.Append(letter);
                    }
                }
            }
            return sb.ToString();
        }
    }
}