using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using static ProfileData.Models.Extenders.StringExtender;

namespace Copy9.Models
{
    /// <summary>
    /// Filer class (Google Drive).
    /// </summary>
    /// <remarks>
    /// Filer class (Google Drive).
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class FilerG : Filer
    {
        private const int KB = 0x1024;
        private const int DOWNLOAD_CHUNK_SIZE = 256 * KB;

        public Connector Connector { get; set; }

        public FilerG(Connector connector)
        {
            Connector = connector;
        }

        public override bool DirectoryExists(string targetDirectory)
        {
            Google.Apis.Drive.v3.Data.File file = GetSpecificDriveEntry(targetDirectory);
            return file != null;
        }

        public override void CreateDirectory(string targetDirectory)
        {
            targetDirectory = Connector.StripDrivePrefix(targetDirectory);
            string firstPart = string.Empty;
            string lastPart = targetDirectory;
            Google.Apis.Drive.v3.Data.File parentFolder;
            Google.Apis.Drive.v3.Data.File childFolder;
            int pos = targetDirectory.LastIndexOf(Connector.BACK_SLASH);
            if (pos != -1)
            {
                firstPart = targetDirectory.Substring(0, pos);
                firstPart = firstPart.Strip(StripMode.Trailing, Connector.BACK_SLASH);
                lastPart = targetDirectory.Substring(pos);
                lastPart = lastPart.Strip(StripMode.Leading, Connector.BACK_SLASH);
                parentFolder = GetSpecificDriveEntry(firstPart);
                childFolder = new Google.Apis.Drive.v3.Data.File();
                childFolder.Name = lastPart;
                childFolder.MimeType = Connector.FOLDER_MIME_TYPE;
                childFolder.Parents = new List<string>() { parentFolder.Id };
                var childRequest = Connector.DriveService.Files.Create(childFolder);
                childRequest.Fields = "id,parents";
                var subFolder = childRequest.Execute();
            }
            else
            {
                var parentRequest = Connector.DriveService.Files.Get("root");
                parentRequest.Fields = "id,parents";
                parentFolder = parentRequest.Execute();
                childFolder = new Google.Apis.Drive.v3.Data.File();
                childFolder.Name = lastPart;
                childFolder.MimeType = Connector.FOLDER_MIME_TYPE;
                childFolder.Parents = new List<string>() { parentFolder.Id };
                var childRequest = Connector.DriveService.Files.Create(childFolder);
                childRequest.Fields = "id,parents";
                var subFolder = childRequest.Execute();
            }
        }

        public void CreateFolder_Sample_NotUsed(string folderName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = Connector.FOLDER_MIME_TYPE
            };
            var request = Connector.DriveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
        }

        public override void RemoveDirectory(string targetDirectory)
        {
            targetDirectory = Connector.StripDrivePrefix(targetDirectory);
            Google.Apis.Drive.v3.Data.File folder = GetSpecificDriveEntry(targetDirectory);
            var folderId = folder.Id;
            var request = Connector.DriveService.Files.Delete(folderId);
            var oldFolder = request.Execute();
            Connector.RemoveFromCache(targetDirectory);
        }

        public override bool FileExists(string targetFileSpec)
        {
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            Google.Apis.Drive.v3.Data.File file = GetSpecificDriveEntry(targetFileSpec);
            return file != null;
        }

        public override DateTime FileModifiedDateTime(string targetFileSpec)
        {
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            Google.Apis.Drive.v3.Data.File file = GetSpecificDriveEntry(targetFileSpec);
            return file.ModifiedTime ?? DateTime.MinValue;
        }

        public override void CopyFile(string sourceFileSpec, string targetFileSpec)
        {
        }

        public override void UploadFile(string localSourceFileSpec, string remoteTargetFileSpec)
        {
            remoteTargetFileSpec = Connector.StripDrivePrefix(remoteTargetFileSpec);
            string targetFolder = string.Empty;
            string targetFileName = remoteTargetFileSpec;
            Google.Apis.Drive.v3.Data.File targetParentFolder;
            int pos = remoteTargetFileSpec.LastIndexOf(Connector.BACK_SLASH);
            if (pos != -1)
            {
                targetFolder = remoteTargetFileSpec.Substring(0, pos);
                targetFolder = targetFolder.Strip(StripMode.Trailing, Connector.BACK_SLASH);
                targetFileName = targetFileName.Substring(pos);
                targetFileName = targetFileName.Strip(StripMode.Leading, Connector.BACK_SLASH);
                targetParentFolder = GetSpecificDriveEntry(targetFolder);
            }
            else
            {
                var parentRequest = Connector.DriveService.Files.Get("root");
                parentRequest.Fields = "id,parents";
                targetParentFolder = parentRequest.Execute();
            }
            UploadFile(localSourceFileSpec, targetParentFolder);
        }

        private void UploadFile(string localSourceFileSpec, Google.Apis.Drive.v3.Data.File targetFolder)
        {
            string description = localSourceFileSpec;
            if (System.IO.File.Exists(localSourceFileSpec))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(localSourceFileSpec);
                body.Description = description;
                body.MimeType = GetMimeType(localSourceFileSpec);
                body.Parents = new List<string>() { targetFolder.Id };
                byte[] byteArray = System.IO.File.ReadAllBytes(localSourceFileSpec);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    var request = Connector.DriveService.Files.Create(body, stream, GetMimeType(localSourceFileSpec));
                    request.Upload();
                    var responseBody = request.ResponseBody;
                }
                catch (Exception e)
                {
                }
            }
        }

        public string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }

        public override void DownloadFile(string remoteSourceFileSpec, string localTargetFileSpec)
        {
            Google.Apis.Drive.v3.Data.File file = GetSpecificDriveEntry(remoteSourceFileSpec);
            DownloadFile(file, localTargetFileSpec);
        }

        private void DownloadFile(Google.Apis.Drive.v3.Data.File file, string localTargetFileSpec)
        {
            var request = Connector.DriveService.Files.Get(file.Id);
            var stream = new System.IO.MemoryStream();
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream, localTargetFileSpec);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream);
        }

        private void SaveStream(System.IO.MemoryStream stream, string localTargetFileSpec)
        {
            using (System.IO.FileStream file = new System.IO.FileStream(localTargetFileSpec, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        public override void DeleteFile(string targetFileSpec)
        {
            targetFileSpec = Connector.StripDrivePrefix(targetFileSpec);
            Google.Apis.Drive.v3.Data.File file = GetSpecificDriveEntry(targetFileSpec);
            var fileId = file.Id;
            var request = Connector.DriveService.Files.Delete(fileId);
            var oldFile = request.Execute();
            Connector.RemoveFromCache(targetFileSpec);
        }

        public override void PathCheck(string targetFileSpec)
        {
            //Do nothing at the moment as this could be quite expensive.
            //The synchronize engine should generate CreateDirectory requests for all necessary directories.
        }

        public override String GetDirectoryName(string fileSpec)
        {
            String folder = fileSpec.Trim();
            int pos = folder.LastIndexOf(@"\");
            if (pos != -1)
            {
                folder = folder.Substring(0, pos);
            }
            return folder;
        }

        public override String GetFileName(string fileSpec)
        {
            String fileName = fileSpec.Trim();
            if (fileName.EndsWith(@"\"))
            {
                fileName = string.Empty;
            }
            else
            {
                int pos = fileName.LastIndexOf(@"\");
                if (pos != -1)
                {
                    fileName = fileName.Substring(pos + 1);
                }
            }
            return fileName;
        }

        public Google.Apis.Drive.v3.Data.File GetSpecificDriveEntry(string remoteFolderOrFileSpec)
        {
            remoteFolderOrFileSpec = Connector.StripDrivePrefix(remoteFolderOrFileSpec);
            string remoteFolderOrFileName = remoteFolderOrFileSpec;
            int pos = remoteFolderOrFileSpec.LastIndexOf(@"\");
            if (pos != -1)
            {
                remoteFolderOrFileName = remoteFolderOrFileSpec.Substring(pos + 1);
            }
            string contentType = Connector.GetContentType(remoteFolderOrFileSpec);
            string searchCriteria = String.Format(@"name = ""{0}"" and trashed=false", remoteFolderOrFileName);
            List<Google.Apis.Drive.v3.Data.File> driveEntries = GetSpecificDriveEntries(remoteFolderOrFileSpec, searchCriteria);
            if (driveEntries.Count > 0)
            {
                return driveEntries[0];
            }
            else
            {
                return null;
            }
        }

        public List<Google.Apis.Drive.v3.Data.File> GetSpecificDriveEntries(string remoteFolderOrFileSpec, string searchCriteria)
        {
            List<Google.Apis.Drive.v3.Data.File> entries = new List<Google.Apis.Drive.v3.Data.File>();
            try
            {
                FilesResource.ListRequest list = Connector.DriveService.Files.List();
                list.PageSize = 100;
                if (searchCriteria != null)
                {
                    list.Q = searchCriteria;
                    list.Fields = "nextPageToken, files(name, id, size, kind, parents, sharedWithMeTime, shared, sharingUser, fileExtension, viewedByMe, viewedByMeTime, trashed, modifiedTime, mimeType, fileExtension)";
                    list.Spaces = "drive";
                }
                FileList filesFeed = list.Execute();
                while (filesFeed.Files != null)
                {
                    foreach (Google.Apis.Drive.v3.Data.File item in filesFeed.Files)
                    {
                        string entryPath = Connector.GetAbsPath(item);
                        if (entryPath == remoteFolderOrFileSpec)
                        {
                            entries.Add(item);
                            break;
                        }
                    }
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }
                    list.PageToken = filesFeed.NextPageToken;
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