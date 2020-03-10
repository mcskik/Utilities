Imports System.IO
Imports Routines8.Routines

Namespace Hex
	''' <summary>
	''' Hex dumper class.
	''' </summary>
	''' <remarks>
	''' Contains methods to generate a Hex dump representation of the specified input file.
	''' </remarks>
	Public Class HexDumper
#Region "Member Variables."
		Private _hexData As String
#End Region

#Region "Properties."
		Public Property HexData() As String
			Get
				Return _hexData
			End Get
			Set(ByVal value As String)
				_hexData = value
			End Set
		End Property
#End Region

#Region "Custom Event Arguments."
		Public Class ProgressStartEventArgs
			Inherits EventArgs
			Private ReadOnly _estimatedTotalSize As Long
			Public ReadOnly Property EstimatedTotalSize() As Long
				Get
					Return _estimatedTotalSize
				End Get
			End Property
			Public Sub New(ByVal estimatedTotalSize As Long)
				Me._estimatedTotalSize = estimatedTotalSize
			End Sub
		End Class
		Public Class ProgressUpdateEventArgs
			Inherits EventArgs
			Private ReadOnly _sizeIncrement As Long
			Private ReadOnly _lineData As String
			Public ReadOnly Property SizeIncrement() As Long
				Get
					Return _sizeIncrement
				End Get
			End Property
			Public ReadOnly Property LineData() As String
				Get
					Return _lineData
				End Get
			End Property
			Public Sub New(ByVal sizeIncrement As Long, ByVal lineData As String)
				Me._sizeIncrement = sizeIncrement
				Me._lineData = lineData
			End Sub
		End Class
		Public Class ProgressEndEventArgs
			Inherits EventArgs
			Private ReadOnly _actualTotalSize As Long
			Public ReadOnly Property ActualTotalSize() As Long
				Get
					Return _actualTotalSize
				End Get
			End Property
			Public Sub New(ByVal actualTotalSize As Long)
				Me._actualTotalSize = actualTotalSize
			End Sub
		End Class
#End Region

#Region "Delegates."
		Public Delegate Sub ProgressStartHandler(ByVal e As ProgressStartEventArgs)
		Public Delegate Sub ProgressUpdateHandler(ByVal e As ProgressUpdateEventArgs)
		Public Delegate Sub ProgressEndHandler(ByVal e As ProgressEndEventArgs)
#End Region

#Region "Event Declarations."
		Public Event OnProgressStart As ProgressStartHandler
		Public Event OnProgressUpdate As ProgressUpdateHandler
		Public Event OnProgressEnd As ProgressEndHandler
		Public Event OnFinished(ByVal sender As Object, ByVal e As ProgressEndEventArgs)
#End Region

#Region "Private Methods."
		''' <summary>
		''' Create a hex dump of the input file and mark ocurrences of the specified
		''' character with the replacement character.
		''' </summary>
        Public Sub Dump(ByVal psIn As String, ByVal psOut As String, ByVal psMarkHex As String, ByVal psWithChar As String, ByRef pnOccurrences As Long, ByVal pnStopAfter As Long)
            Const MAXFILELEN = 1024 * 1024 * 10
            Const MAXLEN = 512
            Const TABCODE = 9
            Dim sData As String
            Dim sChar As String
            Dim sHex As String
            Dim sEol As String
            Dim sEol1 As String
            Dim sEol2 As String
            Dim nFileLen As Long
            Dim nFilePos As Long
            Dim nLinePos As Long
            Dim sLine0 As String
            Dim sLine1 As String
            Dim sLine2 As String
            Dim nLineCount As Long
            Dim sLineNumber As String
            Dim nOccurrences As Long
            Dim nInNum As Integer
            Dim nOutNum As Integer
            Dim bEol As Boolean
            Dim bShowTabs As Boolean
            If ProfSyst.ShowTabs Then
                bShowTabs = True
            Else
                bShowTabs = False
            End If
            psMarkHex = RPad(psMarkHex, 2)
            _hexData = String.Empty
            nInNum = FreeFile()
            FileOpen(nInNum, psIn, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
            nOutNum = FreeFile()
            FileOpen(nOutNum, psOut, OpenMode.Output, OpenAccess.Write, OpenShare.LockWrite)
            Dim fileLength As Long = LOF(nInNum)
            If fileLength > MAXFILELEN Then
                fileLength = MAXFILELEN
            End If
            If pnStopAfter = 0 Then
                pnStopAfter = fileLength
            End If
            If fileLength > pnStopAfter Then
                fileLength = pnStopAfter
            End If
            sData = Space(fileLength)
            FileGet(nInNum, sData)
            FileClose(nInNum)
            bEol = False
            nLinePos = 0
            sLine0 = String.Empty
            sLine1 = String.Empty
            sLine2 = String.Empty
            nFileLen = Len(sData)
            RaiseEvent OnProgressStart(New ProgressStartEventArgs(nFileLen))
            nLineCount = 0
            nOccurrences = 0
            For nFilePos = 0 To nFileLen - 1
                nLinePos = nLinePos + 1
                sEol = String.Empty
                sEol1 = String.Empty
                sEol2 = String.Empty
                sChar = sData.Substring(nFilePos, 1)
                If sChar = vbCr Then
                    sEol = LPad(Asc(vbCr).ToString("X"), 2, "0")
                    sEol1 = Left(sEol, 1)
                    sEol2 = Right(sEol, 1)
                    bEol = True
                    'A lot of duplication here, needs to be fixed later.
                    sHex = Asc(sChar).ToString("X")
                    sHex = LPad(sHex, 2, "0")
                    If sChar = Chr(0) Then
                        sChar = "."
                    End If
                    If sHex = psMarkHex Then
                        nOccurrences = nOccurrences + 1
                        sLine0 = sLine0 & psWithChar
                    Else
                        sLine0 = sLine0 & " "
                    End If
                    If nFilePos < nFileLen - 1 Then
                        If sData.Substring(nFilePos + 1, 1) = vbLf Then
                            sEol = LPad(Asc(vbLf).ToString("X"), 2, "0")
                            sEol1 = sEol1 & Left(sEol, 1)
                            sEol2 = sEol2 & Right(sEol, 1)
                            nFilePos = nFilePos + 1
                            'A lot of duplication here, needs to be fixed later.
                            sHex = Asc(vbLf).ToString("X")
                            sHex = LPad(sHex, 2, "0")
                            If sChar = Chr(0) Then
                                sChar = "."
                            End If
                            If sHex = psMarkHex Then
                                nOccurrences = nOccurrences + 1
                                sLine0 = sLine0 & psWithChar
                            Else
                                sLine0 = sLine0 & " "
                            End If
                        End If
                    End If
                ElseIf sChar = vbLf Then
                    sEol = LPad(Asc(vbLf).ToString("X"), 2, "0")
                    sEol1 = Left(sEol, 1)
                    sEol2 = Right(sEol, 1)
                    bEol = True
                    'A lot of duplication here, needs to be fixed later.
                    sHex = Asc(sChar).ToString("X")
                    sHex = LPad(sHex, 2, "0")
                    If sChar = Chr(0) Then
                        sChar = "."
                    End If
                    If sHex = psMarkHex Then
                        nOccurrences = nOccurrences + 1
                        sLine0 = sLine0 & psWithChar
                    Else
                        sLine0 = sLine0 & sChar
                    End If
                End If
                If nLinePos > MAXLEN Then
                    bEol = True
                End If
                If bEol = True Then
                    nLineCount = nLineCount + 1
                    sLineNumber = LPad(Trim(Str(nLineCount)), 6, "0") & " - "
                    _hexData = _hexData & vbCrLf
                    _hexData = _hexData & sLineNumber & sLine0 & vbCrLf
                    _hexData = _hexData & sLineNumber & sLine1 & sEol1 & vbCrLf
                    _hexData = _hexData & sLineNumber & sLine2 & sEol2 & vbCrLf
                    If sLine1 Is Nothing Then
                        sLine1 = String.Empty
                    End If
                    If sLine2 Is Nothing Then
                        sLine2 = String.Empty
                    End If
                    Print(nOutNum, String.Empty & System.Environment.NewLine)
                    Print(nOutNum, sLineNumber & sLine0 & System.Environment.NewLine)
                    Print(nOutNum, sLineNumber & sLine1 & sEol1 & System.Environment.NewLine)
                    Print(nOutNum, sLineNumber & sLine2 & sEol2 & System.Environment.NewLine)
                    If sLine1 Is Nothing Then
                        sLine1 = String.Empty
                    End If
                    If sLine2 Is Nothing Then
                        sLine2 = String.Empty
                    End If
                    RaiseEvent OnProgressUpdate(New ProgressUpdateEventArgs(sLine1.Length, sLine1))
                    bEol = False
                    nLinePos = 0
                    sLine0 = String.Empty
                    sLine1 = String.Empty
                    sLine2 = String.Empty
                Else
                    sHex = Asc(sChar).ToString("X")
                    sHex = LPad(sHex, 2, "0")
                    If sChar = Chr(0) Then
                        sChar = "."
                    End If
                    If sHex = psMarkHex Then
                        nOccurrences = nOccurrences + 1
                        sLine0 = sLine0 & psWithChar
                    Else
                        sLine0 = sLine0 & sChar
                    End If
                    If Asc(sChar) = TABCODE Then
                        If bShowTabs Then
                            sLine1 = sLine1 & Left(sHex, 1) & Chr(TABCODE)
                            sLine2 = sLine2 & Right(sHex, 1) & Chr(TABCODE)
                        Else
                            sLine1 = sLine1 & Chr(TABCODE)
                            sLine2 = sLine2 & Chr(TABCODE)
                        End If
                    Else
                        sLine1 = sLine1 & Left(sHex, 1)
                        sLine2 = sLine2 & Right(sHex, 1)
                    End If
                End If
                If nFilePos > pnStopAfter Then
                    Exit For
                End If
            Next
            If Len(sLine1) > 0 Then
                'Handle last line if it doesn't have an end-of-line marker.
                nLineCount = nLineCount + 1
                sLineNumber = LPad(Trim(Str(nLineCount)), 6, "0") & " - "
                _hexData = _hexData & vbCrLf
                _hexData = _hexData & sLineNumber & sLine0 & vbCrLf
                _hexData = _hexData & sLineNumber & sLine1 & sEol1 & vbCrLf
                _hexData = _hexData & sLineNumber & sLine2 & sEol2 & vbCrLf
                If sLine1 Is Nothing Then
                    sLine1 = String.Empty
                End If
                If sLine2 Is Nothing Then
                    sLine2 = String.Empty
                End If
                Print(nOutNum, String.Empty & System.Environment.NewLine)
                Print(nOutNum, sLineNumber & sLine0 & System.Environment.NewLine)
                Print(nOutNum, sLineNumber & sLine1 & sEol1 & System.Environment.NewLine)
                Print(nOutNum, sLineNumber & sLine2 & sEol2 & System.Environment.NewLine)
                RaiseEvent OnProgressUpdate(New ProgressUpdateEventArgs(sLine1.Length, sLine1))
                bEol = False
                nLinePos = 0
                sLine0 = String.Empty
                sLine1 = String.Empty
                sLine2 = String.Empty
            End If
            RaiseEvent OnProgressEnd(New ProgressEndEventArgs(nFileLen))
            RaiseEvent OnFinished(Me, New ProgressEndEventArgs(nFileLen))
            FileClose(nOutNum)
            FileClose(nInNum)
            pnOccurrences = nOccurrences
        End Sub
#End Region
	End Class
End Namespace
