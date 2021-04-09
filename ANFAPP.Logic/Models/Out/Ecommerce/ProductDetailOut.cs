using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public enum ProductAvailability
	{
		Undefined = 0,
		Request = 1,
		Immediate = 2,
		Standard = 3,
	}

	public enum LocalProductAvailability
	{
		Undefined = 0,
		Today = 1,
		Tomorrow = 2,
		NextWeek = 3,
	}

	public class ProductDetailImage
	{
		/// <summary>
		/// The URL if a single URL is returned.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }

		/// <summary>
		/// The thumbnail URL.
		/// </summary>
		/// <value>The url.</value>
		public string Url1 { get; set; }

		/// <summary>
		/// The detail URL.
		/// </summary>
		/// <value>The url.</value>
		public string Url2 { get; set; }

		/// <summary>
		/// Is this the main image?
		/// </summary>
		/// <value><c>true</c> if princ; otherwise, <c>false</c>.</value>
		public bool Princ { get; set; }

		[JsonIgnore]
		public UriImageSource ImageSource1
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(Url1)) {
					return new UriImageSource {
						Uri = new Uri (Uri.UnescapeDataString (Url1)),
						#if DEBUG
						CachingEnabled = false
						#else
						CachingEnabled = true
						#endif
					};
				} else {
					return Url != null ? new UriImageSource
					{
						Uri = new Uri(Uri.UnescapeDataString(Url)),
						#if DEBUG
						CachingEnabled = false
						#else
						CachingEnabled = true
						#endif
					} : null;
				}
			}
		}

		[JsonIgnore]
		public UriImageSource ImageSource2
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(Url2)) {
					return new UriImageSource {
						Uri = new Uri (Uri.UnescapeDataString (Url2)),
						#if DEBUG
						CachingEnabled = false
						#else
						CachingEnabled = true
						#endif
					};
				}
				else {
					return null;
				}
			}
		}
	}

	public class ProductContent
	{
		public string Title { get; set; }
		public string Content { get; set; }
	}

	public class ProductDisclaimer 
	{
		public string title { get; set; }
		public string description { get; set; }
	}

	public class ProductDetailOut : ProductBase
	{
		#region MagentoOut

		// XXX: implement as interface?
		public string msg { get; set; }
		[JsonProperty("INFORESULT")]
		public int code { get; set; }
		[JsonProperty("FARMID")]
		public string FarmId { get; set; }

		#endregion

		#region Properties

		[JsonProperty("DESCR")]
		public string Descr { get; set; }
		[JsonProperty("DESCD")]
		public string Descd { get; set; }
		[JsonProperty("PRINC")]
		public string Princ { get; set; }
		[JsonProperty("COMPOSITION")]
		public string Composition { get; set; }
		[JsonProperty("EXCIP")]
		public string Excip { get; set; }
		[JsonProperty("MODEUSE")]
		public string ModeUse { get; set; }
		[JsonProperty("PREC")]
		public string Prec { get; set; }
		[JsonProperty("PRECPREG")]
		public string PrecPreg { get; set; }
		[JsonProperty("PRECBREASTF")]
		public string PrecBreastF { get; set; }
		[JsonProperty("PRECPED")]
		public string PrecPed { get; set; }
		[JsonProperty("NRP")]
		public bool? NRP { get; set; }
		[JsonProperty("NRB")]
		public bool? NRB { get; set; }
		[JsonProperty("NRC")]
		public bool? NRC { get; set; }
		[JsonProperty("CONSERV")]
		public string Conserv { get; set; }
		[JsonProperty("PSBE")]
		public bool? PSBE { get; set; }
		[JsonProperty("MNSRM")]
		public bool? MNSRM { get; set; }
		[JsonProperty("COMPART")]
		public bool? Compart { get; set; }
		[JsonProperty("IVA")]
		public string IVA { get; set; }
		[JsonProperty("AVAIL")]
		public ProductAvailability? Avail { get; set; }
		[JsonProperty("AVAILTEXTO")]
		public string AvailTexto { get; set; }
		[JsonProperty("CATID")]
		[JsonConverter(typeof(NullToTypeConverter))]
		public int CatId { get; set; }
		[JsonProperty("ACTIVEPRINID")]
		public int? ActivePrinid { get; set; }
		[JsonProperty("IMAGES")]
		public ProductDetailImage Images { get; set; }
		[JsonProperty("IMAGESARRAY")]
		public List<ProductDetailImage> ImageList { get; set; }

		[JsonProperty("DISCLAIMERS")]
		public List<ProductDisclaimer> Disclaimers { get; set; }
		[JsonProperty("EFEITOSSECUNDARIOS")]
		public string SecondaryEffects { get; set; }
	
		#endregion

		[JsonIgnore]
		public bool IsNotRecommended 
		{
			get { return NRP.GetValueOrDefault() || NRB.GetValueOrDefault() || NRC.GetValueOrDefault(); }
		}
	
		[JsonIgnore]
		public string NotRecommendedText
		{
			get { 
				var labels = new List<string> ();

				if (NRP.GetValueOrDefault())
					labels.Add("Grávidas");
				if (NRB.GetValueOrDefault())
					labels.Add("Durante Amamentação");
				if (NRC.GetValueOrDefault())
					labels.Add("Crianças");

				return string.Join (", ", labels.ToArray ());
			}
		}
			
		/// <summary>
		/// The product availability based on the local time. 
		/// 
		/// This is not related to the 'Avail' field.
		/// 
		/// </summary>
		/// <value>The availability text.</value>
		[JsonIgnore]
		public string AvailabilityText
		{
			get { 
				string text = "";

				LocalProductAvailability lavail;
				DateTime now = DateTime.UtcNow;
				if (now.TimeOfDay.Hours < 12 && now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday) {
					lavail = LocalProductAvailability.Today;
				} else if (now.TimeOfDay.Hours >= 12 && now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Thursday) {
					lavail = LocalProductAvailability.Tomorrow;
				} else {
					lavail = LocalProductAvailability.NextWeek;
				}

				switch (lavail) {
				case LocalProductAvailability.NextWeek:
					text = "A partir das 10h00 da próxima 2ªF, se adquirido agora";
					break;
				case LocalProductAvailability.Today:
					text = "A partir das 17h30, se adquirido agora";
					break;
				case LocalProductAvailability.Tomorrow:
					text = "A partir das 10h00 de amanhã, se adquirido agora";
					break;

				default:
					break;
				}

				return text;
			}

		}

		[JsonIgnore]
		public string DescriptionText
		{
			get
			{
				return (IsMSRM) ? AppResources.ProductDetailMSRMDescriptionText : Descr;
			}
		}

		[JsonIgnore]
		public string PrescriptionStatusText
		{
			get { 
				return MSRM.GetValueOrDefault() ? "Sujeito a receita médica" : "Não sujeito a receita médica";
			}

		}

		/// <summary>
		/// Returns the product details, if available, observing the following order:
		/// 
		///		DESCD
		///		CONSERV
		///		COMPOSITION
		///		EXCIP
		///		MODEUSE
		///		PREC
		///		PRECPREG
		///		PRECBREASTF
		///		PRECPED
		/// </summary>
		/// <value>The contents.</value>
		[JsonIgnore]
		public List<ProductContent> Contents 
		{
			get { 
				var list = new List<ProductContent>();

				if (!string.IsNullOrWhiteSpace (Descd)) {
					list.Add (new ProductContent {
						Title = "Descritivo",
						Content = Descd
					});
				}
				if (!string.IsNullOrWhiteSpace (Composition)) {
					list.Add (new ProductContent {
						Title = "Composição",
						Content = Composition
					});
				}
				if (!string.IsNullOrWhiteSpace (Princ)) {
					list.Add (new ProductContent {
						Title = "Substâncias Ativas",
						Content = Princ
					});
				}
				if (!string.IsNullOrWhiteSpace (Excip)) {
					list.Add (new ProductContent {
						Title = "Excipientes",
						Content = Excip
					});
				}
				if (!string.IsNullOrWhiteSpace (Conserv)) {
					list.Add (new ProductContent {
						Title = "Conservação",
						Content = Conserv
					});
				}
				if (!string.IsNullOrWhiteSpace (ModeUse)) {
					list.Add (new ProductContent {
						Title = "Modo de Utilização",
						Content = ModeUse
					});
				}
				if (!string.IsNullOrWhiteSpace (Prec)) {
					list.Add (new ProductContent {
						Title = "Precauções",
						Content = Prec
					});
				}
				if (!string.IsNullOrWhiteSpace (PrecPreg)) {
					list.Add (new ProductContent {
						Title = "Precauções na Gravidez",
						Content = PrecPreg
					});
				}
				if (!string.IsNullOrWhiteSpace (PrecBreastF)) {
					list.Add (new ProductContent {
						Title = "Precauções na Amamentação",
						Content = PrecBreastF
					});
				}
				if (!string.IsNullOrWhiteSpace (PrecPed)) {
					list.Add (new ProductContent {
						Title = "Precauções Pediátricas",
						Content = PrecPed
					});
				}

				if (!string.IsNullOrEmpty(SecondaryEffects))
				{
					
					list.Add(new ProductContent() 
					{
						Title = "Efeitos Secundários",
						Content = SecondaryEffects 
					});
				}

				return list;
			}
		}

		[JsonIgnore]
		public UriImageSource MainImageSource
		{
			get { 
				if (ImageList == null || ImageList.Count == 0) {
					if (Images == null)
						return null;

					return Images.ImageSource2;
				}
					
				var mainImage = ImageList [0];
				foreach (ProductDetailImage item in ImageList) {
					if (item.Princ) {
						mainImage = item;
						break;
					}
				}

				return mainImage.ImageSource2;
			}
		}

		[JsonIgnore]
		public string MainImageURL
		{
			get { 
				return MainImageSource !=  null ? MainImageSource.Uri.ToString () : null;
			}
		}

		[JsonIgnore]
		public bool CanZoomImage
		{
			get
			{
				return (!MSRM.HasValue || !MSRM.Value) && ImageList != null && ImageList.Count > 0;
				//return MainImageURL != null && (!MSRM.HasValue || !MSRM.Value);
			}
		}
	}
}
