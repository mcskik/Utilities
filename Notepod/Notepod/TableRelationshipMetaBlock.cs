using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// List of table relationship meta blocks.
    /// </summary>
    public class TableRelationshipMetaBlocks : List<TableRelationshipMetaBlock>
    {
    }

    /// <summary>
    /// Table relationship meta block class containing table relationship information.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class TableRelationshipMetaBlock
    {
        #region Member variables.
        private string _tableName;
        private string _primaryKey;
        private TableRelationships _relationships;
        #endregion

        #region Properties.
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
        /// Primary key.
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
            set
            {
                _primaryKey = value;
            }
        }

        /// <summary>
        /// Relationships.
        /// </summary>
        public TableRelationships Relationships
        {
            get
            {
                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }
        #endregion
 
        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableRelationshipMetaBlock()
        {
            _tableName = "None";
            _primaryKey = "None";
            _relationships = new TableRelationships();
        }
        #endregion
    }
}