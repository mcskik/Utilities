using System.Collections.Generic;

namespace Same8.Models
{
    /// <summary>
    /// Fuzzy matching pairs class.
    /// </summary>
    /// <remarks>
    /// Typesafe collection of MatchingPair objects.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class MatchingPairs : Dictionary<string, MatchingPair>
    {
        public MatchingPairs()
        {
        }
    }
}