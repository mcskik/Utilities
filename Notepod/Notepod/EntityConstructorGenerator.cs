using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Entity constructor generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class EntityConstructorGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _upperLayerBlock;
        private EntityMetaBlock _lowerLayerBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public EntityConstructorGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import member variable and property declarations.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate entity constructor from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import member variable and property declarations.
        /// </summary>
        private void ImportLines()
        {
            int lineNumber = 0;
            _upperLayerBlock = ImportMemberVariables(ref lineNumber);
            _lowerLayerBlock = ImportProperties(ref lineNumber);
        }

        /// <summary>
        /// Generate entity constructor from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = "#region Constructors.";
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Default constructor.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = "public " + _upperLayerBlock.Name + "()";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            if (_upperLayerBlock.Name != _lowerLayerBlock.Name)
            {
                line = "/// <summary>";
                output.Add(line);
                line = "/// Main constructor.";
                output.Add(line);
                line = "/// </summary>";
                output.Add(line);
                line = @"/// <param name=""lowerEntity"">lower entity which corresponds to this private entity.</param>";
                output.Add(line);
                line = "public " + _upperLayerBlock.Name + "(" + _lowerLayerBlock.Name + " lowerEntity)";
                output.Add(line);
                line = "{";
                output.Add(line);
                for (int count = 0; count < _upperLayerBlock.MemberVariables.Count; count++)
                {
                    line = _upperLayerBlock.MemberVariables[count].Variable + " = lowerEntity." + _lowerLayerBlock.Properties[count].Property + ";";
                    output.Add(line);
                }
                line = "}";
                output.Add(line);
            }
            else
            {
                line = "/// <summary>";
                output.Add(line);
                line = "/// Test constructor.";
                output.Add(line);
                line = "/// </summary>";
                output.Add(line);
                line = @"/// <param name=""upperEntity"">upper entity to construct this private entity from.</param>";
                output.Add(line);
                line = "public " + _upperLayerBlock.Name + "(" + _upperLayerBlock.Name + " upperEntity)";
                output.Add(line);
                line = "{";
                output.Add(line);
                for (int count = 0; count < _upperLayerBlock.MemberVariables.Count; count++)
                {
                    line = _upperLayerBlock.MemberVariables[count].Variable + " = upperEntity." + _lowerLayerBlock.Properties[count].Property + ";";
                    output.Add(line);
                }
                line = "}";
                output.Add(line);
            }
            line = "#endregion";
            output.Add(line);
            if (_upperLayerBlock.Name != _lowerLayerBlock.Name)
            {
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
                line = "public " + _lowerLayerBlock.Name + " MapTo()";
                output.Add(line);
                line = "{";
                output.Add(line);
                line = _lowerLayerBlock.Name + " lowerEntity = new " + _lowerLayerBlock.Name + "();";
                output.Add(line);
                for (int count = 0; count < _lowerLayerBlock.Properties.Count; count++)
                {
                    line = "lowerEntity." + _lowerLayerBlock.Properties[count].Property + " = " + _upperLayerBlock.MemberVariables[count].Variable + ";";
                    output.Add(line);
                }
                line = "return lowerEntity;";
                output.Add(line);
                line = "}";
                output.Add(line);
                line = "#endregion";
                output.Add(line);
            }
            GenerateParameterisedConstructor(ref output, _upperLayerBlock);
            GenerateParameterisedConstructor(ref output, _lowerLayerBlock);
            return output.ToArray();
        }

        /// <summary>
        /// Generate parameterised constructor.
        /// </summary>
        /// <param name="output">List of output lines to append to.</param>
        /// <param name="block">Upper layer or lower layer entity meta block.</param>
        private void GenerateParameterisedConstructor(ref List<string> output, EntityMetaBlock block)
        {
            string line = string.Empty;
            output.Add(line);
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Parameterised constructor.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string parameter = block.MemberVariables[count].Variable;
                if (parameter.StartsWith("_"))
                {
                    parameter = parameter.Substring(1);
                }
                line = @"/// <param name=""" + parameter + @""">" + TitleFromCamelCase(parameter) + ".</param>";
                output.Add(line);
            }
            line = @"public " + block.Name + "(";
            string separator = string.Empty;
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string type = block.MemberVariables[count].Type;
                string parameter = block.MemberVariables[count].Variable;
                if (parameter.StartsWith("_"))
                {
                    parameter = parameter.Substring(1);
                }
                line += separator + type + " " + parameter;
                separator = ", ";
            }
            line += ")";
            output.Add(line);
            line = @"{";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string variable = block.MemberVariables[count].Variable;
                string parameter = block.MemberVariables[count].Variable;
                if (parameter.StartsWith("_"))
                {
                    parameter = parameter.Substring(1);
                }
                line = variable + " = " + parameter + ";";
                output.Add(line);
            }
            line = @"}";
            output.Add(line);
        }
        #endregion
    }
}