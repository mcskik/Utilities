using System.Collections.Generic;
using System.Text;

namespace GlobalChange8.Models
{
    public class GeneratorKotlin
    {
        protected string _modelClassName;
        protected List<string> _inputLines;
        protected SortedDictionary<string, string> _apiToModelClassNameMapping;
        protected Vectors _vectors;

        public GeneratorKotlin(string modelClassName, List<string> inputLines, SortedDictionary<string, string> apiToModelClassNameMapping)
        {
            _modelClassName = modelClassName;
            _inputLines = inputLines;
            _apiToModelClassNameMapping = apiToModelClassNameMapping;
            _vectors = new Vectors();
        }

        /// <summary>
        /// Import member variable declarations from top of API interface class.
        /// </summary>
        /// <param name="lineNumber">Enters as starting line number, exits as next starting line number.</param>
        /// <returns>Entity meta block for home entity.</returns>
        public EntityMetaBlock ImportMemberVariables()
        {
            EntityMetaBlock block = new EntityMetaBlock();
            block.Name = _modelClassName;
            Vector vector = null;
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            int nEol = 0;
            string prevLineSummary = string.Empty;
            foreach (string inputLine in _inputLines)
            {
                line = inputLine.Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//"))
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("/*") && trimmedLine.EndsWith("*/"))
                        {
                            prevLineSummary = trimmedLine;
                        }
                        else if (line.StartsWith("val"))
                        {
                            vector = new Vector();
                            nPos1 = line.IndexOf("val");
                            if (nPos1 > -1)
                            {
                                nPos1 = line.IndexOf(' ', nPos1 + 1);
                                if (nPos1 > -1)
                                {
                                    nPos2 = line.IndexOf(':', nPos1 + 1);
                                    if (nPos2 > -1)
                                    {
                                        vector.Modifier = string.Empty;
                                        vector.Variable = line.Substring(nPos1, nPos2 - nPos1).Trim();
                                        vector.Property = vector.Variable;
                                        nPos2++;
                                        nEol = line.Length;
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
                                            nPos1 = line.IndexOf(',', nPos2 + 1);
                                            if (nPos1 > -1)
                                            {
                                                if (nPos1 < nEol)
                                                {
                                                    nEol = nPos1;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (nPos1 > -1)
                                {
                                    vector.Type = line.Substring(nPos2, nPos1 - nPos2).Trim();
                                    vector.Type = StripType(vector.Type);
                                    vector.ApiType = vector.Type;
                                    vector.Type = TransformType(vector.Type);
                                    vector.Summary = prevLineSummary;
                                    prevLineSummary = string.Empty;
                                    string Type = vector.Type;
                                    //Type = TransformType(Type);
                                    string type = vector.Type.ToLower();
                                    string value = Type + "()";
                                    if (Type.StartsWith("Array<"))
                                    {
                                        vector.Type = vector.Type.Replace("Array", "MutableList");
                                        Type = vector.Type;
                                        value = "mutableListOf()";
                                    }
                                    if (Type.StartsWith("List<"))
                                    {
                                        vector.Type = vector.Type.Replace("List", "MutableList");
                                        Type = vector.Type;
                                        value = "mutableListOf()";
                                    }
                                    if (type == "boolean")
                                    {
                                        value = "false";
                                    }
                                    else if (type == "string")
                                    {
                                        value = "Constants.EMPTY_STRING";
                                    }
                                    else if (type == "integer")
                                    {
                                        type = "int";
                                        value = "Constants.ZERO";
                                    }
                                    else if (type == "int")
                                    {
                                        value = "Constants.ZERO";
                                    }
                                    else if (type == "long")
                                    {
                                        value = "Constants.ZERO";
                                    }
                                    vector.DefaultValue = value;
                                    block.MemberVariables.Add(vector);
                                }
                            }
                        }
                    }
                }
            }
            return block;
        }

        /// <summary>
        /// Generate default constructor.
        /// </summary>
        public List<string> GenerateDefaultConstructor(EntityMetaBlock block)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            output.Add(line);
            line = @"  public " + block.Name + "() {";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string Type = block.MemberVariables[count].Type;
                Type = TransformType(Type);
                string type = block.MemberVariables[count].Type.ToLower();
                string value = "new " + Type + "()";
                if (Type.StartsWith("List<"))
                {
                    value = "new Array" + Type + "()";
                }
                if (type == "boolean")
                {
                    value = "false";
                }
                else if (type == "string")
                {
                    value = "Constants.EMPTY_STRING";
                }
                else if (type == "integer")
                {
                    type = "int";
                    value = "Constants.ZERO";
                }
                else if (type == "int")
                {
                    value = "Constants.ZERO";
                }
                else if (type == "long")
                {
                    value = "Constants.ZERO";
                }
                string variable = block.MemberVariables[count].Variable;
                line = "    this." + variable + " = " + value + ";";
                output.Add(line);
            }
            line = @"  }";
            output.Add(line);
            return output;
        }

        private string StripType(string Type)
        {
            Type = Type.Replace("kotlin.", string.Empty);
            Type = Type.Replace("?", string.Empty);
            return Type;
        }

        private string TransformType(string Type)
        {
            string innerType = InnerType(Type);
            if (_apiToModelClassNameMapping.ContainsKey(innerType))
            {
                string modelClassName = _apiToModelClassNameMapping[innerType];
                Type = Type.Replace(innerType, modelClassName);
            }
            return Type;
        }

        private string InnerType(string Type)
        {
            string innerType = Type.Replace("Mutable", string.Empty);
            innerType = innerType.Replace("ArrayList<", string.Empty);
            innerType = innerType.Replace("Array<", string.Empty);
            innerType = innerType.Replace("List<", string.Empty);
            innerType = innerType.Replace(">", string.Empty);
            return innerType;
        }

        /// <summary>
        /// Generate parameterised parcel constructor.
        /// </summary>
        public List<string> GenerateApiObjectConstructor(EntityMetaBlock block)
        {
            string apiClassName = _modelClassName.Replace("ModelState", string.Empty);
            string apiObjectName = apiClassName.Substring(0, 1).ToLower() + apiClassName.Substring(1);
            List<string> output = new List<string>();
            string line = string.Empty;
            output.Add(line);
            line = @"    constructor(" + apiObjectName + ": " + apiClassName + "?) : this() {";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string Type = block.MemberVariables[count].Type;
                string originalType = Type;
                string originalInnerType = InnerType(originalType);
                Type = TransformType(Type);
                string innerType = InnerType(Type);
                string property = block.MemberVariables[count].Property;
                string value = string.Format(@"{0}?.{1}", apiObjectName, property);
                string type = block.MemberVariables[count].Type.ToLower();
                string defaultValue = "Constants.EMPTY_STRING";
                if (type == "boolean")
                {
                    defaultValue = "false";
                }
                else if (type == "string")
                {
                    defaultValue = "Constants.EMPTY_STRING";
                }
                else if (type == "int")
                {
                    defaultValue = "Constants.ZERO";
                }
                else if (type == "long")
                {
                    defaultValue = "Constants.ZERO.toLong()";
                }
                else
                {
                    value = innerType + "(" + value + ")";
                }
                string variable = block.MemberVariables[count].Variable;
                if (innerType == Type)
                {
                    line = "        " + variable + " = " + value + " ?: " + defaultValue;
                    output.Add(line);
                }
                else
                {
                    line = "        " + variable + " = mutableListOf()";
                    output.Add(line);
                    line = string.Format(@"        {0}?.{1}?.forEach", apiObjectName, property) + " {";
                    output.Add(line);
                    line = "          " + variable + ".add(" + innerType + "(it))";
                    output.Add(line);
                    line = "        }";
                    output.Add(line);
                }
            }
            line = @"    }";
            output.Add(line);
            return output;
        }

        /// <summary>
        /// Generate parameterised parcel constructor.
        /// </summary>
        public List<string> GenerateParameterisedParcelConstructor(EntityMetaBlock block)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            output.Add(line);
            line = @"  @ParcelConstructor";
            output.Add(line);
            line = @"  public " + block.Name + "(";
            string separator = string.Empty;
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string Type = block.MemberVariables[count].Type;
                Type = TransformType(Type);
                string type = block.MemberVariables[count].Type.ToLower();
                if (type == "integer")
                {
                    type = "int";
                }
                string parameter = block.MemberVariables[count].Variable;
                if (parameter.StartsWith("_"))
                {
                    parameter = parameter.Substring(1);
                }
                line += separator + Type + " " + parameter;
                separator = ", ";
            }
            line += ") {";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string variable = block.MemberVariables[count].Variable;
                string parameter = block.MemberVariables[count].Variable;
                if (parameter.StartsWith("_"))
                {
                    parameter = parameter.Substring(1);
                }
                line = "    this." + variable + " = " + parameter + ";";
                output.Add(line);
            }
            line = @"  }";
            output.Add(line);
            return output;
        }

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
    }
}