using ANFAPP.Logic.Database.DAOs.DosageScheduler;
using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
	public class DosageDAO : DosageDataDAO<Dosage> 
	{
		/// <summary>
		/// Returns an object of type T with the referenced id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual Task<List<Dosage>> GetByDosageSchedulerId(object id)
		{
			return Task.Run<List<Dosage>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Dosage>()
					.Where(d => d.ScheduleId.Equals(id))
					.OrderBy(d => d.Date)
					.ToList();
			});
		}

        public virtual Task<int> DeleteByDosageSchedulerId(object id)
        {
            return Task.Run<int>(() =>
                {
                    var db = GetDatabaseInstance();
                    var map = db.GetMapping<Dosage>();

                    var query = string.Format("delete from {0} where {1} = {2}", map.TableName, "ScheduleId", id);

                    return db.Execute(query);
                });  
        }
	}
}
