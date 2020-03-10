using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Segment of matching lines.
	/// </summary>
	/// <remarks>
	/// Segment of matching lines.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class SegmentOfLines : Segment
	{
		#region Member variables.
		private BlockOfLines moData = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Actual block of lines which matched.
		/// </summary>
		public override Block Data
		{
			get
			{
				return moData;
			}
			set
			{
				moData = (BlockOfLines)value;
			}
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display segment of lines block appropriately.
		/// </summary>
		public override void Display()
		{
			moData.Type = this.Type;
			moData.Display();
		}
		#endregion
	}
}
