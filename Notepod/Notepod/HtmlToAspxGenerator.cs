using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Create an aspx markup page using the information found in an original html page.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class HtmlToAspxGenerator : BaseGenerator
    {
        #region Member variables.
        private string _htmlPath;
        private string _htmlFile;
        private string _aspxPath;
        private string _aspxFile;
        private List<string> _aspxOutput = null;
        #endregion

        #region Properties.
        /// <summary>
        /// Html path.
        /// </summary>
        public string HtmlPath
        {
            get
            {
                return _htmlPath;
            }
            set
            {
                _htmlPath = value;
            }
        }

        /// <summary>
        /// Html file.
        /// </summary>
        public string HtmlFile
        {
            get
            {
                return _htmlFile;
            }
            set
            {
                _htmlFile = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public HtmlToAspxGenerator(string[] inputLines) : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import html lines.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate aspx lines.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import relationship foreign key details from create table statement.
        /// </summary>
        private void ImportLines()
        {
            string line = string.Empty;
            string data = string.Empty;
            bool startFound = false;
            int firstLine = 0;
            int lastDivLine = 0;
            int lastLine = 0;
            for (int lineNo = 0; lineNo < _inputLines.Length; lineNo++)
            {
                line = _inputLines[lineNo];
                data = line.Trim().ToLower();
                if (data.Length > 0)
                {
                    if (data == @"<div id=""contents"" class=""narrativeportrait"">")
                    {
                        if (!startFound)
                        {
                            startFound = true;
                            firstLine = lineNo + 1;
                        }
                    }
                    else if (data == @"</div>")
                    {
                        lastDivLine = lineNo;
                    }
                    else if (data == @"</body>")
                    {
                        lastLine = lineNo;
                        break;
                    }
                }
            }
            _aspxOutput = new List<string>();
            for (int lineNo = firstLine; lineNo < lastDivLine; lineNo++)
            {
                line = _inputLines[lineNo];
                _aspxOutput.Add(line);
            }
        }

        /// <summary>
        /// Generate table dependency list from create table statement.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            string aspxName = Path.GetFileNameWithoutExtension(_htmlFile);
            line = String.Format(@"<%@ Page Language=""C#"" MasterPageFile=""../../Framework/aspx/Portrait.Master"" AutoEventWireup=""true"" CodeBehind=""{0}.aspx.cs"" Inherits=""SyntaxNet.Content.aspx.{0}"" %>", aspxName);
            output.Add(line);
            line = @"<asp:Content ID=""conMain"" ContentPlaceHolderId=""portraitPlaceHolder"" runat=""server"">";
            output.Add(line);
            foreach (string aspxLine in _aspxOutput)
            {
                output.Add(aspxLine);
            }
            line = @"</asp:Content>";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}