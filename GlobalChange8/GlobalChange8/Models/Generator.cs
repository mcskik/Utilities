using System.Collections.Generic;
using System.Text;

namespace GlobalChange8.Models
{
    public class Generator
    {
        protected string _modelClassName;
        protected List<string> _inputLines;
        protected SortedDictionary<string, string> _apiToModelClassNameMapping;
        protected Vectors _vectors;

        public Generator(string modelClassName, List<string> inputLines, SortedDictionary<string, string> apiToModelClassNameMapping)
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
                                        vector.Property = vector.Variable.Substring(0, 1).ToUpper() + vector.Variable.Substring(1);
                                    }
                                    vector.Summary = ProperCase(DescriptionFromCamelCase(vector.Property)) + ".";
                                }
                            }
                        }
                        block.MemberVariables.Add(vector);
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
            string innerType = Type.Replace("ArrayList<", string.Empty);
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
            line = @"  public " + block.Name + "(" + apiClassName + " " + apiObjectName + ") {";
            output.Add(line);
            line = @"    this();";
            output.Add(line);
            line = string.Format(@"    if ({0} != null)", apiObjectName) + " {";
            output.Add(line);
            for (int count = 0; count < block.MemberVariables.Count; count++)
            {
                string Type = block.MemberVariables[count].Type;
                string originalType = Type;
                string originalInnerType = InnerType(originalType);
                Type = TransformType(Type);
                string innerType = InnerType(Type);
                string property = block.MemberVariables[count].Property;
                string value = string.Format(@"{0}.get{1}()", apiObjectName, property);
                string type = block.MemberVariables[count].Type.ToLower();
                if (type == "boolean")
                {
                    value = string.Format(@"Boolean.valueOf(true).equals({0})", value);
                }
                else if (type == "string")
                {
                }
                else if (type == "integer")
                {
                    type = "int";
                }
                else if (type == "int")
                {
                }
                else if (type == "long")
                {
                }
                else
                {
                    value = "new " + innerType + "(" + value + ")";
                }
                string variable = block.MemberVariables[count].Variable;
                if (innerType == Type)
                {
                    line = "      this." + variable + " = " + value + ";";
                    output.Add(line);
                }
                else
                {
                    line = "      this." + variable + " = new ArrayList<" + innerType + ">();";
                    output.Add(line);
                    line = string.Format(@"      if ({0}.get{1}() != null)", apiObjectName, property) + " {";
                    output.Add(line);
                    line = string.Format(@"        for ({0} item : {1}.get{2}())", originalInnerType, apiObjectName, property) + " {";
                    output.Add(line);
                    line = string.Format(@"          this.{0}.add(new {1}(item));", variable, innerType);
                    output.Add(line);
                    line = "        }";
                    output.Add(line);
                    line = "      }";
                    output.Add(line);
                }
            }
            line = @"    }";
            output.Add(line);
            line = @"  }";
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