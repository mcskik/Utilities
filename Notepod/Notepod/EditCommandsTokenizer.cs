using System;
using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Tokenizer.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsTokenizer
    {
        private Dictionary<string, string> _tokenizedLines = new Dictionary<string, string>();
        private int _tokenId = 0;

        public EditCommandsTokenizer()
        {
        }

        public string[] TokenizeLines(string[] lines)
        {
            _tokenizedLines = new Dictionary<string, string>();
            _tokenId = 0;
            List<string> output = new List<string>();
            foreach (string inputLine in lines)
            {
                string inputRow = inputLine;
                if (inputLine.Trim().StartsWith("//"))
                {
                    _tokenId++;
                    string token = String.Format(@"[@COMMENT_TOKEN({0})@]", _tokenId);
                    _tokenizedLines.Add(token, inputLine);
                    inputRow = token;
                }
                else if (inputLine.Contains(";REGEX;"))
                {
                    _tokenId++;
                    string token = String.Format(@"[@TOKEN({0})@]", _tokenId);
                    _tokenizedLines.Add(token, inputLine);
                    inputRow = token;
                }
                output.Add(inputRow);
            }
            return output.ToArray();
        }

        public string[] DeTokenizeLines(string[] lines)
        {
            return DeTokenizeLines(lines, true);
        }

        public string[] DeTokenizeLines(string[] lines, bool includingComments)
        {
            List<string> output = new List<string>();
            foreach (string outputLine in lines)
            {
                string outputRow = outputLine;
                if (outputLine.Trim().StartsWith("[@COMMENT_TOKEN("))
                {
                    if (includingComments)
                    {
                        if (_tokenizedLines.ContainsKey(outputLine.Trim()))
                        {
                            outputRow = _tokenizedLines[outputLine.Trim()];
                        }
                    }
                }
                else
                {
                    if (_tokenizedLines.ContainsKey(outputLine.Trim()))
                    {
                        outputRow = _tokenizedLines[outputLine.Trim()];
                    }
                }
                output.Add(outputRow);
            }
            return output.ToArray();
        }
    }
}