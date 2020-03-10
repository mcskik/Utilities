using ProfileData.DataLayer.Profile;
using System;
namespace Copy8.DataLayer.Profile
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