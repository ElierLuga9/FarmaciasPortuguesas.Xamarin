using System.Collections.Generic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using System.Threading.Tasks;
using ANFAPP.Logic.Network.Services;
using System;

namespace ANFAPP.Logic.ViewModels
{
    public class DrugDetailViewModel : IViewModel
    {
        #region Properties

        private Medicine _drug; 
        public Medicine Drug { 
            get { return _drug; }
            set {
                _drug = value;
                OnPropertyChanged ("Drug");
				OnPropertyChanged ("Notes");
				OnPropertyChanged ("WarningFlag");
            }
        }
            
        private List<DosingSchedule> _schedules;
        public List<DosingSchedule> Schedules
        {
            get { return _schedules; }
            set { 
                _schedules = value;
                OnPropertyChanged ("Schedules");
            }
        }

		private string _notes;
		public string Notes
		{
			get { return _notes ?? (Drug != null ? Drug.Notes : null); }
			set {
				_notes = value;
				OnPropertyChanged ("Notes");
			}
		}

		private bool? _warningFlag;
		public bool WarningFlag
		{
			get { return _warningFlag ?? (Drug != null ? Drug.WarningFlag : false); }
			set { 
				_warningFlag = value;
				OnPropertyChanged ("WarningFlag");
			}
		}

        #endregion

		#region Event Handlers

		public OnErrorEventHandler OnError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadComplete;
		public OnSuccessEventHandler OnUpdateComplete;

		#endregion

		#region

        private async Task GetAndUpdateSchedules()
        {
            if (_drug != null) {
                var dao = new DosingScheduleDAO ();

                var result = await dao.GetByMedicine(_drug.Id);
                Schedules = result;
            }
        }

        public async Task Reload()
        {
            if (_drug != null) {
                if (null != OnLoadStart) await OnLoadStart();

                var mDao = new MedicineDAO ();
                Drug = await mDao.GetById (_drug.Id);
                await GetAndUpdateSchedules ();

                if (null != OnLoadComplete) OnLoadComplete();
            }
        }

		public async Task UpdateDrug()
		{
			bool updated = Drug.WarningFlag != WarningFlag || !string.Equals(Drug.Notes, Notes);

			if (updated) 
			{
				Drug.WarningFlag = WarningFlag;
				Drug.Notes = Notes;

				try
				{
					// Update the drug on the DB.
					var dao = new MedicineDAO ();
					await dao.Update(Drug);

					// Call WebService to update
					var response = await SchedulerWS.UpdateMedicine(SessionData.PharmacyUser.Username, Drug);
					if (response.OK)
					{
						// Already updated - clear flag
						await dao.Update(Drug, false);
					}
				}
				catch (Exception ex)
				{
					if (OnError != null) OnError (null, ex.Message);
					return;
				} finally {
					OnPropertyChanged ("Drug");
				}
			}

			if (OnUpdateComplete != null) OnUpdateComplete();
		}

        /// <summary>
        /// Load the list of entries from the database.
        /// </summary>
        public async Task LoadData()
        {
            if (null != OnLoadStart) await OnLoadStart();

            await GetAndUpdateSchedules ();

            if (null != OnLoadComplete) OnLoadComplete();
        }

		#endregion
    }
}
