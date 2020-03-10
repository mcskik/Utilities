using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using R = Routines8.Routines;

namespace Log8
{
    public class DummyLogger : ILogger
	{
		#region Member Variables.
		private string _Title = string.Empty;
		private string _Prefix = string.Empty;
		private int _Number = 0;
		#endregion

		#region Properties.
		public bool Active
        {
            get
            {
                return false;
            }
        }

        public string Spec
        {
            get
            {
                return string.Empty;
            }
        }

		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				_Title = value;
			}
		}

		public string Prefix
		{
			get
			{
				return _Prefix;
			}
			set
			{
				_Prefix = value;
			}
		}

		public string HighestSeverity
        {
            get
            {
                return string.Empty;
            }
        }

		public int Number
		{
			get
			{
				return _Number;
			}
			set
			{
				_Number = value;
			}
		}

		public string Source
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion

		#region Constructors.
        public DummyLogger()
        {
        }

        public DummyLogger(string fileSpec) : this()
        {
        }

        public DummyLogger(string fileSpec, string prefix, string title) : this()
        {
        }
        #endregion

		#region Public Methods.
        public void Begin(string fileSpec)
        {
        }

        public void Begin(string fileSpec, Logger.LogMode mode)
        {
        }

        public void Clear()
        {
        }

        public void WriteErr(string serial, string severity, string message, int number, string source)
        {
        }

        public void WriteMsg(string serial, string severity, string message)
        {
        }

        public void WriteTimedMsg(string serial, string severity, string message)
        {
        }

        public void WriteLn()
        {
        }

        public void WriteLn(string line)
        {
        }

        public void Write(string text)
        {
        }

        public void Outcome()
        {
        }

        public void Terminate()
        {
        }

        public void Terminate(string viewerProgram)
        {
        }

        public void Finish()
        {
        }

        public void Finish(string viewerProgram)
        {
        }
        #endregion
    }
}
