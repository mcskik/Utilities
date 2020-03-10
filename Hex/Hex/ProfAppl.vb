Imports System
Imports System.IO
Imports System.Windows.Forms
Imports Routines8.Routines

Namespace Hex
	''' <summary>
	''' Application specific application profile.
	''' </summary>
	''' <remarks>
	''' Application specific application profile.
	''' </remarks>
	''' <author>Kenneth McSkimming</author>
	Public Class ProfAppl
#Region "Member variables."
		Private Shared msStartupPath As String = String.Empty
		Private Shared msSeparator As String = Path.DirectorySeparatorChar.ToString()
		Private Shared msCdUp As String = ".." & Path.DirectorySeparatorChar.ToString()
#End Region

#Region "Properties."
		''' <summary>
		''' Return location of system profile XML file.
		''' </summary>
		Public Shared ReadOnly Property ProfSystXml() As String
			Get
				Dim sAppPath As String = Application.StartupPath.Trim() + msSeparator
				Directory.SetCurrentDirectory(sAppPath)
				Dim sXmlFile As String = Path.GetFullPath(msCdUp + msCdUp + "Xml" + msSeparator + "ProfSyst.xml")
				Return sXmlFile
			End Get
		End Property
		''' <summary>
		''' Return location of work profile XML file.
		''' </summary>
		Public Shared ReadOnly Property ProfWorkXml() As String
			Get
				Dim sAppPath As String = Application.StartupPath.Trim() + msSeparator
				Directory.SetCurrentDirectory(sAppPath)
				Dim sXmlFile As String = Path.GetFullPath(msCdUp + msCdUp + "Xml" + msSeparator + "ProfWork.xml")
				Return sXmlFile
			End Get
		End Property
		''' <summary>
		''' Return program version.
		''' </summary>
		Public Shared ReadOnly Property Version() As String
			Get
				Return "1.0.0"
			End Get
		End Property
#End Region
	End Class
End Namespace
