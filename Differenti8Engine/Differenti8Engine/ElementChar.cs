using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Element char class.
	/// </summary>
	/// <remarks>
	/// Holds the data for one character from a text file.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ElementChar : Element
	{
		#region Member variables.
		private char msContents = ' ';
		#endregion

		#region Properties.
		/// <summary>
		/// Element contents.
		/// </summary>
		public override object Contents
		{
			get
			{
				return msContents;
			}
			set
			{
				msContents = (char)value;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ElementChar(char psContents)
		{
			msContents = psContents;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Return the display form of one element character.
		/// </summary>
		/// <returns>Display form of one element character.</returns>
		public override string ToDisplay()
		{
			string sContents = string.Empty;
			if (msContents == '\n')
			{
				sContents = "\n";
			}
			else
			{
				sContents = msContents.ToString();
			}
			return sContents;
		}

		/// <summary>
		/// Calculate signature of element character.
		/// </summary>
		/// <returns>Signature of element character.</returns>
		public override Signature Signature()
		{
			Signature oSignature = new Signature();
			oSignature.Length = 1;
			oSignature.Area = (long)msContents;
			oSignature.CentreOfMass = (long)msContents;
			return oSignature;
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Check if both elements are equal.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(ElementChar poElement1, ElementChar poElement2)
		{
			bool bEqual = false;
			if ((char)poElement1.Contents == (char)poElement2.Contents)
			{
				bEqual = true;
			}
			else
			{
				bEqual = false;
			}
			return bEqual;
		}
		#endregion

		#region Operator overloads.
		/// <summary>
		/// Equality operator.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		public static bool operator ==(ElementChar poElement1, ElementChar poElement2)
		{
			return AreEqual(poElement1, poElement2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(ElementChar poElement1, ElementChar poElement2)
		{
			return !AreEqual(poElement1, poElement2);
		}

		/// <summary>
		/// Equality method.
		/// </summary>
		/// <param name="poElement2">Object to compare against this one.</param>
		/// <returns></returns>
		public override bool Equals(object poElement2)
		{
			return AreEqual(this, poElement2 as ElementChar);
		}

		/// <summary>
		/// Get Hash Code method.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		#endregion
	}
}
