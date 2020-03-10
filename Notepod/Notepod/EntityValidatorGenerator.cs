using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Entity validator generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class EntityValidatorGenerator : BaseGenerator
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
        public EntityValidatorGenerator(string[] inputLines) : base(inputLines)
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
        /// Generate entity validator methods from imported declarations.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            const string RESPONSE = "response";
            const string ENTITY = "entity";
            string description = DescriptionFromCamelCase(_firstBlock.ShortName);
            //Decide whether to generate an entity validator or a response validator.
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
            return GenerateLines(kind);

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
        /// Generate entity validator methods from imported declarations.
        /// </summary>
        /// <param name="kind">"entity" or "response".</param>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines(string kind)
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = "/// <summary>";
            output.Add(line);
            line = "/// Validate expected " + kind + " list against actual " + kind + " list.";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = @"/// <param name=""expectedList"">Expected " + kind + " list.</param>";
            output.Add(line);
            line = @"/// <param name=""actualList"">Actual " + kind + " list.</param>";
            output.Add(line);
            line = "private void Validate" + _firstBlock.ShortName + "List(List<" + _secondBlock.Name + "> expectedList, List<" + _firstBlock.Name + "> actualList)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "for (int count = 0; count < expectedList.Count; count++)";
            output.Add(line);
            line = "{";
            output.Add(line);
            line = "Validate" + _firstBlock.ShortName + "(expectedList[count], actualList[count]);";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = "}";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = "/// <summary>";
            output.Add(line);
            line = "/// Validate expected " + kind + " against actual " + kind + ".";
            output.Add(line);
            line = "/// </summary>";
            output.Add(line);
            line = @"/// <param name=""expected" + ProperCase(kind) + @""">Expected " + kind + ".</param>";
            output.Add(line);
            line = @"/// <param name=""actual" + ProperCase(kind) + @""">Actual " + kind + ".</param>";
            output.Add(line);
            line = "private void Validate" + _firstBlock.ShortName + "(" + _secondBlock.Name + " expected" + ProperCase(kind) + ", " + _firstBlock.Name + " actual" + ProperCase(kind) + ")";
            output.Add(line);
            line = "{";
            output.Add(line);
            for (int count = 0; count < _secondBlock.Properties.Count; count++)
            {
                string secondItem = _secondBlock.Properties[count].Property;
                string firstItem = _firstBlock.Properties[count].Property;
                line = "Assert.AreEqual(expected" + ProperCase(kind) + "." + secondItem + ", actual" + ProperCase(kind) + "." + firstItem + ");";
                output.Add(line);
            }
            line = "}";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}