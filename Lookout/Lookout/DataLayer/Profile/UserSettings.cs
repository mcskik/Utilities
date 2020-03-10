using ProfileData.DataLayer.Profile;

namespace Lookout.DataLayer.Profile
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
            UpdateItem.Folder = entry.Folder;
            UpdateItem.SenderCriteria = entry.SenderCriteria;
            UpdateItem.SubjectCriteria = entry.SubjectCriteria;
            UpdateItem.BodyCriteria = entry.BodyCriteria;
        }
    }
}