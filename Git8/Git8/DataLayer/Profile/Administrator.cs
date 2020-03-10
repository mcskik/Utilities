using System;
using Git8.DataLayer.Tracing;

namespace Git8.DataLayer.Profile
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
        private static Tracer _performanceTracer = null;

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
                    _tracer = new Tracer(ProfileManager.SystemProfile.LogPath, "TravelFusion Robot", DateTime.Now, "TRN", "TravelFusion Robot Test Harness", true, false);
                }
                return _tracer;
            }
        }

        public static Tracer PerformanceTracer { get; set; }
        public static string Origin { get; set; }
        public static string Destination { get; set; }

        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}