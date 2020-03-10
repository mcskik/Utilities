using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Notepod.Models
{
    /// <summary>
    /// XML attributes spliiter class.
    /// </summary>
    /// <remarks>
    /// Splits all attributes found within an XML tag into separate lines.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class XmlAttributesSplitter
    {
        private const char EQUALS = '=';
        private const char QUOTE = '"';
        private const char COMMA = ',';
        private const char SPACE = ' ';
        private const char CR = '\r';
        private const char LF = '\n';

        public static string Join(List<string> collection, bool firstTag)
        {
            StringBuilder lines = new StringBuilder();
            string statement = string.Empty;
            int bits = 0;
            foreach (var entry in collection)
            {
                string part = entry.Trim();
                if (part.Length > 0)
                {
                    bits++;
                    if (part.StartsWith("<"))
                    {
                        if (firstTag)
                        {
                            statement += part + " ";
                            bits = 0;
                        }
                        else
                        {
                            lines.AppendLine(part);
                            bits = 0;
                        }
                    }
                    else if (part == ">")
                    {
                        lines.Append(part);
                        bits = 0;
                    }
                    else if (part == "/>")
                    {
                        lines.Append(part);
                        bits = 0;
                    }
                    else
                    {
                        //If you want a space either side of the equals sign use the commented out line.
                        //statement += " " + part;
                        statement += part;
                        if (bits == 3)
                        {
                            statement = statement.Trim();
                            lines.AppendLine(statement);
                            statement = string.Empty;
                            bits = 0;
                        }
                    }
                }
            }
            string result = lines.ToString();
            return result;
        }

        public static List<string> Split(string tagText)
        {
            List<string> equalsSplit = Split(tagText, EQUALS);
            List<string> spaceSplit = Split(equalsSplit, SPACE);
            List<string> crSplit = Split(spaceSplit, CR);
            List<string> lfSplit = Split(crSplit, LF);
            return lfSplit;
        }

        public static List<string> Split(List<string> collection, char splitChar)
        {
            List<string> bundle = new List<string>();
            foreach (var entry in collection)
            {
                var list = Split(entry, splitChar);
                if (list.Count > 0)
                {
                    bundle.AddRange(list);
                }
                else
                {
                    bundle.Add(entry);
                }
            }
            return bundle;
        }

        public static List<string> Split(string tagText, char splitChar)
        {
            List<string> fullSplit = new List<string>();
            string part = string.Empty;
            string carry = string.Empty;
            string word = string.Empty;
            char letter;
            string[] basicSplit = tagText.Split(splitChar);
            for (int index = 0; index < basicSplit.Length; index++)
            {
                part = basicSplit[index];
                if (carry.Length > 0)
                {
                    word = carry + splitChar + part;
                }
                else
                {
                    word = part;
                }
                int ptr = 0;
                int quoteCount = 0;
                char[] wordArray = word.ToCharArray();
                for (ptr = 0; ptr < wordArray.Length; ptr++)
                {
                    letter = wordArray[ptr];
                    if (letter == QUOTE)
                    {
                        quoteCount++;
                    }
                }
                if (quoteCount % 2 == 0)
                {
                    if (quoteCount > 0)
                    {
                        word = word.Trim();
                        if (word.Length > 1)
                        {
                            //This is about removing quotes from around a quoted string.
                            //word = word.Substring(1);
                            //word = word.Substring(0, word.Length - 1);
                            //word = UnNestQuotes(word);
                        }
                    }
                    fullSplit.Add(word);
                    if (splitChar != '\r' & splitChar != '\n')
                    {
                        fullSplit.Add(splitChar.ToString());
                    }
                    carry = string.Empty;
                }
                else
                {
                    if (carry.Length > 0)
                    {
                        carry += splitChar + part;
                    }
                    else
                    {
                        carry = part;
                    }
                }
            }
            return fullSplit;
        }

        public static string UnNestQuotes(string text)
        {
            string quote = QUOTE.ToString();
            return text.Replace(quote + quote, quote);
        }
    }
}