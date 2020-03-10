using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Compare same method generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class CompareSameGenerator : BaseGenerator
    {
        #region Member variables.
        private EntityMetaBlock _entityBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public CompareSameGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import one set of property declarations.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate compare same method from imported properties.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import one set of property declarations.
        /// </summary>
        private void ImportLines()
        {
            int lineNumber = 0;
            _entityBlock = ImportProperties(ref lineNumber);
        }

        /// <summary>
        /// Generate compare same method from imported properties.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"/// <summary>";
            output.Add(line);
            line = @"/// Check if existing and new " + _entityBlock.Name + " entities are the same.";
            output.Add(line);
            line = @"/// </summary>";
            output.Add(line);
            line = @"/// <param name=""existingEntity"">Existing entity.</param>";
            output.Add(line);
            line = @"/// <param name=""newEntity"">New entity.</param>";
            output.Add(line);
            line = @"/// <returns>True if both entities are the same.</returns>";
            output.Add(line);
            line = @"private bool " + _entityBlock.Name + "AreTheSame(" + _entityBlock.Name + " existingEntity, " + _entityBlock.Name + " newEntity)";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"bool same = true;";
            output.Add(line);
            foreach (Vector vector in _entityBlock.Properties)
            {
                line = @"same &= (existingEntity." + vector.Property + " == newEntity." + vector.Property + ");";
                output.Add(line);
            }
            line = @"return same;";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}