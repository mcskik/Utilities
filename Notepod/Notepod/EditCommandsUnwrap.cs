using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Unwrap.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsUnwrap : BaseGenerator
    {
        public EditCommandsUnwrap(string[] inputLines)
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
            int outerTagIndentLength = int.MaxValue;
            foreach (string inputLine in _inputLines)
            {
                string statement = inputLine;
                statement = ReplaceTabsWithSpaces(statement, 4);
                int indentLength = int.MaxValue;
                int pos1 = statement.IndexOf("@");
                int pos2 = statement.IndexOf("\"");
                int pos3 = statement.IndexOf("<");
                if (pos1 < indentLength) indentLength = pos1;
                if (pos2 < indentLength) indentLength = pos2;
                if (pos3 < indentLength) indentLength = pos3;
                if (outerTagIndentLength == int.MaxValue)
                {
                    if (pos3 > 0 && indentLength > 0)
                    {
                        outerTagIndentLength = indentLength;
                    }
                }
                if (indentLength < 0)
                {
                    indentLength = 0;
                }
                else if (indentLength >= outerTagIndentLength)
                {
                    indentLength -= outerTagIndentLength;
                }
                else if (indentLength < outerTagIndentLength)
                {
                    indentLength = outerTagIndentLength;
                }
                statement = statement.Trim();
                if (statement.Contains("new string[]"))
                {
                    statement = string.Empty;
                }
                else if (statement.Trim() == "{")
                {
                    statement = string.Empty;
                }
                else if (statement.Trim() == "};")
                {
                    statement = string.Empty;
                }
                if (statement.Length > 0)
                {
                    if (statement.StartsWith("@\""))
                    {
                        statement = statement.Substring(2);
                        statement = statement.Trim();
                        if (statement.EndsWith("+"))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        if (statement.EndsWith(","))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        if (statement.EndsWith("\""))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        statement = UndoAnyDoubleQuotes(statement);
                        statement = statement.Trim();
                    }
                    else if (statement.StartsWith("\""))
                    {
                        statement = statement.Substring(1);
                        statement = statement.Trim();
                        if (statement.EndsWith("+"))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        if (statement.EndsWith(","))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        if (statement.EndsWith("\""))
                        {
                            statement = statement.Substring(0, statement.Length - 1);
                            statement = statement.Trim();
                        }
                        statement = UndoAnyEscapedQuotes(statement);
                        statement = statement.Trim();
                    }
                    line = new string(' ', indentLength) + statement;
                    if (inputLine.Trim().StartsWith("[@COMMENT_TOKEN("))
                    {
                        line = inputLine;
                    }
                    output.Add(line);
                }
            }
            return output.ToArray();
        }
    }
}