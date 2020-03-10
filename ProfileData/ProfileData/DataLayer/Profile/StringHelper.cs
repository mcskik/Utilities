using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProfileData.DataLayer.Profile
{
    /// <summary>
    /// String helper class.
    /// </summary>
    /// <remarks>
    /// Contains methods which are commonly used to manipulate strings but which are not directly available in the .Net framework.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class StringHelper
    {
        /// <remarks>
        /// The standard string split method works on arrays of characters or individual characters but seemingly not on strings with a length greater than 1.
        /// </remarks>
        public static List<string> Split(string text, string separator)
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
    }
}