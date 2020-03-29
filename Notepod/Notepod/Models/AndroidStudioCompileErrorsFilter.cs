using System.Collections.Generic;
using System.IO;

namespace Notepod.Models
{
    /// <summary>
    /// Android Studio compile errors filter class.
    /// </summary>
    /// <remarks>
    /// Used to filter out all the binding errors which make it difficult to spot the real errors.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class AndroidStudioCompileErrorsFilter
    {
        private const int INDENT_SIZE = 4;

        #region Primary.
        private const string BINDING_ERROR_START = @"error: cannot find symbol";
        private const string BINDING_ERROR_CONTAINS = @"Binding";
        private const string BINDING_BR_ERROR = @"error: cannot find symbol class BR";
        private const string BINDING_ERROR_PACKAGE = @"error: package";
        private const string BINDING_ERROR_DATABINDING = @"databinding";
        #endregion

        #region Secondary.
        private const string PARCELER_ERROR = @"Parceler:";
        private const string PARCELER_REFLECTION_ACCESS = @"Reflection is required to access";
        private const string PARCELER_REFLECTION_MODIFY = @"Reflection is required to modify";
        private const string WARNING = @"Warning:";
        #endregion

        public static string[] Filter(string[] lines)
        {
            bool anyHits = false;
            string[] filteredLines = FilterPrimary(lines, ref anyHits);
            if (!anyHits)
            {
                filteredLines = FilterSecondary(filteredLines, ref anyHits);
            }
            if (!anyHits)
            {
                filteredLines = FilterTertiary(filteredLines, ref anyHits);
            }
            return filteredLines;
        }

        private static string[] FilterPrimary(string[] lines, ref bool anyHits)
        {
            anyHits = false;
            List<string> filteredLines = new List<string>();
            foreach (var line in lines)
            {
                bool exclude = false;
                if (line.Contains(BINDING_ERROR_CONTAINS))
                {
                    exclude = true;
                }
                if (line.Contains(BINDING_ERROR_START) && line.Contains(BINDING_ERROR_CONTAINS))
                {
                    exclude = true;
                }
                if (line.Contains(BINDING_ERROR_PACKAGE) && line.Contains(BINDING_ERROR_DATABINDING))
                {
                    exclude = true;
                }
                if (line.Contains(BINDING_BR_ERROR))
                {
                    exclude = true;
                }
                if (exclude)
                {
                    anyHits = true;
                }
                else
                {
                    filteredLines.Add(line);
                }
            }
            return filteredLines.ToArray();
        }

        private static string[] FilterSecondary(string[] lines, ref bool anyHits)
        {
            anyHits = false;
            List<string> filteredLines = new List<string>();
            foreach (var line in lines)
            {
                bool exclude = false;
                if (line.Contains(PARCELER_ERROR) && line.Contains(PARCELER_REFLECTION_ACCESS))
                {
                    exclude = true;
                }
                if (line.Contains(PARCELER_ERROR) && line.Contains(PARCELER_REFLECTION_MODIFY))
                {
                    exclude = true;
                }
                if (line.StartsWith(WARNING))
                {
                    exclude = true;
                }
                if (exclude)
                {
                    anyHits = true;
                }
                else
                {
                    filteredLines.Add(line);
                }
            }
            return filteredLines.ToArray();
        }

        private static string[] FilterTertiary(string[] lines, ref bool anyHits)
        {
            anyHits = false;
            List<string> reversedLines = new List<string>();
            foreach (var line in lines)
            {
                reversedLines.Add(line);
            }
            reversedLines.Reverse();
            List<string> filteredLines = new List<string>();
            string previouslyProcessedRecord = string.Empty;
            foreach (var line in reversedLines)
            {
                bool exclude = false;
                string record = line.Trim();
                if (File.Exists(record))
                {
                    if (File.Exists(previouslyProcessedRecord))
                    {
                        exclude = true;
                    }
                }
                if (exclude)
                {
                    anyHits = true;
                }
                else
                {
                    filteredLines.Add(line);
                }
                previouslyProcessedRecord = record;
            }
            filteredLines.Reverse();
            return filteredLines.ToArray();
        }
    }
}