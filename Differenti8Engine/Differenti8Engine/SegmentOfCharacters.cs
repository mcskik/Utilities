using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Segment of matching characters.
	/// </summary>
	/// <remarks>
	/// Segment of matching characters.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class SegmentOfCharacters : Segment
	{
		#region Member variables.
		private BlockOfCharacters moData = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Actual block of characters which matched.
		/// </summary>
		public override Block Data
		{
			get
			{
				return moData;
			}
			set
			{
				moData = (BlockOfCharacters)value;
			}
		}

		/// <summary>
		/// Current indent.
		/// </summary>
		public string Indent
		{
			get
			{
				return moData.Indent;
			}
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display segment of characters block appropriately.
		/// </summary>
		public override void Display()
		{
			moData.Type = this.Type;
			moData.Display();
		}

		/// <summary>
		/// Display segment of characters block with initial indent.
		/// </summary>
		/// <param name="psPrevIndent">Previous indentation string.</param>
		public void Display(string psPrevIndent)
		{
			moData.Type = this.Type;
			moData.Display(psPrevIndent);
		}
		#endregion
	}
}
