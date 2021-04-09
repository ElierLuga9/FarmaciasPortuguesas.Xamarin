using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace ANFAPP.Logic.Database
{
    public abstract class DAO<T> where T : class, new()
    {

        /// <summary>
        /// Cached SQLite Connection
        /// </summary>
        protected SQLiteConnection _db;

        /// <summary>
        /// Returns the cached SQLite Connection
        /// </summary>
        /// <returns></returns>
        protected SQLiteConnection GetDatabaseInstance()
        {
            if (_db == null) _db = DatabaseHelper.GetDatabaseInstance();

            return _db;
        }

        /// <summary>
        /// Returns an object of type T with the referenced id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<T> GetById(object id)
        {
            return Task.Run<T>(() => 
            {
                var db = GetDatabaseInstance();
                return db.Find<T>(id);
            });
        }

        /// <summary>
        /// Returns all the objects of type T.
        /// </summary>
        /// <returns></returns>
		public virtual Task<List<T>> GetAll()
        {
            return Task.Run<List<T>>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Table<T>().ToList();
            });
        }

        /// <summary>
        /// Insert a new object into the table.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public virtual Task<int> Insert(T obj)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Insert(obj);
            });
        }

        /// <summary>
        /// Insert or replace a new object into the table.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public virtual Task<int> InsertOrUpdate(T obj)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.InsertOrReplace(obj);
            });
        }

        /// <summary>
        /// Insert a list of objects into the table.
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
		public virtual Task<int> InsertAll(List<T> objList)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.InsertAll(objList);
            });
        }

        /// <summary>
        /// Insert or replace a list of objects into the table.
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
		public virtual Task<int> InsertOrUpdateAll(List<T> objList)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();

                int inserts = 0;
                foreach (T obj in objList) {
                    inserts += db.InsertOrReplace(obj);
                }

                return inserts;
            });
        }

        /// <summary>
        /// Update an object in the table.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public virtual Task<int> Update(T obj)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Update(obj);
            });
        }

        /// <summary>
        /// Update a list of objects in the table.
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
		public virtual Task<int> UpdateAll(List<T> objList)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.UpdateAll(objList);
            });
        }

        /// <summary>
        /// Delete an object from the table.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public virtual Task<int> Delete(T obj)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Delete(obj);
            });
        }

        /// <summary>
        /// Delete an object from the table by the referenced id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public virtual Task<int> DeleteById(object id)
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.Delete<T>(id);
            });
        }

        /// <summary>
        /// Delete all data from the table.
        /// </summary>
        /// <returns></returns>
		public virtual Task<int> DeleteAll()
        {
            return Task.Run<int>(() =>
            {
                var db = GetDatabaseInstance();
                return db.DeleteAll<T>();
            });
        }
    }
}
