using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ANFAPP.Logic.Utils;
using ANFAPP.Droid.ServiceProviders;
using ANFAPP.Droid.PlatformSpecific;
using Xamarin.Forms;

[assembly: Dependency(typeof(FitnessServices_Droid))]
namespace ANFAPP.Droid.PlatformSpecific
{
	public class FitnessServices_Droid : IHealthDataProvider
	{

		#region Events & Properties

		public event EventHandler<HealthDataEventArgs> HandleQueryResults = delegate { };
		public event EventHandler<EventArgs> HandlePermissionsChanged = delegate { };

		public Activity Context { get; set; }

		#endregion

		#region Implementation

		public void Init(object context)
		{
			if (context == null || !(context is Activity)) return;
			Context = context as Activity;
		}

		public bool IsSupported()
		{
			// Always true on android!
			return true;
		}

		public void RequestPermissions()
		{
			if (Context == null) return;

			var fitInstance = GoogleFitServices.GetInstance(Context);
			if (fitInstance.IsConnected)
			{
				// Already connected & has permission
				HandlePermissionsChanged(this, null);
				return;
			}

			// Build Handler
			EventHandler handler = null;
			handler = (sender, args) =>
			{
				fitInstance.OnConnectedEvent -= handler;
				HandlePermissionsChanged(sender, args);
			};

			// Connect & request permission
			fitInstance.OnConnectedEvent += handler;
			fitInstance.Connect();
		}

		public void QueryStepCount()
		{
			// Do nothing?
		}

		public void QueryWalkRunDistance()
		{
			QueryWalkRunDistance(null, null);
		}

		public void QueryWalkRunDistance(DateTime? start, DateTime? end = null)
		{
			if (Context == null) return;

			var fitInstance = GoogleFitServices.GetInstance(Context);
			if (!fitInstance.IsConnected)
			{
				// Client not connected - Connect and then query for results

				// Build Handler
				EventHandler handler = null;
				handler = (sender, args) =>
				{
					fitInstance.OnConnectedEvent -= handler;
					fitInstance.QueryFitDistance(start, end);
				};

				// Connect & request permission
				fitInstance.OnConnectedEvent += handler;
				fitInstance.Connect();
				return;
			}

			// Client connected - query for results
			EventHandler<HealthDataEventArgs> queryHandler = null;
			queryHandler = (sender, args) =>
			{
				fitInstance.HandleQueryResults -= queryHandler;
				HandleQueryResults(sender, args);
			};

			// Process query
			fitInstance.HandleQueryResults += queryHandler;
			fitInstance.QueryFitDistance(start, end);
		}

		#endregion

	}
}