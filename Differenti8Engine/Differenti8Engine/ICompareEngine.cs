using System;
using System.Collections.Generic;

namespace Differenti8Engine
{
	public interface ICompareEngine
	{
		void CompareFiles(string psNewFile, string psOldFile);
		void CompareLines(List<string> pcNewLines, List<string> pcOldLines);
		bool CompleteLines { get; set; }
		DateTime CompleteTaskStartTime { get; }
		bool Identical { get; }
		Interrupt Interrupt { get; set; }
		long LimitCharacters { get; set; }
        long LimitLines { get; set; }
        long SubLineMatchLimit { get; set; }
        long MinChars { get; set; }
		long MinLines { get; set; }
		string NewFile { get; set; }
		string OldFile { get; set; }
		event CompareEngine.BeginCompareHandler OnBeginCompare;
		event CompareEngine.BeginReportHandler OnBeginReport;
		event CompareEngine.BeginSegmentHandler OnBeginSegment;
		event CompareEngine.EndOfCompareHandler OnEndOfCompare;
		event CompareEngine.EndOfReportHandler OnEndOfReport;
		event CompareEngine.EndOfSegmentHandler OnEndOfSegment;
		event CompareEngine.UpdateCompareHandler OnUpdateCompare;
		event CompareEngine.UpdateReportHandler OnUpdateReport;
		event CompareEngine.UpdateSegmentHandler OnUpdateSegment;
		Sections Sections { get; }
		List<Comparison> Comparisons { get; }
	}
}
