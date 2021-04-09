using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs.DosageScheduler
{
	public class DosageDataDAO<T> : DAO<T> where T : DosageBase, new()
	{

		/// <summary>
		/// Mar the entry as updated and save the changes in the database.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override Task<int> Update(T obj)
		{
			return Update(obj, true);
		}

		/// <summary>
		/// Mar the entry as updated and save the changes in the database.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public Task<int> Update(T obj, bool markAsUpdated)
		{
			obj.Updated = markAsUpdated;
			if (markAsUpdated) { 
				obj.LastUpdated = DateTime.UtcNow;
			} 

			return base.Update(obj);
		}

		/// <summary>
		/// Mark all entries as updated and save the changes in the database.
		/// </summary>
		/// <param name="objList"></param>
		/// <returns></returns>
		public override Task<int> UpdateAll(List<T> objList)
		{
			foreach (var obj in objList)
			{
				obj.LastUpdated = DateTime.UtcNow;
				obj.Updated = true;
			}

			return base.UpdateAll(objList);
		}

		/// <summary>
		/// Returns all updated entries.
		/// </summary>
		/// <returns></returns>
		public Task<List<T>> GetAllUpdated() 
		{
			return Task.Run<List<T>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<T>().Where(o => o.Updated).ToList();
			});
		}

	}
}
