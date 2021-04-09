using System;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	public class PharmaciesOut {
		[JsonProperty("odata.metadata")]
		public string Metadata { get; set; }
		public List<PharmacyOut> Value { get; set; }
        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
	}
		
	public class PharmacyOut
	{
		[JsonProperty("odata.type")]
		public string TypeDummy { get; set;}
		public string Code { get; set; }
		public string Identifier { get; set; }
		public string Name { get; set; }
		public int? District { get; set; }
		public int? County { get; set; }
		public string Parish { get; set; }
		public string PostalCode { get; set; }
		public string Locale { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public bool? PFPSubscriber { get; set; }
		public bool? HasEcommerce { get; set; }
		public string UpdatedOn { get; set; }
		public string ANFStatus { get; set; }
		public bool? IsActive { get; set; }
		public int? Type { get; set; }
		public GeoCoordinates GeoCoordinates { get; set; }

		public Shift [] Shifts { get; set; }
		public WorkSchedules [] WorkSchedules { get; set; }
		public NonWorkingPeriods[] NonWorkingPeriods { get; set; }
    }
        
	public class Shift {
		public string ShiftType { get; set; }
		public string Date { get; set; }
        public long? Id { get; set; }
	}

    public class GeoCoordinates {
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double? Elevation { get; set; }
	}

    public class Schedule
    {
        public WorkSchedules Morning { get; set; }

        public WorkSchedules Afternoon { get; set; }

        public string ScheduleTypeFromResource {
            get
            {
                switch (this.ScheduleType)
                {
                    case DayOfWeekConstants.Monday:
                        return AppResources.MondayShortString;

                    case DayOfWeekConstants.Tuesday:
                        return AppResources.TuesdayShortString;

                    case DayOfWeekConstants.Wednesday:
                        return AppResources.WednesdayShortString;

                    case DayOfWeekConstants.Thursday:
                        return AppResources.ThursdayShortString;

                    case DayOfWeekConstants.Friday:
                        return AppResources.FridayShortString;

                    case DayOfWeekConstants.Saturday:
                        return AppResources.SaturdayShortString;

                    case DayOfWeekConstants.Sunday:
                        return AppResources.SundayShortString;

                    case DayOfWeekConstants.Holydays:
                        return AppResources.HolydaysShortString;
                };

                return string.Empty;
            }
        }

        public int DayOfWeekNumber
        {
            get
            {
                switch (this.ScheduleType)
                {
                    case DayOfWeekConstants.Monday:
                        return 1;
                    case DayOfWeekConstants.Tuesday:
                        return 2;
                    case DayOfWeekConstants.Wednesday:
                        return 3;
                    case DayOfWeekConstants.Thursday:
                        return 4;
                    case DayOfWeekConstants.Friday:
                        return 5;
                    case DayOfWeekConstants.Saturday:
                        return 6;
                    case DayOfWeekConstants.Sunday:
                        return 0;
                    case DayOfWeekConstants.Holydays:
                        return 7;
                };

                return 0;
            }
        }

        public string ScheduleType { get; set; }

		public string GetHourString() {
			return GetHourString(false);
		}

		public string GetHourString(bool isOpen24H)
        {
			// XXX: Mjornir strikes again!
			// If we recieve the 24h override, ignore the schedules!
			if (isOpen24H) return AppResources.Open24Hours;

            if (this.Morning != null && this.Afternoon != null)
            {
                if (this.Morning.End != this.Afternoon.Start)
                {
                    return
                          ParseServiceTime(this.Morning.Start, this.Morning.StartTimeStamp)
                        + " - "
                        + ParseServiceTime(this.Morning.End, this.Morning.EndTimeStamp)
                        + " / "
                        + ParseServiceTime(this.Afternoon.Start, this.Afternoon.StartTimeStamp)
                        + " - "
                        + ParseServiceTime(this.Afternoon.End, this.Afternoon.EndTimeStamp);
                }
                else
                {
                    if(this.Morning.StartTimeStamp.TotalMinutes == 0 // aberto desde as 0H
                        && this.Afternoon.EndTimeStamp.Hours == 23 && this.Afternoon.EndTimeStamp.Minutes == 59) // fecha a tarde as 23h59
                    {
                        return AppResources.Open24Hours;
                    }

                    return
                          ParseServiceTime(this.Morning.Start, this.Morning.StartTimeStamp)
                        + " - "
                        + ParseServiceTime(this.Afternoon.End, this.Afternoon.EndTimeStamp);
                }
            }
            else if (this.Morning != null)
            {
                return
                      ParseServiceTime(this.Morning.Start, this.Morning.StartTimeStamp)
                    + " - "
                    + ParseServiceTime(this.Morning.End, this.Morning.EndTimeStamp);
            }
            else if (this.Afternoon != null)
            {
                return
                      ParseServiceTime(this.Afternoon.Start, this.Afternoon.StartTimeStamp)
                    + " - "
                    + ParseServiceTime(this.Afternoon.End, this.Afternoon.EndTimeStamp);
            }

            return AppResources.ClosedFemale;
        }
        
        private bool IsOpenTillMidNight(TimeSpan timestamp)
        {
            return timestamp.Hours == 23 && timestamp.Minutes == 59;
        }

        public bool OpenTillMidNight()
        {
            return IsOpenTillMidNight(this.Afternoon.EndTimeStamp);
        }

        public static string ReformatServiceString(string timestring)
        {
            return timestring.Replace("PT", "").Replace("S", "h").Replace("H", "h").Replace("M", "m");
        }

        public string ParseServiceTime(string timestring, TimeSpan timestamp)
        {
            if (this.IsOpenTillMidNight(timestamp))
            {
                return AppResources.OpenTillMidNightString;
            }

            return ReformatServiceString(timestring);
        }
    }

    public class WorkSchedulesHourItem
    {
        public string DayOrDays { get; set; }
        public string HourOrHours { get; set; }
        public bool IsNonWorking { get; set; }
    }

    public class PharmOpenStatus
    {
        public bool IsOpen { get; set; }
        public bool Open24Hours { get; set; }
        public bool LongSchedule { get; set; }
        public Schedule Schedule { get; set; }
        public string CloseTime { get; set; }
    }

    public class WorkSchedules
    {
        public string PeriodType { get; set; }
        public string ScheduleType { get; set; }
        public string Start { get; set; }

		private string end;
		public string End
		{
			get
			{
				return this.end;
			}
			set
			{
				if (this.PeriodType == "Afternoon" && value.Contains("0S"))
				{
					this.end = "PT23H59M";
				}
				else {
					this.end = value;
				}
			}
		}
        public long Id { get; set; }

        public int OrderSchedule
        {
            get
            {
                int resultBase = 0;
                switch (this.ScheduleType)
                {
                    case DayOfWeekConstants.Monday:
                        resultBase = 10;
                        break;
                    case DayOfWeekConstants.Tuesday:
                        resultBase = 20;
                        break;
                    case DayOfWeekConstants.Wednesday:
                        resultBase = 30;
                        break;
                    case DayOfWeekConstants.Thursday:
                        resultBase = 40;
                        break;
                    case DayOfWeekConstants.Friday:
                        resultBase = 50;
                        break;
                    case DayOfWeekConstants.Saturday:
                        resultBase = 60;
                        break;
                    case DayOfWeekConstants.Sunday:
                        resultBase = 70;
                        break;
                    case DayOfWeekConstants.Holydays:
                        resultBase = 80;
                        break;
                    default:
                        return 99;
                };

                if (this.PeriodType == "Afternoon")
                {
                    resultBase++;
                }

                return resultBase;
            }
        }

        public TimeSpan StartTimeStamp
        {
            get
            {
                return ConvertStringToTimeStamp(this.Start);
            }
        }

        public TimeSpan EndTimeStamp
        {
            get
            {
                return ConvertStringToTimeStamp(this.End);
            }
        }

        private TimeSpan ConvertStringToTimeStamp(string str)
        {
			
            str = str.Substring(2);
			if (str.IndexOf("M") == 1 || str.IndexOf("M") == 2)
			{
				str = "00H" + str;
			}
		
            if (str.IndexOf("H") == 1 || str.IndexOf("S") == 1)
            {
                str = "0" + str;
            }

            if (str.Length <= 3)
            {
                str += "00M";
            }

            if (str.IndexOf("M") == 4)
            {
                str = str.Replace("H", "H0");
                str = str.Replace("S", "S0");
            }


            str = str.Replace("H", ":").Replace("S", ":").Replace("M", "");

            var time = TimeSpan.ParseExact(str, @"hh\:mm", System.Globalization.CultureInfo.InvariantCulture);

            return time;
        }
    }

    public class NonWorkingPeriods
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class DayOfWeekConstants
    {
        public const string Monday = "Monday";
        public const string Tuesday = "Tuesday";
        public const string Wednesday = "Wednesday";
        public const string Thursday = "Thursday";
        public const string Friday = "Friday";
        public const string Saturday = "Saturday";
        public const string Sunday = "Sunday";
        public const string Holydays = "Holydays";
    }
}
