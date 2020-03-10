using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Entity list generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class EntityListGenerator : BaseGenerator
    {
        #region Member variables.
        private string _listClass = string.Empty;
        private string _itemClass = string.Empty;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public EntityListGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import public class name declaration.
        /// </summary>
        public override void Import()
        {
            ImportGlobalParameters();
            ImportLines();
        }

        /// <summary>
        /// Generate entity list class from public class name declaration.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import public class name declaration.
        /// </summary>
        private void ImportLines()
        {
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            foreach (string inputLine in _inputLines)
            {
                line = inputLine.Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
                    {
                        nPos1 = line.IndexOf(" class ");
                        if (nPos1 > -1)
                        {
                            nPos1 += (" class ").Length;
                            nPos2 = line.IndexOf(':', nPos1 + 1);
                            if (nPos2 > -1)
                            {
                                _listClass = line.Substring(nPos1, nPos2 - nPos1).Trim();
                                nPos1 = line.IndexOf(" List<");
                                if (nPos1 > -1)
                                {
                                    nPos1 += (" List<").Length;
                                    nPos2 = line.IndexOf('>', nPos1 + 1);
                                    if (nPos2 > -1)
                                    {
                                        _itemClass = line.Substring(nPos1, nPos2 - nPos1).Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Generate entity list class from public class name declaration.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            if (_layer == "AS")
            {
                return GenerateAsLines();
            }
            else if (_layer == "ES")
            {
                return GenerateEsLines();
            }
            else
            {
                string[] messages=new string[1];
                messages[0] = @"Unknown layer: """ + _layer + @"""";
                return messages;
            }
        }

        /// <summary>
        /// Generate AS entity list class from public class name declaration.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateAsLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = "/// <summary>";
            output.Add(line);
            line = "/// Class which inherits from a type safe list to improve code readability.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = "public class " + _listClass + " : List<" + _itemClass + ">";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "#region Constructors.";
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Default constructor.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = "public " + _listClass + "()";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Main constructor.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = @"/// <param name=""lowerEntityList"">lower entity list which corresponds to this upper entity list.</param>";
            output.Add(line);
            line = "public " + _listClass + "(lowerEntities." + _listClass + " lowerEntityList)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "foreach (lowerEntities." + _itemClass + " lowerEntity in lowerEntityList)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "this.Add(new " + _itemClass + "(lowerEntity));";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Test constructor.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = @"/// <param name=""upperEntityList"">upper entity list to construct this upper entity list from.</param>";
            output.Add(line);
            line = "public " + _listClass + "(" + _listClass + " upperEntityList)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "foreach (" + _itemClass + " upperEntity in upperEntityList)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "this.Add(new " + _itemClass + "(upperEntity));";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "#endregion";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = "#region Public methods.";
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Map the private upper entity to the lower entity.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = "/// <returns>A lower entity.</returns>";
            output.Add(line);
            line = "public lowerEntities." + _listClass + " MapTo()";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "lowerEntities." + _listClass + " lowerEntityList = new lowerEntities." + _listClass + "();";
            output.Add(line);
            line = "foreach (" + _itemClass + " upperEntity in this)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "lowerEntities." + _itemClass + " lowerEntity = upperEntity.MapTo();";
            output.Add(line);
            line = "lowerEntityList.Add(lowerEntity);";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "return lowerEntityList;";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "#endregion";
            output.Add(line);
            line = "}";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate ES entity list class from public class name declaration.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEsLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Class which inherits from a type safe list to improve code readability.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"[Serializable]";
            output.Add(line);
            line = @"public class " + _listClass + " : List<" + _itemClass + ">";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"#region Constructors.";
            output.Add(line);
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Default constructor.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"public " + _listClass + "()";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Main constructor.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <param name=""privateupperEntityList"">Private upper entity list to construct this public upper entity list from.</param>";
            output.Add(line);
            line = @"public " + _listClass + "(upperEntities." + _listClass + " privateupperEntityList)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"foreach (upperEntities." + _itemClass + " privateupperEntity in privateupperEntityList)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"this.Add(new " + _itemClass + "(privateupperEntity));";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"#region Public methods.";
            output.Add(line);
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Map the public upper entity list to the private upper entity list.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <returns>A private upper entity.</returns>";
            output.Add(line);
            line = @"public upperEntities." + _listClass + " MapTo()";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"upperEntities." + _listClass + " privateupperEntityList = new upperEntities." + _listClass + "();";
            output.Add(line);
            line = @"foreach (" + _itemClass + " publicupperEntity in this)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"upperEntities." + _itemClass + " privateupperEntity = publicupperEntity.MapTo();";
            output.Add(line);
            line = @"privateupperEntityList.Add(privateupperEntity);";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"return privateupperEntityList;";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}