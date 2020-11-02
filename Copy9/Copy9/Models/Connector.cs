using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using ProfileData.Models.Extenders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Copy9.Models
{
    /// <summary>
    /// Connector class.
    /// </summary>
    /// <remarks>
    /// Connector class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Connector
    {
        public const string FOLDER_MIME_TYPE = "application/vnd.google-apps.folder";
        public const string BACK_SLASH = @"\";
        public const string EXCLAMATION_MARK = @"!";
        public const string PERCENT_SIGN = @"%"; // Not used spare for future use.
        public const string HASH = @"#";
        public const string FULL_STOP = @".";
        public const string COLON = @":";
        public const string DIR_EXTENSION = @"dir";
        public const string TEXT_EXTENSION = @"txt";
        public const string SECRETS_FILE_NAME_1 = @"client_secrets_1.json";
        public const string SECRETS_FILE_NAME_2 = @"client_secrets_2.json";
        public const int PAGE_SIZE = 500;
        public const string DRIVE_QUERY_FIELDS = @"nextPageToken, files(name, id, size, kind, parents, sharedWithMeTime, shared, sharingUser, fileExtension, viewedByMe, viewedByMeTime, trashed, modifiedTime, mimeType, fileExtension)";
        public const string DRIVE_QUERY_SPACES = @"drive";
        public const string DRIVE_FOLDER_ID_SEARCH_CRITERIA_TEMPLATE = @"'{0}' in parents";
        public const string DRIVE_ENTRY_NAME_SEARCH_CRITERIA_TEMPLATE = @"name = ""{0}"" and trashed=false";
        public string BaseDir { get; set; }
        public string HlqPrefix { get; set; }

        public bool IsGoogleDrive
        {
            get
            {
                return HlqPrefix == Connector.EXCLAMATION_MARK || HlqPrefix == Connector.PERCENT_SIGN;
            }
        }

        public bool IsAdb
        {
            get
            {
                return HlqPrefix == Connector.HASH;
            }
        }

        public SortedDictionary<string, string> MimeTypes { get; set; }
        public readonly string[] driveScopes = new[] { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
        public UserCredential DriveCredential { get; set; }
        public DriveService DriveService { get; set; }
        public Google.Apis.Drive.v3.Data.File RemoteFile { get; set; }
        public Dictionary<string, Google.Apis.Drive.v3.Data.File> CachedFiles { get; set; }
        public Dictionary<string, Google.Apis.Drive.v3.Data.File> CachedDriveEntries { get; set; }
        public Connector(string baseDir)
        {
            BaseDir = baseDir;
            if (baseDir.StartsWith(EXCLAMATION_MARK))
            {
                HlqPrefix = EXCLAMATION_MARK;
                Initialize();
                Login(SECRETS_FILE_NAME_1);
            }
            else if (baseDir.StartsWith(PERCENT_SIGN))
            {
                // Not used spare for future use.
                HlqPrefix = Connector.PERCENT_SIGN;
                Initialize();
                Login(SECRETS_FILE_NAME_2);
            }
            else if (baseDir.StartsWith(HASH))
            {
                HlqPrefix = Connector.HASH;
                Initialize();
            }
            else
            {
                HlqPrefix = baseDir.Substring(0, 1);
            }
        }
        private void Initialize()
        {
            MimeTypes = new SortedDictionary<string, string>();
            MimeTypes.Add("arj", "application/arj");
            MimeTypes.Add("bmp", "image/bmp");
            MimeTypes.Add("cab", "application/cab");
            MimeTypes.Add("csv", "text/plain");
            MimeTypes.Add("default", "application/octet-stream");
            //MimeTypes.Add("doc", "application/msword");
            MimeTypes.Add("doc", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            MimeTypes.Add("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            MimeTypes.Add("folder", "application/vnd.google-apps.folder");
            MimeTypes.Add("gif", "image/gif");
            MimeTypes.Add("htm", "text/html");
            MimeTypes.Add("html", "text/html");
            MimeTypes.Add("jpg", "image/jpeg");
            MimeTypes.Add("js", "text/js");
            MimeTypes.Add("mp3", "audio/mpeg");
            MimeTypes.Add("ods", "application/vnd.oasis.opendocument.spreadsheet");
            MimeTypes.Add("pdf", "application/pdf");
            MimeTypes.Add("php", "application/x-httpd-php");
            MimeTypes.Add("png", "image/png");
            MimeTypes.Add("rar", "application/rar");
            MimeTypes.Add("swf", "application/x-shockwave-flash");
            MimeTypes.Add("tar", "application/tar");
            MimeTypes.Add("tmpl", "text/plain");
            MimeTypes.Add("txt", "text/plain");
            MimeTypes.Add("xls", "application/vnd.ms-excel");
            MimeTypes.Add("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            MimeTypes.Add("xml", "text/xml");
            MimeTypes.Add("zip", "application/zip");
            CachedFiles = new Dictionary<string, Google.Apis.Drive.v3.Data.File>();
            CachedDriveEntries = new Dictionary<string, Google.Apis.Drive.v3.Data.File>();
        }

        public void Login(string secretsFileName)
        {
            try
            {
                LoginAsync(secretsFileName).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
        }

        public async Task LoginAsync(string secretsFileName)
        {
            GoogleWebAuthorizationBroker.Folder = "Copy8";
            using (var stream = new System.IO.FileStream(secretsFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                DriveCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, driveScopes, "user", CancellationToken.None);
            }
            DriveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = DriveCredential,
                ApplicationName = "Copy8",
            });
        }

        public void Logout()
        {
            if (DriveService != null)
            {
                DriveService.Dispose();
            }
            DriveService = null;
            DriveCredential = null;
        }

        public string GetContentType(string localFileSpec)
        {
            string contentType = string.Empty;
            string ext = GetFileExtension(localFileSpec);
            if (MimeTypes.ContainsKey(ext))
            {
                contentType = MimeTypes[ext];
            }
            else
            {
                contentType = MimeTypes[TEXT_EXTENSION];
            }
            return contentType;
        }

        public string GetFileExtension(string localFileSpec)
        {
            string ext = string.Empty;
            int pos = localFileSpec.LastIndexOf(FULL_STOP);
            if (pos != -1)
            {
                if (pos < localFileSpec.Length - 1)
                {
                    ext = localFileSpec.Substring(pos + 1);
                }
            }
            if (ext == string.Empty)
            {
                ext = TEXT_EXTENSION;
            }
            return ext;
        }

        #region Shared methods.
        public string StripDrivePrefix(string localFolderOrFileSpec)
        {
            localFolderOrFileSpec = localFolderOrFileSpec.Strip(StringExtender.StripMode.Leading, HASH);
            localFolderOrFileSpec = localFolderOrFileSpec.Strip(StringExtender.StripMode.Leading, EXCLAMATION_MARK);
            localFolderOrFileSpec = localFolderOrFileSpec.Strip(StringExtender.StripMode.Leading, PERCENT_SIGN);
            localFolderOrFileSpec = localFolderOrFileSpec.Strip(StringExtender.StripMode.Leading, COLON);
            localFolderOrFileSpec = localFolderOrFileSpec.Strip(StringExtender.StripMode.Leading, BACK_SLASH);
            return localFolderOrFileSpec;
        }

        public void AddToCache(string remoteFolderOrFileSpec, Google.Apis.Drive.v3.Data.File driveEntry)
        {
            remoteFolderOrFileSpec = StripDrivePrefix(remoteFolderOrFileSpec);
            if (!CachedDriveEntries.ContainsKey(remoteFolderOrFileSpec))
            {
                CachedDriveEntries.Add(remoteFolderOrFileSpec, driveEntry);
            }
        }

        public void RemoveFromCache(string remoteFolderOrFileSpec)
        {
            remoteFolderOrFileSpec = StripDrivePrefix(remoteFolderOrFileSpec);
            if (CachedDriveEntries.ContainsKey(remoteFolderOrFileSpec))
            {
                CachedDriveEntries.Remove(remoteFolderOrFileSpec);
            }
        }

        public Google.Apis.Drive.v3.Data.File GetSpecificDriveEntry(string remoteFolderOrFileSpec)
        {
            Google.Apis.Drive.v3.Data.File driveEntry = null;
            remoteFolderOrFileSpec = StripDrivePrefix(remoteFolderOrFileSpec);
            if (CachedDriveEntries.ContainsKey(remoteFolderOrFileSpec))
            {
                driveEntry = CachedDriveEntries[remoteFolderOrFileSpec];
            }
            else
            {
                string remoteFolderOrFileName = remoteFolderOrFileSpec;
                int pos = remoteFolderOrFileSpec.LastIndexOf(BACK_SLASH);
                if (pos != -1)
                {
                    remoteFolderOrFileName = remoteFolderOrFileSpec.Substring(pos + 1);
                }
                string contentType = GetContentType(remoteFolderOrFileSpec);
                string searchCriteria = String.Format(DRIVE_ENTRY_NAME_SEARCH_CRITERIA_TEMPLATE, remoteFolderOrFileName);
                List<Google.Apis.Drive.v3.Data.File> driveEntries = GetSpecificDriveEntries(remoteFolderOrFileSpec, searchCriteria);
                if (driveEntries.Count > 0)
                {
                    driveEntry = driveEntries[0];
                    AddToCache(remoteFolderOrFileSpec, driveEntry);
                }
                else
                {
                    driveEntry = null;
                }
            }
            return driveEntry;
        }

        public List<Google.Apis.Drive.v3.Data.File> GetSpecificDriveEntries(string remoteFolderOrFileSpec, string searchCriteria)
        {
            List<Google.Apis.Drive.v3.Data.File> entries = new List<Google.Apis.Drive.v3.Data.File>();
            try
            {
                FilesResource.ListRequest list = DriveService.Files.List();
                list.PageSize = PAGE_SIZE;
                if (searchCriteria != null)
                {
                    list.Q = searchCriteria;
                    list.Fields = DRIVE_QUERY_FIELDS;
                    list.Spaces = DRIVE_QUERY_SPACES;
                }
                FileList filesFeed = list.Execute();
                while (filesFeed.Files != null)
                {
                    foreach (Google.Apis.Drive.v3.Data.File item in filesFeed.Files)
                    {
                        string entryPath = GetAbsPath(item);
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

        public string GetAbsPath(Google.Apis.Drive.v3.Data.File file)
        {
            var name = file.Name;
            if (file.Parents == null || file.Parents.Count == 0)
            {
                return name;
            }
            var path = new List<string>();
            while (true)
            {
                var parent = GetParent(file.Parents[0]);
                // Stop when we find the root dir.
                if (parent.Parents == null || parent.Parents.Count == 0)
                {
                    break;
                }
                path.Insert(0, parent.Name);
                file = parent;
            }
            path.Add(name);
            return path.Aggregate((current, next) => System.IO.Path.Combine(current, next));
        }

        public Google.Apis.Drive.v3.Data.File GetParent(string id)
        {
            if (CachedFiles.ContainsKey(id))
            {
                return CachedFiles[id];
            }
            var request = DriveService.Files.Get(id);
            request.Fields = "id,name,parents";
            var parent = request.Execute();
            CachedFiles[id] = parent;
            return parent;
        }

        public Google.Apis.Drive.v3.Data.File GetFileDetails_NotUsed(string id)
        {
            if (CachedFiles.ContainsKey(id))
            {
                return CachedFiles[id];
            }
            var request = DriveService.Files.Get(id);
            request.Fields = "id,name,parents,size";
            var file = request.Execute();
            CachedFiles[id] = file;
            return file;
        }
        #endregion
    }
}