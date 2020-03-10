
namespace GlobalChange8.Models
{
    /// <summary>
    /// Search argument class.
    /// </summary>
    /// <remarks>
    /// One search argument.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Argument
    {
        #region Member variables.
        private bool _exclude = false;
        private string _text = string.Empty;
        #endregion

        #region Properties.
        /// <summary>
        /// Exclude.
        /// </summary>
        public bool Exclude
        {
            get
            {
                return _exclude;
            }
            set
            {
                _exclude = value;
            }
        }

        /// <summary>
        /// Text.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        #endregion
    }
}