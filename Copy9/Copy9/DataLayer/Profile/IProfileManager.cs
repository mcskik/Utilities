using ProfileData.DataLayer.Profile;
namespace Copy9.DataLayer.Profile
{
    public interface IProfileManager
	{
        ProfileCache ProfileCache { get; set; }
        ApplicationProfile ApplicationProfile { get; set; }
        SystemProfile SystemProfile { get; set; }
        UserSettings UserSettings { get; set; }
        Interrupt Interrupt { get; set; }
	}
}