using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Compare characters derived class.
	/// </summary>
	/// <remarks>
	/// Compare characters derived class.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class CompareCharacters : CompareBase
	{
		#region Custom Event Arguments.
		public class UpdateCompareEventArgs : EventArgs
		{
			public readonly long Increment;
			public UpdateCompareEventArgs(long pnIncrement)
			{
				this.Increment = pnIncrement;
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
		#endregion

		#region Delegates.
		public delegate void UpdateCompareHandler(UpdateCompareEventArgs e);
		public delegate void UpdateReportHandler(UpdateReportEventArgs e);
		#endregion

		#region Event Declarations.
		public event UpdateCompareHandler OnUpdateCompare;
		public event UpdateReportHandler OnUpdateReport;
		#endregion

		#region Event raising helper methods.
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
		/// Trigger update report event.
		/// </summary>
		private void SignalUpdateReport(long pnIncrement)
		{
			if (OnUpdateReport != null)
			{
				OnUpdateReport(new UpdateReportEventArgs(pnIncrement));
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CompareCharacters() : base()
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
		/// Compare two blocks of characters against each other.
		/// </summary>
		/// <param name="poAlpha">First block of characters.</param>
		/// <param name="poBeta">Second block of characters.</param>
		public override void Compare(Block poAlpha1, Block poBeta1)
		{
			Sections.OnAddSection += new Sections.AddSectionHandler(Sections_OnAddSection);

			BlockOfCharacters poAlpha = (BlockOfCharacters)poAlpha1;
			BlockOfCharacters poBeta = (BlockOfCharacters)poBeta1;

			this.MinElements = this.MinChars;
			Analyse(1, poAlpha, 1, poBeta);

			//Add missing alpha segments.
			SegmentComparer oSegAComparer = new SegmentComparer();
			oSegAComparer.ComparisonMethod = SegmentComparer.ComparisonType.AlphaPosition;
			AddMissingSegments("A1", oSegAComparer, mcSegA, poAlpha);

			//Add missing beta segments.
			SegmentComparer oSegBComparer = new SegmentComparer();
			oSegBComparer.ComparisonMethod = SegmentComparer.ComparisonType.BetaPosition;
			AddMissingSegments("B1", oSegBComparer, mcSegB, poBeta);

			//Check if user has chosen to cancel run.
			if (Interrupt.Reason == "Cancel")
			{
				return;
			}

			//Merge into report.
			long nCountA = 0;
			long nCountB = 0;
			string sMatchIndent = string.Empty;
			string sInsertIndent = string.Empty;
			SegmentOfCharacters oSegA = new SegmentOfCharacters();
			SegmentOfCharacters oSegB = new SegmentOfCharacters();
			oSegA.Type = "X1";
			oSegA.Data = new BlockOfCharacters();
			oSegB.Type = "Z1";
			oSegB.Data = new BlockOfCharacters();
			SectionOfCharacters oSection = new SectionOfCharacters();
			while (nCountA < mcSegA.Count || nCountB < mcSegB.Count)
			{
				if (nCountA < mcSegA.Count)
				{
					oSegA = (SegmentOfCharacters)mcSegA[(int)nCountA];
				}
				if (nCountB < mcSegB.Count)
				{
					oSegB = (SegmentOfCharacters)mcSegB[(int)nCountB];
				}
				if (oSegA.Type.Substring(0, 1) == "M" && oSegB.Type.Substring(0, 1) == "M")
				{
					if (!oSegA.Data.IsEmpty)
					{
						oSection = new SectionOfCharacters();
						oSection.Type = oSegA.Type;
						oSection.Position = oSegA.PositionA;
						oSection.PositionA = oSegA.PositionA;
						oSection.PositionB = oSegB.PositionB;
						oSection.Size = oSegA.Size;
						oSection.Data = oSegA.Data;
						oSection.Indentation = sInsertIndent;
						Sections.Add(oSection);
						oSegA.Display(sInsertIndent);
						sMatchIndent = oSegA.Indent;
						sInsertIndent = oSegA.Indent;
					}
					nCountA++;
					nCountB++;
				}
				else if (oSegA.Type.Substring(0, 1) == "M" && oSegB.Type.Substring(0, 1) != "M")
				{
					if (!oSegB.Data.IsEmpty)
					{
						oSection = new SectionOfCharacters();
						oSection.Type = oSegB.Type;
						oSection.Position = oSegB.PositionB;
						oSection.PositionA = oSegA.PositionA;
						oSection.PositionB = oSegB.PositionB;
						oSection.Size = oSegB.Size;
						oSection.Data = oSegB.Data;
						oSection.Indentation = sMatchIndent;
						Sections.Add(oSection);
						oSegB.Display(sMatchIndent);
					}
					nCountB++;
				}
				else if (oSegA.Type.Substring(0, 1) != "M" && oSegB.Type.Substring(0, 1) == "M")
				{
					if (!oSegA.Data.IsEmpty)
					{
						oSection = new SectionOfCharacters();
						oSection.Type = oSegA.Type;
						oSection.Position = oSegA.PositionA;
						oSection.PositionA = oSegA.PositionA;
						oSection.PositionB = oSegB.PositionB;
						oSection.Size = oSegA.Size;
						oSection.Data = oSegA.Data;
						oSection.Indentation = sMatchIndent;
						Sections.Add(oSection);
						oSegA.Display(sMatchIndent);
						sInsertIndent = oSegA.Indent;
					}
					nCountA++;
				}
				else if (oSegA.Type.Substring(0, 1) != "M" && oSegB.Type.Substring(0, 1) != "M")
				{
					if (oSegA.PositionA < oSegB.PositionB)
					{
						if (!oSegA.Data.IsEmpty)
						{
							oSection = new SectionOfCharacters();
							oSection.Type = oSegA.Type;
							oSection.Position = oSegA.PositionA;
							oSection.PositionA = oSegA.PositionA;
							oSection.PositionB = oSegB.PositionB;
							oSection.Size = oSegA.Size;
							oSection.Data = oSegA.Data;
							oSection.Indentation = sMatchIndent;
							Sections.Add(oSection);
							oSegA.Display(sMatchIndent);
							sInsertIndent = oSegA.Indent;
						}
						if (!oSegB.Data.IsEmpty)
						{
							oSection = new SectionOfCharacters();
							oSection.Type = oSegB.Type;
							oSection.Position = oSegB.PositionB;
							oSection.PositionA = oSegA.PositionA;
							oSection.PositionB = oSegB.PositionB;
							oSection.Size = oSegB.Size;
							oSection.Data = oSegB.Data;
							oSection.Indentation = sMatchIndent;
							Sections.Add(oSection);
							oSegB.Display(sMatchIndent);
						}
					}
					else
					{
						if (!oSegB.Data.IsEmpty)
						{
							oSection = new SectionOfCharacters();
							oSection.Type = oSegB.Type;
							oSection.Position = oSegB.PositionB;
							oSection.PositionA = oSegA.PositionA;
							oSection.PositionB = oSegB.PositionB;
							oSection.Size = oSegB.Size;
							oSection.Data = oSegB.Data;
							oSection.Indentation = sMatchIndent;
							Sections.Add(oSection);
							oSegB.Display(sMatchIndent);
						}
						if (!oSegA.Data.IsEmpty)
						{
							oSection = new SectionOfCharacters();
							oSection.Type = oSegA.Type;
							oSection.Position = oSegA.PositionA;
							oSection.PositionA = oSegA.PositionA;
							oSection.PositionB = oSegB.PositionB;
							oSection.Size = oSegA.Size;
							oSection.Data = oSegA.Data;
							oSection.Indentation = sMatchIndent;
							Sections.Add(oSection);
							oSegA.Display(sMatchIndent);
							sInsertIndent = oSegA.Indent;
						}
					}
					nCountA++;
					nCountB++;
				}
			}
		}

		/// <summary>
		/// Read the specified text file and return its contents as a block of element characters.
		/// </summary>
		/// <param name="psFile">Text file specification.</param>
		/// <returns>Entire file contents as a block of element characters.</returns>
		public override Block Read(string psFile)
		{
			StreamReader oSr;
			string sContents = string.Empty;
			ElementChar[] aElementChars = null;
			try
			{
				if (File.Exists(psFile))
				{
					oSr = new StreamReader(psFile);
					sContents = oSr.ReadToEnd();
					oSr.Close();
				}
				else
				{
					sContents = string.Empty;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			aElementChars = new ElementChar[sContents.Length];
			char[] aCharacters = sContents.ToCharArray();
			for (int nCount = 0; nCount < aCharacters.Length; nCount++)
			{
				aElementChars[nCount] = new ElementChar(aCharacters[nCount]);
			}
			BlockOfCharacters oBlock = new BlockOfCharacters(aElementChars);
			return oBlock;
		}

		/// <summary>
		/// Allocate a block of characters.
		/// </summary>
		/// <returns>A block of characters.</returns>
		public override Block AllocateBlock()
		{
			return new BlockOfCharacters();
		}

		/// <summary>
		/// Allocate a segment of characters.
		/// </summary>
		/// <returns>A segment of characters.</returns>
		public override Segment AllocateSegment()
		{
			return new SegmentOfCharacters();
		}
		#endregion
	}
}
