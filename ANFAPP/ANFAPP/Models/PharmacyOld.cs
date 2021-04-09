using System;

namespace ANFAPP.Models
{
	public class PharmacyOld
	{
		public PharmacyOld(string name, double latitude, double longitude, bool isService, bool acceptsCard, string phoneNumber, string address,
			string postalCode, string email, string schedule)
		{
			this.Name = name;
			this.Latitude = latitude;
			this.Longitude = longitude;
			this.IsService = isService;
			this.AcceptsCard = acceptsCard;
			this.PhoneNumber = phoneNumber;
			this.Address = address;
			this.PostalCode = postalCode;
			this.EMail = email;
			this.Schedule = schedule;
			this.DummyStr = "A";

		}

		public string Name { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public bool IsService { get; set; }
		public bool AcceptsCard { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string EMail { get; set; }
		public string Schedule { get; set; }
		public string Image { get { return getImage(); } }
		public string DummyStr;


		public string getImage() {
			if (this.AcceptsCard) {
				return "annotation_card";
			} else {
				return "annotation_no_card";
			}
		}

		public string DummySort
		{
			get
			{
				if (string.IsNullOrWhiteSpace(DummyStr) || DummyStr.Length == 0)
					return "?";

				return DummyStr;
			}
		}

	}
}

