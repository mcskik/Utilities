using Differenti8.DataLayer.Profile;
using Differenti8Engine;
using Log8;
using System;
using System.Text;

namespace Differenti8.Models
{
    /// <summary>
    /// Text reporter to report a block of characters to a text log file.
    /// </summary>
    /// <remarks>
    /// Text reporter to report a block of characters to a text log file.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ReporterOfTextCharacters : ReporterOfText
    {
        #region Constants.
        private const string NOTEPAD = "Notepad.exe";
        #endregion

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
        /// True if this block of characters ends with a Windows CRLF carriage-return + line-feed sequence.
        /// </summary>
        private bool EndsWithCrLf
        {
            get
            {
                bool endsWithCrLf = false;
                if (EndsWithLf)
                {
                    if (Elements.Length > 1)
                    {
                        if (Elements[Elements.Length - 2].ToDisplay() == "\r")
                        {
                            endsWithCrLf = true;
                        }
                    }
                }
                return endsWithCrLf;
            }
        }

        /// <summary>
        /// True if this block of characters ends with a unix LF line-feed newline character.
        /// </summary>
        private bool EndsWithLf
        {
            get
            {
                bool endsWithLf = false;
                if (Elements.Length > 0)
                {
                    if (Elements[Elements.Length - 1].ToDisplay() == "\n")
                    {
                        endsWithLf = true;
                    }
                }
                return endsWithLf;
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
        /// Construct a text reporter to report the specified section of characters.
        /// </summary>
        public ReporterOfTextCharacters(SectionOfCharacters poSection, Logger poLog) : base(poSection, poLog)
        {
            moSection = poSection;
            moBlock = (BlockOfCharacters)moSection.Data;
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
        public override void Display(string psPrevIndent)
        {
            bool usingNotepad = Administrator.ProfileManager.SystemProfile.ViewerUnix.Contains(NOTEPAD);
            StringBuilder displayBuilder = new StringBuilder();
            char[] aDisplayTokens = DisplayTokens.ToCharArray();
            if (aDisplayTokens.Length > 0)
            {
                if (Type == HandoverType)
                {
                    displayBuilder.Append(this.SubsequentDisplayedTypeMarker + psPrevIndent + aDisplayTokens[0].ToString());
                }
                else
                {
                    displayBuilder.Append(this.FirstDisplayedTypeMarker + psPrevIndent + aDisplayTokens[0].ToString());
                }
            }
            string prevLetter = string.Empty;
            for (long nIndex = 1; nIndex < aDisplayTokens.Length; nIndex++)
            {
                //TODO: Annoying (Notepad) bug where the insert/delete markers appear on the same line in Notepad.exe becaue Notepad.exe doesn't recognise unix \n end-of-line markers.
                //TODO: D~-  sample"I~+  anothersample"M~=   android:installLocation="preferExternal">
                //TODO:
                //TODO: I have discovered the bug where unix files (Android files which use unix \n LF line feed instead of Windows \r\n CRLF Carriage return line feed.
                //TODO: With a Windows CRLF, Notepad.exe will recognise it as a new line and C# StringBuilder.Append() appends CRLF onto the end of the string.
                //TODO: Therefore if the character array already ends with a CRLF, it has to be suppressed, that is why we have the EndsWithNewline chack
                //TODO: and we use Write() instead of WriteLn() to suppress the extra new line.
                //TODO: However with a unix LF, Notepad.exe will not recognise it as a new line even although C# StringBuilder.Append() does append \n on to the end.
                //TODO: So if you cut and paste the line from Notepad.exe into Wordpad.exe you will see it appear on separate lines.
                //TODO: Therefore I either have to use Wordpad as the text file viewer which recognises \n as a new line,
                //TODO: or I would have to write code to treat Windows files with CRLF different from unix files with LF at the end of each line of text.
                //TODO: This code would have to detect and differentiate between lines ending with CRLF from those ending with just LF, and suppress the CRLF's but not the LF's.
                //TODO: For now I intend to use Wordpad as the text file viewer as it is the easiest solution to the problem.
                //TODO: I may later write code to differentiate between lines ending with CRLF and those ending with just LF.
                string letter = aDisplayTokens[nIndex].ToString();
                if (letter == "\n" && prevLetter != "\r")
                {
                    if (usingNotepad)
                    {
                        //TODO: This is the code I used to discover the difference between Notepad.exe's handling of CRLF and LF and how the (Notepad) bug occurred.
                        letter = Environment.NewLine;
                    }
                }
                prevLetter = letter;
                displayBuilder.Append(letter);
                if (aDisplayTokens[nIndex] == '\n')
                {
                    if (nIndex < aDisplayTokens.Length - 1)
                    {
                        displayBuilder.Append(this.SubsequentDisplayedTypeMarker);
                    }
                }
            }
            //if (usingNotepad)
            //{
            //    if (EndsWithCrLf)
            //    {
            //        moLog.Write(displayBuilder.ToString());
            //    }
            //    else
            //    {
            //        moLog.WriteLn(displayBuilder.ToString());
            //    }
            //}
            //else
            //{
            //    if (EndsWithLf)
            //    {
            //        moLog.Write(displayBuilder.ToString());
            //    }
            //    else
            //    {
            //        moLog.WriteLn(displayBuilder.ToString());
            //    }
            //}
            if (EndsWithLf)
            {
                moLog.Write(displayBuilder.ToString());
            }
            else
            {
                moLog.WriteLn(displayBuilder.ToString());
            }
        }
        #endregion
    }
}