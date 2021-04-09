using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
    public class ShiftTypeDAO : DAO<ShiftType>
    {
        public Task<List<ShiftType>> GetAll ()
        {
            return Task.Run<List<ShiftType>> (() => {
                var db = GetDatabaseInstance ();
                return db.Table<ShiftType> ()
						.OrderByDescending (p => p.Code)
						.ToList ();
            });
        }

        public Task<int> InsertOrUpdateByDate (ShiftType obj)
        {
            return Task.Run<int> (() => {
                var db = GetDatabaseInstance ();

                // Try to find entry with the same date.
                var entry = db.Table<ShiftType> ().Where (o => o.Code.Equals (obj.Code)).FirstOrDefault ();
                if (entry != null) {
                    // Replace entry
                    obj.Id = entry.Id;
                    return db.Update (obj);
                }

                // Insert entry
                return db.Insert (obj);
            });
        }

    }
}
