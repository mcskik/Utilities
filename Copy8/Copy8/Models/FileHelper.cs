using ProfileData.Models.Extenders;
using System.IO;

namespace Copy8
{
    /// <summary>
    /// File helper class. (Plan to combine this with ProfileData FileHelper once stable.)
    /// </summary>
    /// <remarks>
    /// Contains methods which are commonly used to manipulate files.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class FileHelper
    {
        /// <summary>
        /// Returns cross platform directory path separator.
        /// </summary>
        public static string Separator()
        {
            return Path.DirectorySeparatorChar.ToString();
        }

        /// <summary>
        /// Returns cross platform directory path double separator.
        /// </summary>
        public static string DoubleSeparator()
        {
            return Separator() + Separator();
        }

        /// <summary>
        /// Return everything except the file name and extension from the file specification.
        /// </summary>
        public static string FileStem(string fileSpec)
        {
            int pos;
            string fileStem;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length == 0)
            {
                fileStem = string.Empty;
            }
            else
            {
                pos = fileSpec.LastIndexOf(Separator());
                if (pos > -1)
                {
                    fileStem = fileSpec.Substring(0, pos + 1);
                }
                else
                {
                    fileStem = string.Empty;
                }
            }
            return fileStem;
        }

        /// <summary>
        /// Return everything except the drive specification, UNC server, and file name plus ext.
        /// </summary>
        public static string FilePath(string fileSpec)
        {
            string server = string.Empty;
            string drive = string.Empty;
            string filePath = string.Empty;
            int pos = 0;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length == 0)
            {
                filePath = string.Empty;
            }
            else
            {
                pos = fileSpec.LastIndexOf(Separator());
                if (pos == -1)
                {
                    filePath = string.Empty;
                }
                else if (pos == 0)
                {
                    filePath = Separator();
                }
                else
                {
                    filePath = fileSpec.Substring(0, pos + 1);
                }
            }
            return filePath;
        }

        /// <summary>
        /// Return server name from a UNC file specification, otherwise an empty string.
        /// </summary>
        public static string WindowsFileServer(string fileSpec)
        {
            int pos;
            string fileServer;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length < 3)
            {
                fileServer = string.Empty;
            }
            else if (fileSpec.Substring(0, 2) == DoubleSeparator())
            {
                pos = fileSpec.IndexOf(Separator(), 2);
                if (pos > -1)
                {
                    fileServer = fileSpec.Substring(0, pos);
                }
                else
                {
                    fileServer = fileSpec;
                }
                fileServer = fileServer.Substring(2);
            }
            else
            {
                fileServer = string.Empty;
            }
            return fileServer;
        }

        /// <summary>
        /// Return drive letter from a non-UNC file specification, otherwise an empty string.
        /// </summary>
        public static string WindowsFileDrive(string fileSpec)
        {
            string fileDrive;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length < 2)
            {
                fileDrive = string.Empty;
            }
            else if (fileSpec.Substring(1, 1) == ":")
            {
                fileDrive = fileSpec.Substring(0, 1);
            }
            else
            {
                fileDrive = string.Empty;
            }
            return fileDrive;
        }

        /// <summary>
        /// Return everything except the drive specification, UNC server, and file name plus ext.
        /// </summary>
        public static string WindowsFilePath(string fileSpec)
        {
            string server = string.Empty;
            string drive = string.Empty;
            string filePath = string.Empty;
            int pos = 0;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length > 0)
            {
                server = WindowsFileServer(fileSpec);
                if (server != string.Empty)
                {
                    if (fileSpec.Length > server.Length + 2)
                    {
                        fileSpec = fileSpec.Substring(server.Length + 2);
                    }
                    else
                    {
                        fileSpec = string.Empty;
                    }
                }
                else
                {
                    drive = WindowsFileDrive(fileSpec);
                    if (drive != string.Empty)
                    {
                        if (fileSpec.Length > drive.Length + 1)
                        {
                            fileSpec = fileSpec.Substring(drive.Length + 1);
                        }
                        else
                        {
                            fileSpec = string.Empty;
                        }
                    }
                }
            }
            if (fileSpec.Length == 0)
            {
                filePath = string.Empty;
            }
            else
            {
                pos = fileSpec.LastIndexOf(Separator());
                if (pos == -1)
                {
                    filePath = string.Empty;
                }
                else if (pos == 0)
                {
                    filePath = Separator();
                }
                else
                {
                    filePath = fileSpec.Substring(0, pos + 1);
                }
            }
            return filePath;
        }

        /// <summary>
        /// Return file name part of a file specification.
        /// </summary>
        public static string FileName(string fileSpec)
        {
            int pos;
            string fileName = string.Empty;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length == 0)
            {
                fileName = string.Empty;
            }
            else
            {
                pos = fileSpec.LastIndexOf(Separator());
                if (pos > -1)
                {
                    if (fileSpec.Length >= pos + 1)
                    {
                        fileSpec = fileSpec.Substring(pos + 1);
                    }
                    else
                    {
                        fileSpec = string.Empty;
                    }
                }
                if (fileSpec.Length > 0)
                {
                    pos = fileSpec.LastIndexOf(".");
                    if (pos == -1)
                    {
                        fileName = fileSpec;
                    }
                    else if (pos == 0)
                    {
                        fileName = string.Empty;
                    }
                    else
                    {
                        fileName = fileSpec.Substring(0, pos);
                    }
                }
            }
            return fileName;
        }

        /// <summary>
        /// Return file extension part of a file specification.
        /// </summary>
        public static string FileExt(string fileSpec)
        {
            int pos;
            string fileExt;
            fileSpec = fileSpec.Trim();
            if (fileSpec.Length == 0)
            {
                fileExt = string.Empty;
            }
            else
            {
                pos = fileSpec.LastIndexOf(".");
                if (pos == -1)
                {
                    fileExt = string.Empty;
                }
                else
                {
                    if (pos < fileSpec.Length - 1)
                    {
                        fileExt = fileSpec.Substring(pos + 1);
                    }
                    else
                    {
                        fileExt = string.Empty;
                    }
                }
            }
            return fileExt;
        }

        /// <summary>
        /// Return file name and extension parts of a file specification.
        /// </summary>
        public static string FileFullName(string fileSpec)
        {
            string fileFullName;
            fileFullName = FileName(fileSpec);
            string fileExt = FileExt(fileSpec);
            if (fileExt != string.Empty)
            {
                fileFullName += "." + fileExt;
            }
            return fileFullName;
        }

        /// <summary>
        /// Get file information about the specified file.
        /// </summary>
        public static FileInfo GetFileInfo(string fileSpec)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(fileSpec);
            }
            catch
            {
            }
            return fileInfo;
        }

        /// <summary>
        /// Determine absolute spec based on current and relative specs specified.
        /// </summary>
        public static string AbsoluteSpec(string currentFileSpec, string relativeFileSpec)
        {
            string cdMk;
            string cdUp;
            int cdLevels;
            int cdCount;
            int pos;
            string fileSpec;
            string leader;
            string trailer;

            //Clean parameters.
            currentFileSpec = currentFileSpec.Trim();
            relativeFileSpec = relativeFileSpec.Trim();

            //Set change directory mark and change directory up mark.
            cdMk = Separator();
            cdUp = ".." + Separator();
            //Break relative spec into leader and trailer portions.
            currentFileSpec = FileStem(currentFileSpec).Strip(StringExtender.StripMode.Trailing, cdMk);
            pos = relativeFileSpec.LastIndexOf(cdUp);
            if (pos > -1)
            {
                pos += cdUp.Length;
                leader = relativeFileSpec.Substring(0, pos);
                if (pos > relativeFileSpec.Length)
                {
                    trailer = string.Empty;
                }
                else
                {
                    trailer = relativeFileSpec.Substring(pos);
                }
                //Count the number of "CD UP" indicators in the leader.
                cdLevels = 0;
                pos = leader.LastIndexOf(cdUp);
                while (pos > -1)
                {
                    cdLevels += 1;
                    if (pos > -1)
                    {
                        leader = leader.Substring(0, pos + 1);
                    }
                    else
                    {
                        leader = string.Empty;
                    }
                    pos = leader.LastIndexOf(cdUp);
                }
                //Apply the same number of "CD UP" operations to the current spec.
                leader = currentFileSpec;
                cdCount = 0;
                pos = leader.LastIndexOf(cdMk);
                while (cdCount < cdLevels && pos > -1)
                {
                    cdCount += 1;
                    if (pos > -1)
                    {
                        leader = leader.Substring(0, pos);
                    }
                    else
                    {
                        leader = string.Empty;
                    }
                    pos = leader.LastIndexOf(cdMk);
                }
                if (cdCount == cdLevels)
                {
                    //Use leader and trailer portions to derive absolute spec.
                    fileSpec = leader + cdMk + trailer;
                }
                else
                {
                    //Error, unable to change directory up enough times.
                    fileSpec = string.Empty;
                }
            }
            else
            {
                //There are no "CD UP" indicators so use relative spec as the absolute spec.
                fileSpec = currentFileSpec + cdMk + relativeFileSpec;
            }
            return fileSpec;
        }
    }
}