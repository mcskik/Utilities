namespace Same8.Models
{
    /// <summary>
    /// Fuzzy matching pair class.
    /// </summary>
    /// <remarks>
    /// Contains details of the two fuzzy keys which are considered to be the best match for each other.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class MatchingPair
    {
        public string OldKey { get; set; }
        public string NewKey { get; set; }
        public double Score { get; set; }
        public bool Matched { get; set; }

        public MatchingPair()
        {
        }
    }
}