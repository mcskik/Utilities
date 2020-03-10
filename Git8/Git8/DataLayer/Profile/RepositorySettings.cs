using ProfileData.DataLayer.Profile;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Repository settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of repository setting objects.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class RepositorySettings : ProfileEntries<RepositorySetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public RepositorySettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public RepositorySettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "RepositorySettings", "RepositorySetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(RepositorySetting entry)
        {
            UpdateItem.Name = entry.Name;
            UpdateItem.RemoteUrl = entry.RemoteUrl;
            UpdateItem.LocalPath = entry.LocalPath;
        }
    }
}