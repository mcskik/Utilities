
namespace Search8.Models
{
    /// <summary>
    /// Block vector class.
    /// </summary>
    /// <remarks>
    /// Used with method blocks.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class BlockVector
    {
        #region Properties.
        public int BlockNumber { get; set; }
        public int StartLinekNumber { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public string Content { get; set; }
        #endregion
    }
}