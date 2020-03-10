using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Traditional Unwrap.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditTraditionalUnwrap : BaseGenerator
    {
        public EditTraditionalUnwrap(string[] inputLines)
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
            foreach (string inputLine in _inputLines)
            {
                string statement = inputLine.Trim();
                if (statement.Contains("new StringBuilder();"))
                {
                    statement = string.Empty;
                }
                else if (statement.Contains(".ToString();"))
                {
                    statement = string.Empty;
                }
                int pos = statement.IndexOf(@"(@""");
                if (pos > -1)
                {
                    statement = statement.Substring(pos + 3);
                    pos = statement.IndexOf(@"));");
                    if (pos > -1)
                    {
                        statement = statement.Substring(0, pos);
                        statement = statement.Trim();
                        output.Add(statement);
                    }
                }
            }
            return output.ToArray();
        }
    }
}