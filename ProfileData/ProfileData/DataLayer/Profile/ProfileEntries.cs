using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ProfileData.DataLayer.Profile
{
    /// <summary>
    /// Profile entries base class.
    /// </summary>
    /// <remarks>
    /// This is a base class from which many persistable type safe collections of a specific profile entry type can be derived.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ProfileEntries<T> where T : ProfileEntry, new()
    {
        #region Member Variables.
        private XDocument _doc;
        protected Dictionary<string, T> _entries;
        #endregion

        #region Properties.
        public string ProfileName { get; set; }
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
                                        select item).First().Value;
                    }
                }
                catch
                {
                    var items = (from item in _entries
                                 select item);
                    if (items.Count() > 0)
                    {
                        selectedItem = items.First().Value;
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

        public Dictionary<string, T>.KeyCollection Keys
        {
            get
            {
                return _entries.Keys;
            }
        }

        public Dictionary<string, T>.ValueCollection Values
        {
            get
            {
                return _entries.Values;
            }
        }

        public T this[string key]
        {
            get
            {
                return _entries[key];
            }
        }

        public Dictionary<string, T> Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
            }
        }
        #endregion

        #region Constructors.
        public ProfileEntries(string profileName, string entriesName, string entryName, string keyName, string fileSpec)
            : this(profileName, entriesName, entryName, keyName, fileSpec, null)
        {
        }

        public ProfileEntries(string profileName, string entriesName, string entryName, string keyName, string fileSpec, string masterFileSpec)
        {
            ProfileName = profileName;
            EntriesName = entriesName;
            EntryName = entryName;
            KeyName = keyName;
            FileSpec = fileSpec;
            MasterFileSpec = masterFileSpec;
            Load(FileSpec, MasterFileSpec);
        }
        #endregion

        #region Public methods.
        public void Add(string key, T entry)
        {
            _entries.Add(key, entry);
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
                _entries = new Dictionary<string, T>();
                if (MasterFileSpec != null)
                {
                    FileHelper.EnsureExists(FileSpec, MasterFileSpec);
                }
                _doc = XDocument.Load(FileSpec);
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
                ProfileEntryFactory<T> factory = new ProfileEntryFactory<T>();
                foreach (XElement entry in _doc.Descendants(EntryName))
                {
                    string key = (string)entry.Attribute(KeyName);
                    _entries.Add(key, factory.Create(this, entry));
                }
            }
        }

        public virtual void Save()
        {
            XElement profile = new XElement(
                new XElement(ProfileName,
                    new XElement("Selected",
                        new XElement("Key", SelectedKey)
                    ),
                    new XElement(EntriesName)
                )
            );
            XElement entries = profile.Descendants(EntriesName).First();
            foreach (KeyValuePair<string, T> item in _entries.OrderBy(kvp => kvp.Value.Key))
            {
                T entry = item.Value;
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
                if (_entries.ContainsKey(entry.Key))
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
            _entries.Add(entry.Key, entry);
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
                                select item).First().Value;
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
            bool outcome = _entries.Remove(selectedItem.Key);
            Save();
            return outcome;
        }
        #endregion
    }
}