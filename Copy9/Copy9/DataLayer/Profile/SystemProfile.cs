using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Copy9.DataLayer.Profile
{
    /// <summary>
    /// System profile class.
    /// </summary>
    /// <remarks>
    /// Contains information which is stored in the SystemProfile.xml file.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class SystemProfile
    {
        public XmlProfileHelper XmlProfileHelper { get; set; }
        public ProfileCache ProfileCache { get; set; }
        public XDocument XDoc { get; set; }
        public string Logging { get { return ProfileCache.Fetch("Logging"); } }
        public bool Verbose { get { return IsAffirmative(ProfileCache.Fetch("Verbose")); } }
        public bool UseDirectoryExclusions { get { return IsAffirmative(ProfileCache.Fetch("UseDirectoryExclusions")); } }
        public bool UseFileTypeExclusions { get { return IsAffirmative(ProfileCache.Fetch("UseFileTypeExclusions")); } }
        public bool UseFileTypeInclusions { get { return IsAffirmative(ProfileCache.Fetch("UseFileTypeInclusions")); } }
        public int FileSizeLimit { get { return ProfileCache.FetchInt("FileSizeLimit"); } }
        public int LogFileKeepDays { get { return ProfileCache.FetchInt("LogFileKeepDays"); } }
        public string HomePath { get { return ProfileCache.Fetch("HomePath"); } }
        public string DataPath { get { return ProfileCache.Fetch("DataPath"); } }
        public string SystemPath { get { return ProfileCache.Fetch("SystemPath"); } }
        public string NewPath { get { return ProfileCache.Fetch("NewPath"); } }
        public string OldPath { get { return ProfileCache.Fetch("OldPath"); } }
        public string LogPath { get { return ProfileCache.Fetch("LogPath"); } }
        public string MasterSnapshotPath { get { return ProfileCache.Fetch("MasterSnapshotPath"); } }
        public string CurrentSnapshotPath { get { return ProfileCache.Fetch("CurrentSnapshotPath"); } }
        public string MasterUserSettings { get { return ProfileCache.Fetch("MasterUserSettings"); } }
        public string CurrentUserSettings { get { return ProfileCache.Fetch("CurrentUserSettings"); } }
        public string ExcludeDirectoriesFileSpec { get { return ProfileCache.Fetch("ExcludeDirectories"); } }
        public string ExcludeFileTypesFileSpec { get { return ProfileCache.Fetch("ExcludeFileTypes"); } }
        public string IncludeFileTypesFileSpec { get { return ProfileCache.Fetch("IncludeFileTypes"); } }
        public string ViewerWindows { get { return ProfileCache.Fetch("ViewerWindows"); } }
        public string ViewerUnix { get { return ProfileCache.Fetch("ViewerUnix"); } }
        public string CMD { get { return ProfileCache.Fetch("CMD"); } }
        public string ADB { get { return ProfileCache.Fetch("ADB"); } }

        public SystemProfile(ProfileCache profileCache)
        {
            ProfileCache = profileCache;
            Load();
        }

        public void Load()
        {
            XDoc = XDocument.Load(ProfileCache.Fetch("SystemProfileXml"));
            XmlProfileHelper = new XmlProfileHelper(ProfileCache, XDoc.Root);
            XmlProfileHelper.FetchAll("Parameters");
            XmlProfileHelper.FetchAll("Directories");
            XmlProfileHelper.FetchAll("Files");
            XmlProfileHelper.FetchAll("Programs");
        }

        protected bool IsAffirmative(string booleanText)
        {
            bool affirmative = false;
            booleanText = booleanText.ToLower();
            string positive = ":yes:y:true:t:";
            if (positive.Contains(String.Format(@":{0}:", booleanText)))
            {
                affirmative = true;
            }
            return affirmative;
        }
    }
}