using ANFAPP.Logic.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out
{
	public enum AchievementPeriodicity {
		Undefined = 0,
		Daily = 1,
		Weekly,
		Monthly,
		Once,
	}

	public class AchievementCenterOut
	{

		[JsonProperty("Gamification")]
		public List<Achievement> Achievements { get; set; }

		public class Achievement
		{
			public string EventTypeId { get; set; }
            //Points of achieved activity
			public int Points { get; set; }
            //Points of unachieved activity
			public int ValuePoints { get; set; }
			public int MaxPoints { get; set; }
			public bool Active { get; set; }
			public AchievementPeriodicity Periodicity { get; set; }

            [JsonIgnore]
            //For presentation purposes this should be the refered variable in the views
            public int RealPoints
            {
                get 
                {
                    return Achieved ? Points : ValuePoints; 
                }   
            }

			[JsonIgnore]
			public string PointsDescription
			{
				get
				{ 
					if (Achieved)
						return string.Format ("{0} Pts", RealPoints);

					return string.Format ("{0}/{1} Pts", Points, MaxPoints);
				}

			}

			[JsonIgnore]
			public bool Achieved
			{
				get { return Points >= MaxPoints; }
			}

			[JsonIgnore]
			public string Title
			{
				get
				{
					return AppResources.ResourceManager.GetString("AchievementsTitleEvent" + EventTypeId);
				}
			}

			[JsonIgnore]
			public string Description
			{
				get 
				{ 
					return Achieved ?
                        string.Format(AppResources.ResourceManager.GetString("AchievementsDoneDescriptionEvent" + EventTypeId), RealPoints) :
                        string.Format(AppResources.ResourceManager.GetString("AchievementsOngoingDescriptionEvent" + EventTypeId), RealPoints); 
				}
			}

			[JsonIgnore]
			public string Rules
			{
				get 
				{
					return (Periodicity != AchievementPeriodicity.Once)? string.Format(
						AppResources.AchievementRulesLabelSchema, 
						AppResources.ResourceManager.GetString("AchievementPeriodicityType" + (int)Periodicity)):string.Empty; 
				}
			}

			[JsonIgnore]
			public string AchievementBadge
			{
				get 
				{ 
					return Achieved ? "star_point_achieved.png" : "star_point_unachieved.png";
				}
			}

			[JsonIgnore]
			public Color PointsColor
			{
				get
				{
					return Achieved ? ColorResources.ANFGreen : ColorResources.ANFDarkOrange;
				}
			}

		}
	}
}
