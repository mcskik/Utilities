using ProfileData.DataLayer.Profile;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Copy9.DataLayer.Profile
{
    /// <summary>
    /// Application profile.
    /// </summary>
    /// <remarks>
    /// Contains information which is essentially constant for the application.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ApplicationProfile
	{
		#region Member variables.
        private ProfileCache _profileCache;
        private string _startupPath = string.Empty;
		private string _separator = Path.DirectorySeparatorChar.ToString();
        private string _cdUp = ".." + Path.DirectorySeparatorChar.ToString();
        private string _AssemblyVersion = string.Empty;
        private string _userName = string.Empty;
        #endregion

		#region Properties.
        public string DataPath
        {
            get
            {
                string appPath = _startupPath + _separator;
                Directory.SetCurrentDirectory(appPath);
                string dataPath = Path.GetFullPath(String.Format(@"{0}{0}Data{1}", _cdUp, _separator));
                return dataPath;
            }
        }

        public string SystemProfileXml
		{
			get
			{
                string xmlFile = Path.GetFullPath(String.Format(@"{0}System{1}SystemProfile.xml", DataPath, _separator));
                return xmlFile;
			}
		}

        public string UserName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public string Version
        {
            get
            {
                return _AssemblyVersion;
            }
        }
        #endregion

		#region Constructors.
        public ApplicationProfile(ProfileCache profileCache)
        {
            _profileCache = profileCache;
            _startupPath = Application.StartupPath.Trim();
            _userName = System.Environment.UserName;
            _AssemblyVersion = GetAssemblyVersion();
            _profileCache.Store("DataPath", DataPath);
            _profileCache.Store("SystemProfileXml", SystemProfileXml);
            _profileCache.Store("UserName", UserName);
            _profileCache.Store("Version", Version);
        }
		#endregion

        #region Private Methods.
        private string GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            DateTime assemblyDate = File.GetLastWriteTime(assembly.Location);
            string assemblyDateText = assemblyDate.ToString("dd/MM/yyyy@HH:mm:ss");
            return String.Format("{0}.{1}.{2}.{3}", assemblyName.Version.Major, assemblyName.Version.Minor, assemblyName.Version.Build, assemblyName.Version.Revision);
        }
        #endregion
    }
}