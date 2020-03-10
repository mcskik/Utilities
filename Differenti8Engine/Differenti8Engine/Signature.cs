using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Line signature class.
	/// </summary>
	/// <remarks>
	/// Holds characteristics of a line.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Signature
	{
		#region Member variables
		private int mnLength = 0;
		private long mnArea = 0;
		private double mnCentreOfMass = 0;
		#endregion

		#region Properties.
		/// <summary>
		/// Length of line.
		/// </summary>
		public int Length
		{
			get
			{
				return mnLength;
			}
			set
			{
				mnLength = value;
			}
		}

		/// <summary>
		/// Area of line.
		/// </summary>
		public long Area
		{
			get
			{
				return mnArea;
			}
			set
			{
				mnArea = value;
			}
		}

		/// <summary>
		/// CentreOfMass of line.
		/// </summary>
		public double CentreOfMass
		{
			get
			{
				return mnCentreOfMass;
			}
			set
			{
				mnCentreOfMass = value;
			}
		}
		#endregion
	}
}
