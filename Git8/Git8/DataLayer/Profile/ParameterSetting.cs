using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Parameter setting class.
    /// </summary>
    /// <remarks>
    /// One generic parameter setting object.
    /// This will be used for most parameters where only a single parameter value is required.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ParameterSetting : ProfileEntry
    {
        private XmlProfileHelper _xmlProfileHelper;
        public string Value { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as ParameterSettings).ProfileCache;
            }
        }
        public override XElement Element
        {
            get
            {
                XElement parameterSetting = new XElement(
                    new XElement("ParameterSetting", new XAttribute("Key", Key),
                        new XElement("Group",
                            new XElement("Value", Value)
                        )
                   )
                );
                return parameterSetting;
            }
        }

        public ParameterSetting()
            : base()
        {
            Value = string.Empty;
        }

        public ParameterSetting(object parent, XElement parameterSetting)
            : base(parent, parameterSetting)
        {
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, parameterSetting);
            Value = _xmlProfileHelper.Fetch("Group", "Value");
        }

        protected T GetEnumValue<T>(string enumText)
        {
            return (T)Enum.Parse(typeof(T), enumText);
        }
    }
}