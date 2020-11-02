using Copy9.DataLayer.Profile;
using Copy9.Models;
using ProfileData.Models.Extenders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Copy9
{
    /// <summary>
    /// Directory listing class (ADB sdcard).
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class DirectoryEngineA : DirectoryEngine
    {
        private const string DIRECTORY_LIST_COMMAND_TEMPLATE = @" shell ls -l {0}";
        public Connector Connector { get; set; }
        public Filer Filer { get; set; }
        public List<DirectoryEntry> Dirs { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DirectoryEngineA(Connector connector)
        {
            Connector = connector;
            Filer = new FilerA(Connector);
        }

        /// <summary>
        /// Scan directory information into memory resident directory listing object.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        /// <returns>List of directory entries.</returns>
        public override DirectoryEntries DirList(string qualifier, ref long estimate)
        {
            const string SDCARD_PREFIX = @"/sdcard/";
            List<string> dirs = new List<string>();
            string directory = string.Empty;
            long count;
            long entries;
            SignalBeginProgress("DirList");
            try
            {
                this.directoryListing = new DirectoryEntries();
                //Build directory and file list.
                dirs = new List<string>();
                qualifier = qualifier.Trim();
                qualifier = Connector.StripDrivePrefix(qualifier);
                qualifier = qualifier.Trim();
                qualifier = qualifier.Strip(StringExtender.StripMode.Trailing, Path.DirectorySeparatorChar.ToString());
                qualifier = SDCARD_PREFIX + qualifier;
                dirs.Add(qualifier);
                estimate = 1;
                count = 1;
                entries = dirs.Count;
                while (count <= entries)
                {
                    directory = dirs[(int)count - 1].ToString();
                    DirLevel(new DirectoryEntry(), qualifier, directory, ref dirs, ref estimate);
                    count++;
                    entries = dirs.Count;
                    if (Interrupt.Reason == "Cancel")
                    {
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
                SignalEndOfProgress("DirList");
            }
            return directoryListing;
        }

        /// <summary>
        /// Append directory level information into the specified DataTable.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="dirs">Work in progress collection of directories.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        public override void DirLevel(DirectoryEntry parentEntry, string qualifier, string directory, ref List<string> dirs, ref long estimate)
        {
            string spec;
            string path;
            string name;
            path = directory;
            if (directory.Length > qualifier.Length)
            {
                directory = directory.Substring(qualifier.Length);
            }
            else
            {
                directory = string.Empty;
            }
            try
            {
                if (DirectoryExclusionsHelper.AllowDirectory(path))
                {
                    string commandLine = Administrator.ProfileManager.SystemProfile.ADB + string.Format(DIRECTORY_LIST_COMMAND_TEMPLATE, path);
                    CommandLauncher launcher = new CommandLauncher();
                    launcher.Run(commandLine);
                    List<string> lines = launcher.OutputLines;
                    for (int row = 0; row < lines.Count; row++)
                    {
                        string line = lines[row];
                        if (line.Trim().Length == 0) continue;
                        if (line.StartsWith("total")) continue;
                        string fileName = string.Empty;
                        string timeText = string.Empty;
                        string dateText = string.Empty;
                        string sizeText = string.Empty;
                        string dateTimeText = string.Empty;
                        string[] parts;
                        int index = -1;
                        if (line.Contains(":"))
                        {
                            int pos = line.IndexOf(":");
                            pos = line.IndexOf(" ", pos);
                            fileName = line.Substring(pos);
                            fileName = fileName.Trim();
                            line = line.Substring(0, pos);
                            parts = line.Split(' ');
                            index = parts.Length - 1;
                            timeText = parts[index--];
                            dateText = parts[index--];
                            sizeText = parts[index--];
                            dateTimeText = dateText + " " + timeText;
                        }
                        else
                        {
                            parts = line.Split(' ');
                            index = parts.Length - 1;
                            fileName = parts[index--];
                            timeText = parts[index--];
                            dateText = parts[index--];
                            sizeText = parts[index--];
                            dateTimeText = dateText + " " + timeText;
                        }
                        if (line.StartsWith("/"))
                        {
                            //No such file or directory.
                        }
                        else if (line.StartsWith("d"))
                        {
                            //Directory. 
                            try
                            {
                                name = fileName;
                                spec = path + "/" + name;
                                if (DirectoryExclusionsHelper.AllowDirectory(spec))
                                {
                                    dirs.Add(spec);
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = 0;
                                    entry.StdDate = System.DateTime.Parse("01/01/2000");
                                    entry.SourceDate = entry.StdDate;
                                    entry.TargetDate = entry.StdDate;
                                    entry.SourceFile = entry.StdFile;
                                    entry.TargetFile = entry.StdFile;
                                    entry.StdType = "dir";
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList", entry.StdSize);
                                }
                            }
                            catch (Exception oExceptionA)
                            {
                                System.Diagnostics.Debug.WriteLine(oExceptionA.Message);
                            }
                            if (Interrupt.Reason == "Cancel")
                            {
                                break;
                            }
                        }
                        else
                        {
                            //File.
                            try
                            {
                                name = fileName;
                                spec = path + "/" + name;
                                string ext = string.Empty;
                                int pos = fileName.LastIndexOf(".");
                                if (pos > -1)
                                {
                                    ext = fileName.Substring(pos);
                                }
                                if (ext.StartsWith("."))
                                {
                                    if (ext.Length > 1)
                                    {
                                        ext = ext.Substring(1);
                                    }
                                    else
                                    {
                                        ext = string.Empty;
                                    }
                                }
                                //if (MonitoredTypesHelper.AllowFile(spec, monitoredTypesOnly))
                                if (true)
                                {
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    long size = 0;
                                    long.TryParse(sizeText, out size);
                                    entry.StdSize = size;
                                    DateTime dateTime = DateTime.MinValue;
                                    DateTime.TryParse(dateTimeText, out dateTime);
                                    entry.StdDate = dateTime;
                                    entry.SourceDate = entry.StdDate;
                                    entry.TargetDate = entry.StdDate;
                                    entry.SourceFile = entry.StdFile;
                                    entry.TargetFile = entry.StdFile;
                                    entry.StdType = ext;
                                    entry.CtlComparison = string.Empty;
                                    directoryListing.Add(entry);
                                    estimate += entry.StdSize;
                                    SignalUpdateProgress("DirList", entry.StdSize);
                                }
                            }
                            catch (Exception exceptionB)
                            {
                                System.Diagnostics.Debug.WriteLine(exceptionB.Message);
                            }
                            if (Interrupt.Reason == "Cancel")
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException oExceptionNF)
            {
                System.Diagnostics.Debug.WriteLine(oExceptionNF.Message);
            }
            finally
            {
            }
        }
    }
}