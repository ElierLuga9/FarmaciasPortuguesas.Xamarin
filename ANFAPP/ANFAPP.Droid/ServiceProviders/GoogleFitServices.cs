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
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Common;
using Java.Util.Concurrent;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Data;
using Java.Text;
using Android.Gms.Fitness.Result;
using Java.Util;
using ANFAPP.Logic.Utils;
using ANFAPP.Droid.Utils;


namespace ANFAPP.Droid.ServiceProviders
{
	public class GoogleFitServices : GooglePlayAPIService, IResultCallback
	{

		#region Properties

		private static GoogleFitServices INSTANCE = null;

		public event EventHandler<HealthDataEventArgs> HandleQueryResults = delegate { };

		#endregion

		#region Constructors

		public GoogleFitServices(Activity context) : base(context) { }

		#endregion

		#region Instanciation

		/// <summary>
		/// Returns the singleton, or instanciates a new one if required.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static GoogleFitServices GetInstance(Activity context)
		{
			if (INSTANCE == null) INSTANCE = new GoogleFitServices(context);
			return INSTANCE;
		}

		public override GoogleApiClient InitClient(Activity context)
		{
			return new GoogleApiClient.Builder(context)
				                      .AddApiIfAvailable(FitnessClass.HISTORY_API) // Fitness History API & Scope (Location Read only)
				.AddScope(new Scope(Scopes.FitnessActivityRead))
				.AddOnConnectionFailedListener(this)
				.AddConnectionCallbacks(this)
				.UseDefaultAccount()
				.Build();
		}

		public override int GetClientRequestCode()
		{
			return GOOGLE_FIT_REQUEST_CODE;
		}

		public override void OnConnected(Bundle connectionHint)
		{
			base.OnConnected(connectionHint);
		}

		#endregion

		/// <summary>
		/// Query for distance traveled on google fit's history
		/// </summary>
		/// <param name="startDate"></param>
		public void QueryFitDistance(DateTime? startDate, DateTime? endDate)
		{
			// Init end date
			Calendar calendar = Calendar.Instance;
			if (endDate != null && endDate.HasValue)
			{
				calendar.Time = ANFAPP.Droid.Utils.DateUtils.DateTimeToDate(endDate.Value);
			}
			var endTime = calendar.TimeInMillis;

			// Init start date
			if (startDate != null && startDate.HasValue)
			{
				calendar.Time = ANFAPP.Droid.Utils.DateUtils.DateTimeToDate(startDate.Value);
			}
			else
			{
				calendar.Set(CalendarField.HourOfDay, 0);
				calendar.Set(CalendarField.Minute, 0);
				calendar.Set(CalendarField.Second, 0);
				calendar.Set(CalendarField.Millisecond, 0);
			}
			long startTime = calendar.TimeInMillis;

			// Build the request
			var fitRequest = new DataReadRequest.Builder()
				.Aggregate(DataType.TypeDistanceDelta, DataType.AggregateDistanceDelta)
				.BucketByActivitySegment(1, TimeUnit.Minutes)
				.EnableServerQueries()
				.SetTimeRange(
					startTime, // Start time
					endTime, // End time
					TimeUnit.Milliseconds)
				.Build();

			// Call WS async and callback
		//	FitnessClass.HistoryApi.ReadData();
			FitnessClass.HistoryApi.ReadData(Client, fitRequest).SetResultCallback(this, 1, TimeUnit.Minutes);
		}

		/// <summary>
		/// Callback for HistoriAPI reading work.
		/// </summary>
		/// <param name="result"></param>
		public void OnResult(Java.Lang.Object result)
		{
			var samples = new List<HealthDataSample>();
			if (result == null) 
			{
				NotifyHealthDataSample(samples);
				return;
			}

			var readResult = (DataReadResult)result;
			if (readResult.Buckets == null || readResult.Buckets.Count == 0)
			{
				NotifyHealthDataSample(samples);
				return;
			}

			// Parse buckets
			foreach (var bucket in readResult.Buckets)
			{
				if (bucket.DataSets == null) continue;
				foreach (var dataSet in bucket.DataSets)
				{
					if (dataSet.DataPoints == null) continue;
					foreach (var dataPoint in dataSet.DataPoints)
					{
						var dataType = dataPoint.DataType;
						if (dataType.Fields == null) continue;

						var endDate = ANFAPP.Droid.Utils.DateUtils.DateToDateTime(new Date(dataPoint.GetEndTime(TimeUnit.Milliseconds)));
						var startDate = ANFAPP.Droid.Utils.DateUtils.DateToDateTime(new Date(dataPoint.GetStartTime(TimeUnit.Milliseconds)));

						foreach (var field in dataType.Fields)
						{
							// Build new Data Sample
							var value = dataPoint.GetValue(field).AsFloat();
							samples.Add(new HealthDataSample() {
								Type = HealthDataType.Distance,
								StartDate = startDate,
								EndDate = endDate,
								Quantity = value		
							});
						}
					}
				}
			}

			/// Notify listeners
			NotifyHealthDataSample(samples);
		}

		/// <summary>
		/// Notify listeners with health data sample
		/// </summary>
		/// <param name="sampleList"></param>
		private void NotifyHealthDataSample(List<HealthDataSample> sampleList)
		{
			HandleQueryResults(this, new HealthDataEventArgs() {
				Type = HealthDataType.Distance,
				Samples = sampleList
			});
		}

	}
}