using ProfileData.Models.Extenders;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace Copy9
{
    /// <summary>
    /// Directory listing class (Windows/Lan file system).
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class DirectoryEngineL : DirectoryEngine
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DirectoryEngineL()
        {
        }

        /// <summary>
        /// Scan directory information into memory resident directory listing object.
        /// </summary>
        /// <param name="qualifier">Top level directory.</param>
        /// <param name="estimate">Number of directory entries found. Used for future estimating.</param>
        /// <returns>List of directory entries.</returns>
        public override DirectoryEntries DirList(string qualifier, ref long estimate)
        {
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
                qualifier = qualifier.Strip(StringExtender.StripMode.Trailing, Path.DirectorySeparatorChar.ToString());
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
            catch (OleDbException oException)
            {
                System.Diagnostics.Debug.WriteLine(oException.Errors[0].Message);
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
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Refresh();
                if (DirectoryExclusionsHelper.AllowDirectory(path))
                {
                    DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                    for (int row = 0; row < directoryInfoArray.Length; row++)
                    {
                        try
                        {
                            name = directoryInfoArray[row].Name;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (DirectoryExclusionsHelper.AllowDirectory(spec))
                            {
                                if (directoryInfoArray[row].Exists)
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
                        }
                        catch (OleDbException oExceptionA)
                        {
                            System.Diagnostics.Debug.WriteLine(oExceptionA.Errors[0].Message);
                        }
                        if (Interrupt.Reason == "Cancel")
                        {
                            break;
                        }
                    }
                    FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                    for (int row = 0; row < fileInfoArray.Length; row++)
                    {
                        try
                        {
                            name = fileInfoArray[row].Name;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (fileInfoArray[row].Exists)
                            {
                                string ext = Path.GetExtension(spec);
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
                                if (MonitoredTypesHelper.AllowFile(spec, monitoredTypesOnly))
                                {
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = fileInfoArray[row].Length;
                                    entry.StdDate = fileInfoArray[row].LastWriteTime;
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
                        }
                        catch (OleDbException exceptionB)
                        {
                            System.Diagnostics.Debug.WriteLine(exceptionB.Errors[0].Message);
                        }
                        if (Interrupt.Reason == "Cancel")
                        {
                            break;
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