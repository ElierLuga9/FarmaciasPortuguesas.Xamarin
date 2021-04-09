using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class WalletOut
    {
        public int PointsBalance { get; set; }
        public int PointsToPrescribe { get; set; }
        public int PointsToPrescribeNextMonth { get; set; }
		public decimal TotalValueFolded { get; set; }
		public DateTime ValueFoldedStartDate { get; set; }
        public List<VoucherOut> Vouchers { get; set; }
    }
}
