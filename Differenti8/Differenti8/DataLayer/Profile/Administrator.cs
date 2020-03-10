using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;

namespace Differenti8.DataLayer.Profile
{
    /// <summary>
    /// Administrator class.
    /// </summary>
    /// <remarks>
    /// Obtains references to singleton profile objects.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class Administrator
    {
        //private static Profile _profile;
        private static ProfileManager _profileManager;

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

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Administrator()
        {
        }
    }
}