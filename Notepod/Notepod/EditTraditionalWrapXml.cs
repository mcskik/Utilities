using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Traditional Wrap XML.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditTraditionalWrapXml : BaseGenerator
    {
        public EditTraditionalWrapXml(string[] inputLines)
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
            string statement = string.Empty;
            line = @"StringBuilder xml = new StringBuilder();";
            output.Add(line);
            foreach (string inputLine in _inputLines)
            {
                statement = inputLine;
                if (statement.Trim() != string.Empty)
                {
                    statement = DoubleAnyQuotes(statement);
                    line = @"xml.AppendLine(String.Format(@""" + statement + @"""));";
                    output.Add(line);
                }
            }
            line = @"Request = xml.ToString();";
            output.Add(line);
            return output.ToArray();
        }
    }
}