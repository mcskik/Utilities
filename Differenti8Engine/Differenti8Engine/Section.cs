using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Section of data.
	/// </summary>
	/// <remarks>
	/// Section of data.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Section
	{
		#region Member variables.
		private string msType = string.Empty;
		private long mnPosition = 0;
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
		/// Character position or line number in the full file.
		/// </summary>
		public long Position
		{
			get
			{
				return mnPosition;
			}
			set
			{
				mnPosition = value;
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
		/// Actual block of data.
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
	}
}
