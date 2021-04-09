using System;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{	
	public interface IHealthDataProvider
	{
		event EventHandler<HealthDataEventArgs> HandleQueryResults;

		event EventHandler<EventArgs> HandlePermissionsChanged;

		void Init(object context);

		bool IsSupported();

		void RequestPermissions();

		/// <summary>
		/// Queries the step count in the last 24h.
		/// </summary>
		void QueryStepCount();

		/// <summary>
		/// Queries the distance ran or walked in the last 24h.
		/// </summary>
		void QueryWalkRunDistance();

		void QueryWalkRunDistance(DateTime? start, DateTime? end = null);

	}
}

