using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
    public class ShiftPharmacyDAO : DAO<ShiftPharmacy>
    {
        public Task<List<ShiftPharmacy>> GetAllByPharmacyID (int IDPharmacy)
        {
            return Task.Run<List<ShiftPharmacy>> (() => {
                var db = GetDatabaseInstance ();
                return db.Table<ShiftPharmacy> ().Where (o => o.IDPharmacy.Equals (IDPharmacy))
						.ToList ();
            });
        }

        public Task<bool> DeleteAllByPharmacyID (string IDPharmacy)
        {
            return Task.Run<bool> (() => {
                var db = GetDatabaseInstance ();
                List<ShiftPharmacy> toDelete = db.Table<ShiftPharmacy> ().
													Where (o => o.IDPharmacy.Equals (IDPharmacy))
													.ToList ();



                foreach (ShiftPharmacy sp in toDelete) {
                    db.Delete (sp);

                }

                return true;
            });
        }

        public bool SyncGetShiftForPharmacyWithDateString (string code, string strDate)
        {			
            var db = GetDatabaseInstance ();

            var records = from s in db.Table<ShiftPharmacy> ()
                            where s.IDPharmacy.Equals (code)
                                && s.ShiftDate.Equals (strDate)
                            select s;

            if (records.Count<ShiftPharmacy> () > 0) {
                return true;
            } else {
                return false;
            }
        }
    }
}
