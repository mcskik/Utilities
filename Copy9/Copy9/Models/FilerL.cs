using System;
using System.IO;

namespace Copy9.Models
{
    /// <summary>
    /// Filer class (Windows/Lan file system).
    /// </summary>
    /// <remarks>
    /// Filer class (Windows/Lan file system).
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class FilerL : Filer
    {
        public FilerL()
        {
        }

        public override bool DirectoryExists(string targetDirectory)
        {
            return Directory.Exists(targetDirectory);
        }

        public override void CreateDirectory(string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
        }

        public override void RemoveDirectory(string targetDirectory)
        {
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectory);
            targetDirectoryInfo.Delete();
        }

        public override bool FileExists(string targetFileSpec)
        {
            return File.Exists(targetFileSpec);
        }

        public override DateTime FileModifiedDateTime(string targetFileSpec)
        {
            FileInfo targetFileInfo = new FileInfo(targetFileSpec);
            return targetFileInfo.LastWriteTime;
        }

        public override void CopyFile(string sourceFileSpec, string targetFileSpec)
        {
            PathCheck(targetFileSpec);
            FileInfo sourceFileInfo = new FileInfo(sourceFileSpec);
            FileInfo targetFileInfo = new FileInfo(targetFileSpec);
            if (targetFileInfo.Exists)
            {
                targetFileInfo.IsReadOnly = false;
            }
            sourceFileInfo.CopyTo(targetFileSpec, true);
            targetFileInfo = new FileInfo(targetFileSpec);
            if (targetFileInfo.Exists)
            {
                targetFileInfo.IsReadOnly = false;
            }
        }

        public override void UploadFile(string sourceFileSpec, string targetFileSpec)
        {
        }

        public override void DownloadFile(string sourceFileSpec, string targetFileSpec)
        {
        }

        public override void DeleteFile(string targetFileSpec)
        {
            FileInfo targetFileInfo = new FileInfo(targetFileSpec);
            targetFileInfo.IsReadOnly = false;
            targetFileInfo.Delete();
        }

        public override void PathCheck(string targetFileSpec)
        {
            ProfileData.Models.Helpers.FileHelper.PathCheck(targetFileSpec);
        }

        public override String GetFileName(string fileSpec)
        {
            return Path.GetFileName(fileSpec);
        }

        public override String GetDirectoryName(string fileSpec)
        {
            return Path.GetDirectoryName(fileSpec);
        }
    }
}