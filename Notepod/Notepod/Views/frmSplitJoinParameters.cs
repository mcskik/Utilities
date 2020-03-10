using System;
using System.Windows.Forms;

namespace Notepod.Views
{
    /// <summary>
    /// Text editor split/join parameters form.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public partial class frmSplitJoinParameters : Form
    {
        #region Properties.
        /// <summary>
        /// Swap at.
        /// </summary>
        public string Delimiter
        {
            get
            {
                return txtDelimiter.Text;
            }
            set
            {
                txtDelimiter.Text = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmSplitJoinParameters()
        {
            InitializeComponent();
            txtDelimiter.Text = string.Empty;
        }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public frmSplitJoinParameters(string delimiter)
        {
            InitializeComponent();
            txtDelimiter.Text = delimiter;
        }
        #endregion

        #region Event handlers.
        /// <summary>
        /// GO.
        /// </summary>
        private void cmdGo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        #endregion
    }
}