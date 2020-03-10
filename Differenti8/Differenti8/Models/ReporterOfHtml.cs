using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
    /// <summary>
    /// Html reporter to report a block of elements to an html file.
    /// </summary>
    /// <remarks>
    /// Html reporter to report a block of elements to an html file.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public abstract class ReporterOfHtml : Reporter
    {
        /// <summary>
        /// Construct an html reporter to report the specified section of elements.
        /// </summary>
        public ReporterOfHtml(Section poSection, Logger poLog)
            : base(poSection, poLog)
        {
        }

        /// <summary>
        /// Display a block of elements making use of the specified unique identity
        /// </summary>
        public abstract void Display(string identity);

        /// <summary>
        /// Display a block of elements with initial indent and making use of the specific unique identity.
        /// </summary>
        public abstract void Display(string psPrevIndent, string identity);

        public void OutputHtml(string type, string identity, string textRows, string content)
        {
            //Set highlight colours.
            string changeTitle = string.Empty;
            string brColor = string.Empty;
            string bgColor = string.Empty;
            switch (type.Substring(0, 1))
            {
                case "M":
                    changeTitle = "Matched";
                    brColor = "lightyellow";
                    bgColor = "lightyellow";
                    break;
                case "A":
                    changeTitle = "Inserted";
                    brColor = "lightblue";
                    bgColor = "lightblue";
                    break;
                case "B":
                    changeTitle = "Deleted";
                    brColor = "pink";
                    bgColor = "pink";
                    break;
                default:
                    changeTitle = "Unknown";
                    brColor = "white";
                    bgColor = "white";
                    break;

            }
            string textArea = String.Format("<textarea id='txtIdentity{0}' name='txtContent{0}' rows={1} cols=100 style='border:thin solid {2};background-color:{3};width:100%;text-align:left;overflow:hidden' onMouseOver='Cell_OnMouseOver(this)' onMouseOut='Cell_OnMouseOut(this)' title='{4}'>", identity, textRows, brColor, bgColor, changeTitle);
            moLog.WriteLn(textArea);
            moLog.WriteLn(content);
            moLog.WriteLn("</textarea>");
        }

        /// <summary>
        /// Determine the number of rows of text in the specified text content.
        /// </summary>
        public long TextRows(string content)
        {
            long row = 0;
            int pos = 0;
            string text = content;
            if (text.Length > 0)
            {
                do
                {
                    row++;
                    pos = text.IndexOf('\n');
                    if (pos > -1)
                    {
                        pos = pos + 2;
                        if (pos < text.Length)
                        {
                            text = text.Substring(pos);
                        }
                        else
                        {
                            text = string.Empty;
                        }
                    }
                } while (pos != -1);
            }
            return row;
        }
    }
}