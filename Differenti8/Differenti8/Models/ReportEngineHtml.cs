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
	/// Html report engine.
	/// </summary>
	/// <remarks>
	/// Html report engine.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class ReportEngineHtml : ReportEngine
	{
		#region Constructors.
		/// <summary>
		/// Main constructor.
		/// </summary>
		public ReportEngineHtml(ICompareEngine compareEngine, IProfileManager profileManager) : base(compareEngine, profileManager)
		{
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Produce text report.
		/// </summary>
		public override void Report()
		{
			//Start the log.
            string outputFile = _profileManager.ProfileCache.Fetch("HtmlOutputFile");
            FileHelper.PathCheck(outputFile);
            moLog = new Logger(outputFile);

            //Header block.
            moLog.Write(FileHelper.ReadFile(_profileManager.ProfileCache.Fetch("IncludeHeader")));
            moLog.Write(FileHelper.ReadFile(_profileManager.ProfileCache.Fetch("IncludeScripts")));
            moLog.Write(FileHelper.ReadFile(_profileManager.ProfileCache.Fetch("IncludeBody")));

            //Main process.
            Process();

            //Footer block.
            moLog.Write(FileHelper.ReadFile(_profileManager.ProfileCache.Fetch("IncludeFooter")));

            //Finish the log.
            moLog.Finish();
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
				moLog.WriteLn("<br/>");
                moLog.WriteLn("***********************<br/>");
                moLog.WriteLn("* Files are identical *<br/>");
                moLog.WriteLn("***********************<br/>");
                moLog.WriteLn("<br/>");
			}

			//Check if user has chosen to cancel run.
			if (Interrupt.Reason == "Cancel")
			{
				return;
			}

			//Header.
            moLog.WriteLn("New File : " + NewFile + "<br/>");
            moLog.WriteLn("Old File : " + OldFile + "<br/>");
            moLog.WriteLn("Character Limit : " + LimitCharacters + "<br/>");
            moLog.WriteLn("Line Limit      : " + LimitLines + "<br/>");
            moLog.WriteLn("Sub Line Limit  : " + SubLineMatchLimit + "<br/>");
            moLog.WriteLn("<br/>");
            moLog.WriteLn("Delta<br/>");
            moLog.WriteLn("-----<br/>");

			//Report.
            SignalBeginReport(Sections.Count);
            string handoverType = string.Empty;
            bool bCharacterMode = false;
			for (int nCount = 0; nCount < Sections.Count; nCount++)
			{
                string identity = nCount.ToString().Trim();
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
					ReporterOfHtmlLines oReporter = new ReporterOfHtmlLines(oSectionOfLines, moLog);
                    oReporter.HandoverType = handoverType;
                    oReporter.Display(identity);
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
					ReporterOfHtmlCharacters oReporter = new ReporterOfHtmlCharacters(oSectionOfCharacters, moLog);
                    oReporter.HandoverType = handoverType;
                    oReporter.Display(oSectionOfCharacters.Indentation, identity);
                    handoverType = oReporter.Type;
                }
				else
				{
					Console.WriteLine("Unknown kind of section.");
				}
                SignalUpdateReport(1);
            }
            //Padding at the bottom to allow full scrolling.
            for (int blankLine = 1; blankLine <= 5; blankLine++)
            {
                moLog.WriteLn("<br/>");
            }
            SignalEndOfReport();
		}
		#endregion
	}
}