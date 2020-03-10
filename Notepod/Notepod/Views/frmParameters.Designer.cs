using System;
using System.Collections;
using System.Text;

namespace Notepod.Views
{
	/// <summary>
	/// Text editor parameters form.
	/// </summary>
	/// <author>Kenneth McSkimming</author>
	partial class frmParameters
	{
		#region Controls.
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.TextBox txtSwapAt;
		private System.Windows.Forms.Label lblSwapAt;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.Label lblWith;
        private System.Windows.Forms.Button cmdGo;
		private System.Windows.Forms.TextBox txtWith;
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
            this.txtWith = new System.Windows.Forms.TextBox();
            this.lblWith = new System.Windows.Forms.Label();
            this.lblReplace = new System.Windows.Forms.Label();
            this.lblSwapAt = new System.Windows.Forms.Label();
            this.cmdGo = new System.Windows.Forms.Button();
            this.txtSwapAt = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtWith
            // 
            this.txtWith.Location = new System.Drawing.Point(336, 19);
            this.txtWith.Name = "txtWith";
            this.txtWith.Size = new System.Drawing.Size(264, 20);
            this.txtWith.TabIndex = 4;
            // 
            // lblWith
            // 
            this.lblWith.Location = new System.Drawing.Point(335, 3);
            this.lblWith.Name = "lblWith";
            this.lblWith.Size = new System.Drawing.Size(80, 16);
            this.lblWith.TabIndex = 5;
            this.lblWith.Text = "With";
            // 
            // lblReplace
            // 
            this.lblReplace.Location = new System.Drawing.Point(104, 3);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(80, 16);
            this.lblReplace.TabIndex = 3;
            this.lblReplace.Text = "Replace";
            // 
            // lblSwapAt
            // 
            this.lblSwapAt.Location = new System.Drawing.Point(8, 3);
            this.lblSwapAt.Name = "lblSwapAt";
            this.lblSwapAt.Size = new System.Drawing.Size(80, 16);
            this.lblSwapAt.TabIndex = 1;
            this.lblSwapAt.Text = "Swap At";
            // 
            // cmdGo
            // 
            this.cmdGo.Location = new System.Drawing.Point(512, 45);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(88, 24);
            this.cmdGo.TabIndex = 8;
            this.cmdGo.Text = "GO";
            this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
            // 
            // txtSwapAt
            // 
            this.txtSwapAt.Location = new System.Drawing.Point(8, 19);
            this.txtSwapAt.Name = "txtSwapAt";
            this.txtSwapAt.Size = new System.Drawing.Size(88, 20);
            this.txtSwapAt.TabIndex = 0;
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(104, 19);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(224, 20);
            this.txtReplace.TabIndex = 2;
            // 
            // frmParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 82);
            this.Controls.Add(this.cmdGo);
            this.Controls.Add(this.lblWith);
            this.Controls.Add(this.txtWith);
            this.Controls.Add(this.lblReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.lblSwapAt);
            this.Controls.Add(this.txtSwapAt);
            this.Name = "frmParameters";
            this.Text = "Parameters";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

