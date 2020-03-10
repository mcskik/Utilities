using Copy8.DataLayer.Profile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Copy8
{
    /// <summary>
    /// Monitored types helper class.
    /// </summary>
    /// <remarks>
    /// Monitored types helper class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class MonitoredTypesHelper
    {
        private static List<string> excludeFileTypes = new List<string>();
        private static List<string> includeFileTypes = new List<string>();
        private static bool useFileTypeExclusions = false;
        private static bool useFileTypeInclusions = false;
        private static int fileSizeLimit = 0;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MonitoredTypesHelper()
        {
            useFileTypeExclusions = Administrator.ProfileManager.SystemProfile.UseFileTypeExclusions;
            useFileTypeInclusions = Administrator.ProfileManager.SystemProfile.UseFileTypeInclusions;
            fileSizeLimit = Administrator.ProfileManager.SystemProfile.FileSizeLimit;
            if (useFileTypeExclusions)
            {
                XDocument excludeDoc = XDocument.Load(Administrator.ProfileManager.SystemProfile.ExcludeFileTypesFileSpec);
                var excludeTypes = (from ft in excludeDoc.Descendants("FileType")
                                    let active = ft.Attribute("Active").Value
                                    let category = ft.Attribute("Category").Value
                                    let extension = ft.Attribute("Ext").Value
                                    where (active == "Yes")
                                    orderby extension
                                    select extension).ToList();
                excludeFileTypes = new List<string>();
                foreach (var exclude in excludeTypes)
                {
                    excludeFileTypes.Add(exclude);
                }
            }
            if (useFileTypeInclusions)
            {
                XDocument includeDoc = XDocument.Load(Administrator.ProfileManager.SystemProfile.IncludeFileTypesFileSpec);
                var includeTypes = (from ft in includeDoc.Descendants("FileType")
                                    let active = ft.Attribute("Active").Value
                                    let category = ft.Attribute("Category").Value
                                    let extension = ft.Attribute("Ext").Value
                                    where (active == "Yes")
                                    orderby extension
                                    select extension).ToList();
                includeFileTypes = new List<string>();
                foreach (var include in includeTypes)
                {
                    includeFileTypes.Add(include);
                }
            }
        }

        /// <summary>
        /// Check if the specified file should be allowed.
        /// </summary>
        public static bool AllowFile(string fileSpec, bool monitoredTypesOnly)
        {
            bool allowFile = false;
            if (monitoredTypesOnly)
            {
                allowFile = IsMonitoredType(fileSpec);
            }
            else
            {
                allowFile = true;
            }
            Administrator.RecordFileType(fileSpec, allowFile);
            return allowFile;
        }

        /// <summary>
        /// Check if the specified file has and allowed file type and is not too large.
        /// </summary>
        private static bool IsMonitoredType(string fileSpec)
        {
            bool monitoredType = false;
            bool excludeType = false;
            bool includeType = true;
            if (File.Exists(fileSpec))
            {
                FileInfo fi = new FileInfo(fileSpec);
                string type = string.Empty;
                type = Path.GetExtension(fileSpec);
                type = type.ToLower();
                if (type.StartsWith("."))
                {
                    if (type.Length > 1)
                    {
                        type = type.Substring(1);
                    }
                }
                if (useFileTypeExclusions)
                {
                    if (excludeFileTypes.Contains(type))
                    {
                        excludeType = true;
                    }
                    else
                    {
                        excludeType = false;
                    }
                }
                if (useFileTypeInclusions)
                {
                    if (includeFileTypes.Contains(type))
                    {
                        includeType = true;
                    }
                    else
                    {
                        includeType = false;
                    }
                }
                if (!excludeType && includeType)
                {
                    monitoredType = true;
                }
                else
                {
                    monitoredType = false;
                }
                if (fileSizeLimit > 0)
                {
                    //Maximum size check.
                    long maxSize = fileSizeLimit;
                    long size = 0;
                    try
                    {
                        size = fi.Length;
                        if (size > maxSize)
                        {
                            monitoredType = false;
                        }
                    }
                    catch
                    {
                        monitoredType = false;
                    }
                }
            }
            return monitoredType;
        }
    }
}