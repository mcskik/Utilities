using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Differenti8.DataLayer.Profile;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
	/// <summary>
	/// Text report engine.
	/// </summary>
	/// <remarks>
	/// Text report engine.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ReportEngineText : ReportEngine
	{
		#region Constructors.
		/// <summary>
		/// Main constructor.
		/// </summary>
		public ReportEngineText(ICompareEngine compareEngine, IProfileManager profileManager) : base(compareEngine, profileManager)
		{
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Produce text report.
		/// </summary>
		public override void Report()
		{
			//Start log object.
			string sLogFile = msLogPath + Path.DirectorySeparatorChar.ToString() + @"Dif_" + DateTime.Now.ToString("yyyyMMdd@HHmmss") + ".log";
            FileHelper.PathCheck(sLogFile);
            moLog = new Logger(sLogFile, "DIF", "Differenti8 Comparison Utility " + msVersion, this.CompleteTaskStartTime);
			//Main process.
			Process();
			//Terminate and view log object.
			moLog.Outcome();
			moLog.Terminate(msViewerProgram);
			moLog = null;
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Main Process.
		/// </summary>
		private void Process()
		{
			if (Identical)
			{
				moLog.WriteLn();
				moLog.WriteLn("***********************");
				moLog.WriteLn("* Files are identical *");
				moLog.WriteLn("***********************");
				moLog.WriteLn();
			}

			//Check if user has chosen to cancel run.
			if (Interrupt.Reason == "Cancel")
			{
				return;
			}

			//Header.
			moLog.WriteMsg("010", "I", "New File : " + NewFile);
			moLog.WriteMsg("020", "I", "Old File : " + OldFile);
			moLog.WriteMsg("030", "I", "Character Limit : " + LimitCharacters);
            moLog.WriteMsg("040", "I", "Line Limit      : " + LimitLines);
            moLog.WriteMsg("050", "I", "Sub Line Limit  : " + SubLineMatchLimit);
            moLog.WriteLn();
			moLog.WriteLn("Delta");
			moLog.WriteLn("-----");

			//Report.
            SignalBeginReport(Sections.Count);
            string handoverType = string.Empty;
            bool bCharacterMode = false;
			for (int nCount = 0; nCount < Sections.Count; nCount++)
			{
                if (nCount > 1006)
                {
                    string temp = string.Empty;
                }
				Section oSection = (Section)Sections[nCount];
				if (oSection is SectionOfLines)
				{
					if (bCharacterMode)
					{
						bCharacterMode = false;
						//moLog.WriteLn("<<<");
					}
					SectionOfLines oSectionOfLines = (SectionOfLines)oSection;
					ReporterOfTextLines oReporter = new ReporterOfTextLines(oSectionOfLines, moLog);
                    oReporter.HandoverType = handoverType;
                    oReporter.Display();
                    handoverType = oReporter.Type;
				}
				else if (oSection is SectionOfCharacters)
				{
					if (!bCharacterMode)
					{
						bCharacterMode = true;
						//moLog.WriteLn(">>>");
					}
					SectionOfCharacters oSectionOfCharacters = (SectionOfCharacters)oSection;
					ReporterOfTextCharacters oReporter = new ReporterOfTextCharacters(oSectionOfCharacters, moLog);
                    oReporter.HandoverType = handoverType;
                    oReporter.Display(oSectionOfCharacters.Indentation);
                    handoverType = oReporter.Type;
                }
				else
				{
					Console.WriteLine("Unknown kind of section.");
				}
                SignalUpdateReport(1);
            }
            SignalEndOfReport();
		}
		#endregion
	}
}