using ProfileData.DataLayer.Profile;
using System;
using System.Xml.Linq;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Repository setting class.
    /// </summary>
    /// <remarks>
    /// One repository setting object.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class RepositorySetting : ProfileEntry
    {
        private XmlProfileHelper _xmlProfileHelper;
        public string Name { get; set; }
        public string RemoteUrl { get; set; }
        public string LocalPath { get; set; }
        public ProfileCache ProfileCache
        {
            get
            {
                return (Parent as RepositorySettings).ProfileCache;
            }
        }
        public override XElement Element
        {
            get
            {
                XElement repositorySetting = new XElement(
                    new XElement("RepositorySetting", new XAttribute("Key", Key),
                        new XElement("Repository",
                            new XElement("Name", Name),
                            new XElement("RemoteUrl", RemoteUrl),
                            new XElement("LocalPath", LocalPath)
                        )
                    )
                );
                return repositorySetting;
            }
        }

        public RepositorySetting()
            : base()
        {
            Name = string.Empty;
            RemoteUrl = string.Empty;
            LocalPath = string.Empty;
        }

        public RepositorySetting(object parent, XElement repositorySetting)
            : base(parent, repositorySetting)
        {
            _xmlProfileHelper = new XmlProfileHelper(ProfileCache, repositorySetting);
            Name = _xmlProfileHelper.Fetch("Repository", "Name");
            RemoteUrl = _xmlProfileHelper.Fetch("Repository", "RemoteUrl");
            LocalPath = _xmlProfileHelper.Fetch("Repository", "LocalPath");
        }

        protected T GetEnumValue<T>(string enumText)
        {
            return (T)Enum.Parse(typeof(T), enumText);
        }
    }
}