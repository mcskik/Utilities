using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Delini8.DataLayer.Profile
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
        public string HomePath { get { return ProfileCache.Fetch("HomePath"); } }
        public string DataPath { get { return ProfileCache.Fetch("DataPath"); } }
        public string SystemPath { get { return ProfileCache.Fetch("SystemPath"); } }
        public string SourcePath { get { return ProfileCache.Fetch("SourcePath"); } }
        public string LogPath { get { return ProfileCache.Fetch("LogPath"); } }
        public string ViewerWindows { get { return ProfileCache.Fetch("ViewerWindows"); } }
        public string ViewerUnix { get { return ProfileCache.Fetch("ViewerUnix"); } }

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