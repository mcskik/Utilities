using System;
using System.Collections;
using System.Text;

namespace Notepod.Views
{
	/// <summary>
	/// Text editor parameters form.
	/// </summary>
	/// <author>Kenneth McSkimming</author>
	partial class frmSplitJoinParameters
	{
		#region Controls.

        private System.Windows.Forms.TextBox txtDelimiter;
        private System.Windows.Forms.Label lblDelimiter;
        private System.Windows.Forms.Button cmdGo;
		#endregion

		#region System.
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblDelimiter = new System.Windows.Forms.Label();
            this.cmdGo = new System.Windows.Forms.Button();
            this.txtDelimiter = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblDelimiter
            // 
            this.lblDelimiter.Location = new System.Drawing.Point(8, 3);
            this.lblDelimiter.Name = "lblDelimiter";
            this.lblDelimiter.Size = new System.Drawing.Size(80, 16);
            this.lblDelimiter.TabIndex = 1;
            this.lblDelimiter.Text = "Delimiter";
            // 
            // cmdGo
            // 
            this.cmdGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGo.Location = new System.Drawing.Point(102, 16);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(88, 24);
            this.cmdGo.TabIndex = 8;
            this.cmdGo.Text = "GO";
            this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
            // 
            // txtDelimiter
            // 
            this.txtDelimiter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDelimiter.Location = new System.Drawing.Point(8, 19);
            this.txtDelimiter.Name = "txtDelimiter";
            this.txtDelimiter.Size = new System.Drawing.Size(88, 20);
            this.txtDelimiter.TabIndex = 0;
            // 
            // frmSplitJoinParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 50);
            this.Controls.Add(this.cmdGo);
            this.Controls.Add(this.lblDelimiter);
            this.Controls.Add(this.txtDelimiter);
            this.Name = "frmSplitJoinParameters";
            this.Text = "Parameters";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

