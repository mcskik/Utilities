using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Notepod.Models
{
    /// <summary>
    /// JSON Unformatter Class.
    /// </summary>
    /// <remarks>
    /// Used to unformat a JSON string in a readable format back into a continuous string on one line by removing
    /// all presentational white space characters leaving only single spaces in place of all tabs and newlines.
    /// Any text inside quotes will not be affected.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class JsonUnformatter
    {
        private const int INDENT_SIZE = 4;

        public static string Unformat(string str)
        {
            int spaceCount = 0;
            StringBuilder json = new StringBuilder();
            str = (str ?? "").Replace("{}", @"\{\}").Replace("[]", @"\[\]");
            bool quoted = false;
            bool escape = false;
            int length = str.Length;
            for (int index = 0; index < length; index++)
            {
                var chr = str[index];
                if (chr == ' ')
                {
                    spaceCount++;
                }
                else
                {
                    spaceCount = 0;
                }
                if (escape || quoted)
                {
                    //Any character includding any number of spaces is allowed inside quotes.
                    json.Append(chr);
                }
                else
                {
                    switch (chr)
                    {
                        case '\t':
                        case '\r':
                        case '\n':
                            break;
                        case ' ':
                            //With the folling code commented out no spaces are allowed outside of quotes.
                            //Uncomment to allow only one space betwen anything unless inside quotes.
                            //if (spaceCount == 1)
                            //{
                            //   json.Append(chr);
                            //}
                            break;
                        default:
                            json.Append(chr);
                            break;
                    }
                }
                quoted = (chr == '"') ? !quoted : quoted;
                escape = (chr == '\\') ? !escape : false;
            }
            str = json.ToString();
            return str.Replace(@"\{\}", "{}").Replace(@"\[\]", "[]");
        }
    }
}