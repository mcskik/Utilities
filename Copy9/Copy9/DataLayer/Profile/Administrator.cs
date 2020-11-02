using Copy9.DataLayer.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Copy9.DataLayer.Profile
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
                    _tracer = new Tracer(ProfileManager.SystemProfile.LogPath, "Copy9", DateTime.Now, "CP9", String.Format(@"Copy9 ({0})", _profileManager.ApplicationProfile.Version), true, false);
                }
                return _tracer;
            }
        }

        public static Tracer TypesTracer
        {
            get
            {
                //if (_typesTracer == null)
                //{
                //    _typesTracer = new Tracer(ProfileManager.SystemProfile.LogPath, "Types9", DateTime.Now, "TP9", String.Format(@"Types9 ({0})", _profileManager.ApplicationProfile.Version), true, false);
                //}
                //TODO: Write to a separate file.  For now just write to the main log file so that I can see the file types each time.
                return _tracer;
                //return _typesTracer;
            }
        }

        public static Tracer OldLogsTracer
        {
            get
            {
                if (_oldLogsTracer == null)
                {
                    _oldLogsTracer = new Tracer(ProfileManager.SystemProfile.LogPath, "OldLogs9", DateTime.Now, "OL9", String.Format(@"OldLogs9 ({0})", _profileManager.ApplicationProfile.Version), true, false);
                }
                return _oldLogsTracer;
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
            if (_fileTypes != null)
            {
                foreach (KeyValuePair<string, string> entry in _fileTypes)
                {
                    if (entry.Value == "Ignored")
                    {
                        TypesTracer.WriteLine(entry.Key.PadRight(16) + " " + entry.Value);
                    }
                }
            }
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
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            process.Start();
        }
    }
}