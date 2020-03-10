using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Abstract base generator class.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public abstract class BaseGenerator
    {
        #region Member variables.
        #region Global parameters.
        protected bool _topLineParameters = false;
        protected string _shortcutKey = string.Empty;
        protected string _layer = string.Empty;
        protected string _stripMode = string.Empty;
        protected string _namespace = string.Empty;
        #endregion
        protected string[] _inputLines;
        protected Vectors _vectors;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public BaseGenerator(string[] inputLines)
        {
            _inputLines = inputLines;
        }    
        #endregion

        #region Public methods.
        #region Abstract methods.
        /// <summary>
        /// Import member variable declarations.
        /// </summary>
        public abstract void Import();

        /// <summary>
        /// Generate  member variable declarations, property accessor methods or enumerations etc.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public abstract string[] Generate();
        #endregion

        #region Import methods.
        /// <summary>
        /// Import global parameters.
        /// </summary>
        public void ImportGlobalParameters()
        {
            SetDefaultGlobalParameters();
            string[] parameters = null;
            string line = string.Empty;
            int count = 0;
            foreach (string inputLine in _inputLines)
            {
                count++;
                line = inputLine.Trim();
                if (line.Length > 0)
                {
                    if (line.StartsWith("[@") && line.EndsWith("]"))
                    {
                        if (count == 1)
                        {
                            _topLineParameters = true;
                        }
                        line = line.Substring(1);
                        line = line.Substring(0, line.Length - 1);
                        parameters = line.Split(';');
                        for (int index = 0; index < parameters.Length; index++)
                        {
                            _shortcutKey = GetParameter("@K", parameters[index], _shortcutKey);
                            _layer = GetParameter("@L", parameters[index], _layer);
                            _stripMode = GetParameter("@S", parameters[index], _stripMode);
                            _namespace = GetParameter("@NAMESPACE", parameters[index], _namespace);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set default global parameters.
        /// </summary>
        private void SetDefaultGlobalParameters()
        {
            _topLineParameters = false;
            _shortcutKey = "U";
            _layer = "AS";
            _stripMode = "P";
            _namespace = "Customer";
        }

        /// <summary>
        /// Get parameter.
        /// </summary>
        /// <param name="identifier">Identifying keyword.</param>
        /// <param name="pair">"@identifier = value"</param>
        /// <param name="current"></param>
        /// <returns>The parameter value if found or the parameter's current value.</returns>
        private string GetParameter(string identifier, string pair, string current)
        {
            string parm = current;
            string[] args = pair.Split('=');
            if (args.Length > 1)
            {
                if (args[0].Trim().ToUpper() == identifier)
                {
                    parm = args[1].Trim().ToUpper();
                    if (parm.StartsWith(@"""") && parm.EndsWith(@""""))
                    {
                        parm = parm.Substring(1);
                        parm = parm.Substring(0, parm.Length - 1);
                    }
                }
            }
            return parm;
        }

        /// <summary>
        /// Import member variable declarations within region-endregion block.
        /// </summary>
        /// <param name="lineNumber">Enters as starting line number, exits as next starting line number.</param>
        /// <returns>Entity meta block for home entity.</returns>
        public EntityMetaBlock ImportMemberVariables(ref int lineNumber)
        {
            EntityMetaBlock block = new EntityMetaBlock();
            Vector vector = null;
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            int nEol = 0;
            bool memberVariablesOpened = false;
            bool memberVariablesClosed = false;
            while (!memberVariablesClosed && lineNumber < _inputLines.Length)
            {
                line = _inputLines[lineNumber].Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
                    {
                        if (!memberVariablesOpened)
                        {
                            if (line.Contains("#region Member"))
                            {
                                block = new EntityMetaBlock();
                                nPos1 = line.IndexOf("[");
                                if (nPos1 > -1)
                                {
                                    nPos2 = line.IndexOf("]");
                                    if (nPos2 > -1)
                                    {
                                        block.Name = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                        if (block.Name.Contains("."))
                                        {
                                            string[] parts = block.Name.Split('.');
                                            block.ShortName = parts[parts.Length - 1];
                                        }
                                        else
                                        {
                                            block.ShortName = block.Name;
                                        }
                                    }
                                }
                                memberVariablesOpened = true;
                            }
                        }
                        else
                        {
                            if (line.EndsWith("#endregion"))
                            {
                                memberVariablesClosed = true;
                            }
                            else
                            {
                                vector = new Vector();
                                if (line.Contains(" readonly "))
                                {
                                    vector.ReadOnly = true;
                                    line = line.Replace("readonly ", string.Empty);
                                }
                                nPos1 = line.IndexOf(' ');
                                if (nPos1 > -1)
                                {
                                    vector.Modifier = line.Substring(0, nPos1).Trim();
                                    nPos2 = line.IndexOf(' ', nPos1 + 1);
                                    if (nPos2 > -1)
                                    {
                                        vector.Type = line.Substring(nPos1, nPos2 - nPos1).Trim();
                                        nEol = line.Length;
                                        nPos1 = line.IndexOf(' ', nPos2 + 1);
                                        if (nPos1 > -1)
                                        {
                                            if (nPos1 < nEol)
                                            {
                                                nEol = nPos1;
                                            }
                                        }
                                        else
                                        {
                                            nPos1 = line.IndexOf('=', nPos2 + 1);
                                            if (nPos1 > -1)
                                            {
                                                if (nPos1 < nEol)
                                                {
                                                    nEol = nPos1;
                                                }
                                            }
                                            else
                                            {
                                                nPos1 = line.IndexOf(';', nPos2 + 1);
                                                if (nPos1 > -1)
                                                {
                                                    if (nPos1 < nEol)
                                                    {
                                                        nEol = nPos1;
                                                    }
                                                }
                                            }
                                        }
                                        if (nPos1 > -1)
                                        {
                                            vector.Variable = line.Substring(nPos2, nPos1 - nPos2).Trim();
                                            if (vector.Variable.Substring(0, 1) == "_")
                                            {
                                                vector.Property = vector.Variable.Substring(1, 1).ToUpper() + vector.Variable.Substring(2);
                                            }
                                            else
                                            {
                                                vector.Property = vector.Variable.Substring(2, 1).ToUpper() + vector.Variable.Substring(3);
                                            }
                                            vector.Summary = ProperCase(DescriptionFromCamelCase(vector.Property)) + ".";
                                        }
                                    }
                                }
                                block.MemberVariables.Add(vector);
                            }
                        }
                    }
                }
                lineNumber++;
            }
            return block;
        }

        /// <summary>
        /// Import property declarations within region-endregion block.
        /// </summary>
        /// <param name="lineNumber">Enters as starting line number, exits as next starting line number.</param>
        /// <returns>Entity meta block for foreign entity.</returns>
        public EntityMetaBlock ImportProperties(ref int lineNumber)
        {
            EntityMetaBlock block = new EntityMetaBlock();
            Vector vector = null;
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            int nEol = 0;
            bool propertiesOpened = false;
            bool propertiesClosed = false;
            while (!propertiesClosed && lineNumber < _inputLines.Length)
            {
                line = _inputLines[lineNumber].Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
                    {
                        if (!propertiesOpened)
                        {
                            if (line.Contains("#region Properties"))
                            {
                                block = new EntityMetaBlock();
                                nPos1 = line.IndexOf("[");
                                if (nPos1 > -1)
                                {
                                    nPos2 = line.IndexOf("]");
                                    if (nPos2 > -1)
                                    {
                                        block.Name = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                        if (block.Name.Contains("."))
                                        {
                                            string[] parts = block.Name.Split('.');
                                            block.ShortName = parts[parts.Length - 1];
                                        }
                                        else
                                        {
                                            block.ShortName = block.Name;
                                        }
                                    }
                                }
                                propertiesOpened = true;
                            }
                        }
                        else
                        {
                            if (line.EndsWith("#endregion"))
                            {
                                propertiesClosed = true;
                            }
                            else
                            {
                                if (line.StartsWith("public"))
                                {
                                    vector = new Vector();
                                    if (line.Contains(" readonly "))
                                    {
                                        vector.ReadOnly = true;
                                        line = line.Replace("readonly ", string.Empty);
                                    }
                                    nPos1 = line.IndexOf(' ');
                                    if (nPos1 > -1)
                                    {
                                        vector.Modifier = line.Substring(0, nPos1).Trim();
                                        nPos2 = line.IndexOf(' ', nPos1 + 1);
                                        if (nPos2 > -1)
                                        {
                                            vector.Type = line.Substring(nPos1, nPos2 - nPos1).Trim();
                                            nEol = line.Length;
                                            vector.Property = line.Substring(nPos2, nEol - nPos2).Trim();
                                            vector.Variable = "_" + vector.Property.Substring(0, 1).ToLower() + vector.Property.Substring(1);
                                            vector.Summary = TitleFromCamelCase(vector.Property) + ".";
                                        }
                                    }
                                    block.Properties.Add(vector);
                                }
                            }
                        }
                    }
                }
                lineNumber++;
            }
            return block;
        }
        #endregion

        #region Generate methods.
        /// <summary>
        /// Generate member variables and property accessor methods.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public string[] GenerateMemberVariablesAndProperties()
        {
            List<string> variables = GenerateMemberVariables();
            List<string> properties = GenerateProperties();
            List<string> combined = new List<string>();
            foreach (string line in variables)
            {
                combined.Add(line);
            }
            combined.Add(string.Empty);
            foreach (string line in properties)
            {
                combined.Add(line);
            }
            return combined.ToArray();
        }

        /// <summary>
        /// Generate member variable declarations.
        /// </summary>
        /// <returns>List of member variable lines.</returns>
        public List<string> GenerateMemberVariables()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Member variables.";
            output.Add(line);
            foreach (Vector vector in _vectors)
            {
                if (vector.ReadOnly)
                {
                    line = @"private readonly " + vector.Type + " " + vector.Variable + ";";
                }
                else
                {
                    line = @"private " + vector.Type + " " + vector.Variable + ";";
                }
                output.Add(line);
            }
            line = @"#endregion";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate property accessor methods.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public virtual List<string> GenerateProperties()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Properties.";
            output.Add(line);
            int count = 0;
            foreach (Vector vector in _vectors)
            {
                count++;
                if (count > 1)
                {
                    line = string.Empty;
                    output.Add(line);
                }
                line = @"/// <summary>";
                output.Add(line);
                line = @"/// " + vector.Summary;
                output.Add(line);
                line = @"/// </summary>";
                output.Add(line);
                line = "public " + vector.Type + " " + vector.Property;
                output.Add(line);
                line = "{";
                output.Add(line);
                line = "get";
                output.Add(line);
                line = "{";
                output.Add(line);
                line = "return " + vector.Variable + ";";
                output.Add(line);
                line = "}";
                output.Add(line);
                if (!vector.ReadOnly)
                {
                    line = "set";
                    output.Add(line);
                    line = "{";
                    output.Add(line);
                    line = vector.Variable + " = value;";
                    output.Add(line);
                    line = "}";
                    output.Add(line);
                }
                line = "}";
                output.Add(line);
            }
            line = @"#endregion";
            output.Add(line);
            return output;
        }
        #endregion

        #region Helper methods.
        /// <summary>
        /// Create title from camel case variable or property name.
        /// </summary>
        /// <param name="camelCase">Camel case variable or property name.</param>
        /// <returns>Title with very first character capitalized.</returns>
        public string TitleFromCamelCase(string camelCase)
        {
            return ProperCase(DescriptionFromCamelCase(camelCase));
        }

        /// <summary>
        /// Create description from camel case variable or property name.
        /// </summary>
        /// <param name="camelCase">Camel case variable or property name.</param>
        /// <returns>Description.</returns>
        public string DescriptionFromCamelCase(string camelCase)
        {
            StringBuilder description = new StringBuilder();
            string letter = string.Empty;
            for (int pos = 0; pos < camelCase.Length; pos++)
            {
                letter = camelCase.Substring(pos, 1);
                if (letter == letter.ToUpper())
                {
                    description.Append(" " + letter.ToLower());
                }
                else
                {
                    description.Append(letter);
                }
            }
            return description.ToString().Trim();
        }

        /// <summary>
        /// Change word to proper case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word in proper case.</returns>
        public string ProperCase(string word)
        {
            string proper = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                proper = word.Substring(0, 1).ToUpper();
                if (word.Length > 1)
                {
                    proper += word.Substring(1).ToLower();
                }
            }
            return proper;
        }

        /// <summary>
        /// Change first letter of word to upper case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word with first letter in upper case.</returns>
        public string UpperFirst(string word)
        {
            string upper = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                upper = word.Substring(0, 1).ToUpper();
                if (word.Length > 1)
                {
                    upper += word.Substring(1);
                }
            }
            return upper;
        }

        /// <summary>
        /// Change first letter of word to lower case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word with first letter in lower case.</returns>
        public string LowerFirst(string word)
        {
            string lower = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                lower = word.Substring(0, 1).ToLower();
                if (word.Length > 1)
                {
                    lower += word.Substring(1);
                }
            }
            return lower;
        }

        /// <summary>
        /// Double up any embedded quotation marks.
        /// </summary>
        /// <param name="input">Input line.</param>
        /// <returns>Output line.</returns>
        public string DoubleAnyQuotes(string input)
        {
            const string QUOTE = @"""";
            StringBuilder output = new StringBuilder();
            for (int pos = 0; pos < input.Length; pos++)
            {
                string letter = input.Substring(pos, 1);
                if (letter == QUOTE)
                {
                    output.Append(QUOTE + QUOTE);
                }
                else
                {
                    output.Append(letter);
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Undo any embedded double quotation marks.
        /// </summary>
        public string UndoAnyDoubleQuotes(string input)
        {
            const string QUOTE = @"""";
            const string DOUBLE_QUOTE = QUOTE + QUOTE;
            input = input.Replace(DOUBLE_QUOTE,QUOTE);
            return input;
        }

        /// <summary>
        /// Undo any embedded escaped quotation marks.
        /// </summary>
        public string UndoAnyEscapedQuotes(string input)
        {
            const string QUOTE = @"""";
            const string ESCAPED_QUOTE = @"\""";
            input = input.Replace(ESCAPED_QUOTE, QUOTE);
            return input;
        }

        /// <summary>
        /// Replace tabs with specified number of spaces.
        /// </summary>
        /// <param name="input">Input line.</param>
        /// <returns>Output line.</returns>
        public string ReplaceTabsWithSpaces(string input, int spaceCount)
        {
            string indent = new string(' ', spaceCount);
            const string TAB = "\t";
            StringBuilder output = new StringBuilder();
            for (int pos = 0; pos < input.Length; pos++)
            {
                string letter = input.Substring(pos, 1);
                if (letter == TAB)
                {
                    output.Append(indent);
                }
                else
                {
                    output.Append(letter);
                }
            }
            return output.ToString();
        }
        #endregion
        #endregion
    }
}