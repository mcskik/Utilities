using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
    public static class StringHelper
    {
        /// <summary>
        /// Replicate the specified character string the specified number of times.
        /// </summary>
        public static string Replicate(string text, int times)
        {
            int ptr = 0;
            string output = string.Empty;
            for (ptr = 1; ptr <= times; ptr++)
            {
                output += text;
            }
            return output;
        }

        /// <summary>
        /// Left pad a string with spaces to the length required.
        /// </summary>
        public static string LPad(string text, int length)
        {
            text = LPad(text, length, " ");
            return text;
        }

        /// <summary>
        /// Left pad a string with the specified character to the length required.
        /// </summary>
        public static string LPad(string text, int length, string character)
        {
            if (character.Length == 0)
            {
                character = " ";
            }
            if (character.Length > 1)
            {
                character = character.Substring(0, 1);
            }
            text = Replicate(character, length) + text.TrimStart();
            text = text.Substring(text.Length - length);
            return text;
        }

        /// <summary>
        /// Right pad a string with spaces to the length required.
        /// </summary>
        public static string RPad(string text, int length)
        {
            text = RPad(text, length, " ");
            return text;
        }

        /// <summary>
        /// Right pad a string with the specified character to the length required.
        /// </summary>
        public static string RPad(string text, int length, string character)
        {
            if (character.Length == 0)
            {
                character = " ";
            }
            if (character.Length > 1)
            {
                character = character.Substring(0, 1);
            }
            text = text.TrimEnd() + Replicate(character, length);
            text = text.Substring(0, length);
            return text;
        }
    }
}