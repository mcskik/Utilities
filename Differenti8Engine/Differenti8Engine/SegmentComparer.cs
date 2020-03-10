using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Segment comparer class.
	/// </summary>
	/// <remarks>
	/// Class which implements IComparer used when sorting lists of Segments.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class SegmentComparer : IComparer
	{
		#region Enumerations.
		public enum ComparisonType
		{
			AlphaPosition = 1,
			BetaPosition = 2
		}
		#endregion

		#region Member variables.
		private ComparisonType meComparisonType;
		#endregion

		#region Properties.
		public ComparisonType ComparisonMethod
		{
			get
			{
				return meComparisonType;
			}
			set
			{
				meComparisonType = value;
			}
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// IComparer compare method.
		/// </summary>
		/// <param name="poSegment1">First segment to compare.</param>
		/// <param name="poSegment2">Second segment to compare.</param>
		/// <returns>Returns (-1 LT) (0 EQ) or (1 GT)</returns>
		public Int32 Compare(object poSegment1, object poSegment2)
		{
			Segment oSegment1 = (Segment)poSegment1;
			Segment oSegment2 = (Segment)poSegment2;
			return oSegment1.CompareTo(oSegment2, meComparisonType);
		}
		#endregion
	}
}
