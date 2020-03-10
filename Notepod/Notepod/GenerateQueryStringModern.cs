using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Join and format a QueryString into modern format.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class GenerateQueryStringModern : BaseGenerator
    {
        #region Member variables.
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public GenerateQueryStringModern(string[] inputLines)
            : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import split queryString.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Join and format a QueryString into modern format.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import split queryString.
        /// </summary>
        private void ImportLines()
        {
        }

        /// <summary>
        /// Join and format a QueryString into modern format.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string inputLine in _inputLines)
            {
                sb.Append(inputLine);
            }
            line = sb.ToString();
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}