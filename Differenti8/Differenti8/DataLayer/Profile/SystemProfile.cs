using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Differenti8.DataLayer.Profile
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
        #region Properties.
        public XmlProfileHelper XmlProfileHelper { get; set; }
        public ProfileCache ProfileCache { get; set; }
        public XDocument XDoc { get; set; }
        public string Logging
        {
            get
            {
                return ProfileCache.Fetch("Logging");
            }
        }
        public string HomePath
        {
            get
            {
                return ProfileCache.Fetch("HomePath");
            }
        }
        public string DataPath
        {
            get
            {
                return ProfileCache.Fetch("DataPath");
            }
        }
        public string SystemPath
        {
            get
            {
                return ProfileCache.Fetch("SystemPath");
            }
        }
        public string NewPath
        {
            get
            {
                return ProfileCache.Fetch("NewPath");
            }
        }
        public string OldPath
        {
            get
            {
                return ProfileCache.Fetch("OldPath");
            }
        }
        public string LogPath
        {
            get
            {
                return ProfileCache.Fetch("LogPath");
            }
        }
        public string MasterUserSettings
        {
            get
            {
                return ProfileCache.Fetch("MasterUserSettings");
            }
        }
        public string CurrentUserSettings
        {
            get
            {
                return ProfileCache.Fetch("CurrentUserSettings");
            }
        }
        public string ViewerWindows
        {
            get
            {
                return ProfileCache.Fetch("ViewerWindows");
            }
        }
        public string ViewerUnix
        {
            get
            {
                return ProfileCache.Fetch("ViewerUnix");
            }
        }
        public string MeldProgram { get { return ProfileCache.Fetch("MeldProgram"); } }
        #endregion

        #region Constructors.
        public SystemProfile(ProfileCache profileCache)
        {
            ProfileCache = profileCache;
            Load();
        }
        #endregion

        #region Public Methods.
        public void Load()
        {
            XDoc = XDocument.Load(ProfileCache.Fetch("SystemProfileXml"));
            XmlProfileHelper = new XmlProfileHelper(ProfileCache, XDoc.Root);
            XmlProfileHelper.FetchAll("Parameters");
            XmlProfileHelper.FetchAll("Directories");
            XmlProfileHelper.FetchAll("Files");
            XmlProfileHelper.FetchAll("Includes");
            XmlProfileHelper.FetchAll("Programs");
        }
        #endregion
    }
}
