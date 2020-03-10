using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Member variable to property generator.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class MemberVariableToDataMemberGenerator : BaseGenerator
    {
        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public MemberVariableToDataMemberGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import member variable declarations.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate property accessor methods from member variable declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            List<string> output = GenerateProperties();
            return output.ToArray();
        }
        #endregion

        #region Private methods.
        #region Import methods.
        /// <summary>
        /// Import member variable declarations.
        /// </summary>
        private void ImportLines()
        {
            _vectors = new Vectors();
            Vector vector = null;
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            int nEol = 0;
            foreach (string inputLine in _inputLines)
            {
                line = inputLine.Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
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
                                    vector.Summary = TitleFromCamelCase(vector.Property) + ".";
                                }
                            }
                        }
                        _vectors.Add(vector);
                    }
                }
            }
        }
        #endregion

        #region Generate methods.
        /// <summary>
        /// Generate property accessor methods.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override List<string> GenerateProperties()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Data members.";
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
                line = @"[DataMember]";
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
        #endregion
    }
}