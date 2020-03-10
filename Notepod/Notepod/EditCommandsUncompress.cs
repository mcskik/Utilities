using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Uncompress.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsUncompress : BaseGenerator
    {
        public EditCommandsUncompress(string[] inputLines)
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
            foreach (string inputLine in _inputLines)
            {
                string statement = inputLine;
                if (statement.Trim().Length > 0)
                {
                    var sublines = Split(statement);
                    if (sublines.Count > 0)
                    {
                        output.AddRange(sublines);
                    }
                }
            }
            return output.ToArray();
        }

        private List<string> Split(string statement)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            if (statement.Length > 0)
            {
                bool split = false;
                for (int pos = 1; pos < statement.Length; pos++)
                {
                    string letter = statement.Substring(pos, 1);
                    switch (letter)
                    {
                        case "?":
                        case "&":
                        case "¤":
                            line = statement.Substring(0, pos);
                            output.Add(line);
                            statement = statement.Substring(pos);
                            var sublines = Split(statement);
                            if (sublines.Count > 0)
                            {
                                output.AddRange(sublines);
                            }
                            split = true;
                            break;
                        default:
                            break;
                    }
                    if (split) break;
                }
                if (!split)
                {
                    if (statement.Length > 0)
                    {
                        line = statement;
                        output.Add(line);
                    }
                }
            }
            return output;
        }
    }
}