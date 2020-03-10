using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Notepod
{
    /// <summary>
    /// Class to generate an XML test fragment creation method from a cut and pasted fragment of XML.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class GenerateXmlFragmentCreationMethodGenerator : BaseGenerator
    {
        #region Member variables.
        private string _rootElementName;
        private string _surplusIndentation;
        private int _lineStartPos;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public GenerateXmlFragmentCreationMethodGenerator(string[] inputLines)
            : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import cut and pasted XML fragment.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate an XML test fragment creation method from a cut and pasted fragment of XML.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import important properties from cut and pasted XML fragment.
        /// </summary>
        private void ImportLines()
        {
            //Stopped using this method because undeclared namespaces were causing
            //the XElement.Parse() to fail.  Namespaces are not required for the current
            //purpose of this method generator.
            //StringBuilder xml = new StringBuilder();
            //foreach (string line in _inputLines)
            //{
            //    xml.Append(line.Trim());
            //}
            //XElement element = XElement.Parse(xml.ToString());
            //_rootElementName = element.Name.ToString();
            string xml = _inputLines[0];
            string rootTag = string.Empty;
            int rootTagStart = xml.IndexOf("<");
            int rootTagEnd = xml.IndexOf(">");
            if (rootTagStart > -1 && rootTagEnd > -1 && rootTagEnd > rootTagStart)
            {
                rootTag = xml.Substring(rootTagStart, rootTagEnd - rootTagStart + 1);
                if (rootTag.Length > 1)
                {
                    rootTag = rootTag.Substring(1);
                }
                if (rootTag.Length > 1)
                {
                    rootTag = rootTag.Substring(0, rootTag.Length - 1);
                }
                rootTagEnd = rootTag.IndexOf(" ");
                if (rootTagEnd > 0)
                {
                    rootTag = rootTag.Substring(0, rootTagEnd);
                }
            }
            _rootElementName = rootTag;
            string test = string.Empty;
            test = _inputLines[0];
            int pos = test.IndexOf("<" + _rootElementName);
            if (pos > -1)
            {
                _lineStartPos = pos;
                _surplusIndentation = test.Substring(0, _lineStartPos);
            }
            else
            {
                _lineStartPos = 0;
                _surplusIndentation = string.Empty;
            }
        }

        /// <summary>
        /// Generate an XML test fragment creation method from a cut and pasted fragment of XML.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            string statement = string.Empty;
            output.Add(line);
            line = @"/// <summary>";
            output.Add(line);
            line = String.Format(@"/// Generate a test {0} element.", ProperCase(_rootElementName));
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = String.Format(@"private XElement Test{0}()", ProperCase(_rootElementName));
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"StringBuilder xml = new StringBuilder();";
            output.Add(line);
            foreach (string inputLine in _inputLines)
            {
                if (_lineStartPos < inputLine.Length)
                {
                    statement = inputLine.Substring(_lineStartPos);
                }
                else
                {
                    statement = inputLine;
                }
                if (statement.Trim() != string.Empty)
                {
                    statement = DoubleAnyQuotes(statement);
                    line = @"xml.AppendLine(String.Format(@""" + statement + @"""));";
                    output.Add(line);
                }
            }
            line = @"XElement element = XElement.Parse(xml.ToString());";
            output.Add(line);
            line = @"return element;";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}