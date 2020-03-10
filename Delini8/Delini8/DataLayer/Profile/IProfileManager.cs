using ProfileData.DataLayer.Profile;

namespace Delini8.DataLayer.Profile
{
    public interface IProfileManager
    {
        ProfileCache ProfileCache { get; set; }
        ApplicationProfile ApplicationProfile { get; set; }
        SystemProfile SystemProfile { get; set; }
    }
}