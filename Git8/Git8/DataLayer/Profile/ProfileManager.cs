using ProfileData.DataLayer.Profile;
using System.IO;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Profile manager container class.
    /// </summary>
    /// <remarks>
    /// Container class to provide access to individual profiles.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ProfileManager : IProfileManager
    {
        public ProfileCache ProfileCache { get; set; }
        public ApplicationProfile ApplicationProfile { get; set; }
        public SystemProfile SystemProfile { get; set; }
        public ParameterSettings BranchCheckoutSettings { get; set; }
        public ParameterSettings BranchLocalSettings { get; set; }
        public ParameterSettings BranchRemoteSettings { get; set; }
        public CommandSettings CommandSettings { get; set; }
        public ParameterSettings CommentSettings { get; set; }
        public ParameterSettings FileSpecSettings { get; set; }
        public ParameterSettings HeadSettings { get; set; }
        public ParameterSettings ShaSettings { get; set; }
        public ParameterSettings StashSettings { get; set; }
        public TemplateParameterSettings TemplateSettings { get; set; }
        public RepositorySettings RepositorySettings { get; set; }

        public ProfileManager()
        {
            ProfileCache = new ProfileCache();
            ApplicationProfile = new ApplicationProfile(ProfileCache);
            if (!File.Exists(ApplicationProfile.SystemProfileXml))
            {
                if (Directory.Exists(ApplicationProfile.DataPath))
                {
                    FileHelper.DeleteAll(ApplicationProfile.DataPath);
                }
            }
            EnsureXCopy(ApplicationProfile.MasterDataPath, ApplicationProfile.DataPath);
            SystemProfile = new SystemProfile(ProfileCache);
            RepositorySettings = new RepositorySettings(ProfileCache);
            RepositorySettings.Load(SystemProfile.RepositorySettings);
            TemplateSettings = new TemplateParameterSettings(ProfileCache);
            TemplateSettings.Load(SystemProfile.TemplateSettings);
            Reload();            
        }

        public void Reload()
        {
            ApplicationProfile.RepositoryName = RepositorySettings.SelectedKey;
            EnsureXCopy(ApplicationProfile.MasterSettingsPath, ApplicationProfile.RepositorySettingsPath);
            SystemProfile = new SystemProfile(ProfileCache);
            BranchCheckoutSettings = new ParameterSettings(ProfileCache);
            BranchCheckoutSettings.Load(SystemProfile.BranchCheckoutSettings);
            BranchLocalSettings = new ParameterSettings(ProfileCache);
            BranchLocalSettings.Load(SystemProfile.BranchLocalSettings);
            BranchRemoteSettings = new ParameterSettings(ProfileCache);
            BranchRemoteSettings.Load(SystemProfile.BranchRemoteSettings);
            CommandSettings = new CommandSettings(ProfileCache);
            CommandSettings.Load(SystemProfile.CommandSettings);
            CommandSettings.Clear();
            CommentSettings = new ParameterSettings(ProfileCache);
            CommentSettings.Load(SystemProfile.CommentSettings);
            FileSpecSettings = new ParameterSettings(ProfileCache);
            FileSpecSettings.Load(SystemProfile.FileSpecSettings);
            HeadSettings = new ParameterSettings(ProfileCache);
            HeadSettings.Load(SystemProfile.HeadSettings);
            ShaSettings = new ParameterSettings(ProfileCache);
            ShaSettings.Load(SystemProfile.ShaSettings);
            StashSettings = new ParameterSettings(ProfileCache);
            StashSettings.Load(SystemProfile.StashSettings);
        }

        /// <remarks>
        /// Ensure that the sample import and export directories and files exist in the user's MyDocuments folder.
        /// Only overwrite if master copy has a more recent DateTime stamp.
        /// </remarks>
        private void EnsureXCopy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            EnsureXCopy(diSource, diTarget);
        }

        private void EnsureXCopy(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            if (!Directory.Exists(targetDirectory.FullName))
            {
                Directory.CreateDirectory(targetDirectory.FullName);
            }
            foreach (FileInfo fi in sourceDirectory.GetFiles())
            {
                string targetFile = Path.Combine(targetDirectory.ToString(), fi.Name);
                FileInfo targetFileInfo = new FileInfo(targetFile);
                bool doCopy = false;
                if (targetFileInfo.Exists)
                {
                    if (fi.LastWriteTime > targetFileInfo.LastWriteTime)
                    {
                        doCopy = true;
                    }
                }
                else
                {
                    doCopy = true;
                }
                if (doCopy)
                {
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.IsReadOnly = false;
                    }
                    fi.CopyTo(Path.Combine(targetDirectory.ToString(), fi.Name), true);
                    targetFileInfo = new FileInfo(targetFile);
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.IsReadOnly = false;
                    }
                }
            }
            foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
            {
                DirectoryInfo targetDirectoryInfo = targetDirectory.CreateSubdirectory(di.Name);
                EnsureXCopy(di, targetDirectoryInfo);
            }
        }
    }
}