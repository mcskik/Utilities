using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
	/// <summary>
	/// Text reporter to report a section of lines to a text log file.
	/// </summary>
	/// <remarks>
	/// Text reporter to report a section of lines to a text log file.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ReporterOfTextLines : ReporterOfText
	{
		#region Member variables.
		private SectionOfLines moSection = null;
		private BlockOfLines moBlock = null;
		#endregion

		#region Properties.
		/// <summary>
		/// Array of line elements contained in this block of lines.
		/// </summary>
		private ElementLine[] Elements
		{
			get
			{
				return (ElementLine[])moBlock.Elements;
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
				if (Elements.Length > 0)
				{
					sLine = Elements[0].ToDisplay();
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
				if (Elements.Length > 1)
				{
					sLine = Elements[Elements.Length - 1].ToDisplay();
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
		/// Construct a text reporter to report the specified section of lines.
		/// </summary>
		public ReporterOfTextLines(SectionOfLines poSection, Logger poLog) : base(poSection, poLog)
		{
			moSection = poSection;
			moBlock = (BlockOfLines)moSection.Data;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Display a block of line elements.
		/// </summary>
		public override void Display()
		{
			if (Elements.Length > 0)
			{
                if (Type == HandoverType)
                {
                    moLog.WriteLn(this.SubsequentDisplayedTypeMarker + FirstLineTokens);
                }
                else
                {
                    moLog.WriteLn(this.FirstDisplayedTypeMarker + FirstLineTokens);
                }
			}
			for (long nIndex = 1; nIndex < Elements.Length - 1; nIndex++)
			{
				moLog.WriteLn(this.SubsequentDisplayedTypeMarker + Elements[nIndex].ToDisplay());
			}
			if (Elements.Length > 1)
			{
				moLog.WriteLn(this.SubsequentDisplayedTypeMarker + LastLineTokens);
			}
		}

        /// <summary>
        /// Display a block of line elements ignoring initial indent.
        /// </summary>
        public override void Display(string psPrevIndent)
        {
            Display();
        }
        #endregion
	}
}