using Copy9.Models;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using ProfileData.DataLayer.Profile;
using ProfileData.Models.Extenders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace Copy9
{
    /// <summary>
    /// Directory listing class (Google Drive).
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class DirectoryEngineG : DirectoryEngine
    {
        public Connector Connector { get; set; }
        public Filer Filer { get; set; }
        public List<DirectoryEntry> Dirs { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DirectoryEngineG(Connector connector)
        {
            Connector = connector;
            Filer = new FilerG(Connector);
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
            Dirs = new List<DirectoryEntry>();
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
                dirs.Add(qualifier);
                estimate = 1;
                count = 1;
                Google.Apis.Drive.v3.Data.File parentDriveEntry = Connector.GetSpecificDriveEntry(qualifier);
                DirectoryEntry parentEntry = new DirectoryEntry(parentDriveEntry);
                if (parentEntry != null)
                {
                    Dirs.Add(parentEntry);
                    entries = Dirs.Count;
                    while (count <= entries)
                    {
                        directory = dirs[(int)count - 1].ToString();
                        parentEntry = Dirs[(int)count - 1];
                        DirLevel(parentEntry, qualifier, directory, ref dirs, ref estimate);
                        count++;
                        entries = dirs.Count;
                        if (Interrupt.Reason == "Cancel")
                        {
                            break;
                        }
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
                //TODO: Google Drive Instead of File System.
                if (DirectoryExclusionsHelper.AllowDirectory(path))
                {
                    List<DirectoryEntry> driveEntries = GetDriveEntriesInFolder(parentEntry.EntryId);
                    for (int row = 0; row < driveEntries.Count; row++)
                    {
                        DirectoryEntry driveEntry = driveEntries[row];
                        if (driveEntry.MimeType == Connector.FOLDER_MIME_TYPE)
                        {
                            name = driveEntry.EntryName;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (DirectoryExclusionsHelper.AllowDirectory(spec))
                            {
                                if (Filer.DirectoryExists(spec))
                                {
                                    dirs.Add(spec);
                                    Dirs.Add(driveEntry);
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
                        else
                        {
                            name = driveEntry.EntryName;
                            spec = path + Path.DirectorySeparatorChar.ToString() + name;
                            if (Filer.FileExists(spec))
                            {
                                string ext = Path.GetExtension(spec);
                                if (ext.StartsWith(Connector.FULL_STOP))
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
                                if (MonitoredTypesHelperG.AllowFile(spec, monitoredTypesOnly))
                                {
                                    DirectoryEntry entry = new DirectoryEntry();
                                    entry.StdHlq = qualifier.Trim();
                                    entry.StdDir = directory.Trim();
                                    entry.StdFile = name.Trim();
                                    entry.StdSize = driveEntry.StdSize;
                                    entry.StdDate = driveEntry.StdDate;
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

        public List<DirectoryEntry> GetDriveEntriesInFolder(string folderId)
        {
            //string searchCriteria = String.Format(@"trashed = false and {0} in parents", folderId);
            // string searchCriteria = String.Format(@"parent='{0}' and trashed=false", folderId);
            //string searchCriteria = String.Format(@"'{0}' in parents and trashed=false", folderId);
            string searchCriteria = String.Format(Connector.DRIVE_FOLDER_ID_SEARCH_CRITERIA_TEMPLATE, folderId);
            List<DirectoryEntry> driveEntries = GetMatchingDriveEntries(searchCriteria);
            return driveEntries;
        }

        public List<DirectoryEntry> GetMatchingDriveEntries(string searchCriteria)
        {
            List<DirectoryEntry> entries = new List<DirectoryEntry>();
            try
            {
                FilesResource.ListRequest list = Connector.DriveService.Files.List();
                list.PageSize = Connector.PAGE_SIZE;
                if (searchCriteria != null)
                {
                    list.Q = searchCriteria;
                    list.Fields = Connector.DRIVE_QUERY_FIELDS;
                    list.Spaces = Connector.DRIVE_QUERY_SPACES;
                }
                FileList filesFeed = list.Execute();
                //// Loop through until we arrive at an empty page.
                while (filesFeed.Files != null)
                {
                    foreach (Google.Apis.Drive.v3.Data.File item in filesFeed.Files)
                    {
                        DirectoryEntry f = new DirectoryEntry();
                        f.EntryId = item.Id;
                        f.EntryName = item.Name;
                        f.EntryPath = Connector.GetAbsPath(item);
                        f.MimeType = item.MimeType;
                        f.StdSize = item.Size ?? 0;
                        f.StdType = item.FileExtension ?? Connector.DIR_EXTENSION;
                        f.StdDate = item.ModifiedTime ?? DateTime.MinValue;
                        entries.Add(f);
                    }
                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }
                    // Prepare the next page of results
                    list.PageToken = filesFeed.NextPageToken;
                    // Execute and process the next page request
                    filesFeed = list.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return entries;
        }
    }
}