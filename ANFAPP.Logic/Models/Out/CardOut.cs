using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class CardOut
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public bool AllowPromotions { get; set; }
        public bool IsFamilyCard { get; set; }
        public bool IsFamilyHead { get; set; }
		public List<VoucherOut> OfferedVouchers { get; set; }
        public WalletOut Wallet { get; set; }

		/// <summary>
		/// The pharmacy client.
		/// 
		/// On the API V3 entity the Client is the parent entity of the PharmacyCard, 
		/// but we need to handle the output of Pfp-PharmacyCard()?$expand=Client.
		/// </summary>
		/// <value>The Client entity.</value>
		public ClientOut Client { get; set; }
    }
}
