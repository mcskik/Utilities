using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Stripped property generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class StrippedPropertyGenerator : BaseGenerator
    {
        #region Member variables.
        private List<string> _strippedLines;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public StrippedPropertyGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import two sets of property declarations.
        /// </summary>
        public override void Import()
        {
            ImportGlobalParameters();
            ImportLines();
        }

        /// <summary>
        /// Generate stripped properties from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();

        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import two sets of property declarations.
        /// </summary>
        private void ImportLines()
        {
            int lineNumber = 0;
            _strippedLines = new List<string>();
            if (_stripMode == "P")
            {
                if (_topLineParameters)
                {
                    lineNumber++;
                }
                ImportStrippedProperties(ref lineNumber);
                ImportStrippedProperties(ref lineNumber);
            }
            else if (_stripMode == "C")
            {
                ImportStrippedOfComments();
            }
        }

        /// <summary>
        /// Import just the top lines of the property declarations.
        /// </summary>
        /// <param name="lineNumber">Enters as starting line number, exits as next starting line number.</param>
        public void ImportStrippedProperties(ref int lineNumber)
        {
            string line = string.Empty;
            bool propertiesOpened = false;
            bool propertiesClosed = false;
            while (!propertiesClosed && lineNumber < _inputLines.Length)
            {
                line = _inputLines[lineNumber].Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
                    {
                        if (!propertiesOpened)
                        {
                            if (line.Contains("#region Properties"))
                            {
                                _strippedLines.Add(line);
                                propertiesOpened = true;
                            }
                        }
                        else
                        {
                            if (line.EndsWith("#endregion"))
                            {
                                _strippedLines.Add(line);
                                propertiesClosed = true;
                            }
                            else
                            {
                                if (line.StartsWith("public"))
                                {
                                    _strippedLines.Add(line);
                                }
                            }
                        }
                    }
                }
                lineNumber++;
            }
        }

        /// <summary>
        /// Import all lines but strip away all comments and XML comment tags.
        /// </summary>
        private void ImportStrippedOfComments()
        {
            string line = string.Empty;
            int lineNumber = 0;
            int startLine = _topLineParameters ? 2 : 1;
            foreach (string inputLine in _inputLines)
            {
                lineNumber++;
                if (lineNumber >= startLine)
                {
                    line = inputLine.Trim();
                    if (line.Length > 0)
                    {
                        if (!line.StartsWith("//"))
                        {
                            _strippedLines.Add(line);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate stripped properties from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            foreach (string line in _strippedLines)
            {
                output.Add(line);
            }
            return output.ToArray();
        }
        #endregion
    }
}