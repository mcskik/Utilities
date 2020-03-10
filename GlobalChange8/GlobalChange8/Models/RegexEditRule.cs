
namespace GlobalChange8.Models
{
    /// <summary>
    /// Regex Edit Rule class.
    /// </summary>
    /// <remarks>
    /// One Regex Edit Rule.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class RegexEditRule
    {
        public string Order
        {
            get
            {
                string part1 = (1000000 - FindPrefix.Length).ToString().Trim().PadLeft(6, '0');
                string part2 = SequenceNumber.ToString().Trim().PadLeft(4, '0');
                return part1 + part2;
            }
        }
        public string FindPrefix { get; set; }
        public string FindSuffix { get; set; }
        public string Replacement { get; set; }
        public int SequenceNumber { get; set; }

        public RegexEditRule(string findPrefix, string findSuffix, string replacement, int sequenceNumber)
        {
            FindPrefix = findPrefix;
            FindSuffix = findSuffix;
            Replacement = replacement;
            SequenceNumber = sequenceNumber;
        }
    }
}