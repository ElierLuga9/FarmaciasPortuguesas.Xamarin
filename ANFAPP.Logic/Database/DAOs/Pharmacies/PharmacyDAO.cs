using ANFAPP.Logic.Database.Models;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
	public class PharmacyDAO : DAO<Pharmacy>
	{
        public Pharmacy SyncGetPharmacyByID(string identifier)
        {
            var db = GetDatabaseInstance();

            // Try to find entry with the same date.
            return db.Table<Pharmacy>().Where(o => o.Identifier.Equals(identifier)).FirstOrDefault();
        }

        public Task<Pharmacy> GetPharmacyByID (string identifier)
        {
            return Task.Run<Pharmacy> (() => {
                return SyncGetPharmacyByID (identifier);
            });
        }

		/// <summary>
		/// Gets the pharmacy by the Code, which is the pharmacy Magento identifier.
		/// </summary>
		/// <returns>The pharmacy identified by the given code.</returns>
		/// <param name="code">A numeric string that identifies the pharmacy.</param>
		public Task<Pharmacy> GetPharmacyByCode (string code)
		{
			return Task.Run<Pharmacy> (() => {
				var db = GetDatabaseInstance();

				return db.Table<Pharmacy>().Where(o => o.Code.Equals(code)).FirstOrDefault();
			});
		}
	}
}
