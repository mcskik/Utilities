using System;
using System.Linq;
using System.Xml.Linq;

namespace ProfileData.DataLayer.Profile
{
    /// <summary>
    /// Profile list item base class.
    /// </summary>
    /// <remarks>
    /// All profile list item classes are derived from this class.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ProfileListItem
    {
        #region Properties.
        public object Parent { get; set; }
        public string Key { get; set; }
        public virtual XElement Element
        {
            get
            {
                XElement entry = new XElement(
                    new XElement("ProfileListItem", new XAttribute("Key", Key))
                );
                return entry;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProfileListItem()
        {
            Parent = null;
            Key = string.Empty;
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileListItem(XElement entry)
        {
            Parent = null;
            Key = (string)entry.Attribute("Key");
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileListItem(object parent)
        {
            Parent = parent;
            Key = string.Empty;
        }

        /// <summary>
        /// Construct from a specific type of profile entry XML element.
        /// </summary>
        public ProfileListItem(object parent, XElement entry)
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