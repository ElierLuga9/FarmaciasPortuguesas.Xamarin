using System;
using System.Globalization;
using ANFAPP.Logic.Resources;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ProductBase
	{

		#region Properties

		[JsonProperty("CNP")]
		public int? CNP { get; set; }

		[JsonProperty("CNPEM")]
		public int? CNPEM { get; set; }

		[JsonProperty("NAME")]
		public string Name { get; set; }

		[JsonProperty("PRICE")]
		public decimal? Price { get; set; }

		[JsonProperty("PROMOPRICE")]
		public decimal? PromoPrice { get; set; }

		[JsonProperty("HASPOINTS")]
		public bool HasPoints { get; set; }

		[JsonProperty("POINTS")]
		public int? Points { get; set; }

		[JsonProperty("BRAND")]
		public string Brand { get; set; }

		[JsonProperty("BRANDID")]
		public int? BrandId { get; set; }

		[JsonProperty("DOSE")]
		public string Dosage { get; set; }

		[JsonProperty("PACK")]
		public int? Pack { get; set; }

		[JsonProperty("FF")]
		public string FF { get; set; }

		[JsonProperty("PRESENTATION")]
		public string Presentation { get; set; }

		[JsonProperty("MSRM")]
		public bool? MSRM { get; set; }

		[JsonProperty("GENERIC")]
		public bool? Generic { get; set; }

		[JsonProperty("CATGROUP")]
		public int? CatGroup { get; set; }

		[JsonProperty("VALORMIN")]
		public decimal? MinValue { get; set; }

		[JsonProperty("VALORMAX")]
		public decimal? MaxValue { get; set; }

		#endregion

		[JsonIgnore]
		public int ProductId
		{
			get
			{
				return ShowCNPEM ? CNPEM.Value : CNP.Value;
				//return HasCNPEM ? CNPEM.Value : CNP.Value;
			}
		}

		[JsonIgnore]
		public bool HasCNPEM
		{
			get
			{
				return CNPEM.HasValue && CNPEM.Value != 0;
			}
		}

		[JsonIgnore]
		public bool ShowCNPEM
		{
			get
			{ 
				return CNPEM.HasValue && (!CNP.HasValue || CNP.Value == 0);
			}
		}

		[JsonIgnore]
		public bool HasGenerics
		{
			get
			{
				return HasCNPEM && !(Generic.HasValue && Generic.Value);
			}
		}

		[JsonIgnore]
		public bool HasCNP
		{
			get
			{
				return CNP.HasValue && CNP.Value > 0;
				//return !HasCNPEM && (CNP.HasValue && CNP.Value > 0);
			}
		}

		[JsonIgnore]
		public bool IsMSRM
		{
			get
			{
				return MSRM.HasValue && MSRM.Value;
			}
		}

		[JsonIgnore]
		public string DosageDescription
		{
			get { return !string.IsNullOrEmpty(Dosage) ? Dosage : "NA"; }
		}

		[JsonIgnore]
		public string PresentationDescription
		{
			get { return !string.IsNullOrEmpty(Presentation) ? Presentation : "NA"; }
		}
			
		[JsonIgnore]
		public string FullPresentation
		{
			get { 
				if (string.IsNullOrWhiteSpace (Dosage))
					return Presentation;
				if (string.IsNullOrWhiteSpace (Presentation))
					return Dosage;

				return string.Format ("{0} | {1}", Dosage, Presentation); 
			}
		}

		[JsonIgnore]
		public string PackDescription
		{
			get { 
				string sep, ff;

				sep = string.IsNullOrWhiteSpace (Dosage) || Pack == null ? " " : " - ";
				if (string.IsNullOrWhiteSpace (FF)) {
					ff = Pack == null ? "" : "Unidades";
				} else{
					ff = FF;
				}

				var description = Dosage + (string.IsNullOrWhiteSpace (Dosage) ? "" :sep) + (Pack == null ? "" : Pack.GetValueOrDefault().ToString()) + " " + ff;
				return description.Trim ();
			}
		}

		[JsonIgnore]
		public string PriceDescription
		{
			get 
			{ 
				if (!SessionData.IsPharmacySelected || !Price.HasValue || Price.Value == decimal.Zero && HasPoints) {
					return HasPoints ? Points + " PTS" : string.Empty;
				}
				else if (MinValue.HasValue && MaxValue.HasValue && MaxValue.Value > 0)
				{
					return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", MinValue.Value, MaxValue.Value);
				}
				else if (Price.HasValue && Price.Value > 0)
				{
					return string.Format(new CultureInfo("pt-PT"), "{0:C}", Price.GetValueOrDefault()) +
						((HasPoints) ?	
							" ou " + Points + " PTS" : 
							string.Empty);
				}
				else
				{
					return null;
				}
			}
		}

		[JsonIgnore]
		public string PromoPriceDescription
		{
			get
			{ 
				if (PromoPrice.HasValue && PromoPrice.Value > (decimal)0.00001) {
					decimal p = Price.GetValueOrDefault ();
					decimal d = p * PromoPrice.Value;
					decimal r = p - d;

					return string.Format (new CultureInfo ("pt-PT"), "{0:C}", r);
				}

				return null;
			}
		}

		[JsonIgnore]
		public string PriceOrPointsDescription
		{
			get 
			{ 
				if ((string.Equals (SessionData.StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT)/*string.Equals (FarmId, Settings.ST_MG_PHARMACY_ID_DEFAULT)*/)
					|| (HasPoints && (!Price.HasValue || Price.Value == 0))) {
					return HasPoints ? Points + " PTS" : string.Empty;
				} else {
					return string.Format(new CultureInfo("pt-PT"), "{0:C}", Price.GetValueOrDefault()); 
				}
			}
		}

		[JsonIgnore]
		public string PointsDescription
		{
			get {
				// The alternative text, if the price is shown.
				if (string.Equals (SessionData.StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT) || (HasPoints && (!Price.HasValue || Price.Value == 0))) {
					return string.Empty;
				} else {
					return HasPoints ? string.Format("Ou {0} PTS", Points) : string.Empty;
				}
			}
		}

		[JsonIgnore]
		public Color CatColor
		{
			get
			{
				if (!CatGroup.HasValue) return ColorResources.ANFDarkOrange;

				switch (CatGroup.Value)
				{
					//case 1: return Color.FromRgb(55, 177, 202);
					//case 2: return Color.FromRgb(81, 12, 118);
					//case 3: return Color.FromRgb(249, 191, 0);
					//case 4: return Color.FromRgb(0, 58, 112);
					//case 5: return Color.FromRgb(220, 0, 50);
					//case 6: return Color.FromRgb(88, 183, 179);
					//case 7: return Color.FromRgb(169, 173, 0);
					//case 8: return Color.FromRgb(239, 118, 36);
					//case 9: return Color.FromRgb(44, 86, 151);

					case 1: return Color.FromRgb(5, 183, 216);
					case 2: return Color.FromRgb(90, 39, 142);
					case 3: return Color.FromRgb(40, 58, 109);
					case 4: return Color.FromRgb(235, 39, 68);
					case 5: return Color.FromRgb(155, 183, 44);
					case 6: return Color.FromRgb(241, 131, 39);
					case 7: return Color.FromRgb(249, 189, 26);
					case 8: return Color.FromRgb(88, 198, 164);
					case 9: return Color.FromRgb(204, 78, 99);
					case 10: return Color.FromRgb(67, 27, 109);
					case 11: return Color.FromRgb(0, 63, 13);
					case 12: return Color.FromRgb(3, 160, 227);
					case 13: return Color.FromRgb(22, 55, 158);
					case 14: return Color.FromRgb(67, 27, 109);
					case 15: return Color.FromRgb(118, 189, 34);
					case 16: return Color.FromRgb(133, 158, 18);
					case 17: return Color.FromRgb(234, 234, 6);
					case 18: return Color.FromRgb(67, 27, 109);
					case 19: return Color.FromRgb(3, 39, 61);
					case 20: return Color.FromRgb(145, 73, 19);
					case 21: return Color.FromRgb(186, 74, 136);
					case 22: return Color.FromRgb(224, 143, 108);
					case 23: return Color.FromRgb(221, 93, 5);
					case 24: return Color.FromRgb(237, 169, 0);
					case 25: return Color.FromRgb(112, 127, 121);
					case 26: return Color.FromRgb(61, 61, 61);

				default: return ColorResources.ANFDarkOrange;
				}
			}
		}

		[JsonIgnore]
		public string CatIcon
		{
			get
			{
				if (!CatGroup.HasValue) return null;

				if (CatGroup.Value > 0 && CatGroup.Value <= 26) {
					return string.Format ("ic_cat_{0}.png", CatGroup.Value);
				}

				return null;
			}
		}
	}


}

