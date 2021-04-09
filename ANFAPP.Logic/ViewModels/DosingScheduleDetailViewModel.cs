using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class DosingScheduleDetailViewModel : IViewModel
	{

		#region Edition Properties

		private string _description;
		public string Description
		{
			get { return _description ?? DosingSchedule.Description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		private string _notes;
		public string Notes
		{
			get { return _notes ?? DosingSchedule.Notes; }
			set
			{
				_notes = value;
				OnPropertyChanged("Notes");
			}
		}

		#endregion

		#region Bindable Properties

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

		private IList<Dosage> _dosages;
		public IList<Dosage> Dosages
		{
			get { return _dosages; }
			set
			{
				_dosages = value;
				OnPropertyChanged("Dosages");
				OnPropertyChanged("DosagesTaken");
			}
		}

		public string DosagesTaken
		{
			get 
			{
				if (Dosages == null) return "0/0";				
				return Dosages.Where(d => d.Done).Count() + "/" + Dosages.Count;
			}
		}

		#endregion

		#region Event Delegates

		public OnErrorEventHandler OnError;
		public OnSuccessEventHandler OnLoadComplete;
		public OnSuccessEventHandler OnUpdateComplete;

		#endregion

		#region Database

		private DosageDAO _dosageDAO = new DosageDAO();
		private MedicineDAO _medicineDAO = new MedicineDAO();
		private DosingScheduleDAO _schedulesDAO = new DosingScheduleDAO();

		#endregion

		#region Loaders

		/// <summary>
		/// Loads the detail data.
		/// </summary>
		/// <param name="schedule"></param>
		public async void LoadData(DosingSchedule schedule)
		{
			DosingSchedule = schedule;

			// Load the list of dosages
			await LoadDosagesAsync();

			// Notify success
			if (OnLoadComplete != null) OnLoadComplete();
		}

		/// <summary>
		/// Reload the list of dosages.
		/// </summary>
		public async void LoadDosages()
		{
			await LoadDosagesAsync();

			// Notify success
			if (OnLoadComplete != null) OnLoadComplete();	
		}

		/// <summary>
		/// Reload the list of dosages.
		/// </summary>
		/// <returns></returns>
		public async Task LoadDosagesAsync()
		{
			if (DosingSchedule == null) return;

			// Load the list of dosages
			var dosages = await _dosageDAO.GetByDosageSchedulerId(DosingSchedule.Id);
			if (dosages != null)
			{
				// Get Medicine
				var med = await _medicineDAO.GetById(DosingSchedule.MedicineId);

				// Initialize orders
				int i = 1;
				foreach (var dosage in dosages)
				{
					if (med != null) dosage.QuantityWithUnit = dosage.Quantity + " (" + med.PharmaceuticalUnitDescription + ")";
					dosage.Order = i;
					i++;
				}
			}

			Dosages = dosages;
		}

		/// <summary>
		/// Update the dosing schedule.
		/// </summary>
		public async void UpdateSchedule()
		{
			bool updated = !string.Equals(DosingSchedule.Description, Description) || !string.Equals(DosingSchedule.Notes, Notes);
			
			if (updated) 
			{
				DosingSchedule.Description = Description;
				DosingSchedule.Notes = Notes;

				// Update the schedule entry on the DB
				await _schedulesDAO.Update(DosingSchedule);

				try 
				{
					// Call WebService to update
					var response = await SchedulerWS.UpdateDosingSchedule(SessionData.PharmacyUser.Username, DosingSchedule);
					if (response.OK) 
					{
						// Already updated - clear flag
						await _schedulesDAO.Update(DosingSchedule, false);
					}
				} 
				catch (Exception) 
				{ 
					// Silent fail - do noting if it cannot update server
				}

				// Notify for properties changed
				OnPropertyChanged("DosingSchedule");
			}

			if (OnUpdateComplete != null) OnUpdateComplete();
		}

		/// <summary>
		/// Delete a dosage.
		/// </summary>
		/// <param name="dosage"></param>
		public async void DeleteDosage(Dosage dosage)
		{
            try {
                var response = await SchedulerWS.DelToma(SessionData.PharmacyUser.Username, dosage.Id);

                if (response.OK) {
                    // Delete dosage from the database
                    await _dosageDAO.Delete(dosage);
                    // Update list of dosages
                    await LoadDosagesAsync();
                }
            } catch (Exception ex) {
                if (OnError != null) OnError (null, ex.Message);
            } finally {
                if (OnLoadComplete != null) OnLoadComplete();
            }
		}

		/// <summary>
		/// Update a dosage state
		/// </summary>
		/// <param name="dosage"></param>
		public async void UpdateDosage(Dosage dosage)
		{
			// Update the dosages taken
			OnPropertyChanged("Dosages");
			OnPropertyChanged("DosagesTaken");

			// Update the dosage in the database
			dosage.Date = DateTime.SpecifyKind(dosage.Date, DateTimeKind.Utc);
			await _dosageDAO.Update(dosage);

			// Reload the list of dosage
			await LoadDosagesAsync();

			// TODO: Try to call webservice to update the dosage

			if (OnLoadComplete != null) OnLoadComplete();
		}

		/// <summary>
		/// Clears the edition properties
		/// </summary>
		public void ClearEdit()
		{
			Description = Notes = null;
		}

		#endregion

	}
}
