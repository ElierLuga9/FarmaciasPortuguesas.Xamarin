using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class CheckoutStartOut : BasketOutBase
	{
		[JsonProperty("CARTLIST")]
		public List<BasketProductOut> Products { get; set; }

		[JsonProperty("ENTREGDOM")]
		public bool HomeDeliveryAvailable { get; set; }

		[JsonProperty("ONLINEPAY")]
		public bool OnlinePayAvailable { get; set; }

		[JsonProperty("TXRES")] // XXX - Redundant old data... deprecated??
		public bool OldHasReservationTax { get; set; }

		[JsonProperty("FARMACIA_TEM_ENTREGA")]
		public string HasDelivery { get; set; }

		[JsonProperty("FARMACIA_OBSERV")]
		public string BOInfo { get; set; }

		[JsonProperty("MORADA")]
		public AddressOut BillingAddress { get; set; }

		[JsonProperty("MORADAENVIO")]
		public AddressOut DeliveryAddress { get; set; }

		[JsonProperty("MODOSENTREGAPOSSIVEIS")]
		public List<EcommerceParam> DeliveryTypes { get; set; }

		[JsonProperty("METODOSPAGAMENTOPOSSIVEIS")]
		public List<EcommerceParam> PaymentTypes { get; set; }

		[JsonProperty("ENVIOSELECIONADO")]
		public string SelectedDeliveryType { get; set; }

		[JsonProperty("PAGAMENTOSELECIONADO")]
		public string SelectedPaymentType { get; set; }

		[JsonProperty("AVAILTEXTO")]
		public string AvaliableText { get; set; }

		[JsonProperty("LISTAVOUCHERS")]
		public List<ANFAPP.Logic.Models.Out.Ecommerce.VoucherOut> Vouchers { get; set; }

		[JsonProperty("BLOCO_TAXA_RESERVA")]
		public ReservationTaxParams ReservationParams { get; set; }

		[JsonProperty("CANDELIVERHOME")]
		public string HasHomeDeliveryToogasBoolean { get; set; }

		[JsonIgnore]
		public bool HasMSRM
		{
			get
			{
				if (Products == null || Products.Count == 0) return false;
				foreach (var prod in Products)
				{
					if (prod.MSRM.GetValueOrDefault() || (prod.HasCNPEM && !prod.HasCNP)) return true;
				}

				return false;
			}
		}

		[JsonIgnore]
		public bool HasMSRMorMNSRM
		{
			get
			{
				if (Products == null || Products.Count == 0) return false;

				return (TotalMSRM.GetValueOrDefault () + TotalMNSRM.GetValueOrDefault ()) > 0.01m;
			}
		}

		[JsonIgnore]
		public bool HasCNPEM
		{
			get
			{
				if (Products == null || Products.Count == 0) return false;
				foreach (var prod in Products)
				{
					if (prod.HasCNPEM) return true;
				}

				return false;
			}
		}

		[JsonIgnore]
		public bool HasMSRMHomeDelivery
		{
			get
			{
				// XXX: Not a boolean... because Toogas
				return (string.IsNullOrEmpty(HasDelivery) || !HasDelivery.Equals("0"));
			}
		}

		[JsonIgnore]
		public bool HasHomeDelivery
		{
			get
			{
				// XXX: Not a boolean... because Toogas
				return string.Equals(HasHomeDeliveryToogasBoolean, "S", System.StringComparison.CurrentCultureIgnoreCase);
			}
		}

		[JsonIgnore]
		public string BasketAvailability 
		{
			get 
			{ 
				//	Em termos práticos, relembro:
				//	» código 1: disponível em 48 horas (é o tal sob consulta)
				//	» código 2: disponibilidade imediata (não usada)
				// 	» código 3: com data/hora calculada (o caso normal)
				int lowerBound = 4;
				string availabilityText = "";

				foreach (var product in Products) {
					if (product.AvailabilityCode < lowerBound) {
						availabilityText = product.AvailabilityText;
						lowerBound = product.AvailabilityCode;
					}

					if (lowerBound <= 1)
						break;
				}

				return availabilityText;
			}
		}

		[JsonIgnore]
		public bool HasReservationTax 
		{ 
			get 
			{
				return ReservationParams != null && ReservationParams.HasTax;
			}
		}

		#region Payment Methods

		public class EcommerceParam
		{
			[JsonProperty("NOME")]
			public string Description { get; set; }

			[JsonProperty("NAME")] // Because the WS are bipolar.
			public string AlternativeDescription { get; set; }

			[JsonProperty("CODE")]
			public string Id { get; set; }

			[JsonProperty("ACTIVE")]
			public bool IsActive { get; set; }
		}

		public class ReservationTaxParams
		{
			[JsonProperty("TEM_TAXA")]
			public string HasTaxDummy { get; set; }

			[JsonProperty("VALOR")]
			public double Value { get; set; }

			[JsonProperty("MEIPAG")]
			public List<EcommerceParam> PaymentMethods { get; set; }

			[JsonIgnore] // WS returns "S" or "N", because booleans are too overated
			public bool HasTax 
			{ 
				get 
				{ 
					return !string.IsNullOrEmpty(HasTaxDummy) 
						&& string.Equals(HasTaxDummy, "S", System.StringComparison.CurrentCultureIgnoreCase); 
				} 
			}

			[JsonIgnore]
			public string ValueDescription
			{
				get
				{
					return string.Format(new System.Globalization.CultureInfo("pt-PT"), "{0:C}", Value);
				}
			}

			[JsonIgnore]
			public string ReservationTaxDisclaimer
			{
				get
				{
					return string.Format(AppResources.CheckoutReservationTaxDisclaimer, ValueDescription);
				}
			}

		}

		#endregion

	}
}
