namespace GlobalChange8.Models
{
    /// <summary>
    /// Edit rule class.
    /// </summary>
    /// <remarks>
    /// One edit rule.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class EditRule
    {
        public string Find { get; set; }
        public string Replacement { get; set; }
        public int SequenceNumber { get; set; }
        public string Order
        {
            get
            {
                string part1 = (1000000 - Find.Length).ToString().Trim().PadLeft(7, '0');
                string part2 = SequenceNumber.ToString().Trim().PadLeft(7, '0');
                return part1 + part2;
            }
        }

        public EditRule(string find, string replacement, int sequenceNumber)
        {
            Find = find;
            Replacement = replacement;
            SequenceNumber = sequenceNumber;
        }
    }
}