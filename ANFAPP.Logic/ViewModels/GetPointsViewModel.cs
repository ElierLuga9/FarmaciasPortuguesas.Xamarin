using System;
using System.Collections.Generic;
using Xamarin.Forms;

using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Exceptions;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Utils;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{

	public class GetPointsViewModel : IViewModel 
	{
		
		#region Properties

		private bool _hkAchievementsEnabled;

		private ObservableCollection<AchievementGroup> _Achievements;
		public ObservableCollection<AchievementGroup> Achievements
		{
			get { return _Achievements; }
			set
			{
				_Achievements = value;
				OnPropertyChanged("Achievements");

				OnPropertyChanged("FirstBarColor");
				OnPropertyChanged("SecondBarColor");
				OnPropertyChanged("ThirdBarColor");
			}
		}

		private int _totalDoneAchievements;
		public int TotalDoneAchievements
		{
			get { return _totalDoneAchievements; }
			set
			{
				_totalDoneAchievements = value;
				OnPropertyChanged("TotalDoneAchievements");

				OnPropertyChanged("FirstBarColor");
				OnPropertyChanged("SecondBarColor");
				OnPropertyChanged("ThirdBarColor");
			}
		}

		private int _totalAchievements;
		public int TotalAchievements
		{
			get { return _totalAchievements; }
			set
			{
				_totalAchievements = value;
				OnPropertyChanged("TotalAchievements");
			}
		}

		public Color FirstBarColor
		{
			get 
			{ 
				//if (TotalAchievements == 0) return ColorResources.ANFPaleGrey;
				//double ratio = (double) TotalDoneAchievements / TotalAchievements;

				//if (ratio >= 0.33 && ratio <= 1.0)
				//{
				//	return ColorResources.PointsOrangeBar;
				//}
				//else
				//{
				//	return ColorResources.ANFPaleGrey;
				//}
				return TotalDoneAchievements > 0 ? ColorResources.PointsOrangeBar : ColorResources.ANFPaleGrey;
			}
		}

		public Color SecondBarColor
		{
			get
			{
				if (TotalAchievements == 0) return ColorResources.ANFPaleGrey;
				double ratio = (double)TotalDoneAchievements / TotalAchievements;

				if (ratio >= 0.66 && ratio <= 1.0)
				{
					return ColorResources.PointsYellowBar;
				}
				else
				{
					return ColorResources.ANFPaleGrey;
				}
			}
		}

		public Color ThirdBarColor
		{
			get
			{
				if (TotalAchievements == 0) return ColorResources.ANFPaleGrey;
				double ratio = (double)TotalDoneAchievements / TotalAchievements;

				if (ratio >= (1.0 - double.Epsilon))
				{
					return ColorResources.PointsGreenBar;
				}
				else
				{
					return ColorResources.ANFPaleGrey;
				}
			}
		}

		public bool HasCard
		{
			get 
			{ 
				int cardNumber;

				return SessionData.PharmacyUser != null 
					&& !string.IsNullOrEmpty(SessionData.PharmacyUser.CardNumber)
					&& int.TryParse(SessionData.PharmacyUser.CardNumber.Trim(), out cardNumber)
					&& cardNumber > 0;
			}
		}

		public bool CanEnableFitnessData
		{
			get 
			{ 
				return SessionData.HasPharmacyCard 
					&& !SessionData.HasFitnessServicesEnabled 
					&& (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
					&& _hkAchievementsEnabled; 
			}
		}

		#endregion

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		public GetPointsViewModel()
		{
			var hk = DependencyService.Get<IHealthDataProvider>();
			hk.HandlePermissionsChanged += OnPermissionsChanged;
			hk.HandleQueryResults += OnHealthDataReceived;
		}

		#region Loaders

		public async Task LoadData(bool updateFitnessData = false) 
		{
			if (!Settings.IS_GAMIFICATION_ACTIVE) return;

			try
			{
				if (OnLoadStart != null) await OnLoadStart();

				var result = await GameCenterWS.GetDashboard();
				if (result == null || result.Achievements == null || result.Achievements.Count == 0) throw new ServiceErrorException("Servidor Indisponível");

				TotalAchievements = TotalDoneAchievements = 0;

				Achievements = new ObservableCollection<AchievementGroup>();

				// Ongoing achievements
				var onGoingAchievements = new AchievementGroup(
					AppResources.AchievementsOngoingGroupHeader,
					ColorResources.ANFPaleGrey,
					ColorResources.TextColorDark);

				// Complete achievements
				var doneAchievements = new AchievementGroup(
					AppResources.AchievementsDoneGroupHeader,
					ColorResources.ANFGreen,
					ColorResources.TextColorLight);

				int onGoingPoints = 0;
				int donePoints = 0;

				// Initialize the lists
				foreach (var achievement in result.Achievements)
				{
					int type = 0;
					if (!_hkAchievementsEnabled && int.TryParse(achievement.EventTypeId, out type)) {
						if (type > 11 && type <= 14 && achievement.Active) {
							_hkAchievementsEnabled = true;
							OnPropertyChanged("CanEnableFitnessData");
						}
					}


					// Ignore achievements not active
					if (!achievement.Active) continue;

					// On going achievements now go to the "completed" list.
					// See http://issue.innovagency.com/view.php?id=20598 for more context.
					if (achievement.Points > 0)
					{
						// Increment completed achievements
						doneAchievements.Add(achievement);
						donePoints += Math.Min(achievement.Points, achievement.MaxPoints);

						if (!achievement.Achieved) {
							onGoingPoints += achievement.Points;
						}
					}
					else
					{	
						// Increment ongoing achievements
						onGoingAchievements.Add(achievement);
						onGoingPoints += Math.Max(achievement.Points, achievement.MaxPoints);
					}

					// Increment totals
					TotalDoneAchievements += achievement.Points;
					TotalAchievements += Math.Max(achievement.Points, achievement.MaxPoints);
				}

				// Set the total of complete and ongoing achievement points
				onGoingAchievements.Points = onGoingPoints;
				doneAchievements.Points = donePoints;

				// Initialize the groups
				Achievements.Add(onGoingAchievements);
				Achievements.Add(doneAchievements);

				if (updateFitnessData)
				{
					// Update fitness data through HealthKit/Google Fit.
					UpdateFitnessData();
				}

				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e)
			{
				if (OnLoadError != null) OnLoadError(null, e.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateFitnessData()
		{
			// Get health data from HealthKit/Google Fit.
			var hk = DependencyService.Get<IHealthDataProvider>();
			if (hk.IsSupported())
			{
				// Request permissions & query for data
				hk.RequestPermissions();
			}
		}

		#endregion

		#region Fitness Events

		void OnPermissionsChanged(object sender, EventArgs e)
		{
			DateTime lastUpdateUtc = DateTime.UtcNow;
			if (SessionData.GamecenterLastUpdate > 0) lastUpdateUtc = new DateTime(SessionData.GamecenterLastUpdate).ToUniversalTime();

			// If the events with type 12, 13 or 14 are not daily we need to adjust the query accordingly.
			var periodicity = AchievementPeriodicity.Daily;

			if (Achievements != null)
			{
				foreach (AchievementGroup grp in Achievements)
				{
					foreach (AchievementCenterOut.Achievement item in grp)
					{
						int eid;
						if (int.TryParse(item.EventTypeId, out eid))
						{
							if (eid >= 12 && eid <= 14)
							{
								if (item.Periodicity > periodicity) periodicity = item.Periodicity;
							}
						}
					}
				}
			}

			if (periodicity <= AchievementPeriodicity.Daily)
			{
				DependencyService.Get<IHealthDataProvider>().QueryWalkRunDistance(SessionData.GamecenterLastUpdate > 0 ? lastUpdateUtc : (DateTime?)null);
			}
			else if (periodicity == AchievementPeriodicity.Weekly)
			{
				// The week starts on Monday.
				var today = DateTime.UtcNow;
				// DayOfWeek.Sunday = 0.
				var weekStart = today.AddDays(-((Convert.ToInt32(today.DayOfWeek) - 1) % 7));
				//var weekEnd = today.AddDays((7 - Convert.ToInt32(today.DayOfWeek)) % 7);

				var startDate = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day).ToUniversalTime();
				//var endDate = new DateTime(weekEnd.Year, weekEnd.Month, weekEnd.Day, 23, 59, 59).ToUniversalTime();

				DependencyService.Get<IHealthDataProvider>().QueryWalkRunDistance((startDate < lastUpdateUtc ? lastUpdateUtc : startDate));
			}
			else if (periodicity == AchievementPeriodicity.Monthly)
			{
				var today = DateTime.UtcNow;

				var startDate = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
				var endDate = startDate.AddMonths(1).ToUniversalTime();

				DependencyService.Get<IHealthDataProvider>().QueryWalkRunDistance((startDate < lastUpdateUtc ? lastUpdateUtc : startDate));
			}
		}

		public async void OnHealthDataReceived(object sender, HealthDataEventArgs e)
		{
			// Received at least one fitness update!
			SessionData.HasFitnessServicesEnabled = true;
			OnPropertyChanged("CanEnableFitnessData");

			if (e.Type == HealthDataType.Distance)
			{
				try
				{
					foreach (HealthDataSample sample in e.Samples)
					{
						var distance = sample.Quantity;
						if (distance < 5000) continue;

						string eventType = null;
						if (distance >= 5000 && distance < 10000)
						{
							eventType = "12";
						}
						else if (distance >= 10000 && distance <= 15000)
						{
							eventType = "13";
						}
						else if (distance > 15000)
						{
							eventType = "14";
						}

						await GameCenterWS.PostEvent(eventType, sample.EndDate);
					}

					SessionData.GamecenterLastUpdate = DateTime.UtcNow.AddTicks(1).Ticks;

					// Reload the dashboard
					Device.BeginInvokeOnMainThread(() => {
						LoadData(false);
					});

				} catch (Exception ex) {
					if (OnLoadError != null) OnLoadError(null, ex.Message);
				}
			} 
		}

		#endregion

		#region Inner Classes

		public class AchievementGroup : ObservableCollection<AchievementCenterOut.Achievement>
		{
			public string Description { get; set; }
			public int Points { get; set; }
			public Color BackgroundColor { get; set; }
			public Color TextColor { get; set; }

			public AchievementGroup(string description, Color backgroundColor, Color textColor)
			{
				Description = description;
				BackgroundColor = backgroundColor;
				TextColor = textColor;
			}
		}

		#endregion
	}
}