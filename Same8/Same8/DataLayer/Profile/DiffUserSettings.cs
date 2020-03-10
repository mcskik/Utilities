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
    public class DiffUserSettings : ProfileEntries<DiffUserSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public DiffUserSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public DiffUserSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "UserSettings", "UserSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(DiffUserSetting entry)
        {
            UpdateItem.NewBaseDir = entry.NewBaseDir;
            UpdateItem.OldBaseDir = entry.OldBaseDir;
            UpdateItem.NewFile = entry.NewFile;
            UpdateItem.OldFile = entry.OldFile;
            UpdateItem.KeyShrunk = entry.KeyShrunk;
            UpdateItem.MinChars = entry.MinChars;
            UpdateItem.MinLines = entry.MinLines;
            UpdateItem.LimitCharacters = entry.LimitCharacters;
            UpdateItem.LimitLines = entry.LimitLines;
            UpdateItem.SubLineMatchLimit = entry.SubLineMatchLimit;
            UpdateItem.CompleteLines = entry.CompleteLines;
        }
    }
}