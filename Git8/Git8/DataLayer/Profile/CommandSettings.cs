using ProfileData.DataLayer.Profile;

namespace Git8.DataLayer.Profile
{
    /// <summary>
    /// Command settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of command setting objects.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class CommandSettings : ProfileEntries<CommandSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public CommandSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public CommandSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "CommandSettings", "CommandSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(CommandSetting entry)
        {
            UpdateItem.Line = entry.Line;
            UpdateItem.Template = entry.Template;
        }

        public void Clear()
        {
            this.Entries.Clear();
            Save();
        }
    }
}