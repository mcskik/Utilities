using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Traditional Wrap PostData.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditTraditionalWrapPostData : BaseGenerator
    {
        public EditTraditionalWrapPostData(string[] inputLines)
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
            string delimiter = string.Empty;
            line = @"var postData = new StringBuilder();";
            output.Add(line);
            foreach (string inputLine in _inputLines)
            {
                int pos = inputLine.IndexOf('=');
                if (pos > -1)
                {
                    string name = inputLine.Substring(0, pos);
                    if (name.StartsWith("&") || name.StartsWith("?"))
                    {
                        name = name.Substring(1);
                    }
                    string value = inputLine.Substring(pos + 1);
                    if (value == string.Empty)
                    {
                        line = @"postData.Append(String.Format(@""" + delimiter + name + @"={0}"", string.Empty));";
                    }
                    else
                    {
                        line = @"postData.Append(String.Format(@""" + delimiter + name + @"={0}"", """ + value + @"""));";
                    }
                    output.Add(line);
                    delimiter = "&";
                }
            }
            line = @"return postData.ToString();";
            output.Add(line);
            return output.ToArray();
        }
    }
}