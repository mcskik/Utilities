using System;
using System.Collections.Specialized;
using System.Xml.Linq;
using ProfileData.DataLayer.Profile;

namespace GlobalChange8.DataLayer.Profile
{
    /// <summary>
    /// Current User Setting class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class UserSetting : ProfileEntry
    {
        public enum SearchEncoding
        {
            Ascii = 1,
            Unicode = 2
        }
        private XmlProfileHelper _xmlProfileHelper;
        public string SearchPath { get; set; }
        public string Criteria { get; set; }
        public string Find { get; set; }
        public string Replacement { get; set; }
        public string Mode { get; set; }
        public long DirFilesEstimate { get; set; }
        public string FilePattern { get; set; }
        public bool RegexCriteria { get; set; }
        public bool AllTypes { get; set; }
        public SearchEncoding Encoding { get; set; }
        public StringCollection Options { get; set; }
        public StringCollection Extensions { get; set; }
        public StringCollection DirectoryExclusions { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as UserSettings).ProfileCache;
            }
        }
        public override XElement Element
        {
            get
            {
                XElement userSetting = new XElement(
                    new XElement("UserSetting", new XAttribute("Key", Key),
                        new XElement("Parameters",
                            new XElement("SearchPath", SearchPath),
                            new XElement("Criteria", Criteria),
                            new XElement("Find", Find),
                            new XElement("Replacement", Replacement),
                            new XElement("Mode", Mode),
                            new XElement("DirFilesEstimate", DirFilesEstimate),
                            new XElement("FilePattern", FilePattern),
                            new XElement("RegexCriteria", RegexCriteria),
                            new XElement("AllTypes", AllTypes),
                            new XElement("Encoding", Encoding)
                        )
                   )
                );
                if (Options.Count > 0)
                {
                    XElement options = new XElement("Options");
                    foreach (string opt in Options)
                    {
                        options.Add(new XElement("Option", opt));
                    }
                    userSetting.Add(options);
                }
                if (Extensions.Count > 0)
                {
                    XElement extensions = new XElement("Extensions");
                    foreach (string ext in Extensions)
                    {
                        extensions.Add(new XElement("Extension", ext));
                    }
                    userSetting.Add(extensions);
                }
                if (DirectoryExclusions.Count > 0)
                {
                    XElement exclusions = new XElement("DirectoryExclusions");
                    foreach (string exclusion in DirectoryExclusions)
                    {
                        exclusions.Add(new XElement("DirectoryExclusion", exclusion));
                    }
                    userSetting.Add(exclusions);
                }
                return userSetting;
            }
        }

        public UserSetting()
            : base()
        {
            SearchPath = string.Empty;
            Criteria = string.Empty;
            Find = string.Empty;
            Replacement = string.Empty;
            Mode = string.Empty;
            DirFilesEstimate = 0;
            FilePattern = string.Empty;
            RegexCriteria = false;
            AllTypes = true;
            Encoding = SearchEncoding.Ascii;
            Options = new StringCollection();
            Extensions = new StringCollection();
            DirectoryExclusions = new StringCollection();
        }

        public UserSetting(object parent, XElement userSetting)
            : base(parent, userSetting)
        {
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, userSetting);
            SearchPath = _xmlProfileHelper.Fetch("Parameters", "SearchPath");
            Criteria = _xmlProfileHelper.Fetch("Parameters", "Criteria");
            Find = _xmlProfileHelper.Fetch("Parameters", "Find");
            Replacement = _xmlProfileHelper.Fetch("Parameters", "Replacement");
            Mode = _xmlProfileHelper.Fetch("Parameters", "Mode");
            DirFilesEstimate = _xmlProfileHelper.FetchLong("Parameters", "DirFilesEstimate");
            FilePattern = _xmlProfileHelper.Fetch("Parameters", "FilePattern");
            RegexCriteria = _xmlProfileHelper.FetchBool("Parameters", "RegexCriteria");
            AllTypes = _xmlProfileHelper.FetchBool("Parameters", "AllTypes");
            Encoding = GetEnumValue<SearchEncoding>(_xmlProfileHelper.Fetch("Parameters", "Encoding"));
            Options = new StringCollection();
            foreach (string opt in userSetting.Descendants("Option"))
            {
                Options.Add(opt);
            }
            Extensions = new StringCollection();
            foreach (string ext in userSetting.Descendants("Extension"))
            {
                Extensions.Add(ext);
            }
            DirectoryExclusions = new StringCollection();
            foreach (string exclusion in userSetting.Descendants("DirectoryExclusion"))
            {
                DirectoryExclusions.Add(exclusion);
            }
        }

        protected T GetEnumValue<T>(string enumText)
        {
            return (T)Enum.Parse(typeof(T), enumText);
        }
    }
}