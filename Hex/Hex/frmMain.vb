Imports System.IO
''' <summary>
''' Main Form.
''' </summary>
''' <remarks>
''' Main Form.
''' </remarks>
''' <author>Ken McSkimming</author>
Public Class frmMain
#Region "Memver Variables."
	Private WithEvents _HexDumper As Hex.HexDumper
#End Region
#Region "Event Handlers"
	Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		txtInputFile.Text = Hex.ProfWork.SourceFile
		txtOutputFile.Text = Hex.ProfWork.TargetFile
		txtMarkHex.Text = Hex.ProfWork.MarkHex
		txtWithChar.Text = Hex.ProfWork.WithChar
        txtOccurrences.Text = Hex.ProfWork.Occurrences.ToString()
        txtStopAfter.Text = Hex.ProfWork.StopAfter.ToString()
	End Sub

	Private Sub txtInputFile_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtInputFile.DoubleClick
		txtInputFile.Text = BrowseFile(Hex.ProfSyst.SourcePath, txtInputFile.Text)
		txtInputFile.SelectionStart = txtInputFile.Text.Length - 1
		txtInputFile.ScrollToCaret()
		If File.Exists(txtInputFile.Text) Then
			txtOutputFile.Text = Hex.ProfSyst.TargetPath & Path.DirectorySeparatorChar.ToString() & Path.GetFileNameWithoutExtension(txtInputFile.Text) & ".hex"
			txtOutputFile.SelectionStart = txtOutputFile.Text.Length - 1
			txtOutputFile.ScrollToCaret()
		End If
	End Sub

	Private Sub txtOutputFile_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOutputFile.DoubleClick
		txtOutputFile.Text = BrowseFile(Hex.ProfSyst.TargetPath, txtOutputFile.Text)
		txtOutputFile.SelectionStart = txtOutputFile.Text.Length - 1
		txtOutputFile.ScrollToCaret()
	End Sub

	''' <summary>
	''' Create hex dump.
	''' </summary>
	Private Sub cmdGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGo.Click
		Dim nOccurrences As Long
        Dim nStopAfter As Long
        If txtInputFile.Text.Trim() = vbNullString Then
            txtInputFile.Text = BrowseFile(Hex.ProfSyst.SourcePath, txtInputFile.Text)
        End If
		If Trim(txtOutputFile.Text) = vbNullString Then
			txtOutputFile.Text = BrowseFile(Hex.ProfSyst.TargetPath, txtOutputFile.Text)
		End If
		If txtInputFile.Text.Trim() <> vbNullString And _
		   txtOutputFile.Text.Trim() <> vbNullString Then
            txtMarkHex.Text = txtMarkHex.Text.ToUpper()
            Hex.ProfWork.SourceFile = txtInputFile.Text
			Hex.ProfWork.TargetFile = txtOutputFile.Text
			Hex.ProfWork.MarkHex = txtMarkHex.Text
			Hex.ProfWork.WithChar = txtWithChar.Text
            Hex.ProfWork.StopAfter = txtStopAfter.Text
            nStopAfter = Hex.ProfWork.StopAfter
            staToolStripStatusLabel.Text = vbNullString
            Hex.ProfWork.Save()
            nOccurrences = 0
			txtContents.Text = vbNullString
			_HexDumper = New Hex.HexDumper()
			AddHandler _HexDumper.OnFinished, AddressOf Me._HexDumper_OnTheEnd
            _HexDumper.Dump(txtInputFile.Text, txtOutputFile.Text, txtMarkHex.Text, txtWithChar.Text, nOccurrences, nStopAfter)
			txtContents.Text = _HexDumper.HexData
			RemoveHandler _HexDumper.OnFinished, AddressOf Me._HexDumper_OnTheEnd
			_HexDumper = Nothing
			txtOccurrences.Text = nOccurrences.ToString().Trim()
			Hex.ProfWork.Occurrences = nOccurrences
            Hex.ProfWork.Save()
            Call View(txtOutputFile.Text)
		Else
			staToolStripStatusLabel.Text = "Must specify input and output files"
		End If
	End Sub

	Private Sub _HexDumper_OnFinished(ByVal sender As Object, ByVal e As Hex.HexDumper.ProgressEndEventArgs) Handles _HexDumper.OnFinished
		'MsgBox("Finished " & e.ActualTotalSize)
	End Sub

	Private Sub _HexDumper_OnTheEnd(ByVal sender As Object, ByVal e As Hex.HexDumper.ProgressEndEventArgs)
		'MsgBox("The End " & e.ActualTotalSize)
	End Sub

	Private Sub _HexDumper_OnProgressStart(ByVal e As Hex.HexDumper.ProgressStartEventArgs) Handles _HexDumper.OnProgressStart
		pbrProgressBar.Minimum = 0
		pbrProgressBar.Maximum = e.EstimatedTotalSize
		staToolStripStatusLabel.Text = vbNullString
	End Sub

	Private Sub _HexDumper_OnProgressUpdate(ByVal e As Hex.HexDumper.ProgressUpdateEventArgs) Handles _HexDumper.OnProgressUpdate
		If pbrProgressBar.Value + e.SizeIncrement > pbrProgressBar.Maximum Then
			pbrProgressBar.Maximum *= 2
		End If
		pbrProgressBar.Value += e.SizeIncrement
		staToolStripStatusLabel.Text = e.LineData
	End Sub

	Private Sub _HexDumper_OnProgressEnd(ByVal e As Hex.HexDumper.ProgressEndEventArgs) Handles _HexDumper.OnProgressEnd
		pbrProgressBar.Value = pbrProgressBar.Maximum
		staToolStripStatusLabel.Text = "Completed"
	End Sub
#End Region

#Region "Private Methods."
	''' <summary>
	''' File browser.
	''' </summary>
	''' <param name="psBase">Base directory to start from.</param>
	''' <param name="psDefault">Default file to use if no file selected.</param>
	''' <returns>Selected or default file.</returns>
	Private Function BrowseFile(ByVal psBase As String, ByVal psDefault As String) As String
		Dim sFile As String = psDefault
		Dim dlgFile As OpenFileDialog = New OpenFileDialog()
		dlgFile.InitialDirectory = psBase
		dlgFile.AddExtension = True
		dlgFile.CheckPathExists = True
		dlgFile.CheckFileExists = True
		dlgFile.DefaultExt = "txt"
		dlgFile.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Word files (*.doc)|*.doc"
		dlgFile.FilterIndex = 0
		Dim oDr As DialogResult = dlgFile.ShowDialog()
		If oDr = DialogResult.OK Then
			sFile = dlgFile.FileName
		End If
		Return sFile
	End Function

	''' <summary>
	''' View specified file.
	''' </summary>
	Private Sub View(ByVal fileSpec As String)
		Dim oProc As System.Diagnostics.Process = New System.Diagnostics.Process()
		oProc.StartInfo.FileName = Hex.ProfSyst.ViewerUnix
		oProc.StartInfo.Arguments = fileSpec
		oProc.EnableRaisingEvents = False
		oProc.StartInfo.UseShellExecute = True
		oProc.StartInfo.CreateNoWindow = False
		oProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized
		oProc.Start()
	End Sub
#End Region
End Class
