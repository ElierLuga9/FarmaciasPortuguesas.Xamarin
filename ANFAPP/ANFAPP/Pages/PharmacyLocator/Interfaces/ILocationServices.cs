using System;
using ANFAPP.Logic.Models.Objects;

namespace ANFAPP
{
	public interface ILocationServices
	{
		void Init (object context);

		bool LocationServicesAvailable ();

		void StartUpdatingLocation ();

		Location CurrentUserLocation ();

		void StopUpdatingLocation ();
	}
}
