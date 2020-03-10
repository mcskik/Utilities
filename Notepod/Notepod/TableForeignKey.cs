using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Table foreign key class containing table foreign key information.
    /// </summary>
    public class TableForeignKey
    {
        #region Member variables.
        private string _tableName;
        private string _columnName;
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
        /// Column name.
        /// </summary>
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
            }
        }
        #endregion
    }
}
