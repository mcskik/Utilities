using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Compare lines derived class.
	/// </summary>
	/// <remarks>
	/// Compare lines derived class.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class CompareLines : CompareBase
	{
		#region Custom Event Arguments.
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
		public CompareLines() : base()
		{
			mcSegA.OnAddSegment += new Segments.AddSegmentHandler(mcSegA_OnAddSegment);
			mcSegB.OnAddSegment += new Segments.AddSegmentHandler(mcSegB_OnAddSegment);
		}
		#endregion

		#region Event Handlers.
		void mcSegB_OnAddSegment(Segments.AddSegmentEventArgs e)
		{
			SignalUpdateCompare(e.SegmentLength);
		}

		void mcSegA_OnAddSegment(Segments.AddSegmentEventArgs e)
		{
			SignalUpdateCompare(e.SegmentLength);
		}

		void Sections_OnAddSection(Sections.AddSectionEventArgs e)
		{
			SignalUpdateReport(e.SectionLength);
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Compare two blocks of lines against each other.
		/// </summary>
		/// <param name="poAlpha">First block of lines.</param>
		/// <param name="poBeta">Second block of lines.</param>
        public override void Compare(Block poAlpha1, Block poBeta1)
        {
            Sections.OnAddSection += new Sections.AddSectionHandler(Sections_OnAddSection);

            BlockOfLines poAlpha = (BlockOfLines)poAlpha1;
            BlockOfLines poBeta = (BlockOfLines)poBeta1;
            BlockOfCharacters oAlphaChars = null;
            BlockOfCharacters oBetaChars = null;

            if (poAlpha.ContentsLength >= poBeta.ContentsLength)
            {
                mcSegA.ProgressOn = Segments.ProgressOnEnum.Alpha;
                mcSegB.ProgressOn = Segments.ProgressOnEnum.Alpha;
                SignalBeginCompare(poAlpha.ContentsLength);
            }
            else
            {
                mcSegA.ProgressOn = Segments.ProgressOnEnum.Beta;
                mcSegB.ProgressOn = Segments.ProgressOnEnum.Beta;
                SignalBeginCompare(poBeta.ContentsLength);
            }

            this.MinElements = this.MinLines;
            Analyse(1, poAlpha, 1, poBeta);

            //Add missing alpha segments.
            SegmentComparer oSegAComparer = new SegmentComparer();
            oSegAComparer.ComparisonMethod = SegmentComparer.ComparisonType.AlphaPosition;
            AddMissingSegments("A1", oSegAComparer, mcSegA, poAlpha);

            //Add missing beta segments.
            SegmentComparer oSegBComparer = new SegmentComparer();
            oSegBComparer.ComparisonMethod = SegmentComparer.ComparisonType.BetaPosition;
            AddMissingSegments("B1", oSegBComparer, mcSegB, poBeta);

            SignalEndOfCompare();

            //Check if user has chosen to cancel run.
            if (Interrupt.Reason == "Cancel")
            {
                return;
            }

            //Report.
            if (poAlpha.ContentsLength >= poBeta.ContentsLength)
            {
                Sections.ProgressOn = Sections.ProgressOnEnum.Alpha;
                SignalBeginReport(poAlpha.ContentsLength);
            }
            else
            {
                Sections.ProgressOn = Sections.ProgressOnEnum.Beta;
                SignalBeginReport(poBeta.ContentsLength);
            }

            //Merge into report.
            long nCountA = 0;
            long nCountB = 0;
            SegmentOfLines oSegA = new SegmentOfLines();
            SegmentOfLines oSegB = new SegmentOfLines();
            oSegA.Type = "X1";
            oSegA.Data = new BlockOfLines();
            oSegB.Type = "Z1";
            oSegB.Data = new BlockOfLines();
            SectionOfLines oSection = new SectionOfLines();
            while (nCountA < mcSegA.Count || nCountB < mcSegB.Count)
            {
                if (nCountA < mcSegA.Count)
                {
                    oSegA = (SegmentOfLines)mcSegA[(int)nCountA];
                }
                if (nCountB < mcSegB.Count)
                {
                    oSegB = (SegmentOfLines)mcSegB[(int)nCountB];
                }
                if (oSegA.Type.Substring(0, 1) == "M" && oSegB.Type.Substring(0, 1) == "M")
                {
                    if (!oSegA.Data.IsEmpty)
                    {
                        oSegA.Display();
                        oSection = new SectionOfLines();
                        oSection.Type = oSegA.Type;
                        oSection.Position = oSegA.PositionA;
                        oSection.PositionA = oSegA.PositionA;
                        oSection.PositionB = oSegB.PositionB;
                        oSection.Size = oSegA.Size;
                        oSection.Data = oSegA.Data;
                        Sections.Add(oSection);
                    }
                    nCountA++;
                    nCountB++;
                }
                else if (oSegA.Type.Substring(0, 1) == "M" && oSegB.Type.Substring(0, 1) != "M")
                {
                    if (!oSegB.Data.IsEmpty)
                    {
                        oSegB.Display();
                        oSection = new SectionOfLines();
                        oSection.Type = oSegB.Type;
                        oSection.Position = oSegB.PositionB;
                        oSection.PositionA = oSegA.PositionA;
                        oSection.PositionB = oSegB.PositionB;
                        oSection.Size = oSegB.Size;
                        oSection.Data = oSegB.Data;
                        Sections.Add(oSection);
                    }
                    nCountB++;
                }
                else if (oSegA.Type.Substring(0, 1) != "M" && oSegB.Type.Substring(0, 1) == "M")
                {
                    if (!oSegA.Data.IsEmpty)
                    {
                        oSegA.Display();
                        oSection = new SectionOfLines();
                        oSection.Type = oSegA.Type;
                        oSection.Position = oSegA.PositionA;
                        oSection.PositionA = oSegA.PositionA;
                        oSection.PositionB = oSegB.PositionB;
                        oSection.Size = oSegA.Size;
                        oSection.Data = oSegA.Data;
                        Sections.Add(oSection);
                    }
                    nCountA++;
                }
                else if (oSegA.Type.Substring(0, 1) != "M" && oSegB.Type.Substring(0, 1) != "M")
                {
                    if (this.CompleteLines)
                    {
                        if (oSegA.PositionA < oSegB.PositionB)
                        {
                            if (!oSegA.Data.IsEmpty)
                            {
                                oSegA.Display();
                                oSection = new SectionOfLines();
                                oSection.Type = oSegA.Type;
                                oSection.Position = oSegA.PositionA;
                                oSection.PositionA = oSegA.PositionA;
                                oSection.PositionB = oSegB.PositionB;
                                oSection.Size = oSegA.Size;
                                oSection.Data = oSegA.Data;
                                Sections.Add(oSection);
                            }
                            if (!oSegB.Data.IsEmpty)
                            {
                                oSegB.Display();
                                oSection = new SectionOfLines();
                                oSection.Type = oSegB.Type;
                                oSection.Position = oSegB.PositionB;
                                oSection.PositionA = oSegA.PositionA;
                                oSection.PositionB = oSegB.PositionB;
                                oSection.Size = oSegB.Size;
                                oSection.Data = oSegB.Data;
                                Sections.Add(oSection);
                            }
                        }
                        else
                        {
                            if (!oSegB.Data.IsEmpty)
                            {
                                oSegB.Display();
                                oSection = new SectionOfLines();
                                oSection.Type = oSegB.Type;
                                oSection.Position = oSegB.PositionB;
                                oSection.PositionA = oSegA.PositionA;
                                oSection.PositionB = oSegB.PositionB;
                                oSection.Size = oSegB.Size;
                                oSection.Data = oSegB.Data;
                                Sections.Add(oSection);
                            }
                            if (!oSegA.Data.IsEmpty)
                            {
                                oSegA.Display();
                                oSection = new SectionOfLines();
                                oSection.Type = oSegA.Type;
                                oSection.Position = oSegA.PositionA;
                                oSection.PositionA = oSegA.PositionA;
                                oSection.PositionB = oSegB.PositionB;
                                oSection.Size = oSegA.Size;
                                oSection.Data = oSegA.Data;
                                Sections.Add(oSection);
                            }
                        }
                        nCountA++;
                        nCountB++;
                    }
                    else
                    {
                        if (!oSegA.Data.IsEmpty && !oSegB.Data.IsEmpty)
                        {
                            if (oSegA.Data.Length < this.SubLineMatchLimit && oSegB.Data.Length < this.SubLineMatchLimit)
                            {
                                //Look for sub-line matches.
                                oAlphaChars = oSegA.Data.GetCharacters();
                                oBetaChars = oSegB.Data.GetCharacters();
                                CompareCharacters oCompareCharacters = new CompareCharacters();
                                oCompareCharacters.MinElements = this.MinChars;
                                oCompareCharacters.MinChars = this.MinChars;
                                oCompareCharacters.MinLines = this.MinLines;
                                oCompareCharacters.LimitElements = this.LimitCharacters;
                                oCompareCharacters.LimitCharacters = this.LimitCharacters;
                                oCompareCharacters.LimitLines = this.LimitLines;
                                oCompareCharacters.SubLineMatchLimit = this.SubLineMatchLimit;
                                oCompareCharacters.CompleteLines = this.CompleteLines;
                                oCompareCharacters.Sections = this.Sections;
                                oCompareCharacters.Interrupt = this.Interrupt;
                                oCompareCharacters.Compare(oAlphaChars, oBetaChars);
                            }
                            else
                            {
                                //Don't look for sub-line matches if the mismatched blocks are too large,
                                //since this has a large performance impact for little likely benefit.
                                if (oSegA.PositionA < oSegB.PositionB)
                                {
                                    if (!oSegA.Data.IsEmpty)
                                    {
                                        oSegA.Display();
                                        oSection = new SectionOfLines();
                                        oSection.Type = oSegA.Type;
                                        oSection.Position = oSegA.PositionA;
                                        oSection.PositionA = oSegA.PositionA;
                                        oSection.PositionB = oSegB.PositionB;
                                        oSection.Size = oSegA.Size;
                                        oSection.Data = oSegA.Data;
                                        Sections.Add(oSection);
                                    }
                                    if (!oSegB.Data.IsEmpty)
                                    {
                                        oSegB.Display();
                                        oSection = new SectionOfLines();
                                        oSection.Type = oSegB.Type;
                                        oSection.Position = oSegB.PositionB;
                                        oSection.PositionA = oSegA.PositionA;
                                        oSection.PositionB = oSegB.PositionB;
                                        oSection.Size = oSegB.Size;
                                        oSection.Data = oSegB.Data;
                                        Sections.Add(oSection);
                                    }
                                }
                                else
                                {
                                    if (!oSegB.Data.IsEmpty)
                                    {
                                        oSegB.Display();
                                        oSection = new SectionOfLines();
                                        oSection.Type = oSegB.Type;
                                        oSection.Position = oSegB.PositionB;
                                        oSection.PositionA = oSegA.PositionA;
                                        oSection.PositionB = oSegB.PositionB;
                                        oSection.Size = oSegB.Size;
                                        oSection.Data = oSegB.Data;
                                        Sections.Add(oSection);
                                    }
                                    if (!oSegA.Data.IsEmpty)
                                    {
                                        oSegA.Display();
                                        oSection = new SectionOfLines();
                                        oSection.Type = oSegA.Type;
                                        oSection.Position = oSegA.PositionA;
                                        oSection.PositionA = oSegA.PositionA;
                                        oSection.PositionB = oSegB.PositionB;
                                        oSection.Size = oSegA.Size;
                                        oSection.Data = oSegA.Data;
                                        Sections.Add(oSection);
                                    }
                                }
                                nCountA++;
                                nCountB++;
                            }
                        }
                        nCountA++;
                        nCountB++;
                    }
                }
            }
            SignalEndOfReport();
        }

		/// <summary>
		/// Read the specified file and return its contents as a block of element lines.
		/// </summary>
		/// <param name="psFile">Text file specification.</param>
		/// <returns>Entire file contents as a block of element lines.</returns>
		public override Block Read(string psFile)
		{
			StreamReader oSr;
			string sLine = string.Empty;
			StringCollection cLines = new StringCollection();
			ElementLine[] aElementLines = null;
			try
			{
				if (File.Exists(psFile))
				{
					oSr = new StreamReader(psFile);
					while (!oSr.EndOfStream)
					{
						sLine = oSr.ReadLine();
						cLines.Add(sLine);
					}
					oSr.Close();
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			aElementLines = new ElementLine[cLines.Count];
			for (int nCount = 0; nCount < cLines.Count; nCount++) 
			{
				aElementLines[nCount] = new ElementLine(cLines[nCount].ToString());
			}
			BlockOfLines oBlock = new BlockOfLines(aElementLines);
			return oBlock;
		}

		/// <summary>
		/// Read the specified string list and return its contents as a block of element lines.
		/// </summary>
		/// <param name="pcList">List of strings.</param>
		/// <returns>String list as a block of element lines.</returns>
		public Block ReadLines(List<string> pcLines)
		{
			ElementLine[] aElementLines = new ElementLine[pcLines.Count];
			for (int nCount = 0; nCount < pcLines.Count; nCount++)
			{
				aElementLines[nCount] = new ElementLine(pcLines[nCount].ToString());
			}
			BlockOfLines oBlock = new BlockOfLines(aElementLines);
			return oBlock;
		}

		/// <summary>
		/// Allocate a block of lines.
		/// </summary>
		/// <returns>A block of lines.</returns>
		public override Block AllocateBlock()
		{
			return new BlockOfLines();
		}

		/// <summary>
		/// Allocate a segment of lines.
		/// </summary>
		/// <returns>A segment of lines.</returns>
		public override Segment AllocateSegment()
		{
			return new SegmentOfLines();
		}
		#endregion
	}
}
