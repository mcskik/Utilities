using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// List of table relationships.
    /// </summary>
    public class TableRelationships : List<TableRelationship>
    {
    }

    /// <summary>
    /// Table relationship class containing table relationship information.
    /// </summary>
    public class TableRelationship
    {
        #region Member variables.
        private TableForeignKey _dependant;
        private TableForeignKey _dependsOn;
        #endregion

        #region Properties.
        /// <summary>
        /// Dependant.
        /// </summary>
        public TableForeignKey Dependant
        {
            get
            {
                return _dependant;
            }
            set
            {
                _dependant = value;
            }
        }

        /// <summary>
        /// Depends on.
        /// </summary>
        public TableForeignKey DependsOn
        {
            get
            {
                return _dependsOn;
            }
            set
            {
                _dependsOn = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableRelationship()
        {
            _dependant = new TableForeignKey();
            _dependsOn = new TableForeignKey();
        }
        #endregion
    }
}
