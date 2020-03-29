using Notepod.Models;
using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit CSV key value pair to Android resource array XML.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCsvKeyValuePairToAndroidResouceArray : BaseGenerator
    {
        public EditCsvKeyValuePairToAndroidResouceArray(string[] inputLines)
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
            const string CSV_KEY_MARKER = "(Key)";
            const string RES_KEY_MARKER = " (%1$s)";
            List<string> output = new List<string>();
            string line = string.Empty;
            output.Add(line);
            line = @"    <!-- API key value pair mapping -->";
            output.Add(line);
            line = @"    <string-array name=""key_value_pair_mapping"">";
            output.Add(line);
            //Ignore column header line.
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                var values = CsvSplitter.Split(line);
                if (values.Count > 1)
                {
                    string key = values[0];
                    key = key.Trim().ToUpper();
                    string value = values[1];
                    value = value.Trim();
                    if (value.Contains(CSV_KEY_MARKER))
                    {
                        value = value.Replace(CSV_KEY_MARKER, string.Empty);
                        value = value.Trim();
                        value += RES_KEY_MARKER;
                        value = value.Trim();
                    }
                    value = value.Replace(@"&", @"&amp;");
                    value = value.Replace(@"'", @"\'");
                    value = value.Trim();
                    //Append key marker if missing.
                    //Now only have key marker if already present in value.
                    //value += RES_KEY_MARKER;
                    value = value.Trim();
                    line = @"        <item>" + key + "|" + value + "</item>";
                    output.Add(line);
                }
            }
            line = @"    </string-array>";
            output.Add(line);
            return output.ToArray();
        }
    }
}