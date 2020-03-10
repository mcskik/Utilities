Imports System
Imports System.Collections.Generic
Imports System.Text
Imports XmlProfile8

Namespace Hex
	''' <summary>
	''' Application specific system profile.
	''' </summary>
	''' <remarks>
	''' Provides easy access to cached system profile variables.
	''' </remarks>
	''' <author>Kenneth McSkimming</author>
	Public Class ProfSyst
#Region "Member variables."
		Private Shared msLogPath As String
		Private Shared msViewerWindows As String
		Private Shared msViewerUnix As String
		Private Shared msSourcePath As String
		Private Shared msTargetPath As String
		Private Shared mbShowTabs As Boolean
#End Region

#Region "Properties."
		''' <summary>
		''' Log path.
		''' </summary>
		Public Shared Property LogPath() As String
			Get
				Return msLogPath
			End Get
			Set(ByVal Value As String)
				msLogPath = Value
			End Set
		End Property

		''' <summary>
		''' Program to use to view windows text files.
		''' </summary>
		Public Shared Property ViewerWindows() As String
			Get
				Return msViewerWindows
			End Get
			Set(ByVal Value As String)
				msViewerWindows = Value
			End Set
		End Property

		''' <summary>
		''' Program to use to view unix text files.
		''' </summary>
		Public Shared Property ViewerUnix() As String
			Get
				Return msViewerUnix
			End Get
			Set(ByVal Value As String)
				msViewerUnix = Value
			End Set
		End Property

		''' <summary>
		''' Source Path.
		''' </summary>
		Public Shared Property SourcePath() As String
			Get
				Return msSourcePath
			End Get
			Set(ByVal Value As String)
				msSourcePath = Value
			End Set
		End Property

		''' <summary>
		''' Target Path.
		''' </summary>
		Public Shared Property TargetPath() As String
			Get
				Return msTargetPath
			End Get
			Set(ByVal Value As String)
				msTargetPath = Value
			End Set
		End Property

		''' <summary>
		''' Show Tabs.
		''' </summary>
		Public Shared Property ShowTabs() As Boolean
			Get
				Return mbShowTabs
			End Get
			Set(ByVal Value As Boolean)
				mbShowTabs = Value
			End Set
		End Property
#End Region

#Region "Constructors."
		''' <summary>
		''' Default constructor.
		''' </summary>
		Shared Sub New()
			Load()
		End Sub
#End Region

#Region "Private methods."
		''' <summary>
		''' Load cached system profile variables.
		''' </summary>
		Private Shared Sub Load()
			Dim sXPath As String
			Dim sXStem As String
			Dim oProfile As XmlProfile = New XmlProfile()
			oProfile.Load(ProfAppl.ProfSystXml)
			sXPath = "/Profile/Directories/Directory[@Name=""Log""]"
			msLogPath = oProfile.Fetch(sXPath, "Path")
			sXPath = "/Profile/Programs/Program[@Name=""ViewerWindows""]"
			msViewerWindows = oProfile.Fetch(sXPath, "File")
			sXPath = "/Profile/Programs/Program[@Name=""ViewerUnix""]"
			msViewerUnix = oProfile.Fetch(sXPath, "File")
			sXPath = "/Profile/Parameters/Parameter[@Name=""SourcePath""]"
			msSourcePath = oProfile.Fetch(sXPath, "Value")
			sXPath = "/Profile/Parameters/Parameter[@Name=""TargetPath""]"
			msTargetPath = oProfile.Fetch(sXPath, "Value")
			sXStem = "/Profile/Parameters/Parameter"
			mbShowTabs = oProfile.FetchBool(sXStem, "@Name=""ShowTabs""", "Value")
		End Sub
#End Region
	End Class
End Namespace
