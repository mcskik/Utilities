using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ProfileData.DataLayer.Profile
{
    /// <summary>
    /// Sub profile list base class.
    /// </summary>
    /// <remarks>
    /// This is very similar to a ProfileList except the SubProfile represents the contents of the
    /// currently selected profile list item.  So there will be multiple sub profiles in every
    /// profile XDocument and the sub profile cannot be independently saved the whole profile XDocument
    /// must be saved.  This base class allows nested profile lists in a hierarchy.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class SubProfileList<T> : ProfileList<T> where T : ProfileListItem, new()
    {
        #region Properties.
        public XElement SubProfile { get; set; }
        #endregion

        #region Constructors.
        public SubProfileList(string profileName, string instanceName, string entriesName, string entryName, string keyName)
            : this(profileName, instanceName, entriesName, entryName, keyName, null)
        {
        }

        public SubProfileList(string profileName, string instanceName, string entriesName, string entryName, string keyName, XElement subProfile)
            : base(profileName, instanceName, entriesName, entryName, keyName, null, null)
        {
            SubProfile = subProfile;
        }
        #endregion

        #region Public methods.
        public override void Select(string key)
        {
            SelectedKey = key;
            XElement selected = SubProfile.Descendants("Selected").First();
            selected.SetElementValue("Key", SelectedKey);
        }

        public void Load()
        {
            if (SubProfile != null)
            {
                _entries = new List<T>();
                InstanceName = (string)SubProfile.Descendants(ProfileName).First().Attribute("Name");
                SelectedKey = string.Empty;
                if (SubProfile.Descendants("Selected").Count() > 0)
                {
                    SelectedKey = (string)SubProfile.Descendants("Selected").First().Element(KeyName);
                }
                else
                {
                    if (SubProfile.Descendants(EntryName).Count() > 0)
                    {
                        SelectedKey = (string)SubProfile.Descendants(EntryName).First().Attribute(KeyName);
                    }
                }
                ProfileListItemFactory<T> factory = new ProfileListItemFactory<T>();
                foreach (XElement entry in SubProfile.Descendants(EntryName))
                {
                    _entries.Add(factory.Create(this, entry));
                }
            }
        }

        public override void Save()
        {
            XElement subProfile = new XElement(
                new XElement(ProfileName, new XAttribute("Name", InstanceName),
                    new XElement("Selected",
                        new XElement("Key", SelectedKey)
                    ),
                    new XElement(EntriesName)
                )
            );
            XElement entries = subProfile.Descendants(EntriesName).First();
            foreach (T entry in _entries)
            {
                entries.Add(entry.Element);
            }
            SubProfile = subProfile;
        }
        #endregion
    }
}