using System;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Models.Out;
using System.Collections.Generic;
using System.Linq;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic
{
	public static class PharmacyUtils
	{
		public static string FormatPhoneNumber(string phone)
		{
			if (null == phone)
				return "";

			phone = new System.Text.RegularExpressions.Regex(@"\D").Replace(phone, string.Empty);
			phone = phone.TrimStart('1');
			if (phone.Length == 9)
				return Convert.ToInt64(phone).ToString("## ### ## ##");
			if (phone.Length == 10)
				return Convert.ToInt64(phone).ToString("### ### ####");
			return phone;
		}

		// XXX: humm... move these methods to Pharmacy?
		public static string IconForPharmacy(Pharmacy pharmacy)
		{
			string pharmacyCode = pharmacy.Code;
			bool subscriber = pharmacy.PFPSubscriber;

			string image = null;

			var open = IsPharmOpen(pharmacy.WorkSchedules, pharmacy.NonWorkingPeriod).IsOpen;
			//var open = false;
			var inService = PharmacyUtils.GetIsInService(pharmacyCode);

			if ((open || inService) && subscriber)
			{
				image = "pharmacy_annotation_1.png";
			}
			else if ((open || inService) && !subscriber)
			{
				image = "pharmacy_annotation_2.png";
			}
			else if (!open && !inService && subscriber)
			{
				image = "pharmacy_annotation_5.png";
			}
			else if (!open && !inService && !subscriber)
			{
				image = "pharmacy_annotation_6.png";
			}
			//         if (subscriber && inService) {
			//	image = "pharmacy_annotation_1.png";
			//} else if (!subscriber && inService) {
			//	image = "pharmacy_annotation_2.png";
			//} else if (subscriber && !inService) {
			//	image = "pharmacy_annotation_3.png";
			//} else {
			//	image = "pharmacy_annotation_4.png";
			//}

			return image;
		}

		public static string IconForPharmacyDroid(Pharmacy pharmacy)
		{
			var image = IconForPharmacy(pharmacy);
			return image != null ? image.Replace(".png", string.Empty) : null;
		}

		public static bool GetIsInService(string pharmacyCode)
		{
			// 1 - monday...
			DateTime now = DateTime.Now;

			var currentDayOfWeek = now.DayOfWeek;
			var currentHour = now.Hour;

			bool IsInService = false;

			// As requested, no longer used.
			// pharmcy is in service from monday to friday from 10h-13h and 15h-19h
			//if ((currentDayOfWeek != DayOfWeek.Sunday) && (currentDayOfWeek != DayOfWeek.Saturday)) {
			//	if ((currentHour >= 10 && currentHour <= 12) || (currentHour >= 15 && currentHour <= 18)) {
			//		IsInService = true;
			//	}
			//}

			if (!IsInService)
			{
				//if todays hour is >= 9

				var currentDateTime = DateTime.Now;
				if (currentHour < 9)
				{
					currentDateTime = currentDateTime.AddDays(-1);
				}

				var strMonth = currentDateTime.Month.ToString();
				if (strMonth.Length == 1)
				{
					strMonth = "0" + strMonth;
				}

				var strDay = currentDateTime.Day.ToString();
				if (strDay.Length == 1)
				{
					strDay = "0" + strDay;
				}

				var strDate = currentDateTime.Year + "-" + strMonth + "-" + strDay;

				ShiftPharmacyDAO spDAO = new ShiftPharmacyDAO();
				var result = spDAO.SyncGetShiftForPharmacyWithDateString(pharmacyCode, strDate);

				if (result)
				{
					IsInService = true;
				}
			}

			return IsInService;
		}

		public static PharmOpenStatus IsPharmOpen(WorkSchedules[] workSchedules, NonWorkingPeriods[] nonWorkingPeriod)
		{
			
			if (workSchedules == null || workSchedules.Length == 0)
			{
				return new PharmOpenStatus() { IsOpen = false };
			}

			var status = new PharmOpenStatus();

			var orderWorkSchedules = workSchedules.OrderBy(w => w.OrderSchedule);

			var listSchedule = new List<Schedule>();

			var group = orderWorkSchedules.GroupBy(d => d.ScheduleType).ToList();

			for (int i = 0; i < group.Count; i++)
			{
				var elem = group[i];

				var newSchedule = new Schedule();

				newSchedule.Morning = elem.FirstOrDefault(e => e.PeriodType == "Morning");
				newSchedule.Afternoon = elem.FirstOrDefault(e => e.PeriodType == "Afternoon");

				newSchedule.ScheduleType = elem.Key;

				listSchedule.Add(newSchedule);
			}

			var listScheduleNoHoliday = listSchedule.Where(s => s.ScheduleType != DayOfWeekConstants.Holydays).ToList();

			status.Open24Hours = listScheduleNoHoliday.Count == 7;
			if (status.Open24Hours) // tem horario para os 7 dias
			{
				for (var i = 0; i < listScheduleNoHoliday.Count; i++)
				{
					var day = listScheduleNoHoliday[i];
					if (!(day.Morning != null && day.Afternoon != null // tem horario diurno e tarde
						&& day.Morning.StartTimeStamp.TotalMinutes == 0 // aberto desde as 0H
						&& day.Morning.EndTimeStamp == day.Afternoon.StartTimeStamp // fim da manha e inicio da tarde na mesma hora
						&& day.Afternoon.EndTimeStamp.Hours == 23 && day.Afternoon.EndTimeStamp.Minutes == 59)) // fecha a tarde as 23h59
					{
						status.Open24Hours = false;
						break;
					}
				}
			}

			var date = DateTime.Now;
			var dayofweek = (int)date.DayOfWeek;

			var schedule = listSchedule.FirstOrDefault(s => s.DayOfWeekNumber == dayofweek);

			for (var i = 0; nonWorkingPeriod != null && i < nonWorkingPeriod.Length; i++)
			{
				var nonWorking = nonWorkingPeriod[i];
				if (date.Date >= nonWorking.Start.GetValueOrDefault() && date.Date <= nonWorking.End.GetValueOrDefault())
				{
					return new PharmOpenStatus() { IsOpen = false };
				}
			}


			if (schedule != null)
			{
				if (!status.Open24Hours)
				{
					// se tiver algum dia aberto depois das 19h é horario alargado(alterado em issue das 19 para 21)
					status.LongSchedule = schedule.Afternoon != null &&
						(schedule.Afternoon.EndTimeStamp.Hours >= 21 ||
						(schedule.Afternoon.EndTimeStamp.Hours == 21 && schedule.Afternoon.EndTimeStamp.Minutes > 1));
				}

				if (schedule.Morning != null && date.TimeOfDay >= schedule.Morning.StartTimeStamp && date.TimeOfDay <= schedule.Morning.EndTimeStamp)
				{
					status.IsOpen = true;
				}
				else if (schedule.Afternoon != null && date.TimeOfDay >= schedule.Afternoon.StartTimeStamp && date.TimeOfDay <= schedule.Afternoon.EndTimeStamp)
				{
					status.IsOpen = true;
				}

				if (schedule.Afternoon != null)
				{
					status.CloseTime = schedule.ParseServiceTime(schedule.Afternoon.End, schedule.Afternoon.EndTimeStamp);
				}
				else if (schedule.Morning != null)
				{
					status.CloseTime = schedule.ParseServiceTime(schedule.Morning.End, schedule.Morning.EndTimeStamp);
				}

				status.Schedule = schedule;
			}

			return status;
		}

		private static void AddMessingDay(List<Schedule> listOfSchedules, string day, int position)
		{
			if (listOfSchedules.Count <= position || listOfSchedules[position].ScheduleType != day)
			{
				listOfSchedules.Insert(position, new Schedule()
				{
					ScheduleType = day
				});
			}
		}

		public static List<WorkSchedulesHourItem> GetPharmHourList(WorkSchedules[] workSchedules) 
		{
			return GetPharmHourList(workSchedules, false);	
		}

		public static List<WorkSchedulesHourItem> GetPharmHourList(WorkSchedules[] workSchedules, bool isOpen24H)
		{
			if (workSchedules == null || workSchedules.Length == 0)
			{
				return null;
			}

			var orderWorkSchedules = workSchedules.OrderBy(w => w.OrderSchedule);

			var listSchedule = new List<Schedule>();

			var group = orderWorkSchedules.GroupBy(d => d.ScheduleType).ToList();

			for (int i = 0; i < group.Count; i++)
			{
				var elem = group[i];

				var newSchedule = new Schedule();

				newSchedule.Morning = elem.FirstOrDefault(e => e.PeriodType == "Morning");
				newSchedule.Afternoon = elem.FirstOrDefault(e => e.PeriodType == "Afternoon");

				newSchedule.ScheduleType = elem.Key;

				listSchedule.Add(newSchedule);
			}

			AddMessingDay(listSchedule, DayOfWeekConstants.Monday, 0);
			AddMessingDay(listSchedule, DayOfWeekConstants.Tuesday, 1);
			AddMessingDay(listSchedule, DayOfWeekConstants.Wednesday, 2);
			AddMessingDay(listSchedule, DayOfWeekConstants.Thursday, 3);
			AddMessingDay(listSchedule, DayOfWeekConstants.Friday, 4);
			AddMessingDay(listSchedule, DayOfWeekConstants.Saturday, 5);
			AddMessingDay(listSchedule, DayOfWeekConstants.Sunday, 6);
			AddMessingDay(listSchedule, DayOfWeekConstants.Holydays, 7);

			Schedule countingSchedule = null;
			int sameHourString = 0;
			string hourString = string.Empty;
			List<WorkSchedulesHourItem> Result = new List<WorkSchedulesHourItem>();


			var listNoHolidays = listSchedule.Where(l => l.ScheduleType != DayOfWeekConstants.Holydays).ToList();

			for (var i = 0; i < listNoHolidays.Count; i++)
			{
				if (countingSchedule == null)
				{
					countingSchedule = listNoHolidays[i];
					hourString = countingSchedule.GetHourString(isOpen24H);
				}
				else
				{
					var currentRecord = listNoHolidays[i];

					var currentRecordHourString = currentRecord.GetHourString(isOpen24H);

					if (hourString != currentRecordHourString)
					{
						if (sameHourString == 0)
						{
							Result.Add(new WorkSchedulesHourItem() { DayOrDays = countingSchedule.ScheduleTypeFromResource, HourOrHours = hourString });
						}
						else
						{
							var lastRecord = listNoHolidays[i - 1];
							Result.Add(new WorkSchedulesHourItem() { DayOrDays = countingSchedule.ScheduleTypeFromResource + " " + AppResources.To + " " + lastRecord.ScheduleTypeFromResource, HourOrHours = hourString });
						}

						sameHourString = 0;
						countingSchedule = currentRecord;
						hourString = countingSchedule.GetHourString(isOpen24H);
					}
					else
					{
						sameHourString++;
					}
				}
			}

			if (countingSchedule != null)
			{
				if (sameHourString == 0)
				{
					Result.Add(new WorkSchedulesHourItem() { DayOrDays = countingSchedule.ScheduleTypeFromResource, HourOrHours = hourString });
				}
				else
				{
					Result.Add(new WorkSchedulesHourItem() { DayOrDays = countingSchedule.ScheduleTypeFromResource + " " + AppResources.To + " " + listNoHolidays.Last().ScheduleTypeFromResource, HourOrHours = hourString });
				}
			}

			var holiday = listSchedule.FirstOrDefault(f => f.ScheduleType == DayOfWeekConstants.Holydays);

			if (isOpen24H) {
				Result.Add(new WorkSchedulesHourItem() { DayOrDays = holiday.ScheduleTypeFromResource, HourOrHours = AppResources.Open24Hours });
			} 
			else if (holiday != null)
			{
				Result.Add(new WorkSchedulesHourItem() { DayOrDays = holiday.ScheduleTypeFromResource, HourOrHours = holiday.GetHourString(isOpen24H) });
			}

			return Result;
		}
	}
}