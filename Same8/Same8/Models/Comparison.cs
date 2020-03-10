using System;

namespace Same8.Models
{
    /// <summary>
    /// Comparison entry.
    /// </summary>
    /// <remarks>
    /// Comparison entry.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Comparison
    {
        #region Member variables.
        private string msAction;
        private string msOutcome;
        private string msNewType;
        private string msNewEntry;
        private DateTime mdNewDate;
        private string msOldType;
        private string msOldEntry;
        private DateTime mdOldDate;
        #endregion

        #region Properties.
        /// <summary>
        /// Action to perform.
        /// </summary>
        public string Action
        {
            get
            {
                return msAction;
            }
            set
            {
                msAction = value;
            }
        }

        /// <summary>
        /// Outcome of compare.
        /// </summary>
        public string Outcome
        {
            get
            {
                return msOutcome;
            }
            set
            {
                msOutcome = value;
            }
        }

        /// <summary>
        /// New type.
        /// </summary>
        public string NewType
        {
            get
            {
                return msNewType;
            }
            set
            {
                msNewType = value;
            }
        }

        /// <summary>
        /// New entry.
        /// </summary>
        public string NewEntry
        {
            get
            {
                return msNewEntry;
            }
            set
            {
                msNewEntry = value;
            }
        }

        /// <summary>
        /// New date.
        /// </summary>
        public DateTime NewDate
        {
            get
            {
                return mdNewDate;
            }
            set
            {
                mdNewDate = value;
            }
        }

        /// <summary>
        /// Old type.
        /// </summary>
        public string OldType
        {
            get
            {
                return msOldType;
            }
            set
            {
                msOldType = value;
            }
        }

        /// <summary>
        /// Old entry.
        /// </summary>
        public string OldEntry
        {
            get
            {
                return msOldEntry;
            }
            set
            {
                msOldEntry = value;
            }
        }

        /// <summary>
        /// Old date.
        /// </summary>
        public DateTime OldDate
        {
            get
            {
                return mdOldDate;
            }
            set
            {
                mdOldDate = value;
            }
        }
        #endregion
    }
}