using Differenti8.DataLayer.Profile;
using Differenti8Engine;
using Log8;
using System;

namespace Differenti8.Models
{
    /// <summary>
    /// Report engine base class.
    /// </summary>
    /// <remarks>
    /// Report engine base class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ReportEngine
    {
        #region Member variables.
        protected IProfileManager _profileManager;
        private string msNewFile = string.Empty;
        private string msOldFile = string.Empty;
        private long mnMinChars = 0;
        private long mnMinLines = 0;
        private long mnLimitCharacters = 0;
        private long mnLimitLines = 0;
        private long mnSubLineMatchLimit = 0;
        private bool mbCompleteLines = false;
        private bool mbIdentical = false;
        private Sections mcSections = null;
        private DateTime mdCompleteTaskStartTime;
        private Interrupt moInterrupt;
        protected Logger moLog;
        protected string msLogPath;
        protected string msVersion;
        protected string msViewerProgram;
        #endregion

        #region Properties.
        /// <summary>
        /// New file specification.
        /// </summary>
        public string NewFile
        {
            get
            {
                return msNewFile;
            }
            set
            {
                msNewFile = value;
            }
        }

        /// <summary>
        /// Old file specification.
        /// </summary>
        public string OldFile
        {
            get
            {
                return msOldFile;
            }
            set
            {
                msOldFile = value;
            }
        }

        /// <summary>
        /// Minimum number of characters needed to be considered a match.
        /// </summary>
        public long MinChars
        {
            get
            {
                return mnMinChars;
            }
            set
            {
                mnMinChars = value;
            }
        }

        /// <summary>
        /// Minimum number of lines needed to be considered a match.
        /// </summary>
        public long MinLines
        {
            get
            {
                return mnMinLines;
            }
            set
            {
                mnMinLines = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long LimitCharacters
        {
            get
            {
                return mnLimitCharacters;
            }
            set
            {
                mnLimitCharacters = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long LimitLines
        {
            get
            {
                return mnLimitLines;
            }
            set
            {
                mnLimitLines = value;
            }
        }

        /// <summary>
        /// To detect if the largest match has probably been found.
        /// The reasoning here is that if you have got more than
        ///	the limit of elements to match you have probably found
        ///	the largest match so you don't need to keep trying.
        /// </summary>
        public long SubLineMatchLimit
        {
            get
            {
                return mnSubLineMatchLimit;
            }
            set
            {
                mnSubLineMatchLimit = value;
            }
        }

        /// <summary>
        /// Synchronise matches with complete lines.
        /// </summary>
        public bool CompleteLines
        {
            get
            {
                return mbCompleteLines;
            }
            set
            {
                mbCompleteLines = value;
            }
        }

        /// <summary>
        /// Complete Task Start Time.
        /// </summary>
        public DateTime CompleteTaskStartTime
        {
            get
            {
                return mdCompleteTaskStartTime;
            }
            set
            {
                mdCompleteTaskStartTime = value;
            }
        }

        /// <summary>
        /// True if both files are identical.
        /// </summary>
        public bool Identical
        {
            get
            {
                return mbIdentical;
            }
            set
            {
                mbIdentical = value;
            }
        }

        /// <summary>
        /// List of report sections for use by report engine.
        /// </summary>
        public Sections Sections
        {
            get
            {
                return mcSections;
            }
            set
            {
                mcSections = value;
            }
        }

        /// <summary>
        /// User interrupt object.
        /// </summary>
        public Interrupt Interrupt
        {
            get
            {
                return moInterrupt;
            }
            set
            {
                moInterrupt = value;
            }
        }
        #endregion

        #region Custom Event Arguments.
        #region Report.
        public class BeginReportEventArgs : EventArgs
        {
            public readonly long ReportLength;
            public BeginReportEventArgs(long pnReportLength)
            {
                this.ReportLength = pnReportLength;
            }
        }
        public class UpdateReportEventArgs : EventArgs
        {
            public readonly long Increment;
            public UpdateReportEventArgs(long pnIncrement)
            {
                this.Increment = pnIncrement;
            }
        }
        public class EndOfReportEventArgs : EventArgs
        {
            public EndOfReportEventArgs()
            {
            }
        }
        #endregion
        #endregion

        #region Delegates.
        #region Report.
        public delegate void BeginReportHandler(BeginReportEventArgs e);
        public delegate void UpdateReportHandler(UpdateReportEventArgs e);
        public delegate void EndOfReportHandler(EndOfReportEventArgs e);
        #endregion
        #endregion

        #region Event Declarations.
        #region Report.
        public event BeginReportHandler OnBeginReport;
        public event UpdateReportHandler OnUpdateReport;
        public event EndOfReportHandler OnEndOfReport;
        #endregion
        #endregion

        #region Event raising helper methods.
        #region Report.
        /// <summary>
        /// Trigger begin report event.
        /// </summary>
        /// <param name="pnReportLength">Report length.</param>
        protected void SignalBeginReport(long pnReportLength)
        {
            if (OnBeginReport != null)
            {
                OnBeginReport(new BeginReportEventArgs(pnReportLength));
            }
        }

        /// <summary>
        /// Trigger update report event.
        /// </summary>
        protected void SignalUpdateReport(long pnIncrement)
        {
            if (OnUpdateReport != null)
            {
                OnUpdateReport(new UpdateReportEventArgs(pnIncrement));
            }
        }

        /// <summary>
        /// Trigger end of report event.
        /// </summary>
        protected void SignalEndOfReport()
        {
            if (OnEndOfReport != null)
            {
                OnEndOfReport(new EndOfReportEventArgs());
            }
        }
        #endregion
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportEngine()
        {
        }

        /// <summary>
        /// Main constructor.
        /// </summary>
        public ReportEngine(ICompareEngine compareEngine, IProfileManager profileManager)
        {
            _profileManager = profileManager;
            msNewFile = compareEngine.NewFile;
            msOldFile = compareEngine.OldFile;
            mnMinChars = compareEngine.MinChars;
            mnMinLines = compareEngine.MinLines;
            mnLimitCharacters = compareEngine.LimitCharacters;
            mnLimitLines = compareEngine.LimitLines;
            mnSubLineMatchLimit = compareEngine.SubLineMatchLimit;
            mbCompleteLines = compareEngine.CompleteLines;
            mbIdentical = compareEngine.Identical;
            mcSections = compareEngine.Sections;
            mdCompleteTaskStartTime = compareEngine.CompleteTaskStartTime;
            moInterrupt = compareEngine.Interrupt;
            msLogPath = profileManager.SystemProfile.LogPath;
            msVersion = profileManager.ApplicationProfile.Version;
            //TODO: I had to use Wordpad.exe instead of Notepad.exe because Notepad.exe doesn't recognise unix \n end-of-line markers while Wordpad.exe does.
            msViewerProgram = profileManager.SystemProfile.ViewerUnix;
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Report differences between new and old files.
        /// </summary>
        public virtual void Report()
        {
        }
        #endregion
    }
}