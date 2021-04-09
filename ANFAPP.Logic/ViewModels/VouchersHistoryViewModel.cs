using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class VouchersHistoryViewModel : IViewModel
    {

        #region Properties

        private List<Voucher> _vouchers;
        public List<Voucher> Vouchers
        {
            get { return _vouchers; }
            set
            {
                _vouchers = value;
                OnPropertyChanged("Vouchers");
            }
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Load the non-active voucher's list from the database.
        /// </summary>
        public async void LoadData()
        {
            var dao = new VouchersDAO();

			// Get the non-active vouchers from the database
			var vouchers = await dao.GetAllNotActive();
			if (vouchers == null || vouchers.Count == 0) return;

			var sorted = vouchers.OrderByDescending (o => o.EndDate);

			// Initialize order index
			int count = 0;
			foreach (var voucher in sorted)
			{
				voucher.Order = ++count;
			}

			Vouchers = sorted.ToList<Voucher>();//vouchers;
        }

        #endregion

    }
}
