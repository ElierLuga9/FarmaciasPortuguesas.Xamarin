using System;

namespace ANFAPP.Logic.Models.Out
{
	public class ClientOut
	{
		public string Number { get; set; }
		public string Name { get; set; }
		public string BirthDate { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }
		public string Locale { get; set; }
		public string PostalCode { get; set; }
		public string DocumentNumber { get; set; }
		public string ContactPhone { get; set; }
		public int HouseholdSize { get; set; }
		public string FiscalNumber { get; set; }
		public string Gender { get; set; }
		public string DocumentType { get; set; }
		public CardOut PharmacyCard { get; set; }
        public string PreferentialPharmacy { get; set; }
	}
}
