using ProfileData.DataLayer.Profile;
using System.Xml.Linq;

namespace Lookout.DataLayer.Profile
{
    /// <summary>
    /// Current User Setting class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class UserSetting : ProfileEntry
    {
        private XmlProfileHelper _xmlProfileHelper;
        public string Folder { get; set; }
        public string SenderCriteria { get; set; }
        public string SubjectCriteria { get; set; }
        public string BodyCriteria { get; set; }
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
                        new XElement("Folder", Folder),
                        new XElement("Sender", new XCData(SenderCriteria)),
                        new XElement("Subject", new XCData(SubjectCriteria)),
                        new XElement("Body", new XCData(BodyCriteria))
                   )
                );
                return userSetting;
            }
        }

        public UserSetting()
            : base()
        {
            Folder = string.Empty;
            SenderCriteria = string.Empty;
            SubjectCriteria = string.Empty;
            BodyCriteria = string.Empty;
        }

        public UserSetting(object parent, XElement userSetting)
            : base(parent, userSetting)
        {
            Folder = (string)userSetting.Element("Folder");
            SenderCriteria = string.Empty;
            SubjectCriteria = string.Empty;
            BodyCriteria = string.Empty;
            XCData senderCriteriaXCData = (XCData)userSetting.Element("Sender").FirstNode;
            XCData subjectCriteriaXCData = (XCData)userSetting.Element("Subject").FirstNode;
            XCData bodyCriteriaXCData = (XCData)userSetting.Element("Body").FirstNode;
            if (senderCriteriaXCData != null)
            {
                SenderCriteria = senderCriteriaXCData.Value;
            }
            if (subjectCriteriaXCData != null)
            {
                SubjectCriteria = subjectCriteriaXCData.Value;
            }
            if (bodyCriteriaXCData != null)
            {
                BodyCriteria = bodyCriteriaXCData.Value;
            }
        }
    }
}