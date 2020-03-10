using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
	/// <summary>
	/// Html reporter to report a block of characters to an html file.
	/// </summary>
	/// <remarks>
    /// Html reporter to report a block of characters to an html file.
    /// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ReporterOfHtmlCharacters : ReporterOfHtml
	{
		#region Member variables.
		private SectionOfCharacters moSection = null;
		private BlockOfCharacters moBlock = null;
		private string msIndent = string.Empty;
		#endregion

		#region Properties.
		/// <summary>
		/// Array of character elements contained in this block of characters.
		/// </summary>
		private ElementChar[] Elements
		{
			get
			{
				return (ElementChar[])moBlock.Elements;
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
				if (Elements.Length > 0)
				{
					if (Elements[Elements.Length - 1].ToDisplay() == "\n")
					{
						bEndsWithNewline = true;
					}
				}
				return bEndsWithNewline;
			}
		}

        /// <summary>
        /// Display tokens if the first or last lines are invisible, otherwise just display the data as is.
        /// </summary>
        public virtual string DisplayTokens
        {
            get
            {
                //Find end of first (or only) line.
                int nFirstLineEnd = Elements.Length - 1;
                for (int nIndex = 0; nIndex < Elements.Length; nIndex++)
                {
                    if (Elements[nIndex].ToDisplay() == "\n")
                    {
                        nFirstLineEnd = nIndex;
                        break;
                    }
                }
                //Find start of last (or only) line.
                int nLastLineStart = 0;
                for (int nIndex = Elements.Length - 1; nIndex >= 0; nIndex--)
                {
                    if (Elements[nIndex].ToDisplay() == "\n")
                    {
                        if (nIndex == 0)
                        {
                            nLastLineStart = nIndex;
                        }
                        else if (nIndex == Elements.Length)
                        {
                        }
                        else
                        {
                            nLastLineStart = nIndex + 1;
                            break;
                        }
                    }
                }
                //Tokenize the first and last lines if they are comprised solely of non-visible characters.
                string sLeader = string.Empty;
                string sMiddle = string.Empty;
                string sTrailer = string.Empty;
                string sTokenizedLeader = string.Empty;
                string sTokenizedTrailer = string.Empty;
                bool bFirstInvisible = true;
                bool bLastInvisible = true;
                for (int nIndex = 0; nIndex < Elements.Length; nIndex++)
                {
                    string sChar = Elements[nIndex].ToDisplay();
                    if (nIndex <= nFirstLineEnd)
                    {
                        sLeader += sChar;
                        switch (sChar)
                        {
                            case " ":
                                sTokenizedLeader += "[SPACE]";
                                break;
                            case "\t":
                                sTokenizedLeader += "[TAB]";
                                break;
                            case "\r":
                                sTokenizedLeader += "[CR]";
                                break;
                            case "\n":
                                sTokenizedLeader += String.Format("[LF]{0}", Environment.NewLine);
                                break;
                            default:
                                bFirstInvisible = false;
                                break;
                        }
                    }
                    else if (nIndex < nLastLineStart)
                    {
                        sMiddle += sChar;
                    }
                    else
                    {
                        sTrailer += sChar;
                        switch (sChar)
                        {
                            case " ":
                                sTokenizedTrailer += "[SPACE]";
                                break;
                            case "\t":
                                sTokenizedTrailer += "[TAB]";
                                break;
                            case "\r":
                                sTokenizedTrailer += "[CR]";
                                break;
                            case "\n":
                                sTokenizedTrailer += String.Format("[LF]{0}", Environment.NewLine);
                                break;
                            default:
                                bLastInvisible = false;
                                break;
                        }
                    }
                }
                string sDisplayTokens = string.Empty;
                sDisplayTokens = bFirstInvisible ? sTokenizedLeader : sLeader;
                sDisplayTokens += sMiddle;
                sDisplayTokens += bLastInvisible ? sTokenizedTrailer : sTrailer;
                return sDisplayTokens;
            }
        }
		#endregion

		#region Constructors.
		/// <summary>
		/// Construct an html reporter to report the specified section of characters.
		/// </summary>
		public ReporterOfHtmlCharacters(SectionOfCharacters poSection, Logger poLog) : base(poSection, poLog)
		{
			moSection = poSection;
			moBlock = (BlockOfCharacters)moSection.Data;
		}
		#endregion

		#region Public methods.
        /// <summary>
        /// Display a block of character elements with no initial indent.
        /// </summary>
        public override void Display(string identity)
        {
            Display(string.Empty, identity);
        }

        /// <summary>
		/// Display a block of character elements with initial indent.
		/// </summary>
		/// <param name="psPrevIndent">Previous indentation string.</param>
		public override void Display(string psPrevIndent, string identity)
		{
            StringBuilder displayBuilder = new StringBuilder();
			char[] aDisplayTokens = DisplayTokens.ToCharArray();
			if (aDisplayTokens.Length > 0)
			{
                if (Type == HandoverType)
                {
                    displayBuilder.Append(psPrevIndent + aDisplayTokens[0].ToString());
                }
                else
                {
                    displayBuilder.Append(psPrevIndent + aDisplayTokens[0].ToString());
                }
			}
            for (long nIndex = 1; nIndex < aDisplayTokens.Length; nIndex++)
			{
                displayBuilder.Append(aDisplayTokens[nIndex].ToString());
				if (aDisplayTokens[nIndex] == '\n')
				{
					if (nIndex < aDisplayTokens.Length - 1)
					{
                        //displayBuilder.Append(this.SubsequentDisplayedTypeMarker);
					}
				}
			}
            string content = displayBuilder.ToString();
            if (EndsWithNewline)
			{
                OutputHtml(Type, identity, TextRows(content).ToString(), content);
			}
			else
			{
                OutputHtml(Type, identity, TextRows(content).ToString(), content);
			}
		}
		#endregion
	}
}