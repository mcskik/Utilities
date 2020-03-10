using ProfileData.DataLayer.Profile;

namespace GlobalChange8.DataLayer.Profile
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
            UpdateItem.SearchPath = entry.SearchPath;
            UpdateItem.Criteria = entry.Criteria;
            UpdateItem.Find = entry.Find;
            UpdateItem.Replacement = entry.Replacement;
            UpdateItem.Mode = entry.Mode;
            UpdateItem.DirFilesEstimate = entry.DirFilesEstimate;
            UpdateItem.FilePattern = entry.FilePattern;
            UpdateItem.RegexCriteria = entry.RegexCriteria;
            UpdateItem.AllTypes = entry.AllTypes;
            UpdateItem.Encoding = entry.Encoding;
            UpdateItem.Extensions = entry.Extensions;
            UpdateItem.DirectoryExclusions = entry.DirectoryExclusions;
        }
    }
}