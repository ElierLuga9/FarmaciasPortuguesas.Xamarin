
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic.Database.DAOs
{
	public class PromotionsDAO : DAO<Promotion>
	{
		#region Instance

		/*public static PromotionsDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MessageEventReadLogDAO();
				}
				return _instance as MessageEventReadLogDAO;
			}
		}*/

		#endregion

		#region Database Operations

		public Task<List<Promotion>> GetAllPromo()
		{
			return Task.Run<List<Promotion>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Promotion>().ToList();
			});
		}





		#endregion

	}
}


