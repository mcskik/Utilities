using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Join and format a QueryString into traditional format.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class GenerateQueryStringTraditional : BaseGenerator
    {
        #region Member variables.
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public GenerateQueryStringTraditional(string[] inputLines)
            : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import cut and pasted XML fragment.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Join and format a QueryString into traditional format.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import important properties from cut and pasted XML fragment.
        /// </summary>
        private void ImportLines()
        {
        }

        /// <summary>
        /// Join and format a QueryString into traditional format.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            string delimiter = string.Empty;
            line = @"var queryString = new StringBuilder();";
            output.Add(line);
            foreach (string inputLine in _inputLines)
            {
                int pos = inputLine.IndexOf('=');
                if (pos > -1)
                {
                    string name = inputLine.Substring(0, pos);
                    if (name.StartsWith("&"))
                    {
                        name = name.Substring(1);
                    }
                    string value = inputLine.Substring(pos + 1);
                    if (value == string.Empty)
                    {
                        line = @"queryString.Append(String.Format(@""" + delimiter + name + @"={0}"", string.Empty));";
                    }
                    else
                    {
                        line = @"queryString.Append(String.Format(@""" + delimiter + name + @"={0}"", """ + value + @"""));";
                    }
                    output.Add(line);
                    delimiter = "&";
                }
            }
            line = @"return queryString.ToString();";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}