using Delini8.DataLayer.Tracing;
using System;
using System.Diagnostics;

namespace Delini8.DataLayer.Profile
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

        public static String LogPath { get; set; }

        public static Tracer Tracer
        {
            get
            {
                if (_tracer == null)
                {
                    _tracer = new Tracer(ProfileManager.SystemProfile.LogPath, "Delini8", DateTime.Now, "DL8", String.Format(@"Delini8 ({0})", _profileManager.ApplicationProfile.Version), true, false);
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

        public static void View()
        {
            _tracer.Outcome();
            _tracer.Finish();
            View(ProfileManager.SystemProfile.ViewerUnix, _tracer.LogFileSpec);
            Reset();
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