using ANFAPP.Logic.Models.Out;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    public class Voucher
    {

        #region Constructor

        public string Code { get; set; }
        public double Value { get; set; }
        
        public bool IsCash { get; set; }
        public bool IsActive { get; set; }
        public bool ExpiryAlert { get; set; }

		public string Type { get; set; }

        public string Status { get; set; }
        public DateTime EndDate { get; set; }

		public string BurnConditions { get; set; }

		public string PharmacyExclusiveId { get; set; }

        public string DescriptionTitle { get; set; }
        public string DescriptionDetail { get; set; }

		[Ignore]
		public string Source 
		{
			get 
			{
				if (Value == 2)
				{
					return "cartao_vale_sauda_2.png";
				}
				if (Value == 5)
				{
					return "cartao_vale_sauda_5.png";
				}
				if (Value == 10)
				{
					return "cartao_vale_sauda_10.png";
				}
				if (Value == 20)
				{
					return "cartao_vale_sauda_20.png";
				}
				return "vcardb.png";

			}
		}
		[Ignore]
		public string ImgSource
		{
			get
			{
				if (Type == Settings.VOUCHER_TYPE_PHARMACY)
				{
					return "vcardpharm.png";
				}
				else
				{
					return "vcardb.png";
				}

			}
		}


		[Ignore]
		public bool Updating { get; set; }

		[Ignore]
		public bool Selected { get; set; }

        [Ignore]
        public int Order { get; set; }

        [Ignore]
        public string FormatedValue
        {
            get
            {
				var val = (IsCash && ((Value % 1) != 0)) ? Value.ToString("#.#0") : Value + string.Empty;
				return val + (IsCash ? "€" : "%");
            }
        }

		[Ignore]
		public bool IsGift
		{
			get
			{
				return !string.IsNullOrEmpty(Type) && string.Equals(Type, Settings.VOUCHER_TYPE_GIFT, StringComparison.OrdinalIgnoreCase);
			}
		}

        [Ignore]
        public string FormatedStatus
        {
            get
            {
                if (string.IsNullOrEmpty(Status)) return null;

                // Available 
                if (Status.Equals(Settings.VOUCHER_STATE_AVAILABLE)) return AppResources.VouchersStateAvailable;

                // Used
                if (Status.Equals(Settings.VOUCHER_STATE_USED)) return AppResources.VouchersStateUsed;

                // Expired
                if (Status.Equals(Settings.VOUCHER_STATE_EXPIRED)) return AppResources.VouchersStateExpired;

                // Canceled
                if (Status.Equals(Settings.VOUCHER_STATE_CANCELED)) return AppResources.VouchersStateCanceled;

                return null;
            }
        }

		[Ignore]
		public string Summary
		{
			get
			{ 
				if (string.IsNullOrWhiteSpace (DescriptionTitle))
					return FormatedValue;

				return string.Format ("{0} {2} {1}", DescriptionTitle, FormatedValue, string.IsNullOrWhiteSpace(FormatedValue) ? "-" : "");
			}

		}

		[Ignore]
		public string[] CNPRestrictions
		{
			get
			{
				if (string.IsNullOrEmpty(BurnConditions)) return null;
				return BurnConditions.Split(';');
			}
		}

        #endregion

        #region Constructors

        public Voucher() { }

        public Voucher(VoucherOut voucher) {
            Code = voucher.Code;
            Value = voucher.Value;
            Status = voucher.Status;
            EndDate = voucher.EndDate;
            ExpiryAlert = voucher.ExpiryAlert;

			Type = voucher.Type;
                    
            IsCash = Settings.VOUCHER_TYPE_CASH.Equals(voucher.ValueType);
            IsActive = Settings.VOUCHER_STATE_AVAILABLE.Equals(voucher.Status);

            DescriptionTitle = voucher.DetailedDescription != null ? voucher.DetailedDescription.Title : null;
            DescriptionDetail = voucher.DetailedDescription != null ? voucher.DetailedDescription.Description : null;

			// Parse the CNP restrictions if any exist
			if (voucher.BurnCondition != null && voucher.BurnCondition.Products != null && voucher.BurnCondition.Products.Count > 0)
			{
				BurnConditions = string.Empty;
				foreach (var prod in voucher.BurnCondition.Products)
				{
					BurnConditions += prod.Code + ";";
				}
				// Remove the last ";"
				BurnConditions = BurnConditions.Substring(0, BurnConditions.Length - 1);
			}

			if (voucher.BurnCondition != null && voucher.BurnCondition.IsPharmacyExclusive)
			{
				PharmacyExclusiveId = voucher.BurnCondition.Id;
			}
        }
        #endregion

    }
}
