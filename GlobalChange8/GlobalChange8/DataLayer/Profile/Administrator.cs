using GlobalChange8.DataLayer.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GlobalChange8.DataLayer.Profile
{
    /// <summary>
    /// Administrator class.
    /// </summary>
    /// <remarks>
    /// Provides global access to the profile cache, profile manager, and tracer objects.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class Administrator
    {
        private static ProfileManager _profileManager = null;
        private static Tracer _tracer = null;
        private static Tracer _typesTracer = null;
        private static Tracer _oldLogsTracer = null;
        private static SortedDictionary<string, string> _fileTypes;

        /// <summary>
        /// Profile Manager Instance.
        /// </summary>
        public static ProfileManager ProfileManager
        {
            get
            {
                if (_profileManager == null)
                {
                    _profileManager = new ProfileManager();
                }
                return _profileManager;
            }
        }

        public static Tracer Tracer
        {
            get
            {
                if (_tracer == null)
                {
                    _tracer = new Tracer(ProfileManager.SystemProfile.LogPath, "GlobalChange8", DateTime.Now, "GC8", String.Format(@"GlobalChange8 ({0})", _profileManager.ApplicationProfile.Version), true, false);
                }
                return _tracer;
            }
        }

        public static void Reset()
        {
            _tracer = null;
        }

        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public static void RecordFileType(string fileSpec, bool allowFile)
        {
            if (_fileTypes == null)
            {
                _fileTypes = new SortedDictionary<string, string>();
            }
            string fileType = Path.GetExtension(fileSpec);
            if (!_fileTypes.ContainsKey(fileType))
            {
                string processed = allowFile ? "Processed" : "Ignored";
                _fileTypes.Add(fileType, processed);
            }
        }

        public static void View()
        {
            _tracer.Outcome();
            _tracer.Finish();
            if (_typesTracer != null)
            {
                _typesTracer.Outcome();
                _typesTracer.Finish();
            }
            View(ProfileManager.SystemProfile.ViewerUnix, _tracer.LogFileSpec);
            //Reset();
        }

        public static void View(string viewerProgram, string fileSpec)
        {
            Process process = new Process();
            process.StartInfo.FileName = viewerProgram;
            process.StartInfo.Arguments = fileSpec;
            process.EnableRaisingEvents = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
        }
    }
}
