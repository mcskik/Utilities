using ProfileData.DataLayer.Profile;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Template parameter settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of template parameter setting objects.
    /// This will be used for most lists of parameters which only need a single parameter value each.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class TemplateParameterSettings : ProfileEntries<TemplateParameterSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public TemplateParameterSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public TemplateParameterSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "TemplateParameterSettings", "TemplateParameterSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(TemplateParameterSetting entry)
        {
            UpdateItem.Value = entry.Value;
            UpdateItem.Description = entry.Description;
        }
    }
}