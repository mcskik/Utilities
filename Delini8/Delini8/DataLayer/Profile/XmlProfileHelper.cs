using ProfileData.DataLayer.Profile;
using System.Linq;
using System.Xml.Linq;

namespace Delini8.DataLayer.Profile
{
    public class XmlProfileHelper
    {
        ProfileCache ProfileCache { get; set; }
        XElement ContainerElement { get; set; }

        public XmlProfileHelper(ProfileCache profileCache, XElement containerElement)
        {
            ProfileCache = profileCache;
            ContainerElement = containerElement;
        }

        public void FetchAll(string groupName)
        {
            if (ContainerElement.Descendants(groupName).Count() > 0)
            {
                XElement groupElement = ContainerElement.Descendants(groupName).First();
                foreach (XElement element in groupElement.Elements())
                {
                    string elementName = element.Name.LocalName;
                    Fetch(groupName, elementName);
                }
            }
        }

        public int FetchInt(string groupName, string elementName)
        {
            int value = (int)ContainerElement.Descendants(groupName).First().Element(elementName);
            ProfileCache.Store(elementName, value.ToString());
            return value;
        }

        public bool FetchBool(string groupName, string elementName)
        {
            bool value = (bool)ContainerElement.Descendants(groupName).First().Element(elementName);
            ProfileCache.Store(elementName, value.ToString());
            return value;
        }

        public string Fetch(string groupName, string elementName)
        {
            string value = (string)ContainerElement.Descendants(groupName).First().Element(elementName);
            value = Substitute(value);
            ProfileCache.Store(elementName, value);
            return value;
        }

        public string Substitute(string original)
        {
            string value = ProfileCache.Substitute(original);
            return value;
        }
    }
}