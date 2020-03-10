using System;
namespace Log8
{
    interface ILogger
    {
        bool Active { get; }
        void Begin(string fileSpec, Logger.LogMode mode);
        void Begin(string fileSpec);
        void Clear();
        void Finish(string viewerProgram);
        void Finish();
        string HighestSeverity { get; }
        int Number { get; set; }
        void Outcome();
        string Prefix { get; set; }
        string Source { get; }
        string Spec { get; }
        void Terminate();
        void Terminate(string viewerProgram);
        string Title { get; set; }
        void Write(string text);
        void WriteErr(string serial, string severity, string message, int number, string source);
        void WriteLn();
        void WriteLn(string line);
        void WriteMsg(string serial, string severity, string message);
        void WriteTimedMsg(string serial, string severity, string message);
    }
}
