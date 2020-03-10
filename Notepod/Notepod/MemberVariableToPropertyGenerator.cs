using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Member variable to property generator (Original syntax).
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class MemberVariableToPropertyGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _memberVariablesBlock;
        #endregion
        
        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public MemberVariableToPropertyGenerator(string[] inputLines) : base(inputLines)
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
        /// Generate property accessor methods from member variable declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            List<string> output = GenerateProperties();
            return output.ToArray();
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
            _vectors = _memberVariablesBlock.MemberVariables;
        }
        #endregion
    }
}