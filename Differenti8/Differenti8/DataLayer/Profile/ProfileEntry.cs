using System;
using System.Linq;
using System.Xml.Linq;

namespace Differenti8.DataLayer.Profile
{
    /// <summary>
    /// Profile entry base class.
    /// </summary>
    /// <remarks>
    /// All profile entry classes are derived from this class.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ProfileEntry
    {
        #region Properties.
        public object Parent { get; set; }
        public string Key { get; set; }
        public virtual XElement Element
        {
            get
            {
                XElement entry = new XElement(
                    new XElement("ProfileEntry", new XAttribute("Key", Key))
                );
                return entry;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProfileEntry()
        {
            Parent = null;
            Key = string.Empty;
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileEntry(XElement entry)
        {
            Parent = null;
            Key = (string)entry.Attribute("Key");
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileEntry(object parent)
        {
            Parent = parent;
            Key = string.Empty;
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileEntry(object parent, XElement entry)
        {
            Parent = parent;
            Key = (string)entry.Attribute("Key");
        }
        #endregion
    }
}