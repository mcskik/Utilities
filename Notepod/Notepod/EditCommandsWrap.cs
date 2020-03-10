using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Wrap.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsWrap : BaseGenerator
    {
        public EditCommandsWrap(string[] inputLines)
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
            string foundationIndent = new string(' ', 12);
            string baseIndent = new string(' ', 16);
            List<string> output = new List<string>();
            string line = string.Empty;
            line = foundationIndent + "Commands = new string[]";
            output.Add(line);
            line = foundationIndent + "{";
            output.Add(line);
            for (int row = 0; row < _inputLines.Length; row++)
            {
                string inputLine = _inputLines[row];
                string statement = DoubleAnyQuotes(inputLine);
                string contents = statement.Trim();
                string innerContents = GetInnerContents(contents);
                if (innerContents.StartsWith("<") && innerContents.EndsWith(">"))
                {
                    int indentLength = 0;
                    if (contents.StartsWith("{"))
                    {
                        indentLength = statement.IndexOf("{");
                    }
                    else
                    {
                        indentLength = statement.IndexOf("<");
                    }
                    statement = statement.Trim();
                    string lineEnding = "\",";
                    if (row < _inputLines.Length - 1)
                    {
                        string nextInputLine = _inputLines[row + 1];
                        string nextContents = nextInputLine.Trim();
                        string nextInnerContents = GetInnerContents(nextContents);
                        if (nextInnerContents.StartsWith("<") && nextInnerContents.EndsWith(">"))
                        {
                            lineEnding = "\" +";
                        }
                    }
                    line = baseIndent + new string(' ', indentLength) + "@\"" + statement + lineEnding;
                }
                else if (contents.EndsWith("?") || contents.Trim().EndsWith("¤"))
                {
                    statement = statement.Trim();
                    string lineEnding = "\",";
                    if (row < _inputLines.Length - 1)
                    {
                        string nextInputLine = _inputLines[row + 1];
                        string nextContents = nextInputLine.Trim();
                        string nextInnerContents = GetInnerContents(nextContents);
                        if (nextInnerContents.StartsWith("<") && nextInnerContents.EndsWith(">"))
                        {
                            lineEnding = "\" +";
                        }
                    }
                    line = baseIndent + "@\"" + statement + lineEnding;
                }
                else
                {
                    statement = statement.Trim();
                    line = baseIndent + "@\"" + statement + "\",";
                }
                if (inputLine.Trim().StartsWith("[@COMMENT_TOKEN("))
                {
                    line = inputLine;
                }
                output.Add(line);
            }
            line = foundationIndent + "};";
            output.Add(line);
            return output.ToArray();
        }

        private string GetInnerContents(string contents)
        {
            string innerContents = contents;
            if (innerContents.StartsWith("{"))
            {
                int pos = innerContents.IndexOf("<");
                if (pos > -1)
                {
                    innerContents = innerContents.Substring(pos);
                }
            }
            if (innerContents.EndsWith("}"))
            {
                int pos = innerContents.IndexOf(">");
                if (pos > -1)
                {
                    innerContents = innerContents.Substring(0, pos + 1);
                }
            }
            innerContents = innerContents.Trim();
            return innerContents;
        }
    }
}