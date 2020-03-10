using System.Collections;

namespace Copy8
{
    /// <summary>
    /// Generic type safe collection of comparison entries.
    /// </summary>
    /// <remarks>
    /// Generic type safe collection of comparison entries.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Comparisons<Template> : CollectionBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Comparisons()
        {
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">Index number.</param>
        /// <returns>A comparison entry.</returns>
        public Template this[int index]
        {
            get { return (Template)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Add a comparison entry.
        /// </summary>
        /// <param name="value">Comparison entry object.</param>
        /// <returns>Index number of new entry.</returns>
        public int Add(Template value)
        {
            return List.Add(value);
        }
    }
}