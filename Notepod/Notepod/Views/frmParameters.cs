using System;
using System.Windows.Forms;

namespace Notepod.Views
{
    /// <summary>
    /// Text editor parameters form.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public partial class frmParameters : Form
    {
        #region Properties.
        /// <summary>
        /// Swap at.
        /// </summary>
        public string SwapAt
        {
            get
            {
                return txtSwapAt.Text;
            }
            set
            {
                txtSwapAt.Text = value;
            }
        }

        /// <summary>
        /// Replace.
        /// </summary>
        public string Replace
        {
            get
            {
                return txtReplace.Text;
            }
            set
            {
                txtReplace.Text = value;
            }
        }

        /// <summary>
        /// With.
        /// </summary>
        public string With
        {
            get
            {
                return txtWith.Text;
            }
            set
            {
                txtWith.Text = value;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmParameters()
        {
            InitializeComponent();
            txtSwapAt.Text = string.Empty;
            txtReplace.Text = string.Empty;
            txtWith.Text = string.Empty;
        }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public frmParameters(string swapAt, string replace, string with)
        {
            InitializeComponent();
            txtSwapAt.Text = swapAt;
            txtReplace.Text = replace;
            txtWith.Text = with;
        }
        #endregion

        #region Event handlers.
        /// <summary>
        /// Form activate.
        /// </summary>
        private void frmMain_Activated(object sender, System.EventArgs e)
        {
            frmMain_Resize(sender, e);
        }

        /// <summary>
        /// Form resize.
        /// </summary>
        private void frmMain_Resize(object sender, System.EventArgs e)
        {
            const int HGAP = 10;
            const int VGAP = 15;

            System.Windows.Forms.Label oLblSwapAt;
            System.Windows.Forms.Label oLblReplace;
            System.Windows.Forms.Label oLblWith;
            System.Windows.Forms.TextBox oTxtSwapAt;
            System.Windows.Forms.TextBox oTxtReplace;
            System.Windows.Forms.TextBox oTxtWith;
            System.Windows.Forms.Button oCmdGo;

            oLblSwapAt = this.lblSwapAt;
            oLblReplace = this.lblReplace;
            oLblWith = this.lblWith;
            oTxtSwapAt = this.txtSwapAt;
            oTxtReplace = this.txtReplace;
            oTxtWith = this.txtWith;
            oCmdGo = this.cmdGo;

            oTxtSwapAt.Left = HGAP;
            oLblSwapAt.Left = HGAP;
            oTxtReplace.Left = oTxtSwapAt.Left + oTxtSwapAt.Width + HGAP;
            oTxtReplace.Width = (this.Width - oTxtSwapAt.Width - (int)((double)HGAP * 4.7)) / 2;
            oLblReplace.Left = oTxtReplace.Left;
            oTxtWith.Left = oTxtReplace.Left + oTxtReplace.Width + HGAP;
            oTxtWith.Width = oTxtReplace.Width;
            oLblWith.Left = oTxtWith.Left;
            oCmdGo.Top = oTxtSwapAt.Top + oTxtSwapAt.Height + VGAP;
            oCmdGo.Left = oTxtWith.Left + oTxtWith.Width - oCmdGo.Width;
        }

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