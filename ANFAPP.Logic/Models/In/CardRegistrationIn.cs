using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
    public class CardRegistrationIn
    {

        public string Name { get; set; }
		public bool? AllowPromotions { get; set;}
        public ClientIn Client { get; set; }


        public class ClientIn
        {

            public string Name { get; set; }
            public string Gender { get; set; }
		    public string BirthDate { get; set; }
		    public string DocumentType { get; set; }
		    public string DocumentNumber { get; set; }
		    public string HouseholdSize { get; set; }
		    public string Address { get; set; }
		    public string Locale { get; set; }
		    public string PostalCode { get; set; }
		    public string ContactPhone { get; set; }
            public string Email { get; set; }

        }
    }
}
