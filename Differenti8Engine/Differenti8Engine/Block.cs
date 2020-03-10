using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Block of elements base class.
	/// </summary>
	/// <remarks>
	/// Container for an array of elements.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Block
	{
		#region Member variables.
		private string msType = string.Empty;
		private Element[] maElements = null;
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
		/// First displayed type marker.
		/// </summary>
		public string FirstDisplayedTypeMarker
		{
			get
			{
				string sMarker = String.Empty;
				switch (msType.Substring(0, 1))
				{
					case "M":
						sMarker = "M~= ";
						break;
					case "A":
						sMarker = "I~+ ";
						break;
					case "B":
						sMarker = "D~- ";
						break;
					case "X":
						sMarker = "X~? ";
						break;
					case "Z":
						sMarker = "Z~? ";
						break;
				}
				return sMarker;
			}
		}

		/// <summary>
		/// Subsequent displayed type marker.
		/// </summary>
		public string SubsequentDisplayedTypeMarker
		{
			get
			{
				string sMarker = String.Empty;
				switch (msType.Substring(0, 1))
				{
					case "M":
						sMarker = "  = ";
						break;
					case "A":
						sMarker = "  + ";
						break;
					case "B":
						sMarker = "  - ";
						break;
					case "X":
						sMarker = "  ? ";
						break;
					case "Z":
						sMarker = "  ? ";
						break;
				}
				return sMarker;
			}
		}

		/// <summary>
		/// Array of elements.
		/// </summary>
		public virtual Element[] Elements
		{
			get
			{
				return maElements;
			}
			set
			{
				maElements = value;
			}
		}

		/// <summary>
		/// Length of element array.
		/// </summary>
		public long Length
		{
			get
			{
				return Elements.Length;
			}
		}

		/// <summary>
		/// Total character length of contents.
		/// </summary>
		public virtual long ContentsLength
		{
			get
			{
				long contentsLength = 0;
				foreach (Element element in Elements)
				{
					contentsLength += element.Contents.ToString().Length;
				}
				return contentsLength;
			}
		}

		/// <summary>
		/// Is block empty?
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return Elements.Length == 0;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Deliberately construct a completely empty block of elements.
		/// </summary>
		public Block()
		{
			maElements = new Element[0];
		}

		/// <summary>
		/// Construct a new empty block of elements of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of elements.</param>
		public Block(long pnLength)
		{
			maElements = new Element[pnLength];
		}

		/// <summary>
		/// Construct a new block to contain the specified element array.
		/// </summary>
		/// <param name="paElements">Array of elements.</param>
		public Block(Element[] paElements)
		{
			maElements = paElements;
		}

		/// <summary>
		/// Construct a new block to contain the element array contained in the specified block.
		/// </summary>
		/// <param name="poBlock">Block of elements.</param>
		public Block(Block poBlock)
		{
			maElements = poBlock.Elements;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display a block of elements.
		/// </summary>
		public virtual void Display()
		{
			if (maElements.Length > 0)
			{
				//Reporter does all actual writing now, this display is done just to set the correct indentation!
				//G.Log.WriteLn(this.FirstDisplayedTypeMarker + maElements[0].ToDisplay());
			}
			for (long nIndex = 1; nIndex < maElements.Length; nIndex++)
			{
				//Reporter does all actual writing now, this display is done just to set the correct indentation!
				//G.Log.WriteLn(this.SubsequentDisplayedTypeMarker + maElements[nIndex].ToDisplay());
			}
		}

		/// <summary>
		/// Return the contents of this block as a block of characters.
		/// </summary>
		/// <returns>Block of characters.</returns>
		public virtual BlockOfCharacters GetCharacters()
		{
			long nLength = 0;
			for (long nLine = 0; nLine < Elements.Length; nLine++)
			{
				ElementLine oLine = new ElementLine(Elements[nLine].Contents.ToString());
				nLength += oLine.Contents.ToString().Length + 1;
			}
			long nIndex = 0;
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
			}
			BlockOfCharacters oBlock = new BlockOfCharacters(aElements);
			return oBlock;
		}

		/// <summary>
		/// Extract a block of elements from within this block.
		/// </summary>
		/// <remarks>
		/// Because no length is given, the extracted block begins at the
		/// start position and proceeds until the end of this block.
		/// </remarks>
		/// <param name="pnPosition">Starting position.</param>
		/// <returns>A block of elements.</returns>
		public Block Extract(long pnPosition)
		{
			long nPositionIndex = pnPosition - 1;
			long nLength = this.Length - nPositionIndex;
			return Extract(pnPosition, nLength);
		}

		/// <summary>
		/// Extract a block of elements from within this block.
		/// </summary>
		/// <remarks>
		/// Each individual element is a reference copy not a deep data copy.
		/// </remarks>
		/// <param name="pnPosition">Starting position.</param>
		/// <param name="pnLength">Number of elements to extract.</param>
		/// <returns>A block of elements of the specified length.</returns>
		public Block Extract(long pnPosition, long pnLength)
		{
			long nPositionIndex = pnPosition - 1;
			long nLength = this.Length - nPositionIndex;
			if (pnLength > nLength)
			{
				pnLength = nLength;
			}
			Block oBlock = AllocateBlock(pnLength);
			for (long nIndex = 0; nIndex < pnLength; nIndex++)
			{
				oBlock.Elements[nIndex] = this.Elements[nPositionIndex + nIndex];
			}
			return oBlock;
		}

		/// <summary>
		/// Find the position of the first element in this block,
		/// where the block to find was first found, and matched for its entire length.
		/// </summary>
		/// <param name="poBlockToFind">Contiguous block of elements to find.</param>
		/// <returns>Element position in this block where found or zero if not found.</returns>
		public long Find(Block poBlockToFind)
		{
			long nFoundPos = 0;
			if (this.Length == 0)
			{
				nFoundPos = 0;
			}
			else if (poBlockToFind.Length == 0)
			{
				nFoundPos = 0;
			}
			else if (poBlockToFind.Length > this.Length)
			{
				nFoundPos = 0;
			}
			else if (poBlockToFind.Length == this.Length)
			{
				if (this == poBlockToFind)
				{
					nFoundPos = 1;
				}
				else
				{
					nFoundPos = 0;
				}
			}
			else
			{
				for (long nStartPos = 1; nStartPos <= this.Length - poBlockToFind.Length + 1; nStartPos++)
				{
					long nStartIndex = nStartPos - 1;
					nFoundPos = 0;
					bool bMatched = true;
					for (long nPos = 1; nPos <= poBlockToFind.Length; nPos++)
					{
						long nIndex = nPos - 1;
						long nElementIndex = nStartIndex + nIndex;
						if (poBlockToFind.Elements[nIndex] != this.Elements[nElementIndex])
						{
							bMatched = false;
							break;
						}
					}
					if (bMatched)
					{
						nFoundPos = nStartPos;
						break;
					}
				}
			}
			return nFoundPos;
		}

		/// <summary>
		/// Build and return a signature block representing this element block.
		/// </summary>
		/// <remarks>
		/// This is primarily intended for use on blocks of element lines to reduce
		/// them to blocks of element signatures and thereby increase performance
		/// when comparing blocks of lines from a text file for instance.
		/// </remarks>
		/// <returns></returns>
		public Block Signature()
		{
			long nLength = this.Length;
			Block oBlock = new Block(nLength);
			for (long nIndex = 0; nIndex < nLength; nIndex++)
			{
				Signature oSignature = maElements[nIndex].Signature();
				ElementSignature oElementSignature = new ElementSignature(oSignature);
				oBlock.Elements[nIndex] = oElementSignature;
			}
			return oBlock;
		}

		#region Allocation methods.
		/// <summary>
		/// Deliberately allocate a completely empty block of elements.
		/// </summary>
		/// <returns>An empty block of elements.</returns>
		public virtual Block AllocateBlock()
		{
			return new Block();
		}

		/// <summary>
		/// Allocate a new empty block of elements of the specified length.
		/// </summary>
		/// <param name="pnLength">Length of new empty block of elements.</param>
		/// <returns>An empty block of elements of the specified length.</returns>
		public virtual Block AllocateBlock(long pnLength)
		{
			return new Block(pnLength);
		}

		/// <summary>
		/// Allocate a new block containing the specified element array.
		/// </summary>
		// <param name="paElements">Array of elements.</param>
		/// <returns>A new block containing the specified element array.</returns>
		public virtual Block AllocateBlock(Element[] paElements)
		{
			return new Block(paElements);
		}

		/// <summary>
		/// Allocate a new block containing the element array contained in the specified block.
		/// </summary>
		/// <param name="poBlock">Block of elements.</param>
		/// <returns>A new block containing the element array contained in the specified block.</returns>
		public virtual Block AllocateBlock(Block poBlock)
		{
			return new Block(poBlock);
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
		private static bool AreEqual(Block poBlock1, Block poBlock2)
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
				Element[] aElements1 = poBlock1.Elements;
				Element[] aElements2 = poBlock2.Elements;
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
		public static bool operator ==(Block poBlock1, Block poBlock2)
		{
			return AreEqual(poBlock1, poBlock2);
		}

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="poBlock1">Left hand side operand block to compare.</param>
		/// <param name="poBlock2">Right hand side operand block to compare.</param>
		/// <returns>True if not equal, false otherwise.</returns>
		public static bool operator !=(Block poBlock1, Block poBlock2)
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
			return AreEqual(this, poElement2 as Block);
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
