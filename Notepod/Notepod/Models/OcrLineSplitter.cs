using System;
using System.Collections.Generic;

namespace Notepod
{
    /// <summary>
    /// OCR line splitter.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class OcrLineSplitter : BaseGenerator
    {
        private const string CRLF = "#£#£";
        private List<string> startMarkers;
        private List<string> endMarkers;

        public OcrLineSplitter(string[] inputLines)
            : base(inputLines)
        {
            startMarkers = new List<string>();
            startMarkers.Add(@"/**");
            //startMarkers.Add(@"*/");
            startMarkers.Add(@"//");
            //startMarkers.Add(@"<");
            startMarkers.Add(@"package");
            startMarkers.Add(@"import");
            startMarkers.Add(@"abstract");
            startMarkers.Add(@"lateinit");
            startMarkers.Add(@"open");
            startMarkers.Add(@"interface");
            startMarkers.Add(@"class");
            startMarkers.Add(@"object");
            startMarkers.Add(@"private");
            startMarkers.Add(@"protected");
            startMarkers.Add(@"public");
            startMarkers.Add(@"static");
            startMarkers.Add(@"val");
            startMarkers.Add(@"var");
            startMarkers.Add(@"override");
            startMarkers.Add(@"fun");
            startMarkers.Add(@"return");
            startMarkers.Add(@"if (");
            startMarkers.Add(@"when");
            startMarkers.Add(@"else");
            startMarkers.Add(@"switch");
            startMarkers.Add(@"@Override");
            startMarkers.Add(@"void");
            //startMarkers.Add(@"String");
            //startMarkers.Add(@"int");
            //startMarkers.Add(@"boolean");
            //startMarkers.Add(@"Boolean");
            //startMarkers.Add(@"Integer");
            endMarkers = new List<string>();
            endMarkers.Add(@".");
            endMarkers.Add(@"{");
            endMarkers.Add(@";");
            endMarkers.Add(@">");
            endMarkers.Add(@"}");
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
            string line = string.Empty;
            List<string> output = new List<string>();
            List<string> textBlocks = new List<string>();
            for (int index = 0; index < _inputLines.Length; index++)
            {
                string textBlock = _inputLines[index];
                textBlocks.Add(textBlock);
            }
            List<string> textLines = new List<string>();
            foreach(var textBlock in textBlocks)
            {
                string block = textBlock;
                foreach (var startMarker in startMarkers)
                {
                    block = block.Replace(startMarker, CRLF + startMarker);
                }
                string[] parts = block.Split(CRLF.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    string carry = string.Empty;
                    foreach (string part in parts)
                    {
                        if (startMarkers.Contains(part.Trim()))
                        {
                            carry += part.Trim() + " ";
                        }
                        else
                        {
                            textLines.Add(carry + part);
                            carry = string.Empty;
                        }
                    }
                }
                else
                {
                    textLines.Add(textBlock);
                }
            }
            foreach (string textLine in textLines)
            {
                output.Add(textLine);
            }
            return output.ToArray();
        }
    }
}