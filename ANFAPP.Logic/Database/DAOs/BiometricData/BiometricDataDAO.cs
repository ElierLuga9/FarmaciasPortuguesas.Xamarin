using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
    public abstract class BiometricDataDAO<T> : DAO<T> where T : BiometricBase, new()
    {

        /// <summary>
        /// Returns all the objects of type T, ordered by CreationDate, descending.
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAllOrderedByDateDesc(int userId)
        {
            return Task.Run<List<T>>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Table<T>()
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreationDate)
                    .ToList();
            });
        }

        /// <summary>
        /// Returns all the objects of type T, ordered by CreationDate, ascending.
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAllOrderedByDateAsc(int userId)
        {
            return Task.Run<List<T>>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Table<T>()
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.CreationDate)
                    .ToList();
            });
        }

        /// <summary>
        /// Insert or replace a new object into the table, if one already exists with the referenced date
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<int> InsertOrUpdateByDate(T obj)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();

				// EDIT: The biometric data will no longer be restricted to 1 per day
                // Try to find entry with the same date.
				//var entry = db.Table<T>().Where(o => o.CreationDate.Equals(obj.CreationDate)).FirstOrDefault();
				//if (entry != null)
				//{
				//	// Replace entry
				//	obj.Id = entry.Id;
				//	return db.Update(obj);
				//}

                // Insert entry
                return db.Insert(obj);
            });
        }

		/// <summary>
		/// Returns if an entry already exists with the same date
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public Task<bool> ExistsWithSameDate(T obj)
		{

			return Task.Run<bool>(() =>
			{
				var db = GetDatabaseInstance();

				// Try to find entry with the same date.
				var entry = db.Table<T>().Where(o => o.CreationDate.Equals(obj.CreationDate)).FirstOrDefault();
				return entry != null;
			});
		}

    }
}
