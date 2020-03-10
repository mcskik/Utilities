using System;

namespace Copy8
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
        private string sortOrder;
        private string action;
        private string command;
        private string displayCommand;
        private string outcome;
        private string sourceType;
        private string sourceEntry;
        private DateTime sourceDate;
        private long sourceSize;
        private string targetType;
        private string targetEntry;
        private DateTime targetDate;
        private long targetSize;

        /// <summary>
        /// Order to perform actions in.
        /// </summary>
        public string SortOrder
        {
            get
            {
                return sortOrder;
            }
            set
            {
                sortOrder = value;
            }
        }

        /// <summary>
        /// Action from compare.
        /// </summary>
        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        /// <summary>
        /// Command to perform.
        /// </summary>
        public string Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;
            }
        }

        /// <summary>
        /// Command to display.
        /// </summary>
        public string DisplayCommand
        {
            get
            {
                return displayCommand;
            }
            set
            {
                displayCommand = value;
            }
        }

        /// <summary>
        /// Outcome of compare.
        /// </summary>
        public string Outcome
        {
            get
            {
                return outcome;
            }
            set
            {
                outcome = value;
            }
        }

        /// <summary>
        /// Source type.
        /// </summary>
        public string SourceType
        {
            get
            {
                return sourceType;
            }
            set
            {
                sourceType = value;
            }
        }

        /// <summary>
        /// Source entry.
        /// </summary>
        public string SourceEntry
        {
            get
            {
                return sourceEntry;
            }
            set
            {
                sourceEntry = value;
            }
        }

        /// <summary>
        /// Source date.
        /// </summary>
        public DateTime SourceDate
        {
            get
            {
                return sourceDate;
            }
            set
            {
                sourceDate = value;
            }
        }

        /// <summary>
        /// Source size.
        /// </summary>
        public long SourceSize
        {
            get
            {
                return sourceSize;
            }
            set
            {
                sourceSize = value;
            }
        }

        /// <summary>
        /// Target type.
        /// </summary>
        public string TargetType
        {
            get
            {
                return targetType;
            }
            set
            {
                targetType = value;
            }
        }

        /// <summary>
        /// Target entry.
        /// </summary>
        public string TargetEntry
        {
            get
            {
                return targetEntry;
            }
            set
            {
                targetEntry = value;
            }
        }

        /// <summary>
        /// Target date.
        /// </summary>
        public DateTime TargetDate
        {
            get
            {
                return targetDate;
            }
            set
            {
                targetDate = value;
            }
        }

        /// <summary>
        /// Target size.
        /// </summary>
        public long TargetSize
        {
            get
            {
                return targetSize;
            }
            set
            {
                targetSize = value;
            }
        }
    }
}