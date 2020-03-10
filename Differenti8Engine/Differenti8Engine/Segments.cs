using System;
using System.Collections;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Collection of matched segments.
	/// </summary>
	/// <remarks>
	/// Generic type safe collection of matched segments.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Segments : ArrayList //CollectionBase
	{
		#region Enumerations.
		public enum ProgressOnEnum
		{
			Alpha = 0,
			Beta = 1
		}
		#endregion

		#region Member Variables.
		private ProgressOnEnum _progressOn;
		#endregion

		#region Properties.
		public ProgressOnEnum ProgressOn
		{
			get
			{
				return _progressOn;
			}
			set
			{
				_progressOn = value;
			}
		}
		#endregion

		#region Custom Event Arguments.
		public class AddSegmentEventArgs : EventArgs
		{
			public readonly long SegmentLength;
			public AddSegmentEventArgs(long pnSegmentLength)
			{
				this.SegmentLength = pnSegmentLength;
			}
		}
		#endregion

		#region Delegates.
		public delegate void AddSegmentHandler(AddSegmentEventArgs e);
		#endregion

		#region Event Declarations.
		public event AddSegmentHandler OnAddSegment;
		#endregion

		#region Event raising helper methods.
		/// <summary>
		/// Trigger add segment event.
		/// </summary>
		/// <param name="pnSegmentLength">Segment length.</param>
		private void SignalAddSegment(long pnSegmentLength)
		{
			if (OnAddSegment != null)
			{
				OnAddSegment(new AddSegmentEventArgs(pnSegmentLength));
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Segments()
		{
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Indexer.
		/// </summary>
		/// <param name="index">Index number.</param>
		/// <returns>A segment.</returns>
		public override object this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Add a segment.
		/// </summary>
		/// <param name="value">Segment.</param>
		/// <returns>Index number of new segment.</returns>
		public int Add(Segment poSegment)
		{
			int index = base.Add(poSegment);
			if (_progressOn == ProgressOnEnum.Alpha)
			{
				if (poSegment.Type.Substring(0, 1) == "A" || poSegment.Type.Substring(0, 1) == "M")
				{
					SignalAddSegment(poSegment.Data.ContentsLength);
				}
			}
			else if (_progressOn == ProgressOnEnum.Beta)
			{
				if (poSegment.Type.Substring(0, 1) == "B" || poSegment.Type.Substring(0, 1) == "M")
				{
					SignalAddSegment(poSegment.Data.ContentsLength);
				}
			}
			return index;
		}
		#endregion
	}
}
