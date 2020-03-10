using ProfileData.DataLayer.Profile;

namespace Search8.DataLayer.Profile
{
    /// <summary>
    /// Profile manager container interface.
    /// </summary>
    /// <remarks>
    /// Interface that container class must implement to provide access to individual profiles.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public interface IProfileManager
    {
        ProfileCache ProfileCache { get; set; }
        ApplicationProfile ApplicationProfile { get; set; }
        SystemProfile SystemProfile { get; set; }
        UserSettings UserSettings { get; set; }
    }
}