using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Enumeration meta block class containing enumeration information.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class EnumMetaBlock
    {
        #region Member variables.
        private string _title;
        private string _name;
        private Vectors _enumItems;
        #endregion

        #region Properties.
        /// <summary>
        /// Title.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Enumeration items.
        /// </summary>
        public Vectors EnumItems
        {
            get
            {
                return _enumItems;
            }
            set
            {
                _enumItems = value;
            }
        }
        #endregion
        
        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EnumMetaBlock()
        {
            _enumItems = new Vectors();
        }
        #endregion
    }
}