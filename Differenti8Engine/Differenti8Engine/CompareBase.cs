using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Compare engine base class.
	/// </summary>
	/// <remarks>
	/// Compare engine base class.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public abstract class CompareBase
	{
		#region Member variables.
		private long mnMinElements = 0;
		private long mnMinChars = 0;
		private long mnMinLines = 0;
        private long mnLimitElements = 0;
        private long mnLimitCharacters = 0;
        private long mnLimitLines = 0;
        private long mnSubLineMatchLimit = 0;
        private bool mbCompleteLines = false;
		protected Segments mcSegA;
		protected Segments mcSegB;
		private Sections mcSections;
		private Interrupt moInterrupt; 
		#endregion

		#region Properties.
		/// <summary>
		/// Minimum number of elements needed to be considered a match.
		/// </summary>
		public long MinElements
		{
			get
			{
				return mnMinElements;
			}
			set
			{
				mnMinElements = value;
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
        public long LimitElements
        {
            get
            {
                return mnLimitElements;
            }
            set
            {
                mnLimitElements = value;
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
		#endregion

		#region Delegates.
		#region Segment.
		public delegate void BeginSegmentHandler(BeginSegmentEventArgs e);
		public delegate void UpdateSegmentHandler(UpdateSegmentEventArgs e);
		public delegate void EndOfSegmentHandler(EndOfSegmentEventArgs e);
		#endregion
		#endregion

		#region Event Declarations.
		#region Segment.
		public event BeginSegmentHandler OnBeginSegment;
		public event UpdateSegmentHandler OnUpdateSegment;
		public event EndOfSegmentHandler OnEndOfSegment;
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
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CompareBase()
		{
			mcSegA = new Segments();
			mcSegA.OnAddSegment += new Segments.AddSegmentHandler(mcSegA_OnAddSegment);
			mcSegB = new Segments();
			mcSegB.OnAddSegment += new Segments.AddSegmentHandler(mcSegB_OnAddSegment);
		}
		#endregion

		#region Event Handlers.
		void mcSegB_OnAddSegment(Segments.AddSegmentEventArgs e)
		{
			long segmentLength = e.SegmentLength;
			SignalUpdateSegment(segmentLength);
		}

		void mcSegA_OnAddSegment(Segments.AddSegmentEventArgs e)
		{
			long segmentLength = e.SegmentLength;
			SignalUpdateSegment(segmentLength);
		}
		#endregion

		#region Abstract methods.
		/// <summary>
		/// Compare two blocks against each other.
		/// </summary>
		/// <param name="poAlpha">First block.</param>
		/// <param name="poBeta">Second block.</param>
		public abstract void Compare(Block poAlpha, Block poBeta);

		/// <summary>
		/// Read the specified file and return its contents as a block of elements.
		/// </summary>
		/// <param name="psFile">Text file specification.</param>
		/// <returns>Entire file contents as a block of elements.</returns>
		public abstract Block Read(string psFile);
		#endregion

		#region Virtual methods.
		/// <summary>
		/// Allocate an appropriate type of block.
		/// </summary>
		/// <returns>An appropriate block.</returns>
		public virtual Block AllocateBlock()
		{
			return new Block();
		}

		/// <summary>
		/// Allocate an appropriate type of segment.
		/// </summary>
		/// <returns>An appropriate segment.</returns>
		public virtual Segment AllocateSegment()
		{
			return new Segment();
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Add missing alpha or beta segments to alpha or beta ArrayList.
		/// </summary>
		/// <param name="psMissingType">Missing segment type for all missing segments to be added.</param>
		/// <param name="poSegComparer">Segment comparar for alpha or beta segment ArrayList.</param>
		/// <param name="pcSegments">Alpha or beta segment ArrayList.</param>
		/// <param name="poFullBlock">Full alpha or beta block read in from original file.</param>
		protected void AddMissingSegments(string psMissingType, SegmentComparer poSegComparer, Segments pcSegments, Block poFullBlock)
		{
			pcSegments.Sort(poSegComparer);
			long nCount = 0;
			long nPrevPos = 1;
			long nPrevLen = 0;
			long nPrevEnd = 0;
			long nThisPos = 1;
			long nThisLen = 0;
			Segment oSegment = null;
			Segment oMissing = null;
			while (nCount < pcSegments.Count)
			{
				oSegment = (Segment)pcSegments[(int)nCount];
				if (oSegment.Type.Substring(0, 1) == "M")
				{
					if (psMissingType == "A1")
					{
						nThisPos = oSegment.PositionA;
					}
					if (psMissingType == "B1")
					{
						nThisPos = oSegment.PositionB;
					}
					nThisLen = oSegment.Size;
					if (nThisPos - nPrevEnd > 1)
					{
						oMissing = AllocateSegment();
						oMissing.Type = psMissingType;
						oMissing.PositionA = nPrevEnd + 1;
						oMissing.PositionB = nPrevEnd + 1;
						oMissing.Data = poFullBlock.Extract(nPrevEnd + 1, nThisPos - (nPrevEnd + 1));
						oMissing.Size = oMissing.Data.Length;
						SignalBeginSegment(oMissing.Data.ContentsLength);
						pcSegments.Add(oMissing);
						SignalEndOfSegment();
					}
					nPrevPos = nThisPos;
					nPrevLen = nThisLen;
					nPrevEnd = nPrevPos + nPrevLen - 1;
				}
				else
				{
					break;
				}
				nCount++;
			}
			if (nPrevEnd < poFullBlock.Length)
			{
				nThisPos = poFullBlock.Length + 1;
				oMissing = AllocateSegment();
				oMissing.Type = psMissingType;
				oMissing.PositionA = nPrevEnd + 1;
				oMissing.PositionB = nPrevEnd + 1;
				oMissing.Data = poFullBlock.Extract(nPrevEnd + 1, nThisPos - (nPrevEnd + 1));
				oMissing.Size = oMissing.Data.Length;
                SignalBeginSegment(oMissing.Data.ContentsLength);
                pcSegments.Add(oMissing);
                SignalEndOfSegment();
			}
			pcSegments.Sort(poSegComparer);
		}

		/// <summary>
		/// Analyse / compare.
		/// </summary>
		/// <param name="pnAlphaPos">Position of start of passed alpha block in full original alpha file block.</param>
		/// <param name="poAlpha">Passed alpha block.</param>
		/// <param name="pnBetaPos">Position of start of passed beta block in full original beta file block.</param>
		/// <param name="poBeta">Passed beta block.</param>
		protected void Analyse(long pnAlphaPos, Block poAlpha, long pnBetaPos, Block poBeta)
		{
			Block oAlpha1 = AllocateBlock();
			Block oAlpha2 = AllocateBlock();
			Block oBeta1 = AllocateBlock();
			Block oBeta2 = AllocateBlock();
			long nAPos = 0;
			long nALen = 0;
			long nBPos = 0;
			long nBLen = 0;
			Segment oSegment = null;

			//Check if user has chosen to cancel run.
			if (Interrupt.Reason == "Cancel")
			{
				return;
			}

			LargestMatch(poAlpha, ref nAPos, ref nALen, poBeta, ref nBPos, ref nBLen);
			if (nAPos > 0 && nBPos > 0)
			{
				oSegment = AllocateSegment();
				oSegment.Type = "M3";
				oSegment.PositionA = pnAlphaPos + nAPos - 1;
				oSegment.PositionB = pnBetaPos + nBPos - 1;
				oSegment.Size = nALen;
				oSegment.Data = poAlpha.Extract(nAPos, nALen);
				mcSegA.Add(oSegment);
				mcSegB.Add(oSegment);

				oAlpha1 = AllocateBlock();
				oAlpha2 = AllocateBlock();
				if (nAPos == 1)
				{
					oAlpha1 = AllocateBlock();
					oAlpha2 = poAlpha.Extract(nAPos + nALen);
				}
				else if (nAPos + nALen > poAlpha.Length)
				{
					oAlpha1 = poAlpha.Extract(1, nAPos - 1);
					oAlpha2 = AllocateBlock();
				}
				else
				{
					oAlpha1 = poAlpha.Extract(1, nAPos - 1);
					oAlpha2 = poAlpha.Extract(nAPos + nALen);
				}

				oBeta1 = AllocateBlock();
				oBeta2 = AllocateBlock();
				if (nBPos == 1)
				{
					oBeta1 = AllocateBlock();
					oBeta2 = poBeta.Extract(nBPos + nBLen);
				}
				else if (nBPos + nBLen > poBeta.Length)
				{
					oBeta1 = poBeta.Extract(1, nBPos - 1);
					oBeta2 = AllocateBlock();
				}
				else
				{
					oBeta1 = poBeta.Extract(1, nBPos - 1);
					oBeta2 = poBeta.Extract(nBPos + nBLen);
				}

				if (oAlpha1.IsEmpty && oBeta1.IsEmpty)
				{
				}
				else if (oAlpha1.IsEmpty && !oBeta1.IsEmpty)
				{
				}
				else if (!oAlpha1.IsEmpty && oBeta1.IsEmpty)
				{
				}
				else if (!oAlpha1.IsEmpty && !oBeta1.IsEmpty)
				{
					Analyse(pnAlphaPos, oAlpha1, pnBetaPos, oBeta1);
				}

				if (oAlpha2.IsEmpty && oBeta2.IsEmpty)
				{
				}
				else if (oAlpha2.IsEmpty && !oBeta2.IsEmpty)
				{
				}
				else if (!oAlpha2.IsEmpty && oBeta2.IsEmpty)
				{
				}
				else if (!oAlpha2.IsEmpty && !oBeta2.IsEmpty)
				{
					Analyse(pnAlphaPos + nAPos + nALen - 1, oAlpha2, pnBetaPos + nBPos + nBLen - 1, oBeta2);
				}
			}
		}

		/// <summary>
		/// Find largest matching block of elements. (Alpha or Beta length could be greater.)
		/// </summary>
		/// <param name="poAlpha">Passed alpha block.</param>
		/// <param name="pnAlphaPos">Position of start of passed alpha block in full original alpha file block.</param>
		/// <param name="pnAlphaLen">Length of passed alpha block.</param>
		/// <param name="poBeta">Passed beta block.</param>
		/// <param name="pnBetaPos">Position of start of passed beta block in full original beta file block.</param>
		/// <param name="pnBetaLen">Length of passed beta block.</param>
		protected void LargestMatch(Block poAlpha, ref long pnAlphaPos, ref long pnAlphaLen, Block poBeta, ref long pnBetaPos, ref long pnBetaLen)
		{
			if (poAlpha == poBeta)
			{
				SignalBeginSegment(poAlpha.Length);
				pnAlphaPos = 1;
				pnAlphaLen = poAlpha.Length;
				pnBetaPos = 1;
				pnBetaLen = poBeta.Length;
				SignalEndOfSegment();
			}
			else if (poAlpha.Length == 0 && poBeta.Length == 0)
			{
				SignalEndOfSegment();
			}
			else if (poAlpha.Length == 0 && poBeta.Length > 0)
			{
				SignalBeginSegment(poBeta.Length);
				pnAlphaPos = 0;
				pnAlphaLen = 0;
				pnBetaPos = 1;
				pnBetaLen = poBeta.Length;
				SignalEndOfSegment();
			}
			else if (poAlpha.Length > 0 && poBeta.Length == 0)
			{
				SignalBeginSegment(poAlpha.Length);
				pnAlphaPos = 1;
				pnAlphaLen = poAlpha.Length;
				pnBetaPos = 0;
				pnBetaLen = 0;
				SignalEndOfSegment();
			}
			else if (poAlpha.Length <= poBeta.Length)
			{
				LargestMatcher(poAlpha, ref pnAlphaPos, ref pnAlphaLen, poBeta, ref pnBetaPos, ref pnBetaLen);
			}
			else
			{
				LargestMatcher(poBeta, ref pnBetaPos, ref pnBetaLen, poAlpha, ref pnAlphaPos, ref pnAlphaLen);
			}
		}

		/// <summary>
		/// Find largest matching block of elements. (Gamma length is always <= Delta length.)
		/// </summary>
		/// <param name="poGamma">Passed gamma block.</param>
		/// <param name="pnGammaPos">Position of start of passed gamma block in full original alpha file block.</param>
		/// <param name="pnGammaLen">Length of passed gamma block.</param>
		/// <param name="poDelta">Passed delta block.</param>
		/// <param name="pnDeltaPos">Position of start of passed delta block in full original beta file block.</param>
		/// <param name="pnDeltaLen">Length of passed delta block.</param>
		protected void LargestMatcher(Block poGamma, ref long pnGammaPos, ref long pnGammaLen, Block poDelta, ref long pnDeltaPos, ref long pnDeltaLen)
		{
			long nPos1 = 0;
			long nLen1 = 0;
			long nMax1 = 0;
			long nRem1 = 0;
			long nPos2 = 0;
			long nHigh = 0;
			Block oTrail = null;
			Block oBlock = null;

			//New method to find the highest matching length faster.
			long nGood = 0;
			long nBad = 0;
			long nStep = 0;

			//Default returned values to zero.
			pnGammaPos = 0;
			pnGammaLen = 0;
			pnDeltaPos = 0;
			pnDeltaLen = 0;

			//Find largest matching block.
			nHigh = 0;
			nMax1 = poGamma.Length;
			SignalBeginSegment(nMax1);
			for (nPos1 = 1; nPos1 <= nMax1; nPos1++)
			{
				//Check if user has chosen to cancel run.
				if (Interrupt.Reason == "Cancel")
				{
					return;
				}
				SignalUpdateSegment(1);
				nRem1 = nMax1 - nPos1 + 1;
				if (nRem1 <= nHigh)
				{
					break;
				}
				oTrail = poGamma.Extract(nPos1);
				//New method to find the highest matching length faster.
				nGood = 0;
				nBad = nRem1;
				nStep = nBad - nGood;
				nLen1 = nGood + nStep;
				while (nStep > 0)
				{
					nPos2 = poDelta.Find(oTrail.Extract(1, nLen1));
					if (nPos2 > 0)
					{
						if (nLen1 > nHigh)
						{
							nHigh = nLen1;
							pnGammaPos = nPos1;
							pnGammaLen = nLen1;
							pnDeltaPos = nPos2;
							pnDeltaLen = nLen1;
						}
						//Step up.
						nGood = nLen1;
						nStep = (nBad - nGood) / 2;
						nLen1 = nGood + nStep;
					}
					else
					{
						//Step down.
						nBad = nLen1;
						nStep = (nBad - nGood) / 2;
						nLen1 = nGood + nStep;
					}
				}

				//Old method of finding the highest matching length.
				//for (nLen1 = nHigh + 1; nLen1 <= nRem1; nLen++)
				//{
				//nPos2 = poDelta.Find(oTrail.Extract(nLen1));
				//if (nPos2 > 0)
				//{
				//nLen1 must be greater than nHigh
				//since it starts at nHigh plus one.
				//nHigh = nLen1;
				//pnGammaPos = nPos1;
				//pnGammaLen = nLen1;
				//pnDeltaPos = nPos2;
				//pnDeltaLen = nLen1;
				//}
				//else
				//{
				//break;
				//}
				//}

				//New bit to detect if the largest match has probably been found.
				//The reasoning here is that if you have got more than
				//the limit of characters to match you have probably found
				//the largest match so you don't need to keep trying.
				if (nHigh > mnLimitElements)
				{
					break;
				}
			}

			//New bit to detect if the largest match has at least the
			//minimum number of characters and lines necessary
			//to be considered a match.  If these minimums are not
			//met then ignore the match.
			if (nHigh > 0)
			{
				oBlock = poGamma.Extract(pnGammaPos, pnGammaLen);
				if (oBlock.Length < mnMinElements)
				{
					nHigh = 0;
					pnGammaPos = 0;
					pnGammaLen = 0;
					pnDeltaPos = 0;
					pnDeltaLen = 0;
				}
			}

			SignalEndOfSegment();
		}
		#endregion
	}
}
