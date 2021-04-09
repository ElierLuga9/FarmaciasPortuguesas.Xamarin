using ANFAPP.Logic.Utils;
using ANFAPP.WinPhone.PlatformSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(FitnessServices_WP))]
namespace ANFAPP.WinPhone.PlatformSpecific
{
	public class FitnessServices_WP : IHealthDataProvider
	{

		#region Implementation

		public void Init(object context)
		{
			
		}

		public bool IsSupported()
		{
			// Always true on android!
			return true;
		}

		public void RequestPermissions()
		{
			
		}

		public void QueryStepCount()
		{
			// Do nothing?
		}

		public void QueryWalkRunDistance()
		{
		
		}

		public void QueryWalkRunDistance(DateTime? start, DateTime? end = null)
		{
			
		}

		

		#endregion


		public event EventHandler<HealthDataEventArgs> HandleQueryResults;

		public event EventHandler<EventArgs> HandlePermissionsChanged;
	}
}