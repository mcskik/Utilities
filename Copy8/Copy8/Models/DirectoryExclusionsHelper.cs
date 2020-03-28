using Copy8.DataLayer.Profile;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Copy8
{
    /// <summary>
    /// Directory exclusions helper class.
    /// </summary>
    /// <remarks>
    /// Directory exclusions helper class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class DirectoryExclusionsHelper
    {
        private static List<string> excludeDirectories = new List<string>();
        private static bool useDirectoryExclusions = false;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static DirectoryExclusionsHelper()
        {
            useDirectoryExclusions = Administrator.ProfileManager.SystemProfile.UseDirectoryExclusions;
            if (useDirectoryExclusions)
            {
                XDocument excludeDoc = XDocument.Load(Administrator.ProfileManager.SystemProfile.ExcludeDirectoriesFileSpec);
                var excludes = (from dir in excludeDoc.Descendants("Directory")
                                let active = dir.Attribute("Active").Value
                                let category = dir.Attribute("Category").Value
                                let path = dir.Attribute("Path").Value
                                where (active == "Yes")
                                orderby path
                                select path).ToList();
                excludeDirectories = new List<string>();
                foreach (var exclude in excludes)
                {
                    excludeDirectories.Add(exclude);
                }
            }
        }

        /// <summary>
        /// Check if the specified directory should be allowed.
        /// </summary>
        public static bool AllowDirectory(string directory)
        {
            return !IsExcludedDirectory(directory);
        }

        /// <summary>
        /// Check if the specified directory is excluded.
        /// </summary>
        private static bool IsExcludedDirectory(string directory)
        {
            const string BUILD_SRC = "buildSrc";
            const string BUILD_ALT = "alternativeSrc";
            bool exclude = false;
            if (useDirectoryExclusions)
            {
                if (excludeDirectories.Count > 0)
                {
                    foreach (var exclusion in excludeDirectories)
                    {
                        if (directory.Contains(BUILD_SRC))
                        {
                            directory = directory.Replace(BUILD_SRC, BUILD_ALT);
                        }
                        if (directory.Contains(exclusion))
                        {
                            exclude = true;
                            break;
                        }
                    }
                }
            }
            return exclude;
        }
    }
}