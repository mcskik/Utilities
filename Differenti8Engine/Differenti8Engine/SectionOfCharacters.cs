using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Report section of characters.
	/// </summary>
	/// <remarks>
	/// Report section of characters.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class SectionOfCharacters : Section
	{
		#region Member variables.
		private BlockOfCharacters moData = null;
		private string msIndentation = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Actual block of characters to report.
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
		/// Indentation to use at the start of this block of characters.
		/// </summary>
		/// <remarks>
		/// Please note that this is not exactly the same as the segment indent.
		/// </remarks>
		public string Indentation
		{
			get
			{
				return msIndentation;
			}
			set
			{
				msIndentation = value;
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
