using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// User interrupt class.
	/// </summary>
	/// <remarks>
	/// Used to pass user interrupt reasons to lower level objects.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Interrupt
	{
		#region Member variables.
		private string msReason = string.Empty;
		#endregion

		#region Properties.
		/// <summary>
		/// Reason for the interrupt ie "Cancel".
		/// </summary>
		public string Reason
		{
			get
			{
				return msReason;
			}
			set
			{
				msReason = value;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="psReason">Reason for user interrupt.</param>
		public Interrupt(string psReason)
		{
			msReason = psReason;
		}
		#endregion
	}
}
