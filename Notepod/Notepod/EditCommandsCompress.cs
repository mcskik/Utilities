using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Compress.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsCompress : BaseGenerator
    {
        public EditCommandsCompress(string[] inputLines)
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
            string prevStatement = string.Empty;
            foreach (string inputLine in _inputLines)
            {
                string statement = inputLine.Trim();
                if (statement.Length > 0)
                {
                    if (statement.StartsWith("<"))
                    {
                        if (line.Trim().Length > 0)
                        {
                            output.Add(line);
                        }
                        line = inputLine;
                    }
                    else if (statement.StartsWith("?"))
                    {
                        line += statement;
                    }
                    else if (statement.StartsWith("&"))
                    {
                        line += statement;
                    }
                    else if (statement.StartsWith("¤"))
                    {
                        line += statement;
                    }
                    else if (prevStatement.EndsWith("?"))
                    {
                        line += statement;
                    }
                    else if (prevStatement.EndsWith("&"))
                    {
                        line += statement;
                    }
                    else if (prevStatement.EndsWith("¤"))
                    {
                        line += statement;
                    }
                    else
                    {
                        if (line.Trim().Length > 0)
                        {
                            output.Add(line);
                        }
                        line = inputLine;
                    }
                }
                prevStatement = statement;
            }
            if (line.Trim().Length > 0)
            {
                output.Add(line);
            }
            return output.ToArray();
        }
    }
}