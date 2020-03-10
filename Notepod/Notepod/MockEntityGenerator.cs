using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Mock entity generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class MockEntityGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _firstBlock;
        private EntityMetaBlock _secondBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public MockEntityGenerator(string[] inputLines) : base(inputLines)
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
            _secondBlock = ImportProperties(ref lineNumber);
        }

        /// <summary>
        /// Generate mock entity methods from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            const string RESPONSE = "response";
            const string ENTITY = "entity";
            string description = DescriptionFromCamelCase(_firstBlock.ShortName);
            //Decide whether to generate a mock entity or response.
            string[] parts = description.Split(' ');
            string kind = ENTITY;
            if (parts.Length > 0)
            {
                if (parts[parts.Length - 1] == RESPONSE)
                {
                    kind = RESPONSE;
                }
                else
                {
                    kind = ENTITY;
                }
            }
            //Decide whether to generate one upper method or a pair of synchronised upper and lower methods.
            if (_secondBlock.Properties.Count > 0)
            {
                List<string> upperList = GenerateListBlock("upper", kind, _firstBlock);
                List<string> upper = GenerateBlock("upper", kind, _firstBlock);
                List<string> lowerList = GenerateListBlock("lower", kind, _secondBlock);
                List<string> lower = GenerateBlock("lower", kind, _secondBlock);
                List<string> combined = new List<string>();
                foreach (string line in upperList)
                {
                    combined.Add(line);
                }
                combined.Add(string.Empty);
                foreach (string line in upper)
                {
                    combined.Add(line);
                }
                combined.Add(string.Empty);
                foreach (string line in lowerList)
                {
                    combined.Add(line);
                }
                combined.Add(string.Empty);
                foreach (string line in lower)
                {
                    combined.Add(line);
                }
                return combined.ToArray();
            }
            else
            {
                List<string> upperList = GenerateListBlock("upper", kind, _firstBlock);
                List<string> upper = GenerateBlock("upper", kind, _firstBlock);
                List<string> combined = new List<string>();
                foreach (string line in upperList)
                {
                    combined.Add(line);
                }
                combined.Add(string.Empty);
                foreach (string line in upper)
                {
                    combined.Add(line);
                }
                return combined.ToArray();
            }
        }

        /// <summary>
        /// Generate one mock entities method from one imported declaration.
        /// </summary>
        /// <param name="scope">"upper" or "lower".</param>
        /// <param name="kind">"entity" or "response".</param>
        /// <param name="block">Entity meta block for either of the above.</param>
        /// <returns>List of output lines for this list block.</returns>
        private List<string> GenerateListBlock(string scope, string kind, EntityMetaBlock block)
        {
            string blockName = block.Name;
            string blockShortName = block.ShortName;
            kind = kind.Replace("ntity", "ntities");
            kind = kind.Replace("esponse", "esponses");
            blockName = blockName.Replace("ntity", "ntities");
            blockName = blockName.Replace("esponse", "esponses");
            blockShortName = blockShortName.Replace("ntity", "ntities");
            blockShortName = blockShortName.Replace("esponse", "esponses");
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Mock " + scope.ToLower() + " " + DescriptionFromCamelCase(blockShortName) + ".";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <param name=""quantity"">Quantity of " + kind + " to create.</param>";
            output.Add(line);
            line = @"/// <param name=""seed"">Seed number to seed all properties with.</param>";
            output.Add(line);
            line = @"/// <returns>Mock " + scope.ToLower() + " list of " + DescriptionFromCamelCase(blockShortName) + ".</returns>";
            output.Add(line);
            line = @"public " + blockName + " GetMock" + ProperCase(scope) + blockShortName + "(int quantity, int seed)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = blockName +  " " + kind + " = new " + blockName + "();";
            output.Add(line);
            line = @"for (int count = 0; count < quantity; count++)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = kind + ".Add(GetMock" + ProperCase(scope) + block.ShortName + "(seed + count));";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"return " + kind + ";";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate one mock entity method from one imported declaration.
        /// </summary>
        /// <param name="scope">"upper" or "lower".</param>
        /// <param name="kind">"entity" or "response".</param>
        /// <param name="block">Entity meta block for either of the above.</param>
        /// <returns>List of output lines for this block.</returns>
        private List<string> GenerateBlock(string scope, string kind, EntityMetaBlock block)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Mock " + scope.ToLower() + " " + DescriptionFromCamelCase(block.ShortName) + ".";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <param name=""seed"">Seed number to seed all properties with.</param>";
            output.Add(line);
            line = @"/// <returns>Mock " + scope.ToLower() + " " + DescriptionFromCamelCase(block.ShortName) + ".</returns>";
            output.Add(line);
            line = @"public " + block.Name + " GetMock" + ProperCase(scope) + block.ShortName + "(int seed)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = "string digits = seed.ToString().Trim();";
            output.Add(line);
            line = block.Name + " " + kind + " = new " + block.Name + "();";
            output.Add(line);
            for (int count = 0; count < block.Properties.Count; count++)
            {
                string type = block.Properties[count].Type;
                string property = block.Properties[count].Property;
                string construct = GenerateConstruct(scope, type, property);
                line = kind + "." + property + " = " + construct;
                output.Add(line);
            }
            line = @"return " + kind + ";";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate relevant line construct for given type.
        /// </summary>
        /// <param name="scope">"upper" or "lower".</param>
        /// <param name="type">Property type.</param>
        /// <param name="property">Property name.</param>
        /// <returns>Construct to use for mock field value.</returns>
        private string GenerateConstruct(string scope, string type, string property)
        {
            string construct = string.Empty;
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
                construct = @"GetMock" + ProperCase(scope) + type + @"(seed);";
            }
            return construct;
        }
        #endregion
    }
}