using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Member variable to entity store method generator.
    /// </summary>
    /// <remarks>
    /// This is used to support my own MsAccess OleDb Entity framework since Microsoft's entity frame work doesn't support this.
    /// </remarks>
    /// <author>Kenneth McSKimming</author>
    public class MemberVariableToEntityStoreMethodGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _memberVariablesBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public MemberVariableToEntityStoreMethodGenerator(string[] inputLines)
            : base(inputLines)
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
            List<string> output = GenerateEntityStoreMethod();
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

        /// <summary>
        /// Generate entity store method.
        /// </summary>
        public List<string> GenerateEntityStoreMethod()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"public override void Store(DataRow dr)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"base.Store(dr);";
            output.Add(line);
            line = @"DataTable dt = dr.Table;";
            output.Add(line);
            int count = 0;
            foreach (Vector vector in _vectors)
            {
                count++;
                line = String.Format(@"if (dt.Columns.Contains(""{0}""))", vector.Property);
                output.Add(line);
                line = @"{";
                output.Add(line);
                line = String.Format(@"dr[""{1}""] = FromNullable({0});", vector.Variable, vector.Property);
                output.Add(line);
                line = @"}";
                output.Add(line);
            }
            line = @"}";
            output.Add(line);
            return output;
        }
        #endregion
    }
}