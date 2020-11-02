using ProfileData.DataLayer.Profile;

namespace Copy9.DataLayer.Profile
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
            UpdateItem.NewBaseDir = entry.NewBaseDir;
            UpdateItem.OldBaseDir = entry.OldBaseDir;
            UpdateItem.KeyShrunk = entry.KeyShrunk;
            UpdateItem.CopyRule = entry.CopyRule;
            UpdateItem.MonitoredTypesOnly = entry.MonitoredTypesOnly;
            UpdateItem.DateThreshold = entry.DateThreshold;
        }
    }
}