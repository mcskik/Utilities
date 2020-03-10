using System.Collections.Generic;
using Notepod.Models;

namespace Notepod
{
    /// <summary>
    /// Edit Other CsvIsoCodesToJsonStrings.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditOtherCsvIsoCodesToJsonStrings : BaseGenerator
    {
        public EditOtherCsvIsoCodesToJsonStrings(string[] inputLines)
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
            line = @"[";
            output.Add(line);
            int dataLineMax = 0;
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                if (line.Trim().Length > 0)
                {
                    dataLineMax++;
                }
            }
            int dataLineCount = 0;
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                if (line.Trim().Length > 0)
                {
                    dataLineCount++;
                    var values = CsvSplitter.Split(line);
                    if (values.Count > 1)
                    {
                        string name = values[0];
                        string value = values[1];
                        line = @"{";
                        output.Add(line);
                        line = string.Format(@"""Code"" : ""{0}"",", name);
                        output.Add(line);
                        line = string.Format(@"""Name"" : ""{0}""", value);
                        output.Add(line);
                        if (dataLineCount == dataLineMax)
                        {
                            line = "}";
                            output.Add(line);
                        }
                        else
                        {
                            line = "},";
                            output.Add(line);
                        }
                    }
                }
            }
            line = @"]";
            output.Add(line);
            return output.ToArray();
        }
    }
}