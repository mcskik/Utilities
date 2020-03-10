using ProfileData.DataLayer.Profile;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Parameter settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of parameter setting objects.
    /// This will be used for most lists of parameters which only need a single parameter value each.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class ParameterSettings : ProfileEntries<ParameterSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public ParameterSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public ParameterSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "ParameterSettings", "ParameterSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(ParameterSetting entry)
        {
            UpdateItem.Value = entry.Value;
        }
    }
}