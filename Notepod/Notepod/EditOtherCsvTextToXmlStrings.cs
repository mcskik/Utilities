using System.Collections.Generic;
using Notepod.Models;

namespace Notepod
{
    /// <summary>
    /// Edit Other CsvTextToXmlStrings.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditOtherCsvTextToXmlStrings : BaseGenerator
    {
        public EditOtherCsvTextToXmlStrings(string[] inputLines)
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
            line = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            output.Add(line);
            line = @"<resources>";
            output.Add(line);
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                var values = CsvSplitter.Split(line);
                string name = values[0];
                string value = values[1];
                line = string.Format(@"    <string name=""{0}"">{1}</string>", name, value);
                output.Add(line);
            }
            line = @"</resources>";
            output.Add(line);
            return output.ToArray();
        }
    }
}