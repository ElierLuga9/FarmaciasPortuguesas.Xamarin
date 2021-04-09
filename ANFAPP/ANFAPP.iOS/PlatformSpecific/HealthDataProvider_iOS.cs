#undef ANF_LINK_HK
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP.Logic.Utils;
using UIKit;
#if ANF_LINK_HK
using HealthKit;
#endif
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.iOS.PlatformSpecific.HealthDataProvider_iOS))]
namespace ANFAPP.iOS.PlatformSpecific
{
	public class HealthDataProvider_iOS : IHealthDataProvider
	{
		private bool _failedToRequestPermissions = false;

		#if ANF_LINK_HK
		private HKHealthStore _healthStore;
		#else
		private object _healthStore;
		#endif

		public event EventHandler<HealthDataEventArgs> HandleQueryResults;

		public event EventHandler<EventArgs> HandlePermissionsChanged;

		public bool IsSupported()
		{
			#if ANF_LINK_HK
			return UIDevice.CurrentDevice.CheckSystemVersion (8, 0) && HKHealthStore.IsHealthDataAvailable && !_failedToRequestPermissions;
			#else
			return false;
			#endif
		}

		public void Init(object context) { /* Android only. */ }

		public void RequestPermissions()
		{
			#if ANF_LINK_HK
			if (!IsSupported ()) return;

			if (_healthStore == null) {
				_healthStore = new HKHealthStore ();
			}

			var stepsId = HKQuantityTypeIdentifierKey.StepCount;
			var stepsType = HKObjectType.GetQuantityType (stepsId);
			var walkRunId = HKQuantityTypeIdentifierKey.DistanceWalkingRunning;
			var walkRunType = HKObjectType.GetQuantityType (walkRunId);

			var toRead = new NSSet (new [] { stepsType, walkRunType });
			_healthStore.RequestAuthorizationToShare (
				new NSSet(),
				toRead,
				HandleHealthCarePermissions);
			#endif
		}

		private void HandleHealthCarePermissions (bool success, NSError error)
		{
			_failedToRequestPermissions = !success;

			// HealthKit does not return read permissions.

			if (success) {
				if (null != HandlePermissionsChanged) {
					HandlePermissionsChanged (this, new EventArgs ());
				}
			}

		}

		public void QueryStepCount()
		{
			if (!IsSupported ()) return;

			#if ANF_LINK_HK
			var endDate = DateTime.Now.ToUniversalTime();
			var startDate = endDate.AddDays(-1);

			var stepsSampleType = HKSampleType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var predicate = HKQuery.GetPredicateForSamples (startDate.DateTimeToNSDate(), endDate.DateTimeToNSDate(), HKQueryOptions.None);

			var stepsQuery = new HKSampleQuery(stepsSampleType, predicate, 0, new NSSortDescriptor[0], (query, results, error) => {
				if (error != null) {
					System.Diagnostics.Debug.WriteLine(error.LocalizedDescription);
				} else {

					var samples = new List<HealthDataSample>();

					HKUnit unit = HKUnit.Count;

					foreach (HKQuantitySample hkSample in results) {
						var aSample = new HealthDataSample {
							StartDate = hkSample.StartDate.NSDateToDateTime(),
							EndDate = hkSample.EndDate.NSDateToDateTime(),
							Quantity = hkSample.Quantity.GetDoubleValue(unit),
							QuantityType = unit.UnitString,
							IsScalar = true,
							HasMetadata = true,
							WasEnteredByUser = hkSample.Metadata.WasUserEntered.GetValueOrDefault(),
						};

						samples.Add(aSample);
					}

					if (null != HandleQueryResults) {
						HealthDataEventArgs args = new HealthDataEventArgs {
							Type = HealthDataType.Count,
							Samples = samples,
						};

						HandleQueryResults(this, args);
					}
				}	
			});

			_healthStore.ExecuteQuery (stepsQuery);
			#endif
		}



		public void QueryWalkRunDistance()
		{
			var endDate = DateTime.Now.ToUniversalTime();
			var startDate = endDate.AddDays(-1);

			QueryWalkRunDistance (startDate, endDate);
		}

		public void QueryWalkRunDistance(DateTime? start, DateTime? end = null)
		{
			if (!IsSupported ()) return;

			#if ANF_LINK_HK
			var endDate = (end ?? DateTime.UtcNow).ToUniversalTime().DateTimeToNSDate();
			var startDate = start.GetValueOrDefault().ToUniversalTime().DateTimeToNSDate();

			var walkRunSampleType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.DistanceWalkingRunning);
			var predicate = HKQuery.GetPredicateForSamples (startDate, endDate, HKQueryOptions.None);

			var walkRunQuery = new HKSampleQuery(walkRunSampleType, predicate, 0, new NSSortDescriptor[0], (query, results, error) => {
				if (error != null) {
					System.Diagnostics.Debug.WriteLine(error.LocalizedDescription);
				} else {

					var _crashedSample = 0;
					HKSample _debugCurrentSample = null;

					try {

						var samples = new List<HealthDataSample>();

						HKUnit unit = HKUnit.Meter;

						// Xamarin HealthKit implementation (or even HealthKit!) is bonkers and sometimes
						// returns HKSample's which is an abstract class (but with a numeric value on the description string)!
						//
						// This query should always return HKQuantitySamples.
						//
						//foreach (HKSample debug in results) {
						//	System.Diagnostics.Debug.WriteLine(debug.GetType());
						//}

						if (results != null) {
							foreach (HKSample hkSample in results) {

								_debugCurrentSample = hkSample;

								var quantitySample = hkSample as HKQuantitySample;
								if (quantitySample != null) {
									var aSample = new HealthDataSample {
										StartDate = quantitySample.StartDate.NSDateToDateTime(),
										EndDate = quantitySample.EndDate.NSDateToDateTime(),
										Quantity = quantitySample.Quantity.GetDoubleValue(unit),
										QuantityType = unit.UnitString,
										IsScalar = false,
										HasMetadata = quantitySample.Metadata != null,
										WasEnteredByUser = quantitySample.Metadata != null ? quantitySample.Metadata.WasUserEntered.GetValueOrDefault() : false,
									};

									samples.Add(aSample);
								} else {
									// https://insights.xamarin.com/app/ANF-Tests/issues/652
									//
									// In release builds, a HKSample is returned and we need to get the quantity from the description string.
									var aSample = new HealthDataSample {
										StartDate = hkSample.StartDate.NSDateToDateTime(),
										EndDate = hkSample.EndDate.NSDateToDateTime(),
										Quantity = double.Parse(hkSample.Description.Split(null)[0]),
										QuantityType = unit.UnitString,
										IsScalar = false,
										HasMetadata = hkSample.Metadata != null,
										WasEnteredByUser = hkSample.Metadata != null ? hkSample.Metadata.WasUserEntered.GetValueOrDefault() : false,
									};

									samples.Add(aSample);
								}

								_crashedSample++;
							}
						}

						if (null != HandleQueryResults) {
							HealthDataEventArgs args = new HealthDataEventArgs {
								Type = HealthDataType.Distance,
								Samples = samples,
							};

							HandleQueryResults(this, args);
						}
					} catch (Exception ex) {
						var obj = _debugCurrentSample;

						Xamarin.Insights.Report(ex, new Dictionary<string,string> {
							{"SampleRequestType", walkRunSampleType.ToString()},
							{"SampleLength", results.Length.ToString()},
							{"Sample[0]", results.Length > 0 ? results[0].ToString() : ""},
							{"SampleGetType[0]", results.Length > 0 ? results[0].GetType().ToString() : ""},
							{"SampleSampleType[0]", results.Length > 0 ? results[0].SampleType.ToString() : ""},
							{"Sample[C]", obj != null ? obj.ToString() : ""},
							{"SampleGetType[C]", obj != null ? obj.GetType().ToString() : ""},
							{"SampleSampleType[C]", obj != null ? obj.SampleType.ToString() : ""},
							{"SampleAsQuantityType[C]", obj != null ? obj.ToString() : "null"},
							{"SampleMetadata[C]", obj != null && obj.Metadata != null ? obj.Metadata.ToString() : "null"},
							{"C", _crashedSample.ToString()},
						});
					}
				}	
			});

			_healthStore.ExecuteQuery (walkRunQuery);
			#endif
		}
	}
}
