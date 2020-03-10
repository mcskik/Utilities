using ProfileData.DataLayer.Profile;

namespace Same8.DataLayer.Profile
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
        public ProfileCache ProfileCache { get; set; }
        public ApplicationProfile ApplicationProfile { get; set; }
        public SystemProfile SystemProfile { get; set; }
        public UserSettings UserSettings { get; set; }
        public DiffUserSettings DiffUserSettings { get; set; }

        public ProfileManager()
        {
            ProfileCache = new ProfileCache();
            ApplicationProfile = new ApplicationProfile(ProfileCache);
            SystemProfile = new SystemProfile(ProfileCache);
            UserSettings = new UserSettings(ProfileCache);
            UserSettings.Load(SystemProfile.CurrentUserSettings, SystemProfile.MasterUserSettings);
            DiffUserSettings = new DiffUserSettings(ProfileCache);
            DiffUserSettings.Load(SystemProfile.DiffCurrentUserSettings, SystemProfile.DiffMasterUserSettings);
        }
    }
}