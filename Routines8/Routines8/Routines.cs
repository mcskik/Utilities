using System;
using System.IO;
namespace Routines8
{
	/// <summary>
	/// Common routines.
	/// </summary>
	/// <remarks>
	/// Common static routines.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Routines
	{
		#region Cross platform enabling methods.
		/// <summary>
		/// Returns cross platform directory path separator.
		/// </summary>
		/// <returns>Cross platform directory path separator</returns>
		public static string Separator()
		{
			return Path.DirectorySeparatorChar.ToString();
		}

		/// <summary>
		/// Returns cross platform directory path double separator.
		/// </summary>
		/// <returns>Cross platform directory path double separator</returns>
		public static string DoubleSeparator()
		{
			return Separator() + Separator();
		}
		#endregion

		/// <summary>
		/// Remove all leading, trailing, or both, occurences of the specified char.
		/// </summary>
		/// <param name="psInput">Input string.</param>
		/// <param name="psMode">Leading, Trailing, or Both.</param>
		/// <param name="psChar">Character to strip.</param>
		/// <returns>Stripped string.</returns>
		public static string Strip(string psInput, string psMode, string psChar)
		{
			int nPtr = 0;
			string sOutput = string.Empty;
			//Parameter validation.
			if (psInput.Length == 0)
			{
				return psInput;
			}
			psMode = psMode.ToUpper();
			switch (psMode)
			{
				case "BOTH":
					break;
				case "LEADING":
					break;
				case "TRAILING":
					break;
				default:
					return psInput;
			}
			if (psChar.Length != 1)
			{
				return psInput;
			}
			//Main processing.
			if (psMode == "LEADING" || psMode == "BOTH")
			{
				for (nPtr = 0; nPtr < psInput.Length; nPtr++)
				{
					if (psInput.Substring(nPtr, 1) != psChar)
					{
						sOutput = psInput.Substring(nPtr);
						break;
					}
				}
				psInput = sOutput;
			}
			if (psMode == "TRAILING" || psMode == "BOTH")
			{
				for (nPtr = psInput.Length - 1; nPtr >= 0; nPtr--)
				{
					if (psInput.Substring(nPtr, 1) != psChar)
					{
						sOutput = psInput.Substring(0, nPtr + 1);
						break;
					}
				}
			}
			return sOutput;
		}

		/// <summary>
		/// Bespoke replace function.
		/// </summary>
		/// <param name="psString">Original string.</param>
		/// <param name="psReplace">Substring to find and replace.</param>
		/// <param name="psWith">Replacement string.</param>
		/// <returns>String with all replacements applied.</returns>
		public static string Replace(string psString, string psReplace, string psWith)
		{
			int nFoundAt = 0;
			string sInput = psString;
			string sOutput = string.Empty;
			nFoundAt = sInput.IndexOf(psReplace);
			while (nFoundAt > -1)
			{
				sOutput += sInput.Substring(0, nFoundAt) + psWith;
				sInput = sInput.Substring(nFoundAt + psReplace.Length);
				nFoundAt = sInput.IndexOf(psReplace);
			}
			sOutput += sInput;
			return sOutput;
		}

		/// <summary>
		/// Replicate the specified character string the specified number of times.
		/// </summary>
		/// <param name="psString">String to replicate.</param>
		/// <param name="pnTimes">Number of times to replicate.</param>
		/// <returns>String which has been replicated a set number of times.</returns>
		public static string Replicate(string psString, int pnTimes)
		{
			int nPtr = 0;
			string sOutput = string.Empty;
			for (nPtr = 1; nPtr <= pnTimes; nPtr++)
			{
				sOutput += psString;
			}
			return sOutput;
		}

		/// <summary>
		/// Left pad a string with spaces to the length required.
		/// </summary>
		/// <param name="psString">String to pad.</param>
		/// <param name="pnLength">Length of returned string.</param>
		/// <returns>String padded on the left with spaces to the specified length.</returns>
		public static string LPad(string psString, int pnLength)
		{
			psString = LPad(psString, pnLength, " ");
			return psString;
		}

		/// <summary>
		/// Left pad a string with the specified character to the length required.
		/// </summary>
		/// <param name="psString">String to pad.</param>
		/// <param name="pnLength">Length of returned string.</param>
		/// <param name="psChar">Pad character.</param>
		/// <returns>String padded on the left with the specified character to the specified length.</returns>
		public static string LPad(string psString, int pnLength, string psChar)
		{
			if (psChar.Length == 0)
			{
				psChar = " ";
			}
			if (psChar.Length > 1)
			{
				psChar = psChar.Substring(0, 1);
			}
			psString = Replicate(psChar, pnLength) + psString.TrimStart();
			psString = psString.Substring(psString.Length - pnLength);
			return psString;
		}

		/// <summary>
		/// Right pad a string with spaces to the length required.
		/// </summary>
		/// <param name="psString">String to pad.</param>
		/// <param name="pnLength">Length of returned string.</param>
		/// <returns>String padded on the right with spaces to the specified length.</returns>
		public static string RPad(string psString, int pnLength)
		{
			psString = RPad(psString, pnLength, " ");
			return psString;
		}

		/// <summary>
		/// Right pad a string with the specified character to the length required.
		/// </summary>
		/// <param name="psString">String to pad.</param>
		/// <param name="pnLength">Length of returned string.</param>
		/// <param name="psChar">Pad character.</param>
		/// <returns>String padded on the right with the specified character to the specified length.</returns>
		public static string RPad(string psString, int pnLength, string psChar)
		{
			if (psChar.Length == 0)
			{
				psChar = " ";
			}
			if (psChar.Length > 1)
			{
				psChar = psChar.Substring(0, 1);
			}
			psString = psString.TrimEnd() + Replicate(psChar, pnLength);
			psString = psString.Substring(0, pnLength);
			return psString;
		}

		/// <summary>
		/// Return the right most number of characters requested from the input string.
		/// </summary>
		/// <param name="psInput">Input string.</param>
		/// <param name="pnNumber">Number of characters from the right.</param>
		/// <returns>Right most number of characters.</returns>
		public static string RightMost(string psInput, int pnNumber)
		{
			string sOutput = string.Empty;
			int nPos = 0;
			if (psInput.Length < pnNumber)
			{
				pnNumber = psInput.Length;
			}
			if (psInput.Length == 0)
			{
				sOutput = string.Empty;
			}
			else if (pnNumber == 0)
			{
				sOutput = string.Empty;
			}
			else
			{
				nPos = psInput.Length - pnNumber;
				sOutput = psInput.Substring(nPos);
			}
			return sOutput;
		}

		/// <summary>
		/// Bespoke IsDate function.
		/// </summary>
		/// <param name="poDate">Object to test if it contains a date.</param>
		/// <returns>true or false.</returns>
		public static bool IsDate(object poDate)
		{
			string sException;
			DateTime dDate;
			bool bValid = false;
			try
			{
				dDate = DateTime.Parse(poDate.ToString());
				bValid = true;
			}
			catch (FormatException e)
			{
				sException = e.ToString();
				bValid = false;
			}
			return bValid;
		}

		/// <summary>
		/// Bespoke IsTimeSpan function.
		/// </summary>
		/// <param name="poTimeSpan">Object to test if it contains a time span.</param>
		/// <returns>true or false.</returns>
		public static bool IsTimeSpan(object poTimeSpan)
		{
			string sException;
			TimeSpan oTimeSpan;
			bool bValid = false;
			try
			{
				oTimeSpan = TimeSpan.Parse(poTimeSpan.ToString());
				bValid = true;
			}
			catch (FormatException e)
			{
				sException = e.ToString();
				bValid = false;
			}
			return bValid;
		}

		/// <summary>
		/// Bespoke version of format function but only for dates.
		/// </summary>
		/// <param name="pdDate">Date to format.</param>
		/// <param name="psFormat">VB style format string.</param>
		/// <returns>Formatted date.</returns>
		public static string FmtDate(DateTime pdDate, string psFormat)
		{
			string sOutput = string.Empty;
			string sFormat = string.Empty;
			string sChar = string.Empty;
			int nPos = 0;
			if (IsDate(pdDate))
			{
				sFormat = psFormat.Trim();
				sFormat = sFormat.ToUpper();
				sFormat = Replace(sFormat, "YYYY", "C");
				sFormat = Replace(sFormat, "YY", "Y");
				sFormat = Replace(sFormat, "MM", "M");
				sFormat = Replace(sFormat, "DD", "D");
				sFormat = Replace(sFormat, "HH", "H");
				sFormat = Replace(sFormat, "NN", "N");
				sFormat = Replace(sFormat, "SS", "S");
				sFormat = Replace(sFormat, "ZZZ", "Z");
				for (nPos = 0; nPos < sFormat.Length; nPos++)
				{
					sChar = sFormat.Substring(nPos, 1);
					switch (sChar)
					{
						case "C":
							sOutput += LPad(pdDate.Year.ToString(), 4, "0");
							break;
						case "Y":
							sOutput += LPad(pdDate.Year.ToString(), 2, "0");
							break;
						case "M":
							sOutput += LPad(pdDate.Month.ToString(), 2, "0");
							break;
						case "D":
							sOutput += LPad(pdDate.Day.ToString(), 2, "0");
							break;
						case "H":
							sOutput += LPad(pdDate.Hour.ToString(), 2, "0");
							break;
						case "N":
							sOutput += LPad(pdDate.Minute.ToString(), 2, "0");
							break;
						case "S":
							sOutput += LPad(pdDate.Second.ToString(), 2, "0");
							break;
						case "Z":
							sOutput += LPad(pdDate.Millisecond.ToString(), 3, "0");
							break;
						default:
							sOutput += sChar;
							break;
					}
				}
			}
			return sOutput;
		}

		/// <summary>
		/// Bespoke version of format function but only for time spans.
		/// </summary>
		/// <param name="poTimeSpan">Time span to format.</param>
		/// <param name="psFormat">VB style format string.</param>
		/// <returns>Formatted time span.</returns>
		public static string FmtTimeSpan(TimeSpan poTimeSpan, string psFormat)
		{
			string sOutput = string.Empty;
			string sFormat = string.Empty;
			string sChar = string.Empty;
			int nPos = 0;
			if (IsTimeSpan(poTimeSpan))
			{
				sFormat = psFormat.Trim();
				sFormat = sFormat.ToUpper();
				sFormat = Replace(sFormat, "DD", "D");
				sFormat = Replace(sFormat, "HH", "H");
				sFormat = Replace(sFormat, "NN", "N");
				sFormat = Replace(sFormat, "SS", "S");
				sFormat = Replace(sFormat, "ZZZ", "Z");
				for (nPos = 0; nPos < sFormat.Length; nPos++)
				{
					sChar = sFormat.Substring(nPos, 1);
					switch (sChar)
					{
						case "D":
							sOutput += LPad(poTimeSpan.Days.ToString(), 2, "0");
							break;
						case "H":
							sOutput += LPad(poTimeSpan.Hours.ToString(), 2, "0");
							break;
						case "N":
							sOutput += LPad(poTimeSpan.Minutes.ToString(), 2, "0");
							break;
						case "S":
							sOutput += LPad(poTimeSpan.Seconds.ToString(), 2, "0");
							break;
						case "Z":
							sOutput += LPad(poTimeSpan.Milliseconds.ToString(), 3, "0");
							break;
						default:
							sOutput += sChar;
							break;
					}
				}
			}
			return sOutput;
		}

		/// <summary>
		/// Get file stem.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>Everything except the file name and extension from the file specification.</returns>
		public static string FileStem(string psFileSpec)
		{
			int nPos;
			string sFileStem;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length == 0)
			{
				sFileStem = string.Empty;
			}
			else
			{
				nPos = psFileSpec.LastIndexOf(Separator());
				if (nPos > -1)
				{
					sFileStem = psFileSpec.Substring(0, nPos + 1);
				}
				else
				{
					sFileStem = string.Empty;
				}
			}
			return sFileStem;
		}

		/// <summary>
		/// Get file path.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>Everything except the drive specification, UNC server, and file name plus ext.</returns>
		public static string FilePath(string psFileSpec)
		{
			string sServer = string.Empty;
			string sDrive = string.Empty;
			string sFilePath = string.Empty;
			int nPos = 0;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length == 0)
			{
				sFilePath = string.Empty;
			}
			else
			{
				nPos = psFileSpec.LastIndexOf(Separator());
				if (nPos == -1)
				{
					sFilePath = string.Empty;
				}
				else if (nPos == 0)
				{
					sFilePath = Separator();
				}
				else
				{
					sFilePath = psFileSpec.Substring(0, nPos + 1);
				}
			}
			return sFilePath;
		}

		/// <summary>
		/// Get file server from a UNC file specification.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>Server name from a UNC file specification, otherwise an empty string.</returns>
		public static string WindowsFileServer(string psFileSpec)
		{
			string sDoubleSeparator = Separator() + Separator();
			int nPos;
			string sFileServer;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length < 3)
			{
				sFileServer = string.Empty;
			}
			else if (psFileSpec.Substring(0, 2) == sDoubleSeparator)
			{
				nPos = psFileSpec.IndexOf(Separator(), 2);
				if (nPos > -1)
				{
					sFileServer = psFileSpec.Substring(0, nPos);
				}
				else
				{
					sFileServer = psFileSpec;
				}
				sFileServer = sFileServer.Substring(2);
			}
			else
			{
				sFileServer = string.Empty;
			}
			return sFileServer;
		}

		/// <summary>
		/// Get Windows file drive.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>Drive letter from a non-UNC file specification, otherwise an empty string.</returns>
		public static string WindowsFileDrive(string psFileSpec)
		{
			string sFileDrive;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length < 2)
			{
				sFileDrive = string.Empty;
			}
			else if (psFileSpec.Substring(1, 1) == ":")
			{
				sFileDrive = psFileSpec.Substring(0, 1);
			}
			else
			{
				sFileDrive = string.Empty;
			}
			return sFileDrive;
		}

		/// <summary>
		/// Get file path.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>Everything except the drive specification, UNC server, and file name plus ext.</returns>
		public static string WindowsFilePath(string psFileSpec)
		{
			string sServer = string.Empty;
			string sDrive = string.Empty;
			string sFilePath = string.Empty;
			int nPos = 0;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length > 0)
			{
				sServer = WindowsFileServer(psFileSpec);
				if (sServer != string.Empty)
				{
					if (psFileSpec.Length > sServer.Length + 2)
					{
						psFileSpec = psFileSpec.Substring(sServer.Length + 2);
					}
					else
					{
						psFileSpec = string.Empty;
					}
				}
				else
				{
					sDrive = WindowsFileDrive(psFileSpec);
					if (sDrive != string.Empty)
					{
						if (psFileSpec.Length > sDrive.Length + 1)
						{
							psFileSpec = psFileSpec.Substring(sDrive.Length + 1);
						}
						else
						{
							psFileSpec = string.Empty;
						}
					}
				}
			}
			if (psFileSpec.Length == 0)
			{
				sFilePath = string.Empty;
			}
			else
			{
				nPos = psFileSpec.LastIndexOf(Separator());
				if (nPos == -1)
				{
					sFilePath = string.Empty;
				}
				else if (nPos == 0)
				{
					sFilePath = Separator();
				}
				else
				{
					sFilePath = psFileSpec.Substring(0, nPos + 1);
				}
			}
			return sFilePath;
		}

		/// <summary>
		/// Get file name.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>File name part of a file specification.</returns>
		public static string FileName(string psFileSpec) 
		{
			int nPos;
			string sFileName = string.Empty;
			psFileSpec = psFileSpec.Trim();
			if ( psFileSpec.Length == 0 ) {
				sFileName = string.Empty;
			} else {
				nPos = psFileSpec.LastIndexOf(Separator());
				if ( nPos > -1 ) {
					if ( psFileSpec.Length >= nPos + 1 ) {
						psFileSpec = psFileSpec.Substring(nPos + 1);
					} else {
						psFileSpec = string.Empty;
					}
				}
				if ( psFileSpec.Length > 0 ) {
					nPos = psFileSpec.LastIndexOf(".");
					if ( nPos == -1 ) {
						sFileName = psFileSpec;
					} else if ( nPos == 0 ) {
						sFileName = string.Empty;
					} else {
						sFileName = psFileSpec.Substring(0,nPos);
					}
				}
			}
			return sFileName;
		}

		/// <summary>
		/// Get file extension.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>File extension part of a file specification.</returns>
		public static string FileExt(string psFileSpec)
		{
			int nPos;
			string sFileExt;
			psFileSpec = psFileSpec.Trim();
			if (psFileSpec.Length == 0)
			{
				sFileExt = string.Empty;
			}
			else
			{
				nPos = psFileSpec.LastIndexOf(".");
				if (nPos == -1)
				{
					sFileExt = string.Empty;
				}
				else
				{
					if (nPos < psFileSpec.Length - 1)
					{
						sFileExt = psFileSpec.Substring(nPos + 1);
					}
					else
					{
						sFileExt = string.Empty;
					}
				}
			}
			return sFileExt;
		}

		/// <summary>
		/// Get file name and extension.
		/// </summary>
		/// <param name="psFileSpec">File specification.</param>
		/// <returns>File name and extension parts of a file specification.</returns>
		public static string FileFullName(string psFileSpec)
		{
			string sFileFullName;
			sFileFullName = FileName(psFileSpec);
			if (FileExt(psFileSpec) != string.Empty)
			{
				sFileFullName = sFileFullName + "." + FileExt(psFileSpec);
			}
			return sFileFullName;
		}

		/// <summary>
		/// Get file information about the specified file.
		/// </summary>
		/// <param name="psFile">File specification.</param>
		/// <returns>FileInfo object for the specified file.</returns>
		public static FileInfo GetFileInfo(string psFile)
		{
			string sPath = FileStem(psFile);
			string sFile = FileFullName(psFile);
			FileInfo oFileInfo = null;
			try 
			{
				DirectoryInfo oDirectory = new DirectoryInfo(sPath);
				FileInfo[] aFiles = oDirectory.GetFiles(sFile);
				for( int nRow = 0; nRow < aFiles.Length; nRow++ ) 
				{
					if ( aFiles[nRow].Exists ) 
					{
						oFileInfo = aFiles[nRow];
						break;
					}
				}
			}
			catch
			{
				oFileInfo = null;
			}
			return oFileInfo;	
		}

		/// <summary>
		/// Determine the number of windows rows of text in the specified text block.
		/// </summary>
		/// <param name="psText">String containing contents of file.</param>
		/// <returns>Number of rows based on CrLfs.</returns>
		public static long TextRows(string psText)
		{
			long nRow = 0;
			int nPos = 0;
			string sText = psText;
			if ( sText.Length > 0 )
			{
				do
				{
					nRow = nRow + 1;
					nPos = sText.IndexOf("\r\n");
					if ( nPos > -1 )
					{
						nPos = nPos + 2;
						if ( nPos < sText.Length )
						{
							sText = sText.Substring(nPos);
						}
						else
						{
							sText = string.Empty;
						}
					}
				} while ( nPos != -1 );
			}
			return nRow;
		}

		/// <summary>
		/// Determine the number of unix rows of text in the specified text block.
		/// </summary>
		/// <param name="psText">String containing contents of file.</param>
		/// <returns>Number of rows based on Lfs.</returns>
		public static long UnixRows(string psText)
		{
			long nRow = 0;
			int nPos = 0;
			string sText = psText;
			if ( sText.Length > 0 )
			{
				do
				{
					nRow = nRow + 1;
					nPos = sText.IndexOf("\n",nPos);
					if ( nPos != -1 )
					{
						nPos = nPos + 1;
						if ( nPos > sText.Length )
						{
							break;
						}
					}
				} while ( nPos != -1 );
			}
			return nRow;
		}

		/// <summary>
		/// Determine absolute spec based on current and relative specs specified.
		/// </summary>
		/// <param name="psCurrentSpec">Current file specification.</param>
		/// <param name="psRelativeSpec">Relative file specification.</param>
		/// <returns>Absolute file specification.</returns>
		public static string AbsoluteSpec(string psCurrentSpec, string psRelativeSpec) 
		{
			string sCdMk;
			string sCdUp;
			int nCdLevels;
			int nCdCount;
			int nPos;
			string sSpec;
			string sLeader;
			string sTrailer;
    
			//Clean parameters.
			psCurrentSpec = psCurrentSpec.Trim();
			psRelativeSpec = psRelativeSpec.Trim();

			//Set change directory mark and change directory up mark.
			sCdMk = Separator();
			sCdUp = ".." + Separator();
			//Break relative spec into leader and trailer portions.
			psCurrentSpec = Strip(FileStem(psCurrentSpec), "TRAILING", sCdMk);
			nPos = psRelativeSpec.LastIndexOf(sCdUp);
			if (nPos > -1)
			{
				nPos += sCdUp.Length;
				sLeader = psRelativeSpec.Substring(0, nPos);
				if (nPos > psRelativeSpec.Length)
				{
					sTrailer = string.Empty;
				}
				else
				{
					sTrailer = psRelativeSpec.Substring(nPos);
				}
				//Count the number of "CD UP" indicators in the leader.
				nCdLevels = 0;
				nPos = sLeader.LastIndexOf(sCdUp);
				while (nPos > -1)
				{
					nCdLevels += 1;
					if (nPos > -1)
					{
						sLeader = sLeader.Substring(0, nPos + 1);
					}
					else
					{
						sLeader = string.Empty;
					}
					nPos = sLeader.LastIndexOf(sCdUp);
				}
				//Apply the same number of "CD UP" operations to the current spec.
				sLeader = psCurrentSpec;
				nCdCount = 0;
				nPos = sLeader.LastIndexOf(sCdMk);
				while (nCdCount < nCdLevels && nPos > -1)
				{
					nCdCount += 1;
					if (nPos > -1)
					{
						sLeader = sLeader.Substring(0, nPos);
					}
					else
					{
						sLeader = string.Empty;
					}
					nPos = sLeader.LastIndexOf(sCdMk);
				}
				if (nCdCount == nCdLevels)
				{
					//Use leader and trailer portions to derive absolute spec.
					sSpec = sLeader + sCdMk + sTrailer;
				}
				else
				{
					//Error, unable to change directory up enough times.
					sSpec = string.Empty;
				}
			}
			else
			{
				//There are no "CD UP" indicators so use relative spec as the absolute spec.
				sSpec = psCurrentSpec + sCdMk + psRelativeSpec;
			}
			return sSpec;
		}

		/// <summary>
		/// Detect ini file section.
		/// </summary>
		/// <param name="psKeyword">Keyword to look for.</param>
		/// <param name="psRec">Current record.</param>
		/// <returns>true or false.</returns>
		public static bool Section(string psKeyword, string psRec)
		{
			bool bSection = false;
			if (psRec.Trim() == "[" + psKeyword.Trim() + "]")
			{
				bSection = true;
			}
			return bSection;
		}

		/// <summary>
		/// Set profile variable to the matching ini file value.
		/// Only set the profile variable if its value is still an empty string.
		/// </summary>
		/// <param name="psName">Profile variable name.</param>
		/// <param name="psValue">Current profile variable value.</param>
		/// <param name="psRec">Current record.</param>
		/// <returns>Profile variable value if found.</returns>
		public static string Assign(string psName, string psValue, string psRec)
		{
			int nPos1;
			int nPos2;
			int nPos3;
			int nPos4;
			string sValue;
			string sAssign = string.Empty;
			bool bFound;
			if (psValue == string.Empty)
			{
				bFound = false;
				sValue = string.Empty;
				nPos1 = psRec.IndexOf(psName);
				if (nPos1 > -1)
				{
					nPos2 = psRec.Substring(nPos1).IndexOf("=");
					if (nPos2 > -1)
					{
						nPos2 += nPos1;
						nPos3 = psRec.Substring(nPos2).IndexOf(@"""");
						if (nPos3 > -1)
						{
							nPos3 += nPos2;
							nPos4 = psRec.LastIndexOf(@"""");
							if (nPos4 > -1)
							{
								if (nPos4 > nPos3)
								{
									bFound = true;
									nPos3 += 1;
									sValue = psRec.Substring(nPos3, nPos4 - nPos3);
								}
							}
						}
					}
				}
				if (bFound)
				{
					sAssign = sValue;
				}
				else
				{
					sAssign = psValue;
				}
			}
			else
			{
				sAssign = psValue;
			}
			return sAssign;
		}

		/// <summary>
		/// Read the specified file and return its contents as a string.
		/// </summary>
		/// <param name="psFile">Text file specification.</param>
		/// <returns>Entire file contents as a string.</returns>
		public static string ReadFile(string psFile)
		{
			StreamReader oSr;
			string sContents = string.Empty;
			try
			{
				oSr = new StreamReader(psFile);
				sContents = oSr.ReadToEnd();
				oSr.Close();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			return sContents;
		}

		/// <summary>
		/// Write the supplied contents string to the specified file.
		/// </summary>
		/// <param name="psFile">Text file specification.</param>
		/// <param name="psContents">Entire file contents as a string.</param>
		public static void WriteFile(string psFile, string psContents)
		{
			StreamWriter oSw = null;
			try
			{
				File.Delete(psFile);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			finally
			{
				oSw = null;
			}
			try
			{
				oSw = File.CreateText(psFile);
				oSw.Write(psContents);
				oSw.Close();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			finally
			{
				oSw = null;
			}
		}
	}
}
