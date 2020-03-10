using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Element line class.
	/// </summary>
	/// <remarks>
	/// Holds the data for one line from a text file.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ElementLine : Element
	{
		#region Member variables.
		private string msContents = string.Empty;
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
				msContents = value.ToString();
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ElementLine(string psContents)
		{
			msContents = psContents;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Return the display form of one line element.
		/// </summary>
		/// <returns>Display form of one line element.</returns>
		public override string ToDisplay()
		{
			//return msContents.ToString();
            return msContents;
        }

		/// <summary>
		/// Calculate signature of element line.
		/// </summary>
		/// <returns>Signature of line.</returns>
		public override Signature Signature()
		{
			Signature oSignature = new Signature();
			oSignature.Length = msContents.Length;
			oSignature.Area = CalculateArea();
			oSignature.CentreOfMass = CalculateCentreOfMass();
			return oSignature;
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Calculate area of line.
		/// </summary>
		/// <remarks>
		/// The area is calculated by summing the magnitude (character code) of each character in the line.
		/// </remarks>
		/// <returns>Area of line.</returns>
		private long CalculateArea()
		{
			long nArea = 0;
			char[] aContents = msContents.ToCharArray();
			for (int nPos = 0; nPos < aContents.Length; nPos++)
			{
				char sChar = aContents[nPos];
				int nCode = (int)sChar;
				try
				{
					nArea += nCode;
				}
				catch (OverflowException)
				{
					nArea = 0;
				}
			}
			return nArea;
		}

		/// <summary>
		/// Calculate centre of mass of line.
		/// </summary>
		/// <remarks>
		/// The centre of mass is calculated by summing the magnitude (character code) of each character
		/// multiplied by its position along the line, and then all divided by the area of the line
		/// as defined in the Calculate area function.
		/// 
		/// SUM(posN * massN) / SUM(massN) ==(1/SUM(massN)) * (SUM(posN * massN))
		/// 
		/// The numerator is likely to result in an arithmetic overflow.
		/// 
		/// == (1/SUM(massN)) * (pos1 * mass1) + (1/SUM(massN)) * (pos2 * mass2) + (1/SUM(massN)) * (posN * massN)
		/// 
		/// This way we reduce the likelihood of an arithmetic overflow.
		/// </remarks>
		/// <returns>Area of line.</returns>
		private double CalculateCentreOfMass()
		{
			double nCentreOfMass = 0;
			long nArea = CalculateArea();
			double nInverseArea = 1d / (double)nArea;
			double nPositionMass = 0;
			double nSumTerm = 0;
			char[] aContents = msContents.ToCharArray();
			for (int nPos = 0; nPos < aContents.Length; nPos++)
			{
				char sChar = aContents[nPos];
				int nCode = (int)sChar;
				try
				{
					nPositionMass = nPos * nCode;
					nSumTerm = nInverseArea * nPositionMass;
					nCentreOfMass += nSumTerm;
				}
				catch (OverflowException)
				{
					nCentreOfMass = 0;
				}
			}
			return nCentreOfMass;
		}

		/// <summary>
		/// Check if both elements are equal.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(ElementLine poElement1, ElementLine poElement2)
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
		public static bool operator ==(ElementLine poElement1, ElementLine poElement2)
		{
			return AreEqual(poElement1, poElement2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poElement1">Left hand side operand element to compare.</param>
		/// <param name="poElement2">Right hand side operand element to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(ElementLine poElement1, ElementLine poElement2)
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
			return AreEqual(this, poElement2 as ElementLine);
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
