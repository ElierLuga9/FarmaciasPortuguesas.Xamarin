using ANFAPP.Logic.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
	public class ScheduleTypeDAO : DAO<ScheduleType>
	{
		public Task<List<ScheduleType>> GetAllOrderedByDateDesc()
		{
			return Task.Run<List<ScheduleType>>(() =>
				{
					var db = GetDatabaseInstance();
					return db.Table<ScheduleType>()
						.OrderByDescending(p => p.Code)
						.ToList();
				});
		}


		public Task<int> InsertOrUpdateByDate(ScheduleType obj)
		{
			return Task.Run<int>(() =>
				{
					var db = GetDatabaseInstance();

					// Try to find entry with the same date.
					var entry = db.Table<ScheduleType>().Where(o => o.Code.Equals(obj.Code)).FirstOrDefault();
					if (entry != null)
					{
						// Replace entry
						obj.Id = entry.Id;
						return db.Update(obj);
					}

					// Insert entry
					return db.Insert(obj);
				});
		}
	}
}

