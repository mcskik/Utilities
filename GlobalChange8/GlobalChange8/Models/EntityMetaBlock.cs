namespace GlobalChange8.Models
{
    /// <summary>
    /// Entity meta block class containing entity class information.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class EntityMetaBlock
    {
        #region Member variables.
        private string _title;
        private string _name;
        private string _shortName;
        private Vectors _memberVariables;
        private Vectors _properties;
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
        /// Short Name.
        /// </summary>
        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
            }
        }

        /// <summary>
        /// Member variables.
        /// </summary>
        public Vectors MemberVariables
        {
            get
            {
                return _memberVariables;
            }
            set
            {
                _memberVariables = value;
            }
        }

        /// <summary>
        /// Properties.
        /// </summary>
        public Vectors Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntityMetaBlock()
        {
            _memberVariables = new Vectors();
            _properties = new Vectors();
        }
        #endregion
    }
}