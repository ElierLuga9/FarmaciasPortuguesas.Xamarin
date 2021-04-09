using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class BasketProductOut : ProductOut
	{
		[JsonProperty("TIPOLINHA")]
		public int LineType { get; set; }

		[JsonProperty("TIPOPRODUTO")]
		public int ProductType { get; set; }

		[JsonProperty("TIPOAQUISICAO")]
		public int AquisitionType { get; set; }

		[JsonProperty("QTY")]
		public int Quantity { get; set; }

		[JsonProperty("VALOR")]
		public decimal? Value { get; set; }

		[JsonProperty("VALORMINROW")]
		public decimal? RowMinValue { get; set; }

		[JsonProperty("VALORMAXROW")]
		public decimal? RowMaxValue { get; set; }

		[JsonProperty("IVA")]
		public decimal? IVA { get; set; }

		[JsonProperty("IMAGE")]
		public string Image { get; set; }

		[JsonProperty("AVAIL")]
		public int AvailabilityCode { get; set; }

		[JsonProperty("AVAILTEXTO")]
		public string AvailabilityText { get; set; }

		[JsonProperty("SUBSATIVAS")]
		public string Subsativas { get; set; }

		[JsonIgnore]
		public string ValueDescription
		{
			get
			{
				if (RowMinValue.HasValue && RowMaxValue.HasValue && MaxValue > 0)
				{
					return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", RowMinValue.Value, RowMaxValue.Value);
				}
				else
				{
					var pts = HasPoints ? (Points.GetValueOrDefault() * Quantity) + " PTS" : string.Empty;

					if (HasPoints && Value.GetValueOrDefault () == decimal.Zero) {
						return pts;
					} else {
						var suffix = HasPoints ? " ou " + pts : string.Empty;
						return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}{1}", Value.GetValueOrDefault(), suffix);
					}
				}
			}
		}

		[JsonIgnore]
		public string AquisitionValueDescription
		{
			get
			{
				if (AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_POINTS)
				{
					// Points
					return (Points.GetValueOrDefault() * Quantity) + " PTS";
				}
				else
				{
					// Value
					if (MinValue.HasValue && MaxValue.HasValue && MaxValue > 0)
					{
						return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", MinValue.Value, MaxValue.Value);
					}
					else
					{
						return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", Value.GetValueOrDefault());
					}
				}
			}
		}

		[JsonIgnore]
		public bool ShowTaxDescription { get { return AquisitionType != Settings.CHECKOUT_AQUISITIONTYPE_POINTS; } }

		[JsonIgnore]
		public string ValueNoPointsDescription
		{
			get
			{
				if (MinValue.HasValue && MaxValue.HasValue && MaxValue > 0)
				{
					return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", MinValue.Value, MaxValue.Value);
				}
				else
				{
					return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", Value.GetValueOrDefault());
				}
			}
		}

		[JsonIgnore]
		public bool MonetaryAquisition 
		{
			get { return AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY; }
			set { AquisitionType = value ? Settings.CHECKOUT_AQUISITIONTYPE_MONEY : Settings.CHECKOUT_AQUISITIONTYPE_POINTS; }
		}

		[JsonIgnore]
		public bool CanToggleAquisition
		{
			get { return HasPoints && HasMonetaryValue; }
		}

		[JsonIgnore]
		public bool HasMonetaryValue
		{
			get { return (Value.GetValueOrDefault() > 0 || MaxValue.GetValueOrDefault() > 0); }
		}

		[JsonIgnore]
		public bool ShowValues
		{
			// Prices for products with a range of values won't be shown..
			get { return !RowMaxValue.HasValue || RowMaxValue.Value == 0; }
		}

		[JsonIgnore]
		public string ImageURL
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Image) ? Uri.UnescapeDataString(Image) : null;
			}
		}
	}
}
