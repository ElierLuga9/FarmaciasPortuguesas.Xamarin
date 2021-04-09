using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
    public class ListDrugsViewModel : IViewModel
    {
        #region Properties

        private MedicineDAO _dao = new MedicineDAO();
        private DosingScheduleDAO _dosingSchedulesDAO = new DosingScheduleDAO();
        private DosageDAO _dosagesDAO = new DosageDAO();

        private List<Medicine> _drugs;
        public List<Medicine> Drugs
        {
            get { return _drugs; }
            set
            {
                _drugs = value;
                OnPropertyChanged("Drugs");
            }
        }

        #endregion

        #region Event Handlers

        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnLoadComplete;
        public OnErrorEventHandler OnError;

        #endregion

        /// <summary>
        /// Load the list of entries from the database.
        /// </summary>
        public async Task LoadData()
        {
            if (null != OnLoadStart) await OnLoadStart();

            Drugs = await _dao.GetAllWithDosage ();

            if (null != OnLoadComplete) OnLoadComplete();
        }

        public async Task DeleteMedicine(Medicine medicine)
        {
            if (OnLoadStart != null) await OnLoadStart();

            try {
                var response = await SchedulerWS.DelMed(SessionData.PharmacyUser.Username, medicine.Id);

                if (response.OK) {

                    var schedules = await _dosingSchedulesDAO.GetByMedicine(medicine.Id);
                    foreach (DosingSchedule item in schedules) {
                        var count = await _dosagesDAO.DeleteByDosageSchedulerId(item.Id);
                        System.Diagnostics.Debug.WriteLine("Deleted {0} dosages from Schedule {1}", count, item.Id);
                        await _dosingSchedulesDAO.Delete(item);   
                    }

                    var deleted = await _dao.DeleteById(medicine.Id);
                    System.Diagnostics.Debug.WriteLine("Deleted medicine {0}: {1}", medicine.Id, deleted);

                    await LoadData();
				}
				else
				{
					throw new ServiceErrorException(AppResources.DrugListRemovalGenericError);
				}
            } catch (Exception ex) {
                if (OnError != null) OnError (null, ex.Message);
            } finally {
                if (OnLoadComplete != null) OnLoadComplete();
            }
        }

		public Task<Medicine> GetMedicine(string id)
		{
			return _dao.GetById(id);
		}
    }
}
