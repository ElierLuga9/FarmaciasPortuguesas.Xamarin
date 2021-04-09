using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
	public class DosageViewModel : IViewModel
	{

		#region Editable Properties

		private DateTime? _date;
		public DateTime Date
		{
			get { return _date.HasValue ? _date.Value : (Dosage != null && Dosage.ReprDateTime > DateTime.MinValue ? Dosage.ReprDateTime.Date : DateTime.Now); }
			set
			{
				_date = value;
				OnPropertyChanged("Date");
			}
		}

		private TimeSpan? _time;
		public TimeSpan Time
		{
			get  {  return _time.HasValue ? _time.Value : (Dosage != null && Dosage.ReprDateTime > DateTime.MinValue ? Dosage.ReprDateTime.TimeOfDay : DateTime.Now.TimeOfDay); }
			set
			{
				_time = value;
				OnPropertyChanged("Time");
			}
		}

		private string _quantity;
		public string Quantity
		{
			get { return _quantity ?? (Dosage != null ? Dosage.Quantity + string.Empty : null); }
			set
			{
				_quantity = value;
				OnPropertyChanged("Quantity");
			}
		}

		#endregion

		#region Binding Properties

		private DosingSchedule _dosingSchedule;
		public DosingSchedule DosingSchedule
		{
			get { return _dosingSchedule; }
			set
			{
				_dosingSchedule = value;
				OnPropertyChanged("DosingSchedule");
			}
		}

		private Dosage _dosage;
		public Dosage Dosage
		{
			get { return _dosage; }
			set
			{
				_dosage = value;
				OnPropertyChanged("Dosage");
			}
		}

		private Medicine _medicine;
		public Medicine Medicine
		{
			get { return _medicine; }
			set
			{
				_medicine = value;
				OnPropertyChanged("Medicine");
			}
		}

		#endregion

		#region Database DAOs

		private DosageDAO _db = new DosageDAO();
		private MedicineDAO _medicineDAO = new MedicineDAO();

		#endregion

		#region Event Handlers

		public OnSuccessEventHandler OnSuccess;
		public OnErrorEventHandler OnError;

		#endregion

		#region Loaders

		/// <summary>
		/// Load the medicine info
		/// </summary>
		public async void LoadData()
		{
			if (DosingSchedule == null) return;
			
			// Load Medicine
			Medicine = await _medicineDAO.GetById(DosingSchedule.MedicineId);
		}

		/// <summary>
		/// Create or update the dosage.
		/// </summary>
		public async void SaveDosage()
		{
			if (Dosage == null)
			{
				// New Dosage  
				if (!await CreateNewDosage()) return;
			}
			else
			{
				// Update the dosage
				await UpdateDosage();
			}

			if (OnSuccess != null) OnSuccess();
		}

		private DateTime NormalizeInputDate()
		{
			return DosageBase.NormalizedDate (Date, Time);
		}

		/// <summary>
		/// Create a new dosage.
		/// </summary>
		private async Task<bool> CreateNewDosage()
		{
			// Build new dosage object
			double quantity = 0.0;
			double.TryParse(Quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out quantity);

			var dosage = new Dosage()
			{
				Date = NormalizeInputDate(),
				Quantity = quantity,
				ScheduleId = DosingSchedule.Id
			};

			try
			{
				var response = await SchedulerWS.NewTomaToPlan(SessionData.PharmacyUser.Username, dosage.ScheduleId, dosage.Date, dosage.Quantity);
				if (response != null && response.OK) 
				{
					dosage.Id = response.TomaID;

					// Save the dosage in the database
					await _db.Insert(dosage);	
					Dosage = dosage;
					return true;
				}
				else
				{
					if (OnError != null) OnError(null, response.ErrorMessage);
				}
			}
			catch (Exception e)
			{
				if (OnError != null) OnError(null, e.Message);
			}
			
			return false;
		}

		/// <summary>
		/// Update the dosage data.
		/// </summary>
		private async Task UpdateDosage()
		{
			var normalized = NormalizeInputDate ();
			bool updated = !DateTime.Equals(Dosage.Date, normalized) || !string.Equals(Dosage.Quantity, Quantity);
			if (!updated) return;

			double quantity = 0.0;
			double.TryParse(Quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out quantity);

			// Clone the object and update the data
			var updatedDosage = new Dosage(Dosage);
			updatedDosage.Quantity = quantity;
			updatedDosage.Date = normalized;
			updatedDosage.Time = new TimeSpan(0);

			// Update dosage
			await _db.Update(updatedDosage);

			try
			{
				// Call WebService to update
				var response = await SchedulerWS.UpdateDosage(SessionData.PharmacyUser.Username, updatedDosage);
				if (response.OK)
				{
					// Already updated - clear flag
					await _db.Update(updatedDosage, false);
				}
			}
			catch (Exception e)
			{
				// Silent fail - do noting if it cannot update server
			}
		}

		public async void MarkAsDone(string id)
		{
			if (string.IsNullOrEmpty(id)) return;

			try
			{
				// Call WebService to update
				SessionData.LoadUser();
				await SchedulerWS.MarkDosageAsDone(SessionData.PharmacyUser.Username, id);

				return;
			}
			catch (Exception e)
			{
				// Silent fail - do noting if it cannot update server
				return;
			}
		}

		#endregion

	}
}
