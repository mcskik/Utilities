using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Compare engine controller class.
	/// </summary>
	/// <remarks>
	/// Compare engine controller class.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class CompareEngine : ICompareEngine
	{
		#region Member variables.
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
		private List<Comparison> mcComparisons = null;
		private DateTime mdCompleteTaskStartTime;
		private Interrupt moInterrupt;
		private CompareLines moCompareLines;
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
		}

		/// <summary>
		/// List of comparisons in the form of data pairs for use externally.
		/// </summary>
		public List<Comparison> Comparisons
		{
			get
			{
				return mcComparisons;
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
		#region Segment.
		public class BeginSegmentEventArgs : EventArgs
		{
			public readonly long SegmentLength;
			public BeginSegmentEventArgs(long pnSegmentLength)
			{
				this.SegmentLength = pnSegmentLength;
			}
		}
		public class UpdateSegmentEventArgs : EventArgs
		{
			public readonly long Increment;
			public UpdateSegmentEventArgs(long pnIncrement)
			{
				this.Increment = pnIncrement;
			}
		}
		public class EndOfSegmentEventArgs : EventArgs
		{
			public EndOfSegmentEventArgs()
			{
			}
		}
		#endregion

		#region Compare.
		public class BeginCompareEventArgs : EventArgs
		{
			public readonly long CompareLength;
			public BeginCompareEventArgs(long pnCompareLength)
			{
				this.CompareLength = pnCompareLength;
			}
		}
		public class UpdateCompareEventArgs : EventArgs
		{
			public readonly long Increment;
			public UpdateCompareEventArgs(long pnIncrement)
			{
				this.Increment = pnIncrement;
			}
		}
		public class EndOfCompareEventArgs : EventArgs
		{
			public EndOfCompareEventArgs()
			{
			}
		}
		#endregion

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
		#region Segment.
		public delegate void BeginSegmentHandler(BeginSegmentEventArgs e);
		public delegate void UpdateSegmentHandler(UpdateSegmentEventArgs e);
		public delegate void EndOfSegmentHandler(EndOfSegmentEventArgs e);
		#endregion

		#region Compare.
		public delegate void BeginCompareHandler(BeginCompareEventArgs e);
		public delegate void UpdateCompareHandler(UpdateCompareEventArgs e);
		public delegate void EndOfCompareHandler(EndOfCompareEventArgs e);
		#endregion

		#region Report.
		public delegate void BeginReportHandler(BeginReportEventArgs e);
		public delegate void UpdateReportHandler(UpdateReportEventArgs e);
		public delegate void EndOfReportHandler(EndOfReportEventArgs e);
		#endregion
		#endregion

		#region Event Declarations.
		#region Segment.
		public event BeginSegmentHandler OnBeginSegment;
		public event UpdateSegmentHandler OnUpdateSegment;
		public event EndOfSegmentHandler OnEndOfSegment;
		#endregion

		#region Compare.
		public event BeginCompareHandler OnBeginCompare;
		public event UpdateCompareHandler OnUpdateCompare;
		public event EndOfCompareHandler OnEndOfCompare;
		#endregion

		#region Report.
		public event BeginReportHandler OnBeginReport;
		public event UpdateReportHandler OnUpdateReport;
		public event EndOfReportHandler OnEndOfReport;
		#endregion
		#endregion

		#region Event raising helper methods.
		#region Segment.
		/// <summary>
		/// Trigger begin segment event.
		/// </summary>
		/// <param name="pnSegmentLength">Segment length.</param>
		private void SignalBeginSegment(long pnSegmentLength)
		{
			if (OnBeginSegment != null)
			{
				OnBeginSegment(new BeginSegmentEventArgs(pnSegmentLength));
			}
		}

		/// <summary>
		/// Trigger update segment event.
		/// </summary>
		private void SignalUpdateSegment(long pnIncrement)
		{
			if (OnUpdateSegment != null)
			{
				OnUpdateSegment(new UpdateSegmentEventArgs(pnIncrement));
			}
		}

		/// <summary>
		/// Trigger end of segment event.
		/// </summary>
		private void SignalEndOfSegment()
		{
			if (OnEndOfSegment != null)
			{
				OnEndOfSegment(new EndOfSegmentEventArgs());
			}
		}
		#endregion

		#region Compare.
		/// <summary>
		/// Trigger begin compare event.
		/// </summary>
		/// <param name="pnCompareLength">Compare length.</param>
		private void SignalBeginCompare(long pnCompareLength)
		{
			if (OnBeginCompare != null)
			{
				OnBeginCompare(new BeginCompareEventArgs(pnCompareLength));
			}
		}

		/// <summary>
		/// Trigger update compare event.
		/// </summary>
		private void SignalUpdateCompare(long pnIncrement)
		{
			if (OnUpdateCompare != null)
			{
				OnUpdateCompare(new UpdateCompareEventArgs(pnIncrement));
			}
		}

		/// <summary>
		/// Trigger end of compare event.
		/// </summary>
		private void SignalEndOfCompare()
		{
			if (OnEndOfCompare != null)
			{
				OnEndOfCompare(new EndOfCompareEventArgs());
			}
		}
		#endregion

		#region Report.
		/// <summary>
		/// Trigger begin report event.
		/// </summary>
		/// <param name="pnReportLength">Report length.</param>
		private void SignalBeginReport(long pnReportLength)
		{
			if (OnBeginReport != null)
			{
				OnBeginReport(new BeginReportEventArgs(pnReportLength));
			}
		}

		/// <summary>
		/// Trigger update report event.
		/// </summary>
		private void SignalUpdateReport(long pnIncrement)
		{
			if (OnUpdateReport != null)
			{
				OnUpdateReport(new UpdateReportEventArgs(pnIncrement));
			}
		}

		/// <summary>
		/// Trigger end of report event.
		/// </summary>
		private void SignalEndOfReport()
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
		public CompareEngine()
		{
			mdCompleteTaskStartTime = DateTime.Now;
			moCompareLines = new CompareLines();
			moCompareLines.OnBeginSegment += new CompareBase.BeginSegmentHandler(oCompareLines_OnBeginSegment);
			moCompareLines.OnUpdateSegment += new CompareBase.UpdateSegmentHandler(oCompareLines_OnUpdateSegment);
			moCompareLines.OnEndOfSegment += new CompareBase.EndOfSegmentHandler(oCompareLines_OnEndOfSegment);
			moCompareLines.OnBeginCompare += new CompareLines.BeginCompareHandler(oCompareLines_OnBeginCompare);
			moCompareLines.OnUpdateCompare += new CompareLines.UpdateCompareHandler(oCompareLines_OnUpdateCompare);
			moCompareLines.OnEndOfCompare += new CompareLines.EndOfCompareHandler(oCompareLines_OnEndOfCompare);
			moCompareLines.OnBeginReport += new CompareLines.BeginReportHandler(oCompareLines_OnBeginReport);
			moCompareLines.OnUpdateReport += new CompareLines.UpdateReportHandler(oCompareLines_OnUpdateReport);
			moCompareLines.OnEndOfReport += new CompareLines.EndOfReportHandler(oCompareLines_OnEndOfReport);
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Compare new and old files.
		/// </summary>
		public void CompareFiles(string psNewFile, string psOldFile)
		{
			msNewFile = psNewFile;
			msOldFile = psOldFile;
			if (File.Exists(msNewFile))
			{
				if (File.Exists(msOldFile))
				{
					Block oAlpha = moCompareLines.Read(psNewFile);
					Block oBeta = moCompareLines.Read(psOldFile);
					DoCompare(oAlpha, oBeta);
				}
				else
				{
					throw new ParameterException("OldFile", "Old file not found.");
				}
			}
			else
			{
				throw new ParameterException("NewFile", "New file not found.");
			}
		}

		/// <summary>
		/// Compare new and old lines.
		/// </summary>
		public void CompareLines(List<string> pcNewLines, List<string> pcOldLines)
		{
			if (pcNewLines.Count > 0)
			{
				if (pcOldLines.Count > 0)
				{
					Block oAlpha = moCompareLines.ReadLines(pcNewLines);
					Block oBeta = moCompareLines.ReadLines(pcOldLines);
					DoCompare(oAlpha, oBeta);
				}
				else
				{
					throw new ParameterException("OldFile", "No old lines found.");
				}
			}
			else
			{
				throw new ParameterException("NewFile", "No new lines found.");
			}
		}
		#endregion

		#region Event Handlers.
		void oCompareLines_OnBeginSegment(CompareBase.BeginSegmentEventArgs e)
		{
			SignalBeginSegment(e.SegmentLength);
		}

		void oCompareLines_OnUpdateSegment(CompareBase.UpdateSegmentEventArgs e)
		{
			SignalUpdateSegment(e.Increment);
		}

		void oCompareLines_OnEndOfSegment(CompareBase.EndOfSegmentEventArgs e)
		{
			SignalEndOfSegment();
		}

		void oCompareLines_OnBeginCompare(CompareLines.BeginCompareEventArgs e)
		{
			SignalBeginCompare(e.CompareLength);
		}

		void oCompareLines_OnUpdateCompare(CompareLines.UpdateCompareEventArgs e)
		{
			SignalUpdateCompare(e.Increment);
		}

		void oCompareLines_OnEndOfCompare(CompareLines.EndOfCompareEventArgs e)
		{
			SignalEndOfCompare();
		}

		void oCompareLines_OnBeginReport(CompareLines.BeginReportEventArgs e)
		{
			SignalBeginReport(e.ReportLength);
		}

		void oCompareLines_OnUpdateReport(CompareLines.UpdateReportEventArgs e)
		{
			SignalUpdateReport(e.Increment);
		}

		void oCompareLines_OnEndOfReport(CompareLines.EndOfReportEventArgs e)
		{
			SignalEndOfReport();
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Pass parameters and do the compare.
		/// </summary>
		private void DoCompare(Block poAlpha, Block poBeta)
		{
			if (poAlpha == poBeta)
			{
				mbIdentical = true;
			}
			else
			{
				mbIdentical = false;
			}

			//Check if user has chosen to cancel run.
			if (Interrupt.Reason == "Cancel")
			{
				return;
			}

			mcSections = new Sections();
			moCompareLines.MinElements = this.MinLines;
			moCompareLines.MinChars = this.MinChars;
			moCompareLines.MinLines = this.MinLines;
            moCompareLines.LimitElements = this.LimitLines;
            moCompareLines.LimitCharacters = this.LimitCharacters;
            moCompareLines.LimitLines = this.LimitLines;
            moCompareLines.SubLineMatchLimit = this.SubLineMatchLimit;
            moCompareLines.CompleteLines = this.CompleteLines;
			moCompareLines.Sections = this.Sections;
			moCompareLines.Interrupt = this.Interrupt;
			moCompareLines.Compare(poAlpha, poBeta);
			//BuildComparisons();
		}

		/// <summary>
		/// Build comparisons list.
		/// </summary>
		private void BuildComparisons()
		{
			mcComparisons = new List<Comparison>();
			for (int nCount = 0; nCount < Sections.Count; nCount++)
			{
				Section oSection = (Section)Sections[nCount];
				if (oSection is SectionOfLines)
				{
					SectionOfLines oSectionOfLines = (SectionOfLines)oSection;
					BlockOfLines moBlock = (BlockOfLines)oSectionOfLines.Data;
					if (oSectionOfLines.Type.Substring(0, 1) == "M")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfLines.PositionA + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Line, Comparison.MatchEnum.Match, position, sData, position, sData);
							mcComparisons.Add(oComparison);
						}
					}
					else if (oSectionOfLines.Type.Substring(0, 1) == "A")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfLines.PositionA + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Line, Comparison.MatchEnum.InsertNew, position, sData, -1, string.Empty);
							mcComparisons.Add(oComparison);
						}
					}
					else if (oSectionOfLines.Type.Substring(0, 1) == "B")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfLines.PositionB + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Line, Comparison.MatchEnum.DeleteOld, -1, string.Empty, position, sData);
							mcComparisons.Add(oComparison);
						}
					}
				}
				if (oSection is SectionOfCharacters)
				{
					SectionOfCharacters oSectionOfCharacters = (SectionOfCharacters)oSection;
					BlockOfCharacters moBlock = (BlockOfCharacters)oSectionOfCharacters.Data;
					if (oSectionOfCharacters.Type.Substring(0, 1) == "M")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfCharacters.PositionA + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Character, Comparison.MatchEnum.Match, position, sData, position, sData);
							mcComparisons.Add(oComparison);
						}
					}
					else if (oSectionOfCharacters.Type.Substring(0, 1) == "A")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfCharacters.PositionA + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Character, Comparison.MatchEnum.InsertNew, position, sData, -1, string.Empty);
							mcComparisons.Add(oComparison);
						}
					}
					else if (oSectionOfCharacters.Type.Substring(0, 1) == "B")
					{
						for (long nIndex = 0; nIndex < moBlock.Elements.Length; nIndex++)
						{
							long position = oSectionOfCharacters.PositionB + nIndex;
							string sData = moBlock.Elements[nIndex].ToDisplay();
							Comparison oComparison = new Comparison(Comparison.GranularityEnum.Character, Comparison.MatchEnum.DeleteOld, -1, string.Empty, position, sData);
							mcComparisons.Add(oComparison);
						}
					}
				}
			}
		}
		#endregion
	}
}
