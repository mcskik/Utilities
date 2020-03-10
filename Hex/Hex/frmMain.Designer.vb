<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.txtInputFile = New System.Windows.Forms.TextBox()
        Me.txtOutputFile = New System.Windows.Forms.TextBox()
        Me.txtMarkHex = New System.Windows.Forms.TextBox()
        Me.txtWithChar = New System.Windows.Forms.TextBox()
        Me.txtOccurrences = New System.Windows.Forms.TextBox()
        Me.txtContents = New System.Windows.Forms.TextBox()
        Me.cmdGo = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.pbrProgressBar = New System.Windows.Forms.ProgressBar()
        Me.staStatusStrip = New System.Windows.Forms.StatusStrip()
        Me.staToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.txtStopAfter = New System.Windows.Forms.TextBox()
        Me.staStatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtInputFile
        '
        Me.txtInputFile.Location = New System.Drawing.Point(12, 12)
        Me.txtInputFile.Name = "txtInputFile"
        Me.txtInputFile.Size = New System.Drawing.Size(218, 20)
        Me.txtInputFile.TabIndex = 0
        '
        'txtOutputFile
        '
        Me.txtOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOutputFile.Location = New System.Drawing.Point(237, 12)
        Me.txtOutputFile.Name = "txtOutputFile"
        Me.txtOutputFile.Size = New System.Drawing.Size(218, 20)
        Me.txtOutputFile.TabIndex = 1
        '
        'txtMarkHex
        '
        Me.txtMarkHex.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMarkHex.Location = New System.Drawing.Point(462, 12)
        Me.txtMarkHex.Name = "txtMarkHex"
        Me.txtMarkHex.Size = New System.Drawing.Size(41, 20)
        Me.txtMarkHex.TabIndex = 2
        '
        'txtWithChar
        '
        Me.txtWithChar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWithChar.Location = New System.Drawing.Point(510, 12)
        Me.txtWithChar.Name = "txtWithChar"
        Me.txtWithChar.Size = New System.Drawing.Size(41, 20)
        Me.txtWithChar.TabIndex = 3
        '
        'txtOccurrences
        '
        Me.txtOccurrences.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOccurrences.Location = New System.Drawing.Point(558, 12)
        Me.txtOccurrences.Name = "txtOccurrences"
        Me.txtOccurrences.Size = New System.Drawing.Size(41, 20)
        Me.txtOccurrences.TabIndex = 4
        '
        'txtContents
        '
        Me.txtContents.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtContents.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtContents.Location = New System.Drawing.Point(12, 38)
        Me.txtContents.Multiline = True
        Me.txtContents.Name = "txtContents"
        Me.txtContents.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtContents.Size = New System.Drawing.Size(633, 288)
        Me.txtContents.TabIndex = 6
        '
        'cmdGo
        '
        Me.cmdGo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdGo.Location = New System.Drawing.Point(570, 332)
        Me.cmdGo.Name = "cmdGo"
        Me.cmdGo.Size = New System.Drawing.Size(75, 23)
        Me.cmdGo.TabIndex = 8
        Me.cmdGo.Text = "&Go"
        Me.cmdGo.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.Location = New System.Drawing.Point(489, 332)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 7
        Me.cmdCancel.Text = "&Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'pbrProgressBar
        '
        Me.pbrProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbrProgressBar.Location = New System.Drawing.Point(12, 362)
        Me.pbrProgressBar.Name = "pbrProgressBar"
        Me.pbrProgressBar.Size = New System.Drawing.Size(633, 23)
        Me.pbrProgressBar.TabIndex = 8
        '
        'staStatusStrip
        '
        Me.staStatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.staToolStripStatusLabel})
        Me.staStatusStrip.Location = New System.Drawing.Point(0, 399)
        Me.staStatusStrip.Name = "staStatusStrip"
        Me.staStatusStrip.Size = New System.Drawing.Size(653, 22)
        Me.staStatusStrip.TabIndex = 9
        Me.staStatusStrip.Text = "StatusStrip1"
        '
        'staToolStripStatusLabel
        '
        Me.staToolStripStatusLabel.Name = "staToolStripStatusLabel"
        Me.staToolStripStatusLabel.Size = New System.Drawing.Size(0, 17)
        '
        'txtStopAfter
        '
        Me.txtStopAfter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStopAfter.Location = New System.Drawing.Point(606, 12)
        Me.txtStopAfter.Name = "txtStopAfter"
        Me.txtStopAfter.Size = New System.Drawing.Size(41, 20)
        Me.txtStopAfter.TabIndex = 5
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(653, 421)
        Me.Controls.Add(Me.txtStopAfter)
        Me.Controls.Add(Me.staStatusStrip)
        Me.Controls.Add(Me.pbrProgressBar)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdGo)
        Me.Controls.Add(Me.txtContents)
        Me.Controls.Add(Me.txtOccurrences)
        Me.Controls.Add(Me.txtWithChar)
        Me.Controls.Add(Me.txtMarkHex)
        Me.Controls.Add(Me.txtOutputFile)
        Me.Controls.Add(Me.txtInputFile)
        Me.MinimumSize = New System.Drawing.Size(500, 200)
        Me.Name = "frmMain"
        Me.Text = "Hex Browser (VB.NET)"
        Me.staStatusStrip.ResumeLayout(False)
        Me.staStatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
	Friend WithEvents txtInputFile As System.Windows.Forms.TextBox
	Friend WithEvents txtOutputFile As System.Windows.Forms.TextBox
	Friend WithEvents txtMarkHex As System.Windows.Forms.TextBox
	Friend WithEvents txtWithChar As System.Windows.Forms.TextBox
	Friend WithEvents txtOccurrences As System.Windows.Forms.TextBox
	Friend WithEvents txtContents As System.Windows.Forms.TextBox
	Friend WithEvents cmdGo As System.Windows.Forms.Button
	Friend WithEvents cmdCancel As System.Windows.Forms.Button
	Friend WithEvents pbrProgressBar As System.Windows.Forms.ProgressBar
	Friend WithEvents staStatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents staToolStripStatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents txtStopAfter As System.Windows.Forms.TextBox

End Class
