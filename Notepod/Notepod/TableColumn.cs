using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// List of table columns.
    /// </summary>
    public class TableColumns : List<TableColumn>
    {
    }

    /// <summary>
    /// Table column class containing table column information.
    /// </summary>
    public class TableColumn
    {
        #region Member variables.
        private string _name;
        private string _type;
        private int _length;
        private int _precision;
        private bool _nullable;
        #endregion

        #region Properties.
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
        /// Type.
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
        /// Length.
        /// </summary>
        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        /// <summary>
        /// Precision.
        /// </summary>
        public int Precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
            }
        }

        /// <summary>
        /// Nullable.
        /// </summary>
        public bool Nullable
        {
            get
            {
                return _nullable;
            }
            set
            {
                _nullable = value;
            }
        }
        #endregion
    }
}
