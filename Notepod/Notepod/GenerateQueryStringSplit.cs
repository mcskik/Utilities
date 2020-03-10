using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Split a QueryString at every ampersand.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class GenerateQueryStringSplit : BaseGenerator
    {
        #region Member variables.
        private string _allLines;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public GenerateQueryStringSplit(string[] inputLines)
            : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import cut and pasted QueryString.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Split a QueryString at every ampersand.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import cut and pasted QueryString.
        /// </summary>
        private void ImportLines()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in _inputLines)
            {
                sb.Append(line);
            }
            _allLines = sb.ToString();
        }

        /// <summary>
        /// Split a QueryString at every ampersand.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            const char AMPERSAND = '&';
            string delimiter = string.Empty;
            List<string> output = new List<string>();
            string line = string.Empty;
            if (_allLines.Length > 0)
            {
                string[] parts = _allLines.Trim().Split(AMPERSAND);
                foreach (var part in parts)
                {
                    line = String.Format(@"{0}{1}", delimiter, part);
                    output.Add(line);
                    delimiter = AMPERSAND.ToString();
                }
            }
            return output.ToArray();
        }
        #endregion
    }
}