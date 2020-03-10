using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Element signature class.
	/// </summary>
	/// <remarks>
	/// Holds the data for one signature for one line from a text file.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ElementSignature : Element
	{
		#region Member variables.
		private Signature moContents = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Element contents.
		/// </summary>
		public override object Contents
		{
			get
			{
				return moContents;
			}
			set
			{
				moContents = (Signature)value;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ElementSignature(Signature poContents)
		{
			moContents = poContents;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Return the display form of one signature element.
		/// </summary>
		/// <returns>Display form of one signature element.</returns>
		public override string ToDisplay()
		{
			return String.Empty;
		}

		/// <summary>
		/// Simply return this element signature.
		/// </summary>
		/// <remarks>
		/// No need to calculate a signature as this element is itself a signature.
		/// </remarks>
		/// <returns>Element signature.</returns>
		public override Signature Signature()
		{
			return moContents;
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Check if both elements are equal.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(ElementSignature poElement1, ElementSignature poElement2)
		{
			Signature oSignature1 = (Signature)poElement1.Contents;
			Signature oSignature2 = (Signature)poElement2.Contents;
			bool bEqual = false;
			if (oSignature1.Length == oSignature2.Length)
			{
				if (oSignature1.Area == oSignature2.Area)
				{
					if (oSignature1.CentreOfMass == oSignature2.CentreOfMass)
					{
						bEqual = true;
					}
				}
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
		public static bool operator ==(ElementSignature poElement1, ElementSignature poElement2)
		{
			return AreEqual(poElement1, poElement2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(ElementSignature poElement1, ElementSignature poElement2)
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
			return AreEqual(this, poElement2 as ElementSignature);
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
