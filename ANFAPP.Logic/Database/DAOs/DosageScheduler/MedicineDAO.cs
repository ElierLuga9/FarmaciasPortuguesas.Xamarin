using ANFAPP.Logic.Database.DAOs.DosageScheduler;
using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
	public class MedicineDAO : DosageDataDAO<Medicine> { 
    
        public virtual Task<List<Medicine>> GetAllWithDosage()
        {
            return Task.Run<List<Medicine>>(() =>
                {
                    var db = GetDatabaseInstance();

                    var drugs = db.Table<Medicine>().ToList();

                    foreach (var aDrug in drugs) {

						//DateTime next = DateTime.MaxValue;

						//var schedules = db.Table<DosingSchedule>().Where(d => 
						//	d.MedicineId == aDrug.Id)
						//	.ToList();
                        
						//foreach (var aSchedule in schedules) {
						//	var earliest = db.Table<Dosage>().Where(d => 
						//		d.ScheduleId == aSchedule.Id && d.Done == false).OrderBy(d => d.Date).FirstOrDefault();

						//	if (earliest != null && earliest.Date < next) {
						//		next = earliest.Date;
						//	}
						//}

						var schedules = db.Table<DosingSchedule>()
							.Where(d => d.MedicineId == aDrug.Id)
							.ToList();

                        aDrug.ScheduledCount = schedules != null ? schedules.Count() : 0;

						// Check for schedules sent by the pharmacy
						int sentByPharmacy = schedules != null ? schedules.Where(s => s.SentByPharmacy == true).Count() : 0;
						aDrug.HasScheduleSentByPharmacy = sentByPharmacy > 0;

						//if (next < DateTime.MaxValue) {
						//	aDrug.NextDose = next;
						//}
                    }

					

                    return drugs;
                });
        }
    }
}
