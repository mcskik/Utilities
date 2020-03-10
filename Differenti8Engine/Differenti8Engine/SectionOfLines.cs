using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Report section of lines.
	/// </summary>
	/// <remarks>
	/// Report section of lines.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class SectionOfLines : Section
	{
		#region Member variables.
		private BlockOfLines moData = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Actual block of lines to report.
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
		/// Display section of lines block appropriately.
		/// </summary>
		public override void Display()
		{
			moData.Type = this.Type;
			moData.Display();
		}
		#endregion
	}
}
