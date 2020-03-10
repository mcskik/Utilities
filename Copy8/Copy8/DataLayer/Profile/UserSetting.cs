using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Copy8.DataLayer.Profile
{
    /// <summary>
    /// Current User Setting class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class UserSetting : ProfileEntry
    {
        public enum CopyRuleEnum
        {
            SkipMatches,
            ReplaceMatches,
            ReplaceOnlyWithNewer,
            ReplacePreservingNewest
        }

        private ShrinkDir _shrink;
        private XmlProfileHelper _xmlProfileHelper;
        public string NewBaseDir { get; set; }
        public string OldBaseDir { get; set; }
        public string KeyShrunk { get; set; }
        public CopyRuleEnum CopyRule { get; set; }
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
                            new XElement("NewBaseDir", NewBaseDir),
                            new XElement("OldBaseDir", OldBaseDir)
                        ),
                        new XElement("Options",
                            new XElement("CopyRule", CopyRule.ToString()),
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
            NewBaseDir = string.Empty;
            OldBaseDir = string.Empty;
            KeyShrunk = string.Empty;
            CopyRule = CopyRuleEnum.ReplaceOnlyWithNewer;
            MonitoredTypesOnly = false;
        }

        public UserSetting(object parent, XElement userSetting)
            : base(parent, userSetting)
        {
            _shrink = new ShrinkDir();
            _shrink.MaxDisplayLength = 49;
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, userSetting);
            NewBaseDir = _xmlProfileHelper.Fetch("Directories", "NewBaseDir");
            OldBaseDir = _xmlProfileHelper.Fetch("Directories", "OldBaseDir");
            KeyShrunk = "[" + _shrink.ShrinkDirectory(NewBaseDir) + "]-[" + _shrink.ShrinkDirectory(OldBaseDir) + "]";
            string rule = _xmlProfileHelper.Fetch("Options", "CopyRule");
            CopyRuleEnum copyRule = CopyRuleEnum.ReplaceOnlyWithNewer;
            Enum.TryParse(rule, out copyRule);
            CopyRule = copyRule;
            MonitoredTypesOnly = _xmlProfileHelper.FetchBool("Options", "MonitoredTypesOnly");
        }
    }
}