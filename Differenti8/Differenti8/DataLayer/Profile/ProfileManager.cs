using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8.DataLayer.Profile
{
    /// <summary>
    /// Profile manager container class.
    /// </summary>
    /// <remarks>
    /// Container class to provide access to individual profiles.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ProfileManager : IProfileManager
    {
        #region Properties
        public ProfileCache ProfileCache { get; set; }
        public ApplicationProfile ApplicationProfile { get; set; }
        public SystemProfile SystemProfile { get; set; }
        public UserSettings UserSettings { get; set; }
        public Differenti8Engine.Interrupt Interrupt { get; set; }
        #endregion

        #region Constructors.
        public ProfileManager()
        {
            ProfileCache = new ProfileCache();
            ApplicationProfile = new ApplicationProfile(ProfileCache);
            SystemProfile = new SystemProfile(ProfileCache);
            UserSettings = new UserSettings(ProfileCache);
            UserSettings.Load(SystemProfile.CurrentUserSettings, SystemProfile.MasterUserSettings);
            Interrupt = new Differenti8Engine.Interrupt("OK");
        }
        #endregion
    }
}