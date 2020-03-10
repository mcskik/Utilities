using System;
namespace Differenti8.DataLayer.Profile
{
	public interface IProfileManager
	{
        ProfileCache ProfileCache { get; set; }
        ApplicationProfile ApplicationProfile { get; set; }
        SystemProfile SystemProfile { get; set; }
        UserSettings UserSettings { get; set; }
        Differenti8Engine.Interrupt Interrupt { get; set; }
	}
}