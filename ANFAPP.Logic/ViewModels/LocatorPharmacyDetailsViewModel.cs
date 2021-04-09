using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System.Collections.Generic;

namespace ANFAPP.Logic.ViewModels
{
    public class LocatorPharmacyDetailsViewModel : IViewModel
    {
		private bool _updateDetails;

        #region

        private Pharmacy _pharmacy;
        public Pharmacy Pharmacy { get { return _pharmacy; } set { _pharmacy = value; OnPropertyChanged(); } }

        private string _farmId;
        public string FarmId { get { return _farmId; } set { _farmId = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _address;
        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(); } }

        private string _phone;
        public string Phone {  get { return _phone; } set { _phone = value; OnPropertyChanged(); } }

        private string _favouriteImg = "star_unfavourite";
        public string FavouriteImg { get { return _favouriteImg; } set { _favouriteImg = value; OnPropertyChanged(); } }

        private string _schedule;
        public string Schedule { get { return _schedule; } set { _schedule = value; OnPropertyChanged(); } }

        private bool _scheduleVisible;
        public bool ScheduleVisible { get { return _scheduleVisible; } set { _scheduleVisible = value; OnPropertyChanged(); } }

        private bool _farmbuttonVisible;
        public bool FarmButtonVisible { get { return _farmbuttonVisible; } set { _farmbuttonVisible = value; OnPropertyChanged(); } }

        private List<WorkSchedulesHourItem> _farmHourList;
        public List<WorkSchedulesHourItem> FarmHourList { get { return _farmHourList; } set { _farmHourList = value; OnPropertyChanged(); } }

        private List<WorkSchedulesHourItem> _farmNotAvailableList;
        public List<WorkSchedulesHourItem> FarmNotAvailableList { get { return _farmNotAvailableList; } set { _farmNotAvailableList = value; OnPropertyChanged(); } }
        
        private bool _IsPharmOpen;
        public bool IsPharmOpen { get { return _IsPharmOpen; } set { _IsPharmOpen = value; OnPropertyChanged(); } }

        public GetMyFarmDetailOut ECPharmacyDetail { get; set; }

        private PharmacyOut _pharmacyDetail;
        public PharmacyOut PharmacyDetail
        {
            get
            {
                return _pharmacyDetail;
            }
            set
            {
                _pharmacyDetail = value;
				OnPropertyChanged ();
            }
        }

        public OnErrorEventHandler OnLoadError;
        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnLoadSuccess;

        private LocatorPharmacyDetailsViewModel() : base() { }

        public LocatorPharmacyDetailsViewModel(Pharmacy p) : this()
        {
            FarmId = p.Code;
            Pharmacy = p;
        }

        #endregion

        public async Task LoadData()
        {
			if (Pharmacy.LoadFlag) {
				if (OnLoadStart != null) { await OnLoadStart(); }

				try
				{
					if (Pharmacy.CheckEC) {
						try {
							ECPharmacyDetail = await ECommerceWS.GetMyFarmDetail(SessionData.UserAuthentication, FarmId);
						} catch (Exception ex) { 
							System.Diagnostics.Debug.WriteLine (ex); 
						}
					}

					// XXX: I'm very sorry about this... I tried...
					// This WS one and only function is to provide the answer to the ultimate question of life, the universe and everything. 
					// Is this pharmacy open 24h per day!?
					var schedule24HourCheck = await PharmacyWS.GetPharmacyScheduleFor24h(FarmId);

					var result = await PharmacyWS.GetPharmacyDetail(FarmId);
					PharmacyDetail = result;
					_updateDetails = true;

					if (null != result) {
						Name = result.Name;
						Address = result.Address  + ", " + result.PostalCode + ", " + result.Locale;
						Phone = " " + PharmacyUtils.FormatPhoneNumber(result.Phone);
						FarmButtonVisible = ECPharmacyDetail != null ? ECPharmacyDetail.IsOnlineStore : Pharmacy.HasEC;
						FavouriteImg = (Pharmacy.IsFavourite) ? "star_favourite" : "star_unfavourite";
                        
                        Pharmacy.Longitude = result.GeoCoordinates.Longitude;
						Pharmacy.Latitude = result.GeoCoordinates.Latitude;
                        Pharmacy.WorkSchedules = result.WorkSchedules;
						Pharmacy.NonWorkingPeriod = result.NonWorkingPeriods;
						Pharmacy.IsOpen24h = schedule24HourCheck != null ? schedule24HourCheck.IsOpen24H : false;

						FarmHourList = PharmacyUtils.GetPharmHourList(result.WorkSchedules, Pharmacy.IsOpen24h);

                        FarmHourList = FarmHourList ?? new List<WorkSchedulesHourItem>();

                        var notAvaibleList = new List<WorkSchedulesHourItem>();

                        if (result.NonWorkingPeriods != null && result.NonWorkingPeriods.Length > 0)
                        {
                            for (var i = 0; i < result.NonWorkingPeriods.Length; i++)
                            {
                                var nonWorkingPeriod = result.NonWorkingPeriods[i];

								if (nonWorkingPeriod.Start.GetValueOrDefault() > DateTime.Now.Date ||
								    nonWorkingPeriod.End.GetValueOrDefault() >= DateTime.Now.Date)
                                {
                                    var start = nonWorkingPeriod.Start.GetValueOrDefault();
                                    var end = nonWorkingPeriod.End.GetValueOrDefault();
									if (end.Year == start.Year && end.Month == start.Month && end.Day == start.Day)
                                    {
										start = start.AddDays(1);
                                        notAvaibleList.Add(new WorkSchedulesHourItem()
                                        {
                                            DayOrDays = start.ToString("dd/MM/yyyy"),
                                            IsNonWorking = true
                                        });
                                    }
                                    else
                                    {
										start = start.AddDays(1);
										end = end.AddDays(1);
                                        notAvaibleList.Add(new WorkSchedulesHourItem()
                                        {
                                            DayOrDays = start.ToString("dd/MM/yyyy")
                                                            + " " + AppResources.Till + " "
                                                            + end.ToString("dd/MM/yyyy"),
                                            IsNonWorking = true
                                        });
                                    }
                                }
                            }
                        }

						IsPharmOpen = PharmacyUtils.IsPharmOpen(result.WorkSchedules, result.NonWorkingPeriods).IsOpen;

                        FarmNotAvailableList = notAvaibleList;

                        OnPropertyChanged("Pharmacy");
					}

					await AddToRecent();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
					if (OnLoadError != null)
						OnLoadError("", ex.Message);
				}
					
				if (OnLoadSuccess != null) { OnLoadSuccess(); };	
			} else {
				await LoadDBData();

				await AddToRecent();

				if (OnLoadSuccess != null) { 
					OnLoadSuccess(); 
				}	
			}
        }

        private async Task LoadDBData() 
        {
            Name = Pharmacy.Name;
            Address = Pharmacy.Address + ", " + Pharmacy.PostalCode + ", " + Pharmacy.Locale;
            Phone = " " + PharmacyUtils.FormatPhoneNumber(Pharmacy.Phone);
            FavouriteImg = (Pharmacy.IsFavourite) ? "star_favourite" : "star_unfavourite";
            Schedule = ScheduleConstructor();
			FarmButtonVisible = ECPharmacyDetail != null ? ECPharmacyDetail.IsOnlineStore : Pharmacy.HasEC;
        }

        private string ScheduleConstructor()
        {
            var strSchedule = "";
            //Morning Schedule
            if (Pharmacy.ScheduleMorningStart != null)
            {
                strSchedule += "Manhã: " + Pharmacy.ScheduleMorningStart + " - " + Pharmacy.ScheduleMorningEnd;
                //Morning and Afternoon Schedule
                if (Pharmacy.ScheduleAfthernoonStart != null)
                {
                    strSchedule += "/n" + "Tarde: " + Pharmacy.ScheduleAfthernoonStart + " - " + Pharmacy.ScheduleAfthernoonEnd;
                }
                strSchedule = strSchedule.Replace(":00:00", ":00");
                return strSchedule;
            }
            else
            {
                //Only afternoon
                if (Pharmacy.ScheduleAfthernoonStart != null)
                {
                    strSchedule += "Tarde: " + Pharmacy.ScheduleAfthernoonStart + " - " + Pharmacy.ScheduleAfthernoonEnd;
                    strSchedule = strSchedule.Replace(":00:00", ":00");
                    return strSchedule;
                }
                //Schedule not available
                else
                {
                    ScheduleVisible = false;
                    return "";
                }
            }
        }

        private async Task AddToRecent()
        {
			var lastVisited = DateTime.UtcNow;
			PharmacyDAO pDAO = new PharmacyDAO();

			// We must take care with what we write here, for instance, the pharmacy might be a favorite.
			Pharmacy p = null;
			try {
				p = await pDAO.GetPharmacyByCode (Pharmacy.Code);
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine (e.Message);
			}

			var toUpdate = p ?? Pharmacy;

			toUpdate.LastVisitedOn = lastVisited;
			if (_updateDetails) {
				toUpdate.HasEC =  ECPharmacyDetail != null ? ECPharmacyDetail.IsOnlineStore : Pharmacy.HasEC;
				toUpdate.Address = PharmacyDetail.Address;
				toUpdate.PostalCode = PharmacyDetail.PostalCode;
				toUpdate.Locale = PharmacyDetail.Locale;
				toUpdate.Phone = PharmacyDetail.Phone;
                toUpdate.WorkSchedules = PharmacyDetail.WorkSchedules;
			}

            var loc = SessionData.UserLocation;

            if (loc != null && !(loc.Latitude > -double.Epsilon && loc.Latitude < double.Epsilon)
                && !(loc.Longitude > -double.Epsilon && loc.Longitude < double.Epsilon))
            {
                toUpdate.Distance = Math.Round(ANFAPP.Logic.Helper.PharmacyHelper.DistanceTo(loc.Latitude, loc.Longitude, toUpdate.Latitude, toUpdate.Longitude), 2);
                toUpdate.DistanceTxt = "Distancia: " + toUpdate.Distance + " km";
            }

            pDAO.InsertOrUpdate(toUpdate);
        }
    }
}
