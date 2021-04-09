using ANFAPP.Logic.Database.DAOs.DosageScheduler;
using ANFAPP.Logic.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
	public class DosingScheduleDAO : DosageDataDAO<DosingSchedule>
	{ 
    
        public virtual Task<List<DosingSchedule>> GetByMedicine(object id)
        {
            return Task.Run<List<DosingSchedule>>(() =>
            {
                var db = GetDatabaseInstance();
				return InstanciateNextDosage(db,
					db.Table<DosingSchedule>()
						.Where(d => d.MedicineId.Equals(id))
						.ToList());
            });
        }

		public override Task<List<DosingSchedule>> GetAll()
		{
			return Task.Run<List<DosingSchedule>>(() =>
			{
				var db = GetDatabaseInstance();
				return InstanciateNextDosage(db,
					db.Table<DosingSchedule>()
						.ToList());
			});
		}

        public virtual Task<int> DeleteByMedicineId(object id)
        {
            return Task.Run<int>(() =>
                {
                    var db = GetDatabaseInstance();
                    var map = db.GetMapping<DosingSchedule>();

                    var query = string.Format("delete from {0} where {1} = {2}", map.TableName, "MedicineId", id);

                    return db.Execute(query);
                });  
        }

		/// <summary>
		/// Instanciate the list with its corresponding Next Dosage, if one exists.
		/// </summary>
		/// <returns></returns>
		protected List<DosingSchedule> InstanciateNextDosage(SQLiteConnection db, List<DosingSchedule> schedules)
		{
			DateTime limitDate = DateTime.Now.AddHours(-1);

			foreach (var schedule in schedules)
			{
				var earliest = db.Table<Dosage>()
					.Where(d => d.ScheduleId == schedule.Id && d.Done == false && d.Date > limitDate)
					.OrderBy(d => d.Date)
					.FirstOrDefault();
				
				schedule.NextDose = earliest != null ? earliest.Date.ToLocalTime() : (DateTime?)null;
			}
			return schedules;
		}

    }
}
