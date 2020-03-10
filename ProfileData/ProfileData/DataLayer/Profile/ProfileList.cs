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
    /// Profile list base class.
    /// </summary>
    /// <remarks>
    /// This is a base class from which many persistable type safe collections of a specific profile entry type can be derived.
    /// This differs from the ProfileEntries class by using a List instead of a Dictionary which means that it binds better to a GridView.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ProfileList<T> where T : ProfileListItem, new()
    {
        #region Member Variables.
        private XDocument _doc;
        protected List<T> _entries;
        #endregion

        #region Properties.
        public string ProfileName { get; set; }
        public string InstanceName { get; set; }
        public string EntriesName { get; set; }
        public string EntryName { get; set; }
        public string KeyName { get; set; }
        public string FileSpec { get; set; }
        public string MasterFileSpec { get; set; }
        public string SelectedKey { get; set; }
        public T UpdateItem { get; set; }
        public T SelectedItem
        {
            get
            {
                T selectedItem = default(T);
                try
                {
                    if (SelectedKey != string.Empty)
                    {
                        selectedItem = (from item in _entries
                                        where (item.Key == SelectedKey)
                                        select item).First();
                    }
                }
                catch
                {
                    var items = (from item in _entries
                                 select item);
                    if (items.Count() > 0)
                    {
                        selectedItem = items.First();
                        SelectedKey = selectedItem.Key;
                    }
                    else
                    {
                        SelectedKey = string.Empty;
                    }
                }
                return selectedItem;
            }
        }

        public T this[int index]
        {
            get
            {
                return _entries[index];
            }
            set
            {
                _entries[index] = value;
            }
        }

        public List<T> Entries
        {
            get
            {
                return _entries;
            }
        }
        #endregion

        #region Constructors.
        public ProfileList(string profileName, string instanceName, string entriesName, string entryName, string keyName)
            : this(profileName, instanceName, entriesName, entryName, keyName, null, null)
        {
        }

        public ProfileList(string profileName, string instanceName, string entriesName, string entryName, string keyName, string fileSpec)
            : this(profileName, instanceName, entriesName, entryName, keyName, fileSpec, null)
        {
        }

        public ProfileList(string profileName, string instanceName, string entriesName, string entryName, string keyName, string fileSpec, string masterFileSpec)
        {
            ProfileName = profileName;
            InstanceName = instanceName;
            EntriesName = entriesName;
            EntryName = entryName;
            KeyName = keyName;
            FileSpec = fileSpec;
            MasterFileSpec = masterFileSpec;
            _entries = new List<T>();
            Load(FileSpec, MasterFileSpec);
        }
        #endregion

        #region Public methods.
        public void Add(T entry)
        {
            _entries.Add(entry);
        }

        public virtual void Select(string key)
        {
            SelectedKey = key;
            XElement selected = _doc.Descendants("Selected").First();
            selected.SetElementValue("Key", SelectedKey);
            _doc.Save(FileSpec);
        }

        public void Load(string fileSpec)
        {
            Load(fileSpec, null);
        }

        public void Load(string fileSpec, string masterFileSpec)
        {
            FileSpec = fileSpec;
            MasterFileSpec = masterFileSpec;
            if (FileSpec != null)
            {
                _entries = new List<T>();
                if (MasterFileSpec != null)
                {
                    FileHelper.EnsureExists(FileSpec, MasterFileSpec);
                }
                _doc = XDocument.Load(FileSpec);
                InstanceName = (string)_doc.Descendants(ProfileName).First().Attribute("Name");
                SelectedKey = string.Empty;
                if (_doc.Descendants("Selected").Count() > 0)
                {
                    SelectedKey = (string)_doc.Descendants("Selected").First().Element(KeyName);
                }
                else
                {
                    if (_doc.Descendants(EntryName).Count() > 0)
                    {
                        SelectedKey = (string)_doc.Descendants(EntryName).First().Attribute(KeyName);
                    }
                }
                ProfileListItemFactory<T> factory = new ProfileListItemFactory<T>();
                foreach (XElement entry in _doc.Descendants(EntryName))
                {
                    _entries.Add(factory.Create(this, entry));
                }
            }
        }

        public virtual void Save()
        {
            XElement profile = new XElement(
                new XElement(ProfileName, new XAttribute("Name", InstanceName),
                    new XElement("Selected",
                        new XElement("Key", SelectedKey)
                    ),
                    new XElement(EntriesName)
                )
            );
            XElement entries = profile.Descendants(EntriesName).First();
            foreach (T entry in _entries)
            {
                entries.Add(entry.Element);
            }
            _doc = new XDocument();
            _doc.Add(profile);
            FileHelper.PathCheck(FileSpec);
            FileHelper.AllowOverwirte(FileSpec);
            _doc.Save(FileSpec);
        }

        public void Persist(T entry)
        {
            if (SelectedItem != null)
            {
                EntryComparer comparer = new EntryComparer();
                if (_entries.Contains(entry, comparer))
                {
                    Update(entry);
                }
                else
                {
                    Insert(entry);
                }
            }
            else
            {
                Insert(entry);
            }
        }

        public void Insert(T entry)
        {
            _entries.Add(entry);
            Select(entry.Key);
            Save();
        }

        public void Update(T entry)
        {
            UpdateItem = SelectedItem;
            UpdateAssignments(entry);
            Save();
        }

        public virtual void UpdateAssignments(T entry)
        {
        }

        public bool Delete(string key)
        {
            T selectedItem = null;
            if (key.Trim() != string.Empty)
            {
                selectedItem = (from item in _entries
                                where (item.Key == SelectedKey)
                                select item).First();
            }
            XElement selectedNode = (from item in _doc.Descendants(EntryName)
                                     where (item.Attribute(KeyName).Value == SelectedKey)
                                     select item).First();
            XElement prevNode = selectedNode.PreviousNode as XElement;
            XElement nextNode = selectedNode.NextNode as XElement;
            if (prevNode != null)
            {
                Select(prevNode.Attribute(KeyName).Value);
            }
            else if (nextNode != null)
            {
                Select(nextNode.Attribute(KeyName).Value);
            }
            else
            {
                Select(string.Empty);
            }
            bool outcome = _entries.Remove(selectedItem);
            Save();
            return outcome;
        }
        #endregion

        #region Entry comparer.
        public class EntryComparer : IEqualityComparer<T>
        {
            /// <remarks>
            /// Entries are equal if they are not null and their keys are equal.
            /// </remarks>
            public bool Equals(T alpha, T beta)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(alpha, beta))
                {
                    return true;
                }
                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(alpha, null) || Object.ReferenceEquals(beta, null))
                {
                    return false;
                }
                //Check whether the keys for both entries are equal.
                return alpha.Key == beta.Key;
            }

            /// <remarks>
            /// If Equals() returns true for a pair of objects 
            /// then GetHashCode() must return the same value for these objects.
            /// </remarks>
            public int GetHashCode(T entry)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(entry, null))
                {
                    return 0;
                }
                //Get hash code for the Key field if it is not null.
                int hashEntryKey = entry.Key == null ? 0 : entry.Key.GetHashCode();
                return hashEntryKey;
            }
        }
        #endregion
    }
}