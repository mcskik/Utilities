using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Mock entity generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class NewMockEntityGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _firstBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public NewMockEntityGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import two sets of property declarations.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate mock entity methods from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import two sets of property declarations.
        /// </summary>
        private void ImportLines()
        {
            int lineNumber = 0;
            _firstBlock = ImportProperties(ref lineNumber);
        }

        /// <summary>
        /// Generate mock entity methods from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> entityList = GenerateListBlock(_firstBlock);
            List<string> entity = GenerateBlock(_firstBlock);
            List<string> combined = new List<string>();
            combined.Add("#region Mock " + DescriptionFromCamelCase(_firstBlock.ShortName) + " methods.");
            combined.AddRange(entityList);
            combined.Add(string.Empty);
            combined.AddRange(entity);
            combined.Add("#endregion");
            combined.Add(string.Empty);
            return combined.ToArray();
        }

        /// <summary>
        /// Generate one mock entities method from one imported declaration.
        /// </summary>
        /// <param name="block">Entity meta block for either of the above.</param>
        /// <returns>List of output lines for this list block.</returns>
        private List<string> GenerateListBlock(EntityMetaBlock block)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Get mock " + DescriptionFromCamelCase(block.ShortName) + " list.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"public List<" + UpperFirst(block.ShortName) + "> GetMock" + UpperFirst(block.ShortName) + "List(int quantity, int seed)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"List<" + UpperFirst(block.ShortName) + @"> " + LowerFirst(block.ShortName) + @" = new List<" + UpperFirst(block.ShortName) + @">();";
            output.Add(line);
            line = @"for (int count = 0; count < quantity; count++)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = LowerFirst(block.ShortName) + @".Add(GetMock" + UpperFirst(block.ShortName) + @"(quantity, seed + count));";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"return " + LowerFirst(block.ShortName) + ";";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate one mock entity method from one imported declaration.
        /// </summary>
        /// <param name="block">Entity meta block for either of the above.</param>
        /// <returns>List of output lines for this block.</returns>
        private List<string> GenerateBlock(EntityMetaBlock block)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Get mock " + DescriptionFromCamelCase(block.ShortName) + ".";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"public " + UpperFirst(block.ShortName) + " GetMock" + UpperFirst(block.ShortName) + "(int quantity, int seed)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"string digits = seed.ToString().Trim();";
            output.Add(line);
            line = UpperFirst(block.ShortName) + " " + LowerFirst(block.ShortName) + @" = new " + UpperFirst(block.ShortName) + @"();";
            output.Add(line);
            for (int count = 0; count < block.Properties.Count; count++)
            {
                string type = block.Properties[count].Type;
                string property = block.Properties[count].Property;
                string construct = GenerateConstruct(type, property);
                line = LowerFirst(block.ShortName) + "." + property + " = " + construct;
                output.Add(line);
            }
            line = @"return " + LowerFirst(block.ShortName) + ";";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate relevant line construct for given type.
        /// </summary>
        /// <param name="type">Property type.</param>
        /// <param name="property">Property name.</param>
        /// <returns>Construct to use for mock field value.</returns>
        private string GenerateConstruct(string type, string property)
        {
            string construct = string.Empty;
            type = type.Trim();
            if (type.EndsWith("?"))
            {
                type = type.Replace("?", string.Empty);
            }
            if (type == "string")
            {
                construct = @"""" + property + @" "" + digits;";
            }
            else if (type == "DateTime")
            {
                construct = @"GetMockDateTimeToday(seed);";
            }
            else if (type == "bool")
            {
                construct = @"GetMockBoolean(seed);";
            }
            else if (type.StartsWith("decimal"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("double"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("single"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("byte"))
            {
                construct = @"GetMockByte(seed);";
            }
            else if (type.StartsWith("long"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("int"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("short"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("ulong"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("uint"))
            {
                construct = @"seed;";
            }
            else if (type.StartsWith("ushort"))
            {
                construct = @"seed;";
            }
            else
            {
                construct = @"GetMock" + type + @"(seed);";
            }
            return construct;
        }
        #endregion
    }
}