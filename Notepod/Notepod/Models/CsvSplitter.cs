using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Notepod.Models
{
    /// <summary>
    /// CSV spliiter class.
    /// </summary>
    /// <remarks>
    /// This enables a CSV line to be split taking into account the presence, abscence, or nesting of quotation marks,
    /// including the presence or abscence of commas within quotation marks.  A normal split would be thrown off by
    /// the presence of commas within quotation marks which should be ignored for splitting purposes.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class CsvSplitter
    {
        private const char QUOTE = '"';
        private const char COMMA = ',';

        public static List<string> Split(string csvText)
        {
            List<string> fullSplit = new List<string>();
            string part = string.Empty;
            string carry = string.Empty;
            string word = string.Empty;
            char letter;
            string[] basicSplit = csvText.Split(COMMA);
            for (int index = 0; index < basicSplit.Length; index++)
            {
                part = basicSplit[index];
                if (carry.Length > 0)
                {
                    word = carry + COMMA + part;
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
                            word = word.Substring(1);
                            word = word.Substring(0, word.Length - 1);
                            word = UnNestQuotes(word);
                        }
                    }
                    fullSplit.Add(word);
                    carry = string.Empty;
                }
                else
                {
                    if (carry.Length > 0)
                    {
                        carry += COMMA + part;
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