namespace Delini8
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.cmdCount = new System.Windows.Forms.Button();
			this.txtLines = new System.Windows.Forms.TextBox();
			this.lblLines = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cmdCount
			// 
			this.cmdCount.Location = new System.Drawing.Point(156, 6);
			this.cmdCount.Name = "cmdCount";
			this.cmdCount.Size = new System.Drawing.Size(75, 23);
			this.cmdCount.TabIndex = 0;
			this.cmdCount.Text = "Count";
			this.cmdCount.UseVisualStyleBackColor = true;
			this.cmdCount.Click += new System.EventHandler(this.cmdCount_Click);
			// 
			// txtLines
			// 
			this.txtLines.Location = new System.Drawing.Point(50, 6);
			this.txtLines.Name = "txtLines";
			this.txtLines.Size = new System.Drawing.Size(100, 20);
			this.txtLines.TabIndex = 1;
			// 
			// lblLines
			// 
			this.lblLines.AutoSize = true;
			this.lblLines.Location = new System.Drawing.Point(12, 9);
			this.lblLines.Name = "lblLines";
			this.lblLines.Size = new System.Drawing.Size(32, 13);
			this.lblLines.TabIndex = 2;
			this.lblLines.Text = "Lines";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(244, 39);
			this.Controls.Add(this.lblLines);
			this.Controls.Add(this.txtLines);
			this.Controls.Add(this.cmdCount);
			this.Name = "Form1";
			this.Text = "Delini8";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCount;
		private System.Windows.Forms.TextBox txtLines;
		private System.Windows.Forms.Label lblLines;
	}
}

