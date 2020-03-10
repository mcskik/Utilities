using System;
using System.IO;
using System.Reflection;
using ProfileData.DataLayer.Profile;
using System.Configuration;

namespace Git8.DataLayer.Profile
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
        private ProfileCache _profileCache;
        private string _separator = Path.DirectorySeparatorChar.ToString();
        private string _cdUp = ".." + Path.DirectorySeparatorChar.ToString();
        private string _assemblyVersion = string.Empty;
        private string _masterDataPath = string.Empty;
        private string _dataPath = string.Empty;
        private string _RepositoryName = string.Empty;
        private string _assemblyTitle = string.Empty;
        private string _assemblyProduct = string.Empty;

        public string MasterDataPath
        {
            get
            {
                return _masterDataPath;
            }
        }

        public string DataPath
        {
            get
            {
                return _dataPath;
            }
        }

        public string RepositoryName
        {
            get
            {
                return _RepositoryName;
            }
            set
            {
                _RepositoryName = value;
                _profileCache.Store("RepositoryName", RepositoryName);
                _profileCache.Store("RepositorySettingsPath", RepositorySettingsPath);
            }
        }

        public string MasterConfigPath
        {
            get
            {
                return String.Format(@"{0}Config", MasterDataPath);
            }
        }

        public string ConfigPath
        {
            get
            {
                return String.Format(@"{0}Config", DataPath);
            }
        }

        public string MasterSettingsPath
        {
            get
            {
                return String.Format(@"{0}Config\Settings", MasterDataPath);
            }
        }

        public string RepositorySettingsPath
        {
            get
            {
                return String.Format(@"{0}Config\{1}\", DataPath, RepositoryName);
            }
        }

        public string SystemProfileXml
        {
            get
            {
                string xmlFile = Path.GetFullPath(String.Format(@"{0}Config{1}SystemProfile.xml", DataPath, _separator));
                return xmlFile;
            }
        }

        public string UserName
        {
            get
            {
                return System.Environment.UserName;
            }
        }

        public string Version
        {
            get
            {
                return _assemblyVersion;
            }
        }

        public string AssemblyTitle
        {
            get
            {
                return _assemblyTitle;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                return _assemblyProduct;
            }
        }

        public ApplicationProfile(ProfileCache profileCache)
        {
            _profileCache = profileCache;
            GetCustomAttributes();
            _masterDataPath = GetMasterDataPath();
            _dataPath = GetApplicationDataPath();
            _assemblyVersion = GetAssemblyVersion();
            _profileCache.Store("MasterDataPath", MasterDataPath);
            _profileCache.Store("DataPath", DataPath);
            _profileCache.Store("SystemProfileXml", SystemProfileXml);
            _profileCache.Store("UserName", UserName);
            _profileCache.Store("Version", Version);
            _profileCache.Store("AssemblyTitle", AssemblyTitle);
            _profileCache.Store("AssemblyProduct", AssemblyProduct);
        }

        private string GetMasterDataPath()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string startupPath = string.Empty;
            startupPath = assembly.Location;
            startupPath = Path.GetDirectoryName(startupPath);
            string appPath = startupPath + _separator;
            Directory.SetCurrentDirectory(appPath);
            _masterDataPath = Path.GetFullPath(String.Format(@"{0}{0}Data{1}", _cdUp, _separator));
            try
            {
                string overrideDataPath = ConfigurationManager.AppSettings["OverrideDataPath"];
                if (overrideDataPath != null && overrideDataPath.ToLower() == "true")
                {
                    string originDataPathValue = ConfigurationManager.AppSettings["OriginDataPath"];
                    if (originDataPathValue != null)
                    {
                        _masterDataPath = originDataPathValue;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return _masterDataPath;
        }

        private string GetApplicationDataPath()
        {
            string applicationDataPath = string.Empty;
            applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            applicationDataPath = Path.Combine(applicationDataPath, AssemblyProduct);
            applicationDataPath = Path.Combine(applicationDataPath, "Data");
            applicationDataPath += _separator;
            return applicationDataPath;
        }

        private string GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            return String.Format("{0}.{1}.{2}.{3}", assemblyName.Version.Major, assemblyName.Version.Minor, assemblyName.Version.Build, assemblyName.Version.Revision);
        }

        private void GetCustomAttributes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is AssemblyTitleAttribute)
                {
                    AssemblyTitleAttribute attr = attribute as AssemblyTitleAttribute;
                    if (attr != null)
                    {
                        _assemblyTitle = attr.Title;
                    }
                }
                if (attribute is AssemblyProductAttribute)
                {
                    AssemblyProductAttribute attr = attribute as AssemblyProductAttribute;
                    if (attr != null)
                    {
                        _assemblyProduct = attr.Product;
                    }
                }
            }
        }
    }
}