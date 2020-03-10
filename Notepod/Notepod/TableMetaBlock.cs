using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// List of table meta blocks.
    /// </summary>
    public class TableMetaBlocks : List<TableMetaBlock>
    {
    }

    /// <summary>
    /// Table meta block class containing table and column information.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class TableMetaBlock
    {
        #region Member variables.
        private bool _hasIdentityColumn;
        private string _tableName;
        private string _changeNumber;
        private TableColumns _columns;
        #endregion

        #region Properties.
        /// <summary>
        /// Has identity column.
        /// </summary>
        public bool HasIdentityColumn
        {
            get
            {
                return _hasIdentityColumn;
            }
            set
            {
                _hasIdentityColumn = value;
            }
        }

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

        /// <summary>
        /// Change number.
        /// </summary>
        public string ChangeNumber
        {
            get
            {
                return _changeNumber;
            }
            set
            {
                _changeNumber = value;
            }
        }

        /// <summary>
        /// Columns.
        /// </summary>
        public TableColumns Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }
        #endregion        
 
        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableMetaBlock()
        {
            _hasIdentityColumn = false;
            _tableName = string.Empty;
            _changeNumber = string.Empty;
            _columns = new TableColumns();
        }
        #endregion
    }
}