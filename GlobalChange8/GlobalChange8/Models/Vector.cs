using System.Collections.Generic;

namespace GlobalChange8.Models
{
    /// <summary>
    /// List of vectors.
    /// </summary>
    public class Vectors : List<Vector>
    {
    }

    /// <summary>
    /// Vector class containing property information.
    /// </summary>
    public class Vector
    {
        #region Member variables.
        private string _modifier;
        private bool _readOnly;
        private string _type;
        private string _variable;
        private string _property;
        private string _summary;
        #endregion

        #region Properties.
        /// <summary>
        /// Access modifier.
        /// </summary>
        public string Modifier
        {
            get
            {
                return _modifier;
            }
            set
            {
                _modifier = value;
            }
        }

        /// <summary>
        /// Read only.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }

        /// <summary>
        /// Variable type.
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Variable name.
        /// </summary>
        public string Variable
        {
            get
            {
                return _variable;
            }
            set
            {
                _variable = value;
            }
        }

        /// <summary>
        /// Property name.
        /// </summary>
        public string Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        /// <summary>
        /// Text to use in summary comment.
        /// </summary>
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                _summary = value;
            }
        }
        #endregion
    }
}