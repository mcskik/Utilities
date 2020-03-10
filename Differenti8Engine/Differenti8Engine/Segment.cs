using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Segment of matching data.
	/// </summary>
	/// <remarks>
	/// Segment of matching data.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Segment : IComparable
	{
		#region Member variables.
		private string msType = string.Empty;
		private long mnPositionA = 0;
		private long mnPositionB = 0;
		private long mnSize = 0;
		private Block moData = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Type of match ("M". "I", "D").
		/// </summary>
		public string Type
		{
			get
			{
				return msType;
			}
			set
			{
				msType = value;
			}
		}

		/// <summary>
		/// Character position or line number in the first or "Alpha" file.
		/// </summary>
		public long PositionA
		{
			get
			{
				return mnPositionA;
			}
			set
			{
				mnPositionA = value;
			}
		}

		/// <summary>
		/// Character position or line number in the first or "Alpha" file in sortable key form.
		/// </summary>
		public string PositionASortKey
		{
			get
			{
				return "K" + StringHelper.LPad(mnPositionA.ToString().Trim(),6,"0");
			}
		}

		/// <summary>
		/// Character position or line number in the second or "Beta" file.
		/// </summary>
		public long PositionB
		{
			get
			{
				return mnPositionB;
			}
			set
			{
				mnPositionB = value;
			}
		}

		/// <summary>
		/// Character position or line number in the second or "Beta" file in sortable key form.
		/// </summary>
		public string PositionBSortKey
		{
			get
			{
				return "K" + StringHelper.LPad(mnPositionB.ToString().Trim(), 6, "0");
			}
		}

		/// <summary>
		/// Size of the match as number of characters or number of lines.
		/// </summary>
		public long Size
		{
			get
			{
				return mnSize;
			}
			set
			{
				mnSize = value;
			}
		}

		/// <summary>
		/// Actual block of data which matched.
		/// </summary>
		public virtual Block Data
		{
			get
			{
				return moData;
			}
			set
			{
				moData = value;
			}
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display segment block appropriately.
		/// </summary>
		public virtual void Display()
		{
			moData.Type = this.Type;
			moData.Display();
		}
		#endregion

        #region Private methods.
        #endregion

        #region IComparable methods.
		/// <summary>
		/// IComparable compare to method.
		/// </summary>
		/// <param name="poSegment2">Segment to compare with this one.</param>
		/// <returns>Returns (-1 LT) (0 EQ) or (1 GT)</returns>
		public Int32 CompareTo(object poSegment2)
		{
			int nResult = 0;
			Segment oSegment2 = (Segment)poSegment2;
			nResult = PositionA.CompareTo(oSegment2.PositionA);
			return nResult;
		}

		/// <summary>
		/// IComparable compare to method.
		/// </summary>
		/// <param name="poSegment2">Segment to compare with this one.</param>
		/// <param name="peComparisonMethod">Sort by PositionASortKey or by PositionBSortKey.</param>
		/// <returns>Returns (-1 LT) (0 EQ) or (1 GT)</returns>
		public Int32 CompareTo(Segment poSegment2, SegmentComparer.ComparisonType peComparisonMethod)
		{
			int nResult = 0;
			switch (peComparisonMethod)
			{
				case SegmentComparer.ComparisonType.AlphaPosition:
					nResult = PositionA.CompareTo(poSegment2.PositionA);
					break;
				case SegmentComparer.ComparisonType.BetaPosition:
					nResult = PositionB.CompareTo(poSegment2.PositionB);
					break;
				default:
					nResult = PositionA.CompareTo(poSegment2.PositionA);
					break;
			}
			return nResult;
		}
		#endregion
	}
}
