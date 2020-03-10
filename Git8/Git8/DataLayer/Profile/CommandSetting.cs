using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Command setting class.
    /// </summary>
    /// <remarks>
    /// One command setting object.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class CommandSetting : ProfileEntry
    {
        private XmlProfileHelper _xmlProfileHelper;
        public string Line { get; set; }
        public string Template { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as CommandSettings).ProfileCache;
            }
        }
        public override XElement Element
        {
            get
            {
                XElement commandSetting = new XElement(
                    new XElement("CommandSetting", new XAttribute("Key", Key),
                        new XElement("Command",
                            new XElement("Line", Line),
                            new XElement("Template", Template)
                        )
                    )
                );
                return commandSetting;
            }
        }

        public CommandSetting()
            : base()
        {
            Line = string.Empty;
            Template = string.Empty;
        }

        public CommandSetting(object parent, XElement commandSetting)
            : base(parent, commandSetting)
        {
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, commandSetting);
            Line = _xmlProfileHelper.Fetch("Command", "Line");
            Template = _xmlProfileHelper.Fetch("Command", "Template");
        }

        protected T GetEnumValue<T>(string enumText)
        {
            return (T)Enum.Parse(typeof(T), enumText);
        }
    }
}