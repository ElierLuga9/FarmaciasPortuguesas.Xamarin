using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class VoucherOut
    {

        #region Properties

        public string Code { get; set; }
        public double Value { get; set; }
        public string ValueType { get; set; }

		public string Type { get; set; }

        public string Status { get; set; }
        public DateTime EndDate { get; set; }

		public Condition BurnCondition { get; set; }
        public VoucherDescription DetailedDescription { get; set; }

        public bool ExpiryAlert { get; set; }

		#endregion

		#region Inner Classes

		public class VoucherDescription
        {
            public string Title { get; set; }
            public string Description { get; set; }
			public string Sponsor { get; set; }
			public string UsageMessage { get; set; }


		}

		public class Condition
		{
			public string Id { get; set; }
			public bool IsPharmacyExclusive { get; set; }
			public List<Product> Products { get; set; }
			public List<ExclusivePharmacies> ExclusivePharmacies { get; set; }
		}
		public class ExclusivePharmacies
		{
			public GeoCoordinates GeoCoordinates { get; set; }
			public double? ANFSituation { get; set; }
			public double Code { get; set; }
			public string Identifier { get; set; }
			public string Name { get; set; }
			public string District { get; set; }
			public string County { get; set; }
			public string Parish { get; set; }
			public string PostalCode { get; set; }
			public string Locale { get; set; }
			public string Address { get; set; }
			public string Phone { get; set; }
			public string PFPSubscriber { get; set; }
			public DateTime UpdatedOn { get; set; }
			public bool IsActive { get; set; }
			public int Type { get; set; }
			public int? ImageId { get; set; }
			public bool HasEcommerce { get; set; }
			public double? VATNumber { get; set; }
			public string Email { get; set; }
			public string Owner { get; set; }
			public string Fax { get; set; }
			public string TechnicalDirector { get; set; }


		}
		public class GeoCoordinates 
		{
			public double? Elevation;
			public double? Longitude;
			public double? Latitude;

		}
		public class Product
		{
			public string Code { get; set; }
		}

        #endregion

    }
}
