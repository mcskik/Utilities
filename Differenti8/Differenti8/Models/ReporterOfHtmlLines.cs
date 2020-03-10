using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
    /// <summary>
    /// Html reporter to report a section of lines to an html file.
    /// </summary>
    /// <remarks>
    /// Html reporter to report a section of lines to an html file.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ReporterOfHtmlLines : ReporterOfHtml
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
        /// Construct an html reporter to report the specified section of lines.
        /// </summary>
        public ReporterOfHtmlLines(SectionOfLines poSection, Logger poLog)
            : base(poSection, poLog)
        {
            moSection = poSection;
            moBlock = (BlockOfLines)moSection.Data;
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Display a block of line elements.
        /// </summary>
        public override void Display(string identity)
        {
            StringBuilder content = new StringBuilder();
            int textRows = 0;
            if (Elements.Length > 0)
            {
                if (Type == HandoverType)
                {
                    content.Append(FirstLineTokens);
                    content.Append(Environment.NewLine);
                    textRows++;
                }
                else
                {
                    content.Append(FirstLineTokens);
                    content.Append(Environment.NewLine);
                    textRows++;
                }
            }
            for (long nIndex = 1; nIndex < Elements.Length - 1; nIndex++)
            {
                content.Append(Elements[nIndex].ToDisplay());
                content.Append(Environment.NewLine);
                textRows++;
            }
            if (Elements.Length > 1)
            {
                content.Append(LastLineTokens);
                content.Append(Environment.NewLine);
                textRows++;
            }
            OutputHtml(Type, identity, textRows.ToString(), content.ToString());
        }

        /// <summary>
        /// Display a block of line elements ignoring initial indent.
        /// </summary>
        public override void Display(string psPrevIndent, string identity)
        {
            Display(identity);
        }
        #endregion
    }
}