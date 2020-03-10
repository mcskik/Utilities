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
    /// Sub profile entries base class.
    /// </summary>
    /// <remarks>
    /// This is very similar to the ProfileEntries class except the SubProfile represents the contents of the
    /// currently selected profile entry.  So there will be multiple sub profiles in every
    /// profile XDocument and the sub profile cannot be independently saved the whole profile XDocument
    /// must be saved.  This base class allows nested sub profile entries in a hierarchy.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class SubProfileEntries<T> : ProfileEntries<T> where T : ProfileEntry, new()
    {
        #region Properties.
        public XElement SubProfile { get; set; }
        #endregion

        #region Constructors.
        public SubProfileEntries(string profileName, string entriesName, string entryName, string keyName)
            : this(profileName, entriesName, entryName, keyName, null)
        {
        }

        public SubProfileEntries(string profileName, string entriesName, string entryName, string keyName, XElement subProfile)
            : base(profileName, entriesName, entryName, keyName, null, null)
        {
            SubProfile = subProfile;
            Load();
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
                _entries = new Dictionary<string, T>();
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
                ProfileEntryFactory<T> factory = new ProfileEntryFactory<T>();
                foreach (XElement entry in SubProfile.Descendants(EntryName))
                {
                    string key = (string)entry.Attribute(KeyName);
                    _entries.Add(key, factory.Create(this, entry));
                }
            }
        }

        public override void Save()
        {
            XElement subProfile = new XElement(
                new XElement(ProfileName,
                    new XElement("Selected",
                        new XElement("Key", SelectedKey)
                    ),
                    new XElement(EntriesName)
                )
            );
            XElement entries = subProfile.Descendants(EntriesName).First();
            foreach (KeyValuePair<string, T> item in _entries)
            {
                T entry = item.Value;
                entries.Add(entry.Element);
            }
            SubProfile = subProfile;
        }
        #endregion
    }
}