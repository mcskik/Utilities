using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    public class TestGenerator
    {
        /// <summary>
        /// Test generator.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Create the property summary text from the property name (No full stop).";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <remarks>";
            output.Add(line);
            line = @"/// This is used when there is nothing else to base the summary text on.";
            output.Add(line);
            line = @"/// </remarks>";
            output.Add(line);
            line = @"/// <param name=""property"">Property name.</param>";
            output.Add(line);
            line = @"/// <returns>Summary only without full stop.</returns>";
            output.Add(line);
            line = @"public string SummaryOnlyFromPropertyName(string property)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"StringBuilder summary = new StringBuilder();";
            output.Add(line);
            line = @"string letter = string.Empty;";
            output.Add(line);
            line = @"for (int pos = 0; pos < property.Length; pos++)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"letter = property.Substring(pos, 1);";
            output.Add(line);
            line = @"if (letter == letter.ToUpper())";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"summary.Append("" "" + letter);";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"else";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"summary.Append(letter);";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"return summary.ToString().Trim();";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"";
            output.Add(line);
            return output.ToArray();
        }
    }
}