using ProfileData.DataLayer.Profile;

namespace GlobalChange8.DataLayer.Profile
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
        #endregion

        #region Constructors.
        public ProfileManager()
        {
            ProfileCache = new ProfileCache();
            ApplicationProfile = new ApplicationProfile(ProfileCache);
            SystemProfile = new SystemProfile(ProfileCache);
            if (SystemProfile.MasterCriteria != null) FileHelper.EnsureExists(SystemProfile.CurrentCriteria, SystemProfile.MasterCriteria);
            UserSettings = new UserSettings(ProfileCache);
            UserSettings.Load(SystemProfile.CurrentUserSettings, SystemProfile.MasterUserSettings);
        }
        #endregion
    }
}