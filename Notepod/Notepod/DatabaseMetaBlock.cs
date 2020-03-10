using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Database meta block class containing database information.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class DatabaseMetaBlock
    {
        #region Member variables.
        private TableRelationshipMetaBlocks _tables;
        #endregion

        #region Properties.
        /// <summary>
        /// Tables.
        /// </summary>
        public TableRelationshipMetaBlocks Tables
        {
            get
            {
                return _tables;
            }
            set
            {
                _tables = value;
            }
        }
        #endregion        
 
        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DatabaseMetaBlock()
        {
            _tables = new TableRelationshipMetaBlocks(); 
        }
        #endregion
    }
}