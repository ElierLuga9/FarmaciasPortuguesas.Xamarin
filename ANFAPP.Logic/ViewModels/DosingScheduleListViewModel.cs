using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
    public class DosingScheduleListViewModel : IViewModel
    {

        #region Properties

		private DosageDAO _dosagesDAO = new DosageDAO();
		private MedicineDAO _medicineDAO = new MedicineDAO();
		private DosingScheduleDAO _dosingSchedulesDAO = new DosingScheduleDAO();

		private List<DosingSchedule> _dosingSchedules;
		public List<DosingSchedule> DosingSchedules 
		{
			get { return _dosingSchedules; }
			set
			{
				_dosingSchedules = value;
				OnPropertyChanged("DosingSchedules");
			}
		}
        
        #endregion

        #region Event Handlers

		public OnSuccessEventHandler OnLoadComplete;
		public OnLoadStartEventHandler OnLoadStart;
		public OnErrorEventHandler OnError;

        #endregion

        #region DB Loaders

        /// <summary>
        /// Load the list of entries.
        /// </summary>
        public async Task LoadData(bool synchronize = false)
        {
			// Load Schedules
			await LoadDataFromDB();

			if (synchronize) { 
				// Synchronization
				await SynchronizeSchedules();

				// Load from database
				await LoadDataFromDB();
			}

			if (OnLoadComplete != null) OnLoadComplete();
        }
		
		/// <summary>
		/// Load the list of entries from the database.
		/// </summary>
		/// <returns></returns>
		public async Task LoadDataFromDB()
		{
			var schedules = await _dosingSchedulesDAO.GetAll();
			if (schedules != null && schedules.Count == 0) schedules = null;

			// Initialize list
			DosingSchedules = schedules;
		}

		/// <summary>
		/// Delete the referenced dosing schedule.
		/// </summary>
		/// <param name="dosingSchedule"></param>
		public async void DeleteDosingSchedule(DosingSchedule dosingSchedule)
		{
			if (OnLoadStart != null) await OnLoadStart();
			
            try {
                var response = await SchedulerWS.DelPlanoTomas(SessionData.PharmacyUser.Username, dosingSchedule.Id);

                if (response.OK) {
                    await LocalDeleteDosingSchedule(dosingSchedule);
                    await LoadDataFromDB();
                }
            } catch (Exception ex) {
                if (OnError != null) OnError (null, ex.Message);
            } finally {
                if (OnLoadComplete != null) OnLoadComplete();
            }
		}

        protected async Task LocalDeleteDosingSchedule(DosingSchedule dosingSchedule)
        {
            // Delete the schedule from the database.
            var deleted = await _dosagesDAO.DeleteByDosageSchedulerId(dosingSchedule.Id);
            System.Diagnostics.Debug.WriteLine("Deleted {0} dosages from Schedule {1}", deleted, dosingSchedule.Id);
            await _dosingSchedulesDAO.Delete(dosingSchedule);   
        }

		public Task<Dosage> GetDosage(string id)
		{
			return _dosagesDAO.GetById(id);
		}

		public Task<DosingSchedule> GetDosingSchedule(Dosage dosage)
		{
			if (dosage == null) return null;

			return _dosingSchedulesDAO.GetById(dosage.ScheduleId);
		}

        #endregion

		#region Synchronization

		/// <summary>
		/// Synchronize the list of medicines, schedules and dosages.
		/// </summary>
		/// <returns></returns>
		public async Task SynchronizeSchedules()
		{
			// Synchronize Meds
			if (!await SynchronizeMeds()) return;

			// Synchronize Dosing Schedules
			if (!await SynchronizeDosingSchedules()) return;

			// Synchronize Dosages
			if (!await SynchronizeDosages()) return;
		}

		/// <summary>
		/// Synchronize the list of medicines.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> SynchronizeMeds()
		{
			// Get all the updated entries
			var meds = new List<Medicine>();
			var updatedMeds = await _medicineDAO.GetAllUpdated();

			try
			{
				// Update the meds list
				var medsResponse = await SchedulerWS.GetMedicines(
					SessionData.PharmacyUser.Username, 
					SessionData.PharmacyUser.CardNumber, 
					SessionData.ParseInstallationId);

				// Clear the meds table
				await _medicineDAO.DeleteAll();

                // If the response is OK we assume that the device token was added sucessfully.
				// Should we turn on the Scheduler PN setting if PN are enabled?
                // if (medsResponse.OK && !string.IsNullOrWhiteSpace(SessionData.ParseInstallationId) 
				//	&& Settings.AppSettings.GetValueOrDefault<bool>("Scheduler.PN")) {
				//	Settings.AppSettings.AddOrUpdateValue ("Scheduler.PN", true);
                //} 

				if (medsResponse == null || medsResponse.ListaMedicamentosApp == null || medsResponse.ListaMedicamentosApp.Count == 0)
				{
					// No meds for this user - clear db
					await _dosagesDAO.DeleteAll();
					await _dosingSchedulesDAO.DeleteAll();
					return false;
				}
				else
				{
					// Parse meds
					foreach (var med in medsResponse.ListaMedicamentosApp)
					{
						DateTime updated = DateTime.Parse(med.DataHoraUpdate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
						updated = updated.ToUniversalTime();
						System.Diagnostics.Debug.WriteLine("STR: {0} DATE {1} KIND {2}", med.DataHoraUpdate, updated.ToString("o"), updated.Kind);

						meds.Add(new Medicine()
						{
							Id = med.MedID,
							Name = med.NomeMed,
							CNP = med.CNP,
                            Dosage = med.Dosagem,
                            Qty = med.Embalagem.ToString(),
							Notes = med.Nota,
							PharmaceuticalUnit = med.FormaFarmaceutica,
							LastUpdated = updated,
							WarningFlag = med.AvisoMedicamento,
							AutoGeneratedSchedule = med.IsPilula
						});
					}

					// Add all meds to the database
					await _medicineDAO.InsertAll(meds);
				}
			}
			catch (Exception e)
			{
				// Error while getting the list of meds
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			// Validate if local data is more recent than the new meds	
			if (updatedMeds == null || updatedMeds.Count == 0) return true;

			var medsToUpdate = new List<Medicine>();
			foreach (var updatedMed in updatedMeds)
			{
				var dbMed = await _medicineDAO.GetById(updatedMed.Id);

				// Validate if the entry exists and the local version is more recent than the WS version
				if (dbMed == null || dbMed.LastUpdated.ToUniversalTime() > updatedMed.LastUpdated.ToUniversalTime()) continue;

				// Update the db with the local version and add it to the processing list
				await _medicineDAO.Update(updatedMed);
				medsToUpdate.Add(updatedMed);
			}

			// Validate if there are any medicine to update
			if (medsToUpdate == null || medsToUpdate.Count == 0) return true;

			try
			{
				// Update Medicines 
				var response = await SchedulerWS.UpdateMedicines(SessionData.PharmacyUser.Username, medsToUpdate);
				if (!response.OK) 
				{
					// Error while updating the list
					if (OnError != null) OnError(null, response.ErrorMessage);
					return false;
				}
			}
			catch (Exception e)
			{
				// Error while updating the list
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Synchronize the list of Dosing Schedules.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> SynchronizeDosingSchedules()
		{
			// Get all the updated entries
			var schedules = new List<DosingSchedule>();
			var updatedSchedules = await _dosingSchedulesDAO.GetAllUpdated();

			try
			{
				// Update the schedules list
				var schedulesResponse = await SchedulerWS.GetDosingSchedules(SessionData.PharmacyUser.Username);
				// Clear the table
				await _dosingSchedulesDAO.DeleteAll();

				// No schedules for this user - clear dosages
				if (schedulesResponse == null || schedulesResponse.ListaPlanoTomas == null || schedulesResponse.ListaPlanoTomas.Count == 0)
				{
					await _dosagesDAO.DeleteAll();
					return false;
				}
				else
				{
					// Parse schedules
					Medicine auxMed = null;
					foreach (var schedule in schedulesResponse.ListaPlanoTomas)
					{
						if (auxMed == null || auxMed.Id != schedule.MedID)
						{
							// Get the medicine for this schedule
							auxMed = await _medicineDAO.GetById(schedule.MedID);
						}

						// Don't save the dosings which have no medicine in the DB.
						if (auxMed == null) continue;
						schedules.Add(new DosingSchedule()
						{
							Id = schedule.PlanoTomasID,
							Notes = schedule.Nota,
							MedicineId = schedule.MedID,
							MedicineName = auxMed != null ? auxMed.FullDescription : null,
							Description = schedule.NomeUser,
							SentByPharmacy = schedule.EnviadoFarmacia,
							Quantity = schedule.Qtd,
							HourInterval = schedule.IntervaloEscala == 2,
							Interval = schedule.IntervaloQtd,
							Duration = schedule.DuracaoQtd,
							DurationTypeId = schedule.DuracaoEscala,
							LastUpdated = schedule.DataHoraUpdate.ToUniversalTime()
						});
					}

					// Add all schedules to the database
					await _dosingSchedulesDAO.InsertAll(schedules);
				}
			}
			catch (Exception e)
			{
				// Error while getting the list of schedules
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			// Validate if local data is more recent than the new schedules	
			if (updatedSchedules == null || updatedSchedules.Count == 0) return true;

			var schedulesToUpdate = new List<DosingSchedule>();
			foreach (var updatedSchedule in updatedSchedules)
			{
				var dbSchedule = await _dosingSchedulesDAO.GetById(updatedSchedule.Id);

				// Validate if the entry exists and the local version is more recent than the WS version
				if (dbSchedule == null || dbSchedule.LastUpdated.ToUniversalTime() > updatedSchedule.LastUpdated.ToUniversalTime()) continue;

				// Update the db with the local version and add it to the processing list
				await _dosingSchedulesDAO.Update(updatedSchedule);
				schedulesToUpdate.Add(updatedSchedule);
			}

			// Validate if there are any schedule to update
			if (schedulesToUpdate == null || schedulesToUpdate.Count == 0) return true;

			try
			{
				// Update schedules 
				var response = await SchedulerWS.UpdateDosingSchedules(SessionData.PharmacyUser.Username, schedulesToUpdate);
				if (!response.OK)
				{
					// Error while updating the list
					if (OnError != null) OnError(null, response.ErrorMessage);
					return false;
				}
			}
			catch (Exception e)
			{
				// Error while updating the list
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Synchronize the list of Dosages.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> SynchronizeDosages()
		{
			// Get all the updated entries
			var dosages = new List<Dosage>();
			var updatedDosages = await _dosagesDAO.GetAllUpdated();

			try
			{
				// Update the schedules list
				var dosagesResponse = await SchedulerWS.GetDosages(SessionData.PharmacyUser.Username);
				// Clear the table
				await _dosagesDAO.DeleteAll();

				// No schedules for this user - clear dosages
				if (dosagesResponse != null && dosagesResponse.ListaTomas != null && dosagesResponse.ListaTomas.Count > 0)
				{
					// Parse dosages
					foreach (var dosage in dosagesResponse.ListaTomas)
					{
						DateTime date = DateTime.Parse(dosage.DataHora, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
						date = date.ToUniversalTime();
						System.Diagnostics.Debug.WriteLine("STR: {0} DATE {1} KIND {2}", dosage.DataHora, date.ToString("o"), date.Kind);

						DateTime updated = DateTime.Parse(dosage.DataHoraUpdate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
						updated = updated.ToUniversalTime();

						dosages.Add(new Dosage()
						{
							Id = dosage.TomaID,
							ScheduleId = dosage.PlanoID,
							Date = date,
							Time = new TimeSpan(0),
							Done = dosage.FlagTomado,
							Quantity = dosage.Qtd,
							LastUpdated = updated,
						});
					}

					// Add all dosages to the database
					await _dosagesDAO.InsertAll(dosages);
				}
			}
			catch (Exception e)
			{
				// Error while getting the list of dosages
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			// Validate if local data is more recent than the new dosages	
			if (updatedDosages == null || updatedDosages.Count == 0) return true;

			var dosagesToUpdate = new List<Dosage>();
			foreach (var updatedDosage in updatedDosages)
			{
				var dbDosage = await _dosagesDAO.GetById(updatedDosage.Id);

				// Validate if the entry exists and the local version is more recent than the WS version
				if (dbDosage == null || dbDosage.LastUpdated.ToUniversalTime() > updatedDosage.LastUpdated.ToUniversalTime()) continue;

				// Update the db with the local version and add it to the processing list
				await _dosagesDAO.Update(updatedDosage);
				dosagesToUpdate.Add(updatedDosage);
			}

			// Validate if there are any dosage to update
			if (dosagesToUpdate == null || dosagesToUpdate.Count == 0) return true;

			try
			{
				// Update schedules 
				var response = await SchedulerWS.UpdateDosages(SessionData.PharmacyUser.Username, dosagesToUpdate);
				if (!response.OK)
				{
					// Error while updating the list
					if (OnError != null) OnError(null, response.ErrorMessage);
					return false;
				}
			}
			catch (Exception e)
			{
				// Error while updating the list
				if (OnError != null) OnError(null, e.Message);
				return false;
			}

			return true;
		}

		#endregion

    }
}