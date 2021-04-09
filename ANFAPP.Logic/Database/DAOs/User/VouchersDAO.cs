using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.DAOs
{
    public class VouchersDAO : DAO<Voucher> 
    {

        /// <summary>
        /// Returns all the active vouchers.
        /// </summary>
        /// <returns></returns>
        public Task<List<Voucher>> GetAllActive()
        {
            return Task.Run<List<Voucher>>(() =>
            {
                var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.IsActive).OrderBy(v => v.EndDate).ToList();
            });
        }

		/// <summary>
		/// Gets all active vouchers by type (default is non-gifts).
		/// </summary>
		/// <param name="isGift"></param>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllActiveVouchers(bool isGift = false)
		{
			return Task.Run<List<Voucher>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.IsActive && ((Settings.VOUCHER_TYPE_VOUCHER == v.Type))).OrderBy(v => v.EndDate).ToList();
			});
		}

		/// <summary>
		/// Gets all active gifts
		/// </summary>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllActiveGifts()
		{
			//return GetAllActiveVouchers(true);
			return Task.Run<List<Voucher>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.IsActive && (Settings.VOUCHER_TYPE_SPONSORED == v.Type)).OrderBy(v => v.EndDate).ToList();
			});
		}/// <summary>
		 /// Gets all active gifts
		 /// </summary>
		 /// <returns></returns>
		public Task<List<Voucher>> GetAllActiveGiftsPharmacy()
		{
			//return GetAllActiveVouchers(true);
			return Task.Run<List<Voucher>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.IsActive && (Settings.VOUCHER_TYPE_PHARMACY == v.Type)).OrderBy(v => v.EndDate).ToList();
			});
		}


        /// <summary>
        /// Returns all the non-active vouchers.
        /// </summary>
        /// <returns></returns>
        public Task<List<Voucher>> GetAllNotActive()
        {
            return Task.Run<List<Voucher>>(() =>
            {
                var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.IsActive == false).OrderBy(v => v.EndDate).ToList();
            });
        }

		/// <summary>
		/// Gets all the non active vouchers by type (default is non-gifts).
		/// </summary>
		/// <param name="isGift"></param>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllNotActiveVouchers(bool isGift = false)
		{
			return Task.Run<List<Voucher>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => (v.IsActive == false) && ((Settings.VOUCHER_TYPE_SPONSORED == v.Type) == isGift)).OrderBy(v => v.EndDate).ToList();
			});
		}

		/// <summary>
		/// Gets all non active gifts
		/// </summary>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllNotActiveGifts()
		{
			return GetAllNotActiveVouchers(true);
		}

        /// <summary>
        /// Returns all the expiring vouchers.
        /// </summary>
        /// <returns></returns>
        public Task<List<Voucher>> GetAllExpiring()
        {
            return Task.Run<List<Voucher>>(() =>
            {
                var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.ExpiryAlert && v.Status != "Used").OrderBy(v => v.EndDate).ToList();
            });
        }

		/// <summary>
		/// Gets all expiring vouchers by type (default is non-gifts)
		/// </summary>
		/// <param name="isGift"></param>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllExpiringVouchers(bool isGift = false)
		{
			return Task.Run<List<Voucher>>(() =>
			{
				var db = GetDatabaseInstance();
				return db.Table<Voucher>().Where(v => v.ExpiryAlert && v.Status != "Used" && ((Settings.VOUCHER_TYPE_SPONSORED == v.Type) == isGift)).OrderBy(v => v.EndDate).ToList();
			});
		}

		/// <summary>
		/// Gets all expiring gifts
		/// </summary>
		/// <returns></returns>
		public Task<List<Voucher>> GetAllExpiringGifts()
		{
			return GetAllExpiringVouchers(true);
		}
    }
}
