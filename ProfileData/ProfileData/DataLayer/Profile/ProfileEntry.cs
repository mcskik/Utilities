using System;
using System.Linq;
using System.Xml.Linq;

namespace ProfileData.DataLayer.Profile
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

        #region Protected Methods.
        protected int GetQuantity(string quantityText)
        {
            int quantity = 0;
            if (quantityText != null)
            {
                int.TryParse(quantityText, out quantity);
            }
            return quantity;
        }

        protected decimal GetAmount(string amountText)
        {
            int quantity = 0;
            decimal amount = 0;
            if (amountText != null)
            {
                if (!decimal.TryParse(amountText, out amount))
                {
                    if (int.TryParse(amountText, out quantity))
                    {
                        amount = quantity;
                    }
                }
            }
            return amount;
        }
        #endregion
    }
}