using System;

namespace Copy8.DataLayer.Tracing
{
    /// <summary>
    /// ITracer interface.
    /// </summary>
    /// <remarks>
    /// Common interface used by the Tracer and SqlTracer objects.
    /// This makes it possible to switch between the file based tracer
    /// and the SqlContext.Pipe based tracer with minimal change.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public interface ITracer
    {
        void Begin();
        void BeginTask();
        void Finish();
        void FinishTask(string message);
        void FinishTask(string serial, string severity, string message);
        void FinishTask(string severity, string message);
        string HighestSeverity { get; set; }
        void Outcome();
        string Prefix { get; set; }
        DateTime StartedAt { get; }
        string Title { get; set; }
        void WriteLine();
        void WriteLine(string line);
        void WriteMsg(string message);
        void WriteMsg(string serial, string severity, string message);
        void WriteMsg(string severity, string message);
        void WriteTimedMsg(string message);
        void WriteTimedMsg(string serial, string severity, string message);
        void WriteTimedMsg(string severity, string message);
    }
}