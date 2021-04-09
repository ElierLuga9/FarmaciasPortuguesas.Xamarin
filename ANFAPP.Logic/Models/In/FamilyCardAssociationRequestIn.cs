using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
    public class FamilyCardAssociationRequestIn
    {
        public PharmacyCardRequest Receiver { get; set; }
        public PharmacyCardRequest FamilyHead { get; set; }
        public PharmacyCardRequest Requester { get; set; }

        public class PharmacyCardRequest
        {
            public string Number { get; set; }
        }
    }
}
