using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Class to generate a basic generate lines method from a cut and pasted C# method code snippet.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class GenerateMethodGenerator : BaseGenerator
    {
        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public GenerateMethodGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import cut and pasted C# method code snippet.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate a basic generate lines method from a cut and pasted C# method code snippet.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import cut and pasted C# method code snippet.
        /// </summary>
        private void ImportLines()
        {
            //Intentionally does nothing.
        }

        /// <summary>
        /// Generate a basic generate lines method from a cut and pasted C# method code snippet.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            string statement = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// ";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <returns>Array of output lines to paste into the text window.</returns>";
            output.Add(line);
            line = @"private string[] GenerateLines()";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "List<string> output = new List<string>();";
            output.Add(line);
            line = "string line = string.Empty;";
            output.Add(line);
            foreach (string inputLine in _inputLines)
            {
                statement = inputLine.Trim();
                statement = DoubleAnyQuotes(statement);
                line = @"line = @""" + statement + @""";";
                output.Add(line);
                line = @"output.Add(line);";
                output.Add(line);
            }
            line = "return output.ToArray();";
            output.Add(line);
            line = "}";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}