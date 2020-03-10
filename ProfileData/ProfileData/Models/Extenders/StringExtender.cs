using System.Collections.Generic;

namespace ProfileData.Models.Extenders
{
    /// <summary>
    /// String extender class.
    /// </summary>
    /// <remarks>
    /// Adds useful string manipulation methods to the string class.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class StringExtender
    {
        public enum StripMode
        {
            Both = 1,
            Leading = 2,
            Trailing = 3
        }

        /// <summary>
        /// Remove all leading, trailing, or both, occurences of the specified char.
        /// </summary>
        public static string Strip(this string input, StripMode mode, string character)
        {
            int ptr = 0;
            string output = string.Empty;
            //Parameter validation.
            if (input.Length == 0)
            {
                return input;
            }
            switch (mode)
            {
                case StripMode.Both:
                    break;
                case StripMode.Leading:
                    break;
                case StripMode.Trailing:
                    break;
                default:
                    return input;
            }
            if (character.Length != 1)
            {
                return input;
            }
            //Main processing.
            if (mode == StripMode.Leading || mode == StripMode.Both)
            {
                for (ptr = 0; ptr < input.Length; ptr++)
                {
                    if (input.Substring(ptr, 1) != character)
                    {
                        output = input.Substring(ptr);
                        break;
                    }
                }
                input = output;
            }
            if (mode == StripMode.Trailing || mode == StripMode.Both)
            {
                for (ptr = input.Length - 1; ptr >= 0; ptr--)
                {
                    if (input.Substring(ptr, 1) != character)
                    {
                        output = input.Substring(0, ptr + 1);
                        break;
                    }
                }
            }
            return output;
        }

        /// <remarks>
        /// The standard string split method works on arrays of characters or individual characters but seemingly not on strings with a length greater than 1.
        /// </remarks>
        public static List<string> Split(this string text, string separator)
        {
            List<string> parts = new List<string>();
            int pos = 0;
            do
            {
                pos = text.IndexOf(separator, pos + 1);
                if (pos > -1)
                {
                    string part = text.Substring(0, pos);
                    parts.Add(part);
                }
            } while (pos >= 0);
            return parts;
        }

        public static string CoalesceNullToEmpty(this string input)
        {
            return input != null ? input : string.Empty;
        }

        /// <summary>
        /// Replicate the specified character string the specified number of times.
        /// </summary>
        public static string Replicate(this string input, int times)
        {
            int ptr = 0;
            string output = string.Empty;
            for (ptr = 1; ptr <= times; ptr++)
            {
                output += input;
            }
            return output;
        }

        /// <summary>
        /// Left pad a string with spaces to the length required.
        /// </summary>
        public static string LPad(this string input, int length)
        {
            input = input.LPad(length, " ");
            return input;
        }

        /// <summary>
        /// Left pad a string with the specified character to the length required.
        /// </summary>
        public static string LPad(this string input, int length, string padChar)
        {
            if (padChar.Length == 0)
            {
                padChar = " ";
            }
            if (padChar.Length > 1)
            {
                padChar = padChar.Substring(0, 1);
            }
            input = padChar.Replicate(length) + input.TrimStart();
            input = input.Substring(input.Length - length);
            return input;
        }

        /// <summary>
        /// Right pad a string with spaces to the length required.
        /// </summary>
        public static string RPad(this string input, int length)
        {
            input = input.RPad(length, " ");
            return input;
        }

        /// <summary>
        /// Right pad a string with the specified character to the length required.
        /// </summary>
        public static string RPad(this string input, int length, string padChar)
        {
            if (padChar.Length == 0)
            {
                padChar = " ";
            }
            if (padChar.Length > 1)
            {
                padChar = padChar.Substring(0, 1);
            }
            input = input.TrimEnd() + padChar.Replicate(length);
            input = input.Substring(0, length);
            return input;
        }

        /// <summary>
        /// Return the right most number of characters requested from the input string.
        /// </summary>
        public static string RightMost(this string input, int number)
        {
            string output = string.Empty;
            int pos = 0;
            if (input.Length < number)
            {
                number = input.Length;
            }
            if (input.Length == 0)
            {
                output = string.Empty;
            }
            else if (number == 0)
            {
                output = string.Empty;
            }
            else
            {
                pos = input.Length - number;
                output = input.Substring(pos);
            }
            return output;
        }
    }
}