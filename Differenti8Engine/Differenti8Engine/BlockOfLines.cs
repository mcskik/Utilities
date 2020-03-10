using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Block of line elements.
	/// </summary>
	/// <remarks>
	/// Container for an array of line elements.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class BlockOfLines : Block
	{
		#region Member variables.
		private ElementLine[] maElements = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Array of line elements.
		/// </summary>
		public override Element[] Elements
		{
			get
			{
				return maElements;
			}
			set
			{
				maElements = (ElementLine[])value;
			}
		}

		/// <summary>
		/// Display tokens if the first line is invisible, otherwise just display the data as is.
		/// </summary>
		private string FirstLineTokens
		{
			get
			{
				string sLine = string.Empty;
				string sTokenized = string.Empty;
				bool bInvisible = true;
				if (maElements.Length > 0)
				{
					sLine = maElements[0].ToDisplay();
					char[] aLine = sLine.ToCharArray();
					for (int nIndex = 0; nIndex < aLine.Length; nIndex++)
					{
						string sChar = aLine[nIndex].ToString();
						switch (sChar)
						{
							case " ":
								sTokenized += "[SPACE]";
								break;
							case "\t":
								sTokenized += "[TAB]";
								break;
							case "\r":
								sTokenized += "[CR]";
								break;
							case "\n":
                                sTokenized += String.Format("[LF]{0}", Environment.NewLine);
                                break;
							default:
								bInvisible = false;
								break;
						}
					}
				}
				string sFirstLineTokens = bInvisible ? sTokenized + "[LF]" : sLine;
				return sFirstLineTokens;
			}
		}

		/// <summary>
		/// Display tokens if the last line is invisible, otherwise just display the data as is.
		/// </summary>
		private string LastLineTokens
		{
			get
			{
				string sLine = string.Empty;
				string sTokenized = string.Empty;
				bool bInvisible = true;
				if (maElements.Length > 1)
				{
					sLine = maElements[maElements.Length - 1].ToDisplay();
					char[] aLine = sLine.ToCharArray();
					for (int nIndex = 0; nIndex < aLine.Length; nIndex++)
					{
						string sChar = aLine[nIndex].ToString();
						switch (sChar)
						{
							case " ":
								sTokenized += "[SPACE]";
								break;
							case "\t":
								sTokenized += "[TAB]";
								break;
							case "\r":
								sTokenized += "[CR]";
								break;
							case "\n":
                                sTokenized += String.Format("[LF]{0}", Environment.NewLine);
								break;
							default:
								bInvisible = false;
								break;
						}
					}
				}
				string sLastLineTokens = bInvisible ? sTokenized + "[LF]" : sLine;
				return sLastLineTokens;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Deliberately construct a completely empty block of line elements.
		/// </summary>
		public BlockOfLines()
		{
			maElements = new ElementLine[0];
		}

		/// <summary>
		/// Construct a new empty block of line elements of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of line elements.</param>
		public BlockOfLines(long pnLength)
		{
			maElements = new ElementLine[pnLength];
		}

		/// <summary>
		/// Construct a new block to contain the specified line element array.
		/// </summary>
		/// <param name="paElements">Array of line elements.</param>
		public BlockOfLines(ElementLine[] paElements)
		{
			maElements = paElements;
		}

		/// <summary>
		/// Construct a new block to contain the line element array contained in the specified block.
		/// </summary>
		/// <param name="poBlock">Block of line elements.</param>
		public BlockOfLines(BlockOfLines poBlock)
		{
			maElements = (ElementLine[])poBlock.Elements;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display a block of line elements.
		/// </summary>
		public override void Display()
		{
            //if (maElements.Length > 0)
            //{
            //    //Reporter does all actual writing now, this display is done just to set the correct indentation!
            //    //G.Log.WriteLn(this.FirstDisplayedTypeMarker + FirstLineTokens);
            //}
            //for (long nIndex = 1; nIndex < maElements.Length - 1; nIndex++)
            //{
            //    //Reporter does all actual writing now, this display is done just to set the correct indentation!
            //    //G.Log.WriteLn(this.SubsequentDisplayedTypeMarker + maElements[nIndex].ToDisplay());
            //}
            //if (maElements.Length > 1)
            //{
            //    //Reporter does all actual writing now, this display is done just to set the correct indentation!
            //    //G.Log.WriteLn(this.SubsequentDisplayedTypeMarker + LastLineTokens);
            //}
		}

		/// <summary>
		/// Return the contents of this block as a block of characters.
		/// </summary>
		/// <returns>Block of characters.</returns>
		public override BlockOfCharacters GetCharacters()
		{
			long nLength = 0;
			for (long nLine = 0; nLine < Elements.Length; nLine++)
			{
				ElementLine oLine = new ElementLine(Elements[nLine].Contents.ToString());
				nLength += oLine.Contents.ToString().Length + 1;
			}
			long nIndex=0;
			ElementChar[] aElements = new ElementChar[nLength];
			for (long nLine = 0; nLine < Elements.Length; nLine++)
			{
				ElementLine oLine = new ElementLine(Elements[nLine].Contents.ToString());
				ElementChar oElement = null;
				char[] aContents = oLine.Contents.ToString().ToCharArray();
				for (int nPos = 0; nPos < aContents.Length; nPos++)
				{
					oElement = new ElementChar(aContents[nPos]);
					aElements[nIndex] = oElement;
					nIndex++;
				}
				oElement = new ElementChar('\n');
				aElements[nIndex] = oElement;
				nIndex++;
			}
			BlockOfCharacters oBlock = new BlockOfCharacters(aElements);
			return oBlock;
		}

		#region Allocation methods.
		/// <summary>
		/// Deliberately allocate a completely empty block of lines.
		/// </summary>
		/// <returns>An empty block of lines.</returns>
		public override Block AllocateBlock()
		{
			return new BlockOfLines();
		}

		/// <summary>
		/// Allocate a new empty block of lines of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of lines.</param>
		/// <returns>An empty block of lines of the specified length.</returns>
		public override Block AllocateBlock(long pnLength)
		{
			return new BlockOfLines(pnLength);
		}

		/// <summary>
		/// Allocate a new block containing the specified element line array.
		/// </summary>
		/// <param name="paElements">Array of element lines.</param>
		/// <returns>A new block containing the specified element line array.</returns>
		public override Block AllocateBlock(Element[] paElements)
		{
			return new BlockOfLines((ElementLine[])paElements);
		}

		/// <summary>
		/// Allocate a new block containing the element line array contained in the specified block of lines.
		/// </summary>
		/// <param name="poBlock">Block of lines.</param>
		/// <returns>A new block containing the element line array contained in the specified block of lines.</returns>
		public override Block AllocateBlock(Block poBlock)
		{
			return new BlockOfLines((BlockOfLines)poBlock);
		}
		#endregion
		#endregion

		#region Private methods.
		/// <summary>
		/// Check if both blocks of elements are equal.
		/// </summary>
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool AreEqual(BlockOfLines poBlock1, BlockOfLines poBlock2)
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
				ElementLine[] aElements1 = (ElementLine[])poBlock1.Elements;
				ElementLine[] aElements2 = (ElementLine[])poBlock2.Elements;
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
		public static bool operator ==(BlockOfLines poBlock1, BlockOfLines poBlock2)
		{
			return AreEqual(poBlock1, poBlock2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(BlockOfLines poBlock1, BlockOfLines poBlock2)
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
			return AreEqual(this, poElement2 as BlockOfLines);
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
