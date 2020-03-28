using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Template parameter setting class.
    /// </summary>
    /// <remarks>
    /// One template parameter setting object.
    /// This will be used for most parameters where only a single parameter value is required.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class TemplateParameterSetting : ProfileEntry
    {
        private XmlProfileHelper _xmlProfileHelper;
        public string Value { get; set; }
        public string Description { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as TemplateParameterSettings).ProfileCache;
            }
        }
        public override XElement Element
        {
            get
            {
                XElement parameterSetting = new XElement(
                    new XElement("TemplateParameterSetting", new XAttribute("Key", Key),
                        new XElement("Group",
                            new XElement("Value", Value),
                            new XElement("Description", Description)
                        )
                   )
                );
                return parameterSetting;
            }
        }

        public TemplateParameterSetting()
            : base()
        {
            Value = string.Empty;
        }

        public TemplateParameterSetting(object parent, XElement templateParameterSetting)
            : base(parent, templateParameterSetting)
        {
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, templateParameterSetting);
            Value = _xmlProfileHelper.Fetch("Group", "Value");
            try
            {
                Description = _xmlProfileHelper.Fetch("Group", "Description");
            }
            catch (Exception ex)
            {
                Description = string.Empty;
            }
        }

        protected T GetEnumValue<T>(string enumText)
        {
            return (T)Enum.Parse(typeof(T), enumText);
        }
    }
}