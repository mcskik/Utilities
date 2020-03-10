using ProfileData.DataLayer.Profile;
using Same8.Models;
using System.Xml.Linq;

namespace Same8.DataLayer.Profile
{
    /// <summary>
    /// Current User Setting class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class UserSetting : ProfileEntry
    {
        private ShrinkDir _shrink;
        private XmlProfileHelper _xmlProfileHelper;
        public string NewPath { get; set; }
        public string OldPath { get; set; }
        public string KeyShrunk { get; set; }
        public long NewFilesEstimate { get; set; }
        public long OldFilesEstimate { get; set; }
        public long ChgFilesEstimate { get; set; }
        public bool MonitoredTypesOnly { get; set; }
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
                        new XElement("Directories",
                            new XElement("NewPath", NewPath),
                            new XElement("OldPath", OldPath),
                            new XElement("NewFilesEstimate", NewFilesEstimate),
                            new XElement("OldFilesEstimate", OldFilesEstimate),
                            new XElement("ChgFilesEstimate", ChgFilesEstimate)
                        ),
                        new XElement("Options",
                            new XElement("MonitoredTypesOnly", MonitoredTypesOnly)
                        )
                   )
                );
                return userSetting;
            }
        }

        public UserSetting()
            : base()
        {
            NewPath = string.Empty;
            OldPath = string.Empty;
            NewFilesEstimate = 100;
            OldFilesEstimate = 100;
            ChgFilesEstimate = 100;
            KeyShrunk = string.Empty;
            MonitoredTypesOnly = false;
        }

        public UserSetting(object parent, XElement userSetting)
            : base(parent, userSetting)
        {
            _shrink = new ShrinkDir();
            _shrink.MaxDisplayLength = 49;
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, userSetting);
            NewPath = _xmlProfileHelper.Fetch("Directories", "NewPath");
            OldPath = _xmlProfileHelper.Fetch("Directories", "OldPath");
            NewFilesEstimate = _xmlProfileHelper.FetchInt("Directories", "NewFilesEstimate");
            OldFilesEstimate = _xmlProfileHelper.FetchInt("Directories", "OldFilesEstimate");
            ChgFilesEstimate = _xmlProfileHelper.FetchInt("Directories", "ChgFilesEstimate");
            KeyShrunk = "[" + _shrink.ShrinkDirectory(NewPath) + "]-[" + _shrink.ShrinkDirectory(OldPath) + "]";
            MonitoredTypesOnly = _xmlProfileHelper.FetchBool("Options", "MonitoredTypesOnly");
        }
    }
}