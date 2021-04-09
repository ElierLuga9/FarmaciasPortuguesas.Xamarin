using ANFAPP.Logic.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{

    public class DeliveryOut
    {

        #region Properties

        [JsonProperty("ANFENCID")]
        public string AnfId{get;set;}

        [JsonProperty("FARMID")]
        public string FarmId { get; set; }

        [JsonProperty("FARMNAME")]
        public string FarmName { get; set; }

        [JsonProperty("DATAENCOMENDA")]
        public DateTime DeliveryDate { get; set; }

		[JsonProperty("PRODUCTLIST")]
		public List<BasketProductOut> ProductList { get; set; }

        [JsonProperty("TIPLOCENTREGA")]
        public string DeliveryPlaceType { get; set; }

        [JsonProperty("MORADA")]
        public AddressOut BillingAddress { get; set; }

        [JsonProperty("MORADAENVIO")]
        public AddressOut DeliveryAddress { get; set; }

        [JsonProperty("FATURARNOME")]
        public string BillingName { get; set; }

        [JsonProperty("FATURARNIF")]
        public long BillingNIF { get; set; }

        [JsonProperty("VALTOTPRODS")]
        public decimal ProductsValue { get; set; }

        [JsonProperty("VALMINMSRM")]
        public decimal ProductsMinValue { get; set; }

        [JsonProperty("VALMAXMSRM")]
        public decimal ProductsMaxValue { get; set; }

        [JsonProperty("VALTAXARESERVA")]
        public decimal ReservationTax { get; set; }

        [JsonProperty("VALORAPAGAR")]
        public decimal TotalValue { get; set; }

        [JsonProperty("TOTALPONTOSREBATIDOS")]
        public int Points { get; set; }

        [JsonProperty("MEIOPAGAMENTO")]
        public string PayMethod { get; set; }

        [JsonProperty("PAYSTATUS")]
        public string PayStatus { get; set; }

		[JsonProperty("ENCSTATUSDESC")]
		public string StatusDescription { get; set; }

        [JsonProperty("PAYDATE")]
        [JsonConverter(typeof(NullToTypeConverter))]
        public DateTime PayDate { get; set; }

        [JsonProperty("PAYVALUE")]
        public decimal? PayValue { get; set; }

		[JsonProperty("ENCISOPEN")]
		public int IsDeliveredVal { get; set; }

		[JsonProperty("NUMLLINHAS")]
		public int ProductsQuantity { get; set; }

        #endregion

		[JsonIgnore]
		public string ProductsValueDescription
		{
			get 
			{
				if (ProductsValue == decimal.Zero) return "0 €";

				return ProductsValue.ToString ("C", new System.Globalization.CultureInfo ("pt-PT"));
			}

		}

		[JsonIgnore]
		public bool ShowProductValue
		{
			get
			{ 
				return ProductsValue != decimal.Zero;
			}
		}

		[JsonIgnore]
		public string PointsDescription
		{
			get
			{ 
				return string.Format ("{0} PTS", Points);
			}
		}

		[JsonIgnore]
		public bool ShowProductPoints
		{
			get 
			{
				return Points > 0;
			}
		}

		[JsonIgnore]
		public bool IsDelivered
		{
			get 
			{
				// Because magento..
				return IsDeliveredVal == 0;
			}
		}

		[JsonIgnore]
		public Color IsDeliveredColor
		{
			get
			{
				return IsDelivered ? ColorResources.ANFGreen : ColorResources.ANFDarkOrange;
			}
		}

    }
}
