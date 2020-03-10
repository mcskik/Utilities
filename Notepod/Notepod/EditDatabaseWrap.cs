using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Database Wrap.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditDatabaseWrap : BaseGenerator
    {
        public EditDatabaseWrap(string[] inputLines)
            : base(inputLines)
        {
        }

        public override void Import()
        {
            ImportLines();
        }

        public override string[] Generate()
        {
            return GenerateLines();
        }

        private void ImportLines()
        {
        }

        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"StringBuilder sql = new StringBuilder();";
            output.Add(line);
            for (int index = 0; index < _inputLines.Length; index++)
            {
                string statement = _inputLines[index];
                line = @"sql.AppendLine(""" + statement + @" "");";
                output.Add(line);
            }
            return output.ToArray();
        }
    }
}