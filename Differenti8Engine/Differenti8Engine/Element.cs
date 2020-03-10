using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Element base class.
	/// </summary>
	/// <remarks>
	/// All comparisons will be performed on element classes derived from this one.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public abstract class Element
	{
		#region Properties.
		/// <summary>
		/// Element contents.
		/// </summary>
		public abstract object Contents
		{
			get;
			set;
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Element() 
		{ 
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Return the display form of one element.
		/// </summary>
		/// <returns>Display form of one element.</returns>
		public abstract string ToDisplay();

		/// <summary>
		/// Calculate signature of element.
		/// </summary>
		/// <returns>Signature of element.</returns>
		public abstract Signature Signature();
		#endregion

		#region Private methods.
		/// <summary>
		/// Check if both elements are equal.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(Element poElement1, Element poElement2)
		{
			bool bEqual = false;
			if (poElement1.Contents.ToString() == poElement2.Contents.ToString())
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
		public static bool operator ==(Element poElement1, Element poElement2)
		{
			return AreEqual(poElement1, poElement2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(Element poElement1, Element poElement2)
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
			return AreEqual(this, poElement2 as Element);
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
