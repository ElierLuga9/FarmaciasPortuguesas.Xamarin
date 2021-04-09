using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
    public class Services 
    { 
        [JsonProperty("ID")]  
        public string Id {get;set;}
        [JsonProperty("DESCRICAO")]  
        public string Description {get;set;}   
    }

	public class GeoLoc
	{
		[JsonProperty("ELEVATION")]
		public double Elevation { get; set; }
		[JsonProperty("LONGITUDE")]
		public double Longitude { get; set; }
		[JsonProperty("LATITUDE")]
		public double Latitude { get; set; }
	}

    public class FarmDetailImage
    {
        /// <summary>
        /// Image URL.
        /// </summary>
        /// <value>The url.</value>
        [JsonProperty("URL1")]
        public string Url1 { get; set; }
        [JsonProperty("URL2")]
        public string Url2 { get; set; }

        [JsonIgnore]
        public UriImageSource ImageSource
        {
            get
            {
				if (string.IsNullOrEmpty(Url2))
				{
					return (!string.IsNullOrEmpty(Url1)) ? new UriImageSource
					{
						Uri = new Uri(Url1),
						#if DEBUG
						CachingEnabled = false
						#else
						CachingEnabled = true
						#endif
					} : null;
				}
				else
				{
					return new UriImageSource
					{
						Uri = new Uri(Url2),
						#if DEBUG
						CachingEnabled = false
						#else
						CachingEnabled = true
						#endif
					};
				}
            }
        }
    }

    public class GetMyFarmDetailOut : MagentoOut
    {
        #region Properties
        [JsonProperty("NOME")]
        public string Name { get; set; }
        [JsonProperty("MORADA")]
        public string Address { get; set; }
        [JsonProperty("CODIGOPOSTAL")]
        public string PostalCode { get; set; }
        [JsonProperty("EMAIL")]
        public string Email { get; set; }
        [JsonProperty("TELEFONE")]
        public string Phone { get; set; }
		[JsonProperty("COMERCIOELETRONICO")]
        public bool IsOnlineStore { get; set; }
        [JsonProperty("TEXTOAPRESENTACAO")]
        public string Description { get; set; }
        [JsonProperty("TEXTOEQUIPA")]
        public string Team { get; set; }
        [JsonProperty("SERVICOSFORNECIDOS")]
        public List<Services> ServicesList { get; set; }
        [JsonProperty("IMAGESFARM")]
        public List<FarmDetailImage> Images { get; set; }
		[JsonProperty("HASDELIVERYSERVICE")]
		public bool HasDeliveryService { get; set; }
		[JsonProperty("HASMSRMDELIVERYSERVICE")]
		public bool HasMSRMDeliveryService { get; set; }
		[JsonProperty("CUSTOENTREGADOMICILIO")]
		public double DeliveryFee { get; set; }
		[JsonProperty("VALORMINIMOPORTESGRATIS")]
		public double MinValueForFreeDelivery { get; set; }
		[JsonProperty("TEMPORTESGRATIS")]
		public string HasFreeDeliveryDummy { get; set; }
		[JsonProperty("OBSERVACOES")]
		public string Observations { get; set; }
		[JsonProperty("GEOLOC")]
		public GeoLoc Location { get; set; }

		[JsonProperty("URL")]
		public string Url { get; set; }
		[JsonProperty("CONTACTO")]
		public string Contacts { get; set; }

        #endregion

        #region auxiliary
       [JsonIgnore]
		public string ViewDescription 
        {
            get 
            {
                return (string.IsNullOrEmpty(Description)) ? AlternativeDescription : Description;
            }
        }
        //waiting for the rest
        [JsonIgnore]
        private string AlternativeDescription
        {
            get
            {
				return AppResources.KnowMyPharmacyPlaceholderDescriptionText;
                //return "Nome: " + Name + Environment.NewLine + "Morada: " + Address + " " + PostalCode + Environment.NewLine + "Telefone: " + Phone + Environment.NewLine + Environment.NewLine;
            }
        }

		[JsonIgnore]
		public string DeliveryFeeDescription
		{
			get
			{
				return string.Format(AppResources.KnowThisPharmacyDeliveryFeeLabel, DeliveryFee);
			}
		}

		[JsonIgnore]
		public string MinValueForFreeDeliveryDescription
		{
			get
			{
				return MinValueForFreeDelivery > 0 ? 
					string.Format(AppResources.KnowThisPharmacyMinValueToFreeDeliveryLabel, MinValueForFreeDelivery) : 
					AppResources.KnowThisPharmacyFreeDeliveryLabel;
			}
		}

		[JsonIgnore]
		public bool HasFreeDelivery 
		{
			get
			{
				return !string.IsNullOrEmpty(HasFreeDeliveryDummy) && string.Equals(HasFreeDeliveryDummy, "s", StringComparison.CurrentCultureIgnoreCase);
			}
		}
        
		[JsonIgnore]
        public bool IsTeamAvailable 
        {
            get
            {
                return (!string.IsNullOrEmpty(Team));
            }
        }

		[JsonIgnore]
		public string DecodedTeam
		{
			get { 
				if (Team != null) {
					return System.Net.WebUtility.HtmlDecode (Team);
				}
				return null;
			}
		}

		[JsonIgnore]
        public bool IsServicesAvailable
        {
            get
            {
                return (!(ServicesList.Count==0));
            }
        }

		[JsonIgnore]
		public bool IsContactsAvailable
		{
			get
			{
				return (!string.IsNullOrEmpty(Contacts));
			}
		}

		[JsonIgnore]
		public bool HasUrl
		{
			get
			{
				return !string.IsNullOrEmpty(Url);
			}
		}

        [JsonIgnore]
        public bool IsPhoneAvailable
        {
            get
            {
                return (!(string.IsNullOrEmpty(Phone)));
            }
        }

        [JsonIgnore]
        public bool IsEmailAvailable
        {
            get
            {
                return (!(string.IsNullOrEmpty(Email)));
            }
		}

        [JsonIgnore]
        public bool IsAddressAvailable
        {
            get
            {
                return (!(string.IsNullOrEmpty(Address)));
            }
        }
        #endregion
	}
}
