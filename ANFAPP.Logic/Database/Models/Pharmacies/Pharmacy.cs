using System;
using SQLite;
using ANFAPP.Logic.Models.Out;
using Xamarin.Forms;

namespace ANFAPP.Logic.Database.Models
{

	[Table("Pharmacy")]
	public class Pharmacy
	{
        [PrimaryKey]
        public long Id { get; set; }

		public int ServerID { get; set; }
        public string Identifier { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public int District { get; set; }
		public int County { get; set; }
		public string Parish { get; set; }
		public string PostalCode { get; set; }
		public string Locale { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public bool PFPSubscriber { get; set; }
		public DateTime UpdatedOn { get; set; }

		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int Elevation { get; set; }

        // XXX: is this used?
        public int ScheduleType { get; set; }
        public string ScheduleMorningStart { get; set; }
        public string ScheduleMorningEnd { get; set; }
        public string ScheduleAfthernoonStart { get; set; }
        public string ScheduleAfthernoonEnd { get; set; }

		public bool IsOpen24h { get; set; }

		public string ANFStatus { get; set; }
		public bool IsActive { get; set; }
		public int Type { get; set; }
		public int ShiftType { get; set; }

		public bool IsFavourite { get; set; }
		public DateTime FavouritedOn { get; set; } 
		public DateTime LastVisitedOn { get; set;} 

		public double Distance { get; set; }
		public string DistanceTxt { get; set; }

		public bool HasEC { get; set; }



		private bool _loadFlag = true;
		[Ignore]
		public bool LoadFlag 
		{
			get { return _loadFlag; }
			set
			{
				_loadFlag = value;
			}
		}

		private bool _checkEC = true;
		[Ignore]
		public bool CheckEC
		{
			get { return _checkEC; }
			set
			{
				_checkEC = value;
			}
		}

		[Ignore]
		public string PharmacyIcon
		{
			get { return (!string.IsNullOrEmpty(Code)) ? PharmacyUtils.IconForPharmacy(this) : null; }
		}

        [Ignore]
        public WorkSchedules[] WorkSchedules { get; set; }

        private string _workSchedulesJSON;
        public string WorkSchedulesJSON
        {
            get
            {
                this._workSchedulesJSON = Newtonsoft.Json.JsonConvert.SerializeObject(this.WorkSchedules);
                return this._workSchedulesJSON;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.WorkSchedules = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkSchedules[]>(value);
                }
                else
                {
                    this.WorkSchedules = (new System.Collections.Generic.List<Logic.Models.Out.WorkSchedules>()).ToArray();
                }

                this._workSchedulesJSON = value;
            }
        }

		[Ignore]
		public NonWorkingPeriods[] NonWorkingPeriod { get; set; }

		private string _nonWorkingPeriodJson;
		public string NonWorkingPeriodJson
		{
			get
			{
				this._nonWorkingPeriodJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.NonWorkingPeriod);
				return this._nonWorkingPeriodJson;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					this.NonWorkingPeriod = Newtonsoft.Json.JsonConvert.DeserializeObject<NonWorkingPeriods[]>(value);
				}
				else
				{
					this.NonWorkingPeriod = (new System.Collections.Generic.List<Logic.Models.Out.NonWorkingPeriods>()).ToArray();
				}

				this._nonWorkingPeriodJson = value;
			}
		}

        [Ignore]
        public string OpenStatusTxt { get; set; }

		[Ignore]
		public Color OpenStatusTextColor { get; set; }

        [Ignore]
        public bool IsOpen { get; set; }

        [Ignore]
        public bool LongSchedule { get; set; }

        [Ignore]
        public bool OpenFull { get; set; }
    }
}

