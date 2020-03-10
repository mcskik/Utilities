using ProfileData.DataLayer.Profile;

namespace Same8.DataLayer.Profile
{
    /// <summary>
    /// Current user settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of user setting objects.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class UserSettings : ProfileEntries<UserSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public UserSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public UserSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "UserSettings", "UserSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(UserSetting entry)
        {
            UpdateItem.NewPath = entry.NewPath;
            UpdateItem.OldPath = entry.OldPath;
            UpdateItem.KeyShrunk = entry.KeyShrunk;
            UpdateItem.NewFilesEstimate = entry.NewFilesEstimate;
            UpdateItem.OldFilesEstimate = entry.OldFilesEstimate;
            UpdateItem.ChgFilesEstimate = entry.ChgFilesEstimate;
            UpdateItem.MonitoredTypesOnly = entry.MonitoredTypesOnly;
        }
    }
}