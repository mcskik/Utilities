using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Block of character elements.
	/// </summary>
	/// <remarks>
	/// Container for an array of character elements.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class BlockOfCharacters : Block
	{
		#region Member variables.
		private ElementChar[] maElements = null;
		private string msIndent = string.Empty;
		#endregion

		#region Properties.
		/// <summary>
		/// Array of character elements.
		/// </summary>
		public override Element[] Elements
		{
			get
			{
				return maElements;
			}
			set
			{
				maElements = (ElementChar[])value;
			}
		}

		/// <summary>
		/// Current indent.
		/// </summary>
		public string Indent
		{
			get
			{
				return msIndent;
			}
		}

		/// <summary>
		/// True if this block of characters has a newline character anywhere.
		/// </summary>
		private bool HasNewlineAnywhere
		{
			get
			{
				bool bHasNewlineAnywhere = false;
				for (int nPos = 0; nPos < maElements.Length; nPos++)
				{
					if (maElements[nPos].ToDisplay() == "\n")
					{
						bHasNewlineAnywhere = true;
						break;
					}
				}
				return bHasNewlineAnywhere;
			}
		}

		/// <summary>
		/// True if this block of characters ends with a newline character.
		/// </summary>
		private bool EndsWithNewline
		{
			get
			{
				bool bEndsWithNewline = false;
				if (maElements.Length > 0)
				{
					if (maElements[maElements.Length - 1].ToDisplay() == "\n")
					{
						bEndsWithNewline = true;
					}
				}
				return bEndsWithNewline;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Deliberately construct a completely empty block of character elements.
		/// </summary>
		public BlockOfCharacters()
		{
			maElements = new ElementChar[0];
		}

		/// <summary>
		/// Construct a new empty block of character elements of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of character elements.</param>
		public BlockOfCharacters(long pnLength)
		{
			maElements = new ElementChar[pnLength];
		}

		/// <summary>
		/// Construct a new block to contain the specified character element array.
		/// </summary>
		/// <param name="paElements">Array of character elements.</param>
		public BlockOfCharacters(ElementChar[] paElements)
		{
			maElements = paElements;
		}

		/// <summary>
		/// Construct a new block to contain the character element array contained in the specified block.
		/// </summary>
		/// <param name="poBlock">Block of character elements.</param>
		public BlockOfCharacters(BlockOfCharacters poBlock)
		{
			maElements = (ElementChar[])poBlock.Elements;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display a block of character elements.
		/// </summary>
		public override void Display()
		{
			Display(string.Empty);
		}

		/// <summary>
		/// Display a block of character elements with initial indent.
		/// </summary>
		/// <param name="psPrevIndent">Previous indentation string.</param>
		public void Display(string psPrevIndent)
		{
            //Only used to set the indentation as the Reporter* classes are responsible for the actual writing now.
			SetIndent(psPrevIndent);
		}

		/// <summary>
		/// Return the contents of this block as a block of characters.
		/// </summary>
		/// <returns>Block of characters.</returns>
		public override BlockOfCharacters GetCharacters()
		{
			return this;
		}

		#region Allocation methods.
		/// <summary>
		/// Deliberately allocate a completely empty block of characters.
		/// </summary>
		/// <returns>An empty block of characters.</returns>
		public override Block AllocateBlock()
		{
			return new BlockOfCharacters();
		}

		/// <summary>
		/// Allocate a new empty block of characters of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of characters.</param>
		/// <returns>An empty block of characters of the specified length.</returns>
		public override Block AllocateBlock(long pnLength)
		{
			return new BlockOfCharacters(pnLength);
		}

		/// <summary>
		/// Allocate a new block containing the specified element character array.
		/// </summary>
		/// <param name="paElements">Array of element characters.</param>
		/// <returns>A new block containing the specified element character array.</returns>
		public override Block AllocateBlock(Element[] paElements)
		{
			return new BlockOfCharacters((ElementChar[])paElements);
		}

		/// <summary>
		/// Allocate a new block containing the element character array contained in the specified block of characters.
		/// </summary>
		/// <param name="poBlock">Block of characters.</param>
		/// <returns>A new block containing the element character array contained in the specified block of characters.</returns>
		public override Block AllocateBlock(Block poBlock)
		{
			return new BlockOfCharacters((BlockOfCharacters)poBlock);
		}
		#endregion
		#endregion

		#region Private methods.
		/// <summary>
		/// Set the indent string to be the same length as the current display string,
		/// unless the current display string ends with a newline character.
		/// </summary>
		/// <param name="psPrevIndent">Previous indentation string.</param>
		private void SetIndent(string psPrevIndent)
		{
			//Find start of last (or only) line.
			int nLastLineStart = 0;
			for (int nIndex = maElements.Length - 1; nIndex >= 0; nIndex--)
			{
				if (maElements[nIndex].ToDisplay() == "\n")
				{
					if (nIndex == 0)
					{
						nLastLineStart = nIndex;
					}
					else if (nIndex == maElements.Length)
					{
					}
					else
					{
						nLastLineStart = nIndex + 1;
						break;
					}
				}
			}
			//Generate indent string.
			if (EndsWithNewline)
			{
				msIndent = string.Empty;
			}
			else
			{
				msIndent = string.Empty;
				for (int nIndex = nLastLineStart; nIndex < maElements.Length; nIndex++)
				{
					string sChar = maElements[nIndex].ToDisplay();
					switch (sChar)
					{
						case "\t":
							msIndent += "\t";
							break;
						default:
							msIndent += " ";
							break;
					}
				}
				if (!HasNewlineAnywhere)
				{
					msIndent = psPrevIndent + msIndent;
				}
			}
		}

		/// <summary>
		/// Check if both blocks of elements are equal.
		/// </summary>
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(BlockOfCharacters poBlock1, BlockOfCharacters poBlock2)
		{
			bool bEqual = false;
			if (poBlock1.IsEmpty && !poBlock2.IsEmpty)
			{
				bEqual = false;
			}
			else if (!poBlock1.IsEmpty && poBlock2.IsEmpty)
			{
				bEqual = false;
			}
			else if (poBlock1.IsEmpty && poBlock2.IsEmpty)
			{
				bEqual = true;
			}
			else if (poBlock1.Length == 0 && poBlock2.Length == 0)
			{
				bEqual = true;
			}
			else if (poBlock1.Length != poBlock2.Length)
			{
				bEqual = false;
			}
			else if (poBlock1.Length == poBlock2.Length)
			{
				ElementChar[] aElements1 = (ElementChar[])poBlock1.Elements;
				ElementChar[] aElements2 = (ElementChar[])poBlock2.Elements;
				bEqual = true;
				for (long nCount = 0; nCount < aElements1.Length; nCount++)
				{
					if (aElements1[nCount] != aElements2[nCount])
					{
						bEqual = false;
						break;
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
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		public static bool operator ==(BlockOfCharacters poBlock1, BlockOfCharacters poBlock2)
		{
			return AreEqual(poBlock1, poBlock2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(BlockOfCharacters poBlock1, BlockOfCharacters poBlock2)
		{
			return !AreEqual(poBlock1, poBlock2);
		}

		/// <summary>
		/// Equality method.
		/// </summary>
		/// <param name="poElement2">Object to compare against this one.</param>
		/// <returns></returns>
		public override bool Equals(object poElement2)
		{
			return AreEqual(this, poElement2 as BlockOfCharacters);
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