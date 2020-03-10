using GlobalChange8.DataLayer.Profile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GlobalChange8.Models
{
    /// <summary>
    /// Text types helper class.
    /// </summary>
    /// <remarks>
    /// Text types helper class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class TextTypesHelper
    {
        private static List<string> includeFileTypes = new List<string>();
        private static bool useFileTypeInclusions = false;
        private static int fileSizeLimit = 0;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TextTypesHelper()
        {
            useFileTypeInclusions = Administrator.ProfileManager.SystemProfile.UseFileTypeInclusions;
            fileSizeLimit = Administrator.ProfileManager.SystemProfile.FileSizeLimit;
            if (useFileTypeInclusions)
            {
                XDocument includeDoc = XDocument.Load(Administrator.ProfileManager.SystemProfile.IncludeFileTypesFileSpec);
                var includeTypes = (from ft in includeDoc.Descendants("FileType")
                                    let active = ft.Attribute("Active").Value
                                    let category = ft.Attribute("Category").Value
                                    let extension = ft.Attribute("Ext").Value
                                    where (active == "Yes" && category == "Text")
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
            return allowFile;
        }

        /// <summary>
        /// Check if the specified file has and allowed file type and is not too large.
        /// </summary>
        private static bool IsMonitoredType(string fileSpec)
        {
            bool monitoredType = false;
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
                if (includeType)
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