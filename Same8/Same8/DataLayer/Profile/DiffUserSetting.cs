using ProfileData.DataLayer.Profile;
using Same8.Models;
using System.Xml.Linq;

namespace Same8.DataLayer.Profile
{
    /// <summary>
    /// Current User Setting class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class DiffUserSetting : ProfileEntry
    {
        private ShrinkDir _shrink;
        private XmlProfileHelper _xmlProfileHelper;
        public string NewBaseDir { get; set; }
        public string OldBaseDir { get; set; }
        public string NewFile { get; set; }
        public string OldFile { get; set; }
        public string KeyShrunk { get; set; }
        public long MinChars { get; set; }
        public long MinLines { get; set; }
        public long LimitCharacters { get; set; }
        public long LimitLines { get; set; }
        public long SubLineMatchLimit { get; set; }
        public bool CompleteLines { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as DiffUserSettings).ProfileCache;
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
                        new XElement("Files",
                            new XElement("NewFile", NewFile),
                            new XElement("OldFile", OldFile)
                        ),
                        new XElement("Options",
                            new XElement("MinChars", MinChars),
                            new XElement("MinLines", MinLines),
                            new XElement("LimitCharacters", LimitCharacters),
                            new XElement("LimitLines", LimitLines),
                            new XElement("SubLineMatchLimit", SubLineMatchLimit),
                            new XElement("CompleteLines", CompleteLines)
                        )
                   )
                );
                return userSetting;
            }
        }

        public DiffUserSetting()
            : base()
        {
            NewBaseDir = string.Empty;
            OldBaseDir = string.Empty;
            NewFile = string.Empty;
            OldFile = string.Empty;
            KeyShrunk = string.Empty;
            MinChars = 8;
            MinLines = 1;
            LimitCharacters = 255;
            LimitLines = 20;
            SubLineMatchLimit = 1024;
            CompleteLines = false;
        }

        public DiffUserSetting(object parent, XElement userSetting)
            : base(parent, userSetting)
        {
            _shrink = new ShrinkDir();
            _shrink.MaxDisplayLength = 49;
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, userSetting);
            NewBaseDir = _xmlProfileHelper.Fetch("Directories", "NewBaseDir");
            OldBaseDir = _xmlProfileHelper.Fetch("Directories", "OldBaseDir");
            NewFile = _xmlProfileHelper.Fetch("Files", "NewFile");
            OldFile = _xmlProfileHelper.Fetch("Files", "OldFile");
            KeyShrunk = "[" + _shrink.ShrinkDirectory(NewFile) + "]-[" + _shrink.ShrinkDirectory(OldFile) + "]";
            MinChars = _xmlProfileHelper.FetchInt("Options", "MinChars");
            MinLines = _xmlProfileHelper.FetchInt("Options", "MinLines");
            LimitCharacters = _xmlProfileHelper.FetchInt("Options", "LimitCharacters");
            LimitLines = _xmlProfileHelper.FetchInt("Options", "LimitLines");
            SubLineMatchLimit = _xmlProfileHelper.FetchInt("Options", "SubLineMatchLimit");
            CompleteLines = _xmlProfileHelper.FetchBool("Options", "CompleteLines");
        }
    }
}