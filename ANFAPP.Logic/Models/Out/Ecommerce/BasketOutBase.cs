using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class BasketOutBase : MagentoOut
	{

		#region Properties

		[JsonProperty("VALTOTPRODS")]
		public decimal? TotalValue { get; set; }

		// XXX: move to CheckoutStartOut?
		[JsonProperty("VALORMINTOTAL")]
		public decimal? MinValue { get; set; }

		// XXX: move to CheckoutStartOut?
		[JsonProperty("VALORMAXTOTAL")]
		public decimal? MaxValue { get; set; }

		[JsonProperty("VALTAXARESERVA")]
		public decimal? ReservationTax { get; set; }

		[JsonProperty("VALORAPAGAR")]
		public decimal? ValueToPay { get; set; }

		[JsonProperty("TOTALPONTOSREBATIDOS")]
		public int? TotalUsedPoints { get; set; }

		[JsonProperty("RESUMOIVA")]
		public List<IVAOut> Taxes { get; set; }

		[JsonProperty("PRODUCTOSQUANTIDADETOTALNOCART")]
		public int TotalQuantityInCart { get; set; }

		[JsonProperty("PRODUTOSDISTINTOSQUANTIDADENOCART")]
		public int DistinctProductsInCart { get; set; }

		[JsonProperty("TOTALPSBE")]
		public decimal? TotalPSBE { get; set; }

		[JsonProperty("TOTALMNSRM")]
		public decimal? TotalMNSRM { get; set; }

		[JsonProperty("TOTALMSRM")]
		public decimal? TotalMSRM { get; set; }

		[JsonProperty("VALMINMSRM")]
		public decimal? MSRMMinValue { get; set; }

		[JsonProperty("VALMAXMSRM")]
		public decimal? MSRMMaxValue { get; set; }

		[JsonProperty("VALORTOTALVALES")]
		public decimal? TotalVouchers { get; set; }

		[JsonProperty("VALORTOTALPORTES")]
		public decimal? TotalPostage { get; set; }

		[JsonProperty("VALORTOTALFATURA")]
		public decimal? TotalInvoice { get; set; }

		#endregion

		#region Errors

		[JsonProperty("QUOTESTATUS")]
		public string RequestStatus { get; set; }

		[JsonProperty("QUOTESTATUSMESSAGE")]
		public string ErrorMessage { get; set; }

		[JsonIgnore]
		public bool HasError { get { return !string.IsNullOrEmpty(RequestStatus) && !RequestStatus.Equals("OK"); } }

		#endregion

		#region Auxiliary Properties

		[JsonIgnore]
		public string TotalValueDescription
		{
			get
			{
				// XXX - No more price ranges!
				//if (MSRMMinValue.HasValue && MSRMMaxValue.HasValue && MSRMMaxValue > 0)
				//{
				//	return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", (TotalInvoice.GetValueOrDefault() + MSRMMinValue.Value), (TotalInvoice + MSRMMaxValue.Value));
				//}
				//else
				//{
				//	return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalInvoice.GetValueOrDefault());
				//}

				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalInvoice.GetValueOrDefault());
			}
		}
			
		[JsonIgnore]
		// XXX: used to simplify XAML bindings...
		// See https://consulting.glintt.com/mantis/view.php?id=35511
		public bool TotalInvoiceHasValue
		{
			get { return TotalInvoice.GetValueOrDefault () > decimal.Zero; }
			
		}

		[JsonIgnore]
		// XXX: used to simplify XAML bindings...
		public bool HasAnyKindOfValue
		{
			get { return TotalInvoiceHasValue || HasSubtotals; }

		}

		[JsonIgnore]
		public string TotalPayableValueDescription
		{
			get
			{
				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", ValueToPay.GetValueOrDefault());
			}
		}


		[JsonIgnore]
		public string TotalProductValueDescription
		{
			get
			{
				// XXX - No more price ranges!
				//if (MSRMMinValue.HasValue && MSRMMaxValue.HasValue && MSRMMaxValue > 0)
				//{
				//	return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C} - {1:C}", (TotalValue.GetValueOrDefault() + MSRMMinValue.Value), (TotalValue + MSRMMaxValue.Value));
				//}
				//else
				//{
				//	return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalValue.GetValueOrDefault());
				//}

				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalValue.GetValueOrDefault());
			}
		}

		[JsonIgnore]
		public string ReservationTaxDescription
		{
			get
			{ 
				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", ReservationTax.GetValueOrDefault());
			}
		}

		[JsonIgnore]
		public bool HasReservationTaxValue
		{
			get
			{ 
				return ReservationTax.HasValue && ReservationTax.Value > decimal.Zero;
			}
		}

		[JsonIgnore]
		public string PostageValueDescription
		{
			get
			{ 
				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalPostage.GetValueOrDefault());
			}
		}

		[JsonIgnore]
		public bool HasPostageValue
		{
			get
			{ 
				return TotalPostage.HasValue && TotalPostage.Value > decimal.Zero;
			}
		}

		[JsonIgnore]
		public string VouchersValueDescription
		{
			get
			{ 
				return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", TotalVouchers.GetValueOrDefault());
			}
		}

		[JsonIgnore]
		public bool HasVouchersValue
		{
			get
			{ 
				return TotalVouchers.HasValue && TotalVouchers.Value > decimal.Zero;
			}
		}

		[JsonIgnore]
		public bool HasUsedPoints
		{
			get
			{
				return TotalUsedPoints.HasValue && TotalUsedPoints.Value > 0;
			}
		}

		[JsonIgnore]
		public bool HasSurcharges
		{
			get
			{
				return (TotalValue.GetValueOrDefault() != TotalInvoice.GetValueOrDefault()) && TotalValue.GetValueOrDefault() > 0;
			}
		}
			
		[JsonIgnore]
		// XXX: used to simplify XAML bindings...
		public bool HasSubtotals
		{
			get  
			{
				return HasUsedPoints || HasSurcharges || HasPostageValue || HasReservationTaxValue || HasVouchersValue;
			}
		}

		#endregion

	}
}
