using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Member variable to new property syntax generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class MemberVariableToNewPropertySyntaxGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _memberVariablesBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public MemberVariableToNewPropertySyntaxGenerator(string[] inputLines) : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import member variable declarations.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate properties using the new property accessor syntax.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import member variable declarations.
        /// </summary>
        private void ImportLines()
        {
            int lineNumber = 0;
            _memberVariablesBlock = ImportMemberVariables(ref lineNumber);
        }

        /// <summary>
        /// Generate properties using the new property accessor syntax.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Properties.";
            output.Add(line);
            int count = 0;
            foreach (Vector vector in _memberVariablesBlock.MemberVariables)
            {
                count++;
                if (count > 1)
                {
                    line = string.Empty;
                    output.Add(line);
                }
                line = @"/// <summary>";
                output.Add(line);
                line = @"/// " + vector.Summary;
                output.Add(line);
                line = @"/// </summary>";
                output.Add(line);
                line = "public " + vector.Type + " " + vector.Property + " { get; set; } ";
                output.Add(line);
            }
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}