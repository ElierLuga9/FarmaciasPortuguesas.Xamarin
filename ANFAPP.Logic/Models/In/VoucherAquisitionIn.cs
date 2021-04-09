using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
    public class VoucherAquisitionIn
    {

        public PharmacyIn Pharmacy { get; set; }
        public CardIn SourceCard { get; set; }
        public CardIn TargetCard { get; set; }
        public string VoucherReference { get; set; }
        public string PaymentType { get; set; }

        #region Inner Classes

        public class PharmacyIn
        {
            public string Code { get; set; }
        }

        public class CardIn
        {
            public string Number { get; set; }
        }

        #endregion

    }
}
