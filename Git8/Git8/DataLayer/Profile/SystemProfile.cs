using System;
using System.Xml.Linq;
using ProfileData.DataLayer.Profile;
using System.Collections.Generic;

namespace Git8.DataLayer.Profile
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
        private List<string> _commandsWithInput;
        public bool Debug
        {
            get
            {
                string debugText = ProfileCache.Fetch("Debug");
                bool debug = IsAffirmative(debugText);
                return debug;
            }
        }
        public List<string> CommandsWithInput
        {
            get
            {
                if (_commandsWithInput == null)
                {
                    _commandsWithInput = new List<string>();
                    foreach (var command in XDoc.Descendants("CommandsWithInput").Descendants("Command"))
                    {
                        _commandsWithInput.Add((string)command);
                    }
                }
                return _commandsWithInput;
            }
        }
        public string HomePath { get { return ProfileCache.Fetch("HomePath"); } }
        public string ConfigPath { get { return ProfileCache.Fetch("ConfigPath"); } }
        //public string RepositoryName { get { return ProfileCache.Fetch("RepositoryName"); } }
        //public string RepositoryConfigPath { get { return ProfileCache.Fetch("RepositoryConfigPath"); } }
        public string LogPath { get { return ProfileCache.Fetch("LogPath"); } }
        public string OutputPath { get { return ProfileCache.Fetch("OutputPath"); } }
        public string OutputCommandsPath { get { return ProfileCache.Fetch("OutputCommandsPath"); } }
        public string JournalPath { get { return ProfileCache.Fetch("JournalPath"); } }
        public string BranchCheckoutSettings { get { return ProfileCache.Fetch("BranchCheckoutSettings"); } }
        public string BranchLocalSettings { get { return ProfileCache.Fetch("BranchLocalSettings"); } }
        public string BranchRemoteSettings { get { return ProfileCache.Fetch("BranchRemoteSettings"); } }
        public string CommandSettings { get { return ProfileCache.Fetch("CommandSettings"); } }
        public string CommentSettings { get { return ProfileCache.Fetch("CommentSettings"); } }
        public string FileSpecSettings { get { return ProfileCache.Fetch("FileSpecSettings"); } }
        public string HeadSettings { get { return ProfileCache.Fetch("HeadSettings"); } }
        public string ShaSettings { get { return ProfileCache.Fetch("ShaSettings"); } }
        public string StashSettings { get { return ProfileCache.Fetch("StashSettings"); } }
        public string TemplateSettings { get { return ProfileCache.Fetch("TemplateSettings"); } }
        public string RepositorySettings { get { return ProfileCache.Fetch("RepositorySettings"); } }
        public string Viewer { get { return ProfileCache.Fetch("Viewer"); } }
        public string Git { get { return ProfileCache.Fetch("Git"); } }

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
            XmlProfileHelper.FetchAll("Connections");
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