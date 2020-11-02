using System;

namespace Copy9.Models
{
    /// <summary>
    /// Abstract filer class.
    /// </summary>
    /// <remarks>
    /// Abstract filer class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public abstract class Filer
    {
        public Filer()
        {
        }

        public abstract bool DirectoryExists(string targetDirectory);

        public abstract void CreateDirectory(string targetDirectory);

        public abstract void RemoveDirectory(string targetDirectory);

        public abstract bool FileExists(string targetFileSpec);

        public abstract DateTime FileModifiedDateTime(string targetFileSpec);

        public abstract void CopyFile(string sourceFileSpec, string targetFileSpec);

        public abstract void UploadFile(string sourceFileSpec, string targetFileSpec);

        public abstract void DownloadFile(string sourceFileSpec, string targetFileSpec);

        public abstract void DeleteFile(string targetFileSpec);

        public abstract void PathCheck(string targetFileSpec);

        public abstract String GetFileName(string fileSpec);

        public abstract String GetDirectoryName(string fileSpec);
    }
}