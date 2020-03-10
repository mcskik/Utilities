Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Text
Imports XmlProfile8

Namespace Hex
	''' <summary>
	''' Application specific working profile.
	''' </summary>
	''' <remarks>
	''' Provides easy access to working profile variables.
	''' </remarks>
	''' <author>Kenneth McSkimming</author>
	Public Class ProfWork
#Region "Member variables."
		Private Shared msSourceFile As String = String.Empty
		Private Shared msTargetFile As String = String.Empty
		Private Shared msMarkHex As String = String.Empty
		Private Shared msWithChar As String = String.Empty
		Private Shared mnOccurrences As Integer = 0
        Private Shared mnStopAfter As Integer = 0
#End Region

#Region "Properties."
		''' <summary>
		''' Source File.
		''' </summary>
		Public Shared Property SourceFile() As String
			Get
				Return msSourceFile
			End Get
			Set(ByVal Value As String)
				msSourceFile = Value
			End Set
		End Property

		''' <summary>
		''' Target File.
		''' </summary>
		Public Shared Property TargetFile() As String
			Get
				Return msTargetFile
			End Get
			Set(ByVal Value As String)
				msTargetFile = Value
			End Set
		End Property

		''' <summary>
		''' Mark Hex.
		''' </summary>
		Public Shared Property MarkHex() As String
			Get
				Return msMarkHex
			End Get
			Set(ByVal Value As String)
				msMarkHex = Value
			End Set
		End Property

		''' <summary>
		''' With Char.
		''' </summary>
		Public Shared Property WithChar() As String
			Get
				Return msWithChar
			End Get
			Set(ByVal Value As String)
				msWithChar = Value
			End Set
		End Property

		''' <summary>
		''' Occurrences.
		''' </summary>
		Public Shared Property Occurrences() As Integer
			Get
				Return mnOccurrences
			End Get
			Set(ByVal Value As Integer)
				mnOccurrences = Value
			End Set
		End Property

        ''' <summary>
        ''' Stop after.
        ''' </summary>
        Public Shared Property StopAfter() As Integer
            Get
                Return mnStopAfter
            End Get
            Set(ByVal Value As Integer)
                mnStopAfter = Value
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

#Region "Public methods."
		''' <summary>
		''' Load working profile variables.
		''' </summary>
		Public Shared Sub Load()
			Dim sXPath As String
			Dim sXStem As String
			Dim oProfile As XmlProfile = New XmlProfile()
			oProfile.Load(ProfAppl.ProfWorkXml)
			sXPath = "/Profile/Parameters/Parameter[@Name=""SourceFile""]"
			msSourceFile = oProfile.Fetch(sXPath, "Value")
			sXPath = "/Profile/Parameters/Parameter[@Name=""TargetFile""]"
			msTargetFile = oProfile.Fetch(sXPath, "Value")
			sXPath = "/Profile/Parameters/Parameter[@Name=""MarkHex""]"
			msMarkHex = oProfile.Fetch(sXPath, "Value")
			sXPath = "/Profile/Parameters/Parameter[@Name=""WithChar""]"
			msWithChar = oProfile.Fetch(sXPath, "Value")
			sXStem = "/Profile/Parameters/Parameter"
			mnOccurrences = oProfile.FetchInteger(sXStem, "@Name=""Occurrences""", "Value")
            mnStopAfter = oProfile.FetchInteger(sXStem, "@Name=""StopAfter""", "Value")
        End Sub

		''' <summary>
		''' Save working profile variables.
		''' </summary>
		Public Shared Sub Save()
			Dim sXPath As String
			Dim sXStem As String
			Dim sXPathArg As String
			Dim oProfile As XmlProfile = New XmlProfile()
			oProfile.Load(ProfAppl.ProfWorkXml)
			sXPath = "/Profile/Parameters/Parameter[@Name=""SourceFile""]"
			oProfile.Store(sXPath, "Value", msSourceFile)
			sXPath = "/Profile/Parameters/Parameter[@Name=""TargetFile""]"
			oProfile.Store(sXPath, "Value", msTargetFile)
			sXPath = "/Profile/Parameters/Parameter[@Name=""MarkHex""]"
			oProfile.Store(sXPath, "Value", msMarkHex)
			sXPath = "/Profile/Parameters/Parameter[@Name=""WithChar""]"
			oProfile.Store(sXPath, "Value", msWithChar)
            sXStem = "/Profile/Parameters/Parameter"
			sXPathArg = "@Name=""Occurrences"""
			oProfile.StoreInteger(sXStem, sXPathArg, "Value", mnOccurrences)
            sXPathArg = "@Name=""StopAfter"""
            oProfile.StoreInteger(sXStem, sXPathArg, "Value", mnStopAfter)
            oProfile.Save()
		End Sub
#End Region
	End Class
End Namespace
