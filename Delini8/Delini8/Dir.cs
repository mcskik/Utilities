using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using R = Delini8.Routines;

namespace Delini8
{
    /// <summary>
    /// Directory listing object.
    /// </summary>
    /// <remarks>
    /// Return a list of all directories and files contained
    /// within the specified top level directory.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Dir
    {
        #region Events.
        public event EventDelegate EventBeginProgress;
        public event EventDelegate EventUpdateProgress;
        public event EventDelegate EventEndOfProgress;
        #endregion

        #region Event helper routines.
        /// <summary>
        /// Trigger begin progress event.
        /// </summary>
        /// <param name="psMethod">Which method in overall process is running.</param>
        public void SignalBeginProgress(string psMethod)
        {
            if (EventBeginProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(psMethod);
                EventBeginProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger update progress event.
        /// </summary>
        /// <param name="psMethod">Which method in overall process is running.</param>
        public void SignalUpdateProgress(string psMethod)
        {
            if (EventUpdateProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(psMethod);
                EventUpdateProgress(this, oEventParameters);
            }
        }

        /// <summary>
        /// Trigger end of progress event.
        /// </summary>
        /// <param name="psMethod">Which method in overall process is running.</param>
        public void SignalEndOfProgress(string psMethod)
        {
            if (EventEndOfProgress != null)
            {
                EventParameters oEventParameters = new EventParameters(psMethod);
                EventEndOfProgress(this, oEventParameters);
            }
        }
        #endregion

        #region Properties.
        public SortedDictionary<string, string> Exts { get; set; }
        public Dictionary<string, Dictionary<string, long>> Counters { get; set; }
        public SortedDictionary<int, string> GroupNames { get; set; }
        public SortedDictionary<int, string> CounterNames { get; set; }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Dir()
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Scan directory information into the directory ArrayList.
        /// </summary>
	public virtual void DirList(string psQualifier, ref long pnEstimate)
        {
            InitializeCounts();
            ArrayList oDirs = new ArrayList();
            string sDirectory = "";
            long nCount;
            long nEntries;
            SignalBeginProgress("DirList");
            try
            {
                //Build directory and file list.
                oDirs = new ArrayList();
                psQualifier = psQualifier.Trim();
                psQualifier = R.Strip(psQualifier, "TRAILING", @"\");
                oDirs.Add(psQualifier);
                pnEstimate = 1;
                nCount = 1;
                nEntries = oDirs.Count;
                while (nCount <= nEntries)
                {
                    sDirectory = oDirs[(int)nCount - 1].ToString();
                    DirLevel(psQualifier, sDirectory, ref oDirs, ref pnEstimate);
                    nCount = nCount + 1;
                    nEntries = oDirs.Count;
                }
            }
            catch (Exception oException)
            {
                Console.WriteLine(oException.Message);
            }
            finally
            {
                FinalizeCounts();
                SignalEndOfProgress("DirList");
            }
        }

        /// <summary>
        /// Append directory level information into the specified recordset.
        /// </summary>
	public void DirLevel(string psQualifier, string psDirectory, ref ArrayList poDirs, ref long pnEstimate)
        {
            string sSpec;
            string sPath;
            string sName;
            sPath = psDirectory;
            if (psDirectory.Length > psQualifier.Length)
            {
                psDirectory = psDirectory.Substring(psQualifier.Length);
            }
            else
            {
                psDirectory = string.Empty;
            }
            try
            {
                DirectoryInfo oDirectory = new DirectoryInfo(sPath);
                oDirectory.Refresh();
                DirectoryInfo[] aDirs = oDirectory.GetDirectories();
                for (int nRow = 0; nRow < aDirs.Length; nRow++)
                {
                    try
                    {
                        sName = aDirs[nRow].Name;
                        sSpec = sPath + "\\" + sName;
                        if (aDirs[nRow].Exists)
                        {
                            poDirs.Add(sSpec);
                            pnEstimate += 1;
                            SignalUpdateProgress("DirList");
                        }
                    }
                    catch (Exception oExceptionA)
                    {
                        Console.WriteLine(oExceptionA.Message);
                    }
                }
                FileInfo[] aFiles = oDirectory.GetFiles();
                for (int nRow = 0; nRow < aFiles.Length; nRow++)
                {
                    try
                    {
                        sName = aFiles[nRow].Name;
                        sSpec = sPath + "\\" + sName;
                        if (aFiles[nRow].Exists)
                        {
                            string sContents = R.ReadFile(sSpec);
                            Classify(aFiles[nRow], R.TextRows(sContents));
                            pnEstimate += 1;
                            SignalUpdateProgress("DirList");
                        }
                    }
                    catch (Exception oExceptionB)
                    {
                        Console.WriteLine(oExceptionB.Message);
                    }
                }
            }
            catch (Exception oExceptionD)
            {
                Console.WriteLine(oExceptionD.Message);
            }
            finally
            {
            }
        }

        public void InitializeCounts()
        {
            Counters = new Dictionary<string, Dictionary<string, long>>();
            GroupNames = new SortedDictionary<int, string>();
            GroupNames.Add(1, "All");
            GroupNames.Add(2, "Main");
            GroupNames.Add(3, "AAA");
            GroupNames.Add(4, "BBB");
            CounterNames = new SortedDictionary<int, string>();
            CounterNames.Add(1, "TotalLines");
            CounterNames.Add(2, "TotalFiles");
            CounterNames.Add(3, "JavaLines");
            CounterNames.Add(4, "JavaFiles");
            CounterNames.Add(5, "KotlinLines");
            CounterNames.Add(6, "KotlinFiles");
            CounterNames.Add(7, "ProgramLines");
            CounterNames.Add(8, "ProgramFiles");
            CounterNames.Add(9, "HtmlLines");
            CounterNames.Add(10, "HtmlFiles");
            CounterNames.Add(11, "JsonLines");
            CounterNames.Add(12, "JsonFiles");
            CounterNames.Add(13, "DataLines");
            CounterNames.Add(14, "DataFiles");
            CounterNames.Add(15, "GradleLines");
            CounterNames.Add(16, "GradleFiles");
            CounterNames.Add(17, "PropertiesLines");
            CounterNames.Add(18, "PropertiesFiles");
            CounterNames.Add(19, "ProLines");
            CounterNames.Add(20, "ProFiles");
            CounterNames.Add(21, "BuildLines");
            CounterNames.Add(22, "BuildFiles");
            CounterNames.Add(23, "CommonLines");
            CounterNames.Add(24, "CommonFiles");
            CounterNames.Add(25, "FeaturesLines");
            CounterNames.Add(26, "FeaturesFiles");
            CounterNames.Add(27, "CodeLines");
            CounterNames.Add(28, "CodeFiles");
            CounterNames.Add(29, "XmlLines");
            CounterNames.Add(30, "XmlFiles");
            CounterNames.Add(31, "ManifestLines");
            CounterNames.Add(32, "ManifestFiles");
            CounterNames.Add(33, "ColorLines");
            CounterNames.Add(34, "ColorFiles");
            CounterNames.Add(35, "DrawableLines");
            CounterNames.Add(36, "DrawableFiles");
            CounterNames.Add(37, "LayoutLines");
            CounterNames.Add(38, "LayoutFiles");
            CounterNames.Add(39, "MenuLines");
            CounterNames.Add(40, "MenuFiles");
            CounterNames.Add(41, "ValuesLines");
            CounterNames.Add(42, "ValuesFiles");
            CounterNames.Add(43, "MarkupLines");
            CounterNames.Add(44, "MarkupFiles");
            CounterNames.Add(45, "OtherLines");
            CounterNames.Add(46, "OtherFiles");
            CounterNames.Add(47, "ImageFiles");
            CounterNames.Add(48, "GrandTotalLines");
            CounterNames.Add(49, "GrandTotalFiles");
            foreach (KeyValuePair<int, string> group in GroupNames)
            {
                Dictionary<string, long> groupCounters = new Dictionary<string, long>();
                foreach (KeyValuePair<int, string> name in CounterNames)
                {
                    groupCounters.Add(name.Value, 0);
                }
                Counters.Add(group.Value, groupCounters);
            }
            Exts = new SortedDictionary<string, string>();
        }

        private void Classify(FileInfo fileInfo, long lines)
        {
            bool classified = false;
            string path = fileInfo.DirectoryName;
            string name = fileInfo.Name;
            string ext = fileInfo.Extension;
            ext = ext.ToLower();
            if (ext.Length > 1 && ext.StartsWith("."))
            {
                ext = ext.Substring(1);
            }
            //TODO: Any types you want to ignore.
            if (ext == "apk") return;
            if (ext == "bat") return;
            if (ext == "gitignore") return;
            if (ext == "gradlew") return;
            if (name == "gradlew") return;
            if (ext == "jar") return;
            if (ext == "jks") return;
            if (ext == "log") return;
            if (ext == "ttf") return;
            if (!Exts.ContainsKey(ext))
            {
                Exts.Add(ext, ext);
            }
            SubClassify("Total", path, name, ext, lines);

            //Types.
            switch (ext)
            {
                case "java":
                    classified = true;
                    SubClassify("Java", path, name, ext, lines);
                    SubClassify("Program", path, name, ext, lines);
                    break;
                case "kt":
                    classified = true;
                    SubClassify("Kotlin", path, name, ext, lines);
                    SubClassify("Program", path, name, ext, lines);
                    break;
                case "html":
                    classified = true;
                    SubClassify("Html", path, name, ext, lines);
                    SubClassify("Data", path, name, ext, lines);
                    break;
                case "json":
                    classified = true;
                    SubClassify("Json", path, name, ext, lines);
                    SubClassify("Data", path, name, ext, lines);
                    break;
                case "gradle":
                    classified = true;
                    SubClassify("Gradle", path, name, ext, lines);
                    SubClassify("Build", path, name, ext, lines);
                    break;
                case "properties":
                    classified = true;
                    SubClassify("Properties", path, name, ext, lines);
                    SubClassify("Build", path, name, ext, lines);
                    break;
                case "pro":
                    classified = true;
                    SubClassify("Pro", path, name, ext, lines);
                    SubClassify("Build", path, name, ext, lines);
                    break;
                case "xml":
                    //These are sub classified separately as well into the different types of xml files.
                    classified = true;
                    SubClassify("Xml", path, name, ext, lines);
                    break;
                case "jpg":
                    //No line count for these.
                    classified = true;
                    SubClassify("Image", path, name, ext, lines);
                    break;
                case "png":
                    //No line count for these.
                    classified = true;
                    SubClassify("Image", path, name, ext, lines);
                    break;
                default:
                    break;
            }

            //Groups.
            if (path.Contains("common"))
            {
                SubClassify("Common", path, name, ext, lines);
                SubClassify("Code", path, name, ext, lines);
            }
            else if (path.Contains("features"))
            {
                SubClassify("Features", path, name, ext, lines);
                SubClassify("Code", path, name, ext, lines);
            }

            //Folders.
            if (ext == "xml")
            {
                if (name.Contains("AndroidManifest"))
                {
                    SubClassify("Manifest", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                else if (path.Contains("color"))
                {
                    SubClassify("Color", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                else if (path.Contains("drawable"))
                {
                    SubClassify("Drawable", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                else if (path.Contains("layout"))
                {
                    SubClassify("Layout", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                else if (path.Contains("menu"))
                {
                    SubClassify("Menu", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                if (path.Contains("mipmap"))
                {
                    SubClassify("Mipmap", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
                else if (path.Contains("values"))
                {
                    SubClassify("Values", path, name, ext, lines);
                    SubClassify("Markup", path, name, ext, lines);
                }
            }

            //Other.
            if (!classified)
            {
                SubClassify("Other", path, name, ext, lines);
            }
        }

        public void SubClassify(string counterNamePrefix, string path, string name, string ext, long lines)
        {
            if (ext == "jpg" || ext == "png")
            {
                // Lines count not avaialble for image files.
                lines = 0;
            }
            if (Counters.ContainsKey("All"))
            {
                SubClassifyHelper("All", counterNamePrefix, lines);
            }
            if (path.Contains(@"\" + "main" + @"\"))
            {
                SubClassifyHelper("Main", counterNamePrefix, lines);
            }
            if (path.Contains(@"\" + "test" + @"\"))
            {
                SubClassifyHelper("Main", counterNamePrefix, lines);
            }
            if (path.Contains(@"\" + "aaa" + @"\"))
            {
                SubClassifyHelper("AAA", counterNamePrefix, lines);
            }
            if (path.Contains(@"\" + "bbb" + @"\"))
            {
                SubClassifyHelper("BBB", counterNamePrefix, lines);
            }
        }

        public void SubClassifyHelper(string groupName, string counterNamePrefix, long lines)
        {
            string counterNameLines = counterNamePrefix + "Lines";
            string counterNameFiles = counterNamePrefix + "Files";
            if (Counters.ContainsKey(groupName))
            {
                var groupCounters = Counters[groupName];
                if (groupCounters.ContainsKey(counterNameLines))
                {
                    groupCounters[counterNameLines] += lines;
                }
                if (groupCounters.ContainsKey(counterNameFiles))
                {
                    groupCounters[counterNameFiles]++;
                }
            }
        }

        private void FinalizeCounts()
        {
            FinalizeCount("Program");
            FinalizeCount("Data");
            FinalizeCount("Build");
            FinalizeCount("Xml");
            FinalizeCount("Image");
            FinalizeCount("Other");
        }

        private void FinalizeCount(string counterNamePrefix)
        {
            FinalizeCountHelper("All", counterNamePrefix);
            FinalizeCountHelper("Main", counterNamePrefix);
            FinalizeCountHelper("AAA", counterNamePrefix);
            FinalizeCountHelper("BBB", counterNamePrefix);
        }

        private void FinalizeCountHelper(string groupName, string counterNamePrefix)
        {
            string counterNameLines = counterNamePrefix + "Lines";
            string counterNameFiles = counterNamePrefix + "Files";
            if (Counters.ContainsKey(groupName))
            {
                var groupCounters = Counters[groupName];
                if (groupCounters.ContainsKey(counterNameLines))
                {
                    if (counterNamePrefix != "Image")
                    {
                        // Lines count not avaialble for image files.
                        groupCounters["GrandTotalLines"] += groupCounters[counterNameLines];
                    }
                }
                if (groupCounters.ContainsKey(counterNameFiles))
                {
                    groupCounters["GrandTotalFiles"] += groupCounters[counterNameFiles];
                }
            }
        }
        #endregion
    }

    #region Delegate declarations.
    public delegate void EventDelegate(object poSender, EventParameters poEventArgs);
    #endregion

    #region Event parameters.
    /// <summary>
    /// Class that defines data for event handling.
    /// </summary>
    public class EventParameters : EventArgs
    {
        #region Member Variables.
        private string mEventData;
        #endregion

        #region Properties.
        /// <summary>
        /// Event data.
        /// </summary>
        public string EventData
        {
            get
            {
                return mEventData;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psEventData">Event data.</param>
        public EventParameters(string psEventData)
        {
            mEventData = psEventData;
        }
        #endregion
    }
    #endregion
}
