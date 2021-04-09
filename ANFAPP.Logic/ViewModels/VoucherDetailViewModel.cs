using ANFAPP.Logic.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.In.Ecommerce;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic.ViewModels
{
    public class VoucherDetailViewModel : IViewModel
    {

		#region Properties
		private double _value;
		public double Value
		{
			get { return _value;}
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}
        private Voucher _voucher;
        public Voucher Voucher
        {
            get { return _voucher; }
            set
            {
                _voucher = value;
                OnPropertyChanged("Voucher");
            }
        }
	
		//ImageURL
		private string _imageUrl{ get; set; }
		public string imageUrl
		{
			get { return _imageUrl; }
			set
			{
				_imageUrl = value;
				OnPropertyChanged("ImageURL");

			}
		}

		//MSRM
		private bool _MSRM { get; set; }
		public bool MSRM
		{
			get { return _MSRM; }
			set
			{
				_MSRM = value;
				OnPropertyChanged("MSRM");

			}
		}

		//Generic
		private bool _generic { get; set; }
		public bool Generic
		{
			get { return _generic; }
			set
			{
				_generic = value;
				OnPropertyChanged("Generic");

			}
		}

		//Presentation
		private string _presentation { get; set; }
		public string Presentation
		{
			get { return _presentation; }
			set
			{
				_presentation = value;
				OnPropertyChanged("Presentation");

			}
		}

		//PriceDescription
		private string _priceDescription { get; set; }
		public string PriceDescription
		{
			get { return _priceDescription; }
			set
			{
				_priceDescription = value;
				OnPropertyChanged("PriceDescription");

			}
		}

		private string _name { get; set; }
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			
			}
		}
		private ObservableCollection<ProductOut> _productsVouchers { get; set; }
		public ObservableCollection<ProductOut> ProductsVouchers
		{
			get { return _productsVouchers; }
			set
			{
				_productsVouchers = value;
				OnPropertyChanged("ProductsVouchers");

			}
		}
        #endregion

        #region Event Handler
        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;
        public OnLoadStartEventHandler OnStart;
        #endregion

        #region Loaders

        /// <summary>
        /// Load the voucher for data binding.
        /// </summary>
        /// <param name="voucher"></param>
        public void LoadData(Voucher voucher)
        {
			Voucher = voucher;

	
			ProdVoucherRequest(voucher); //Envia o vale junto
            //testes
            if (OnSuccess != null) OnSuccess();


        }
		//Voucher neste caso é o vale
		/// <summary>
		/// Recebe os dados do Método ProdsVoucher da classe EcommerceWS, o que estabelece a ligação a um web service, enviando um JSON com o ID da farmacia e do vale e recebendo a lista de produtos associados a esse vale
		/// Esses produtos são colocados numa classe (ProductVoucherOut) que contém uma lista do tipo ProductOut onde consequentemente faz o Databiding para uma listview"> 
		/// </summary>
		/// <param name="vale">Vale.</param>
		public async void ProdVoucherRequest(Voucher vale)
		{



			//var a = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, vale.Code, 0, 0); //Recebe os produtos do WebService, enviando primeiro o TOKEN ID, O ID do vale e os 2 parametros para recepção dos dados

		//	ProductsVouchers.Add( a.Products[1]);       //Inicializa uma observebleCollection para o ItemSource da listView

		/*	foreach (var product in a.Products)
			{
				System.Diagnostics.Debug.WriteLine("CNP: " + product.CNP + "\n" +
												   "NAME: " + product.Name + "\n" +
												   "PRICE: " + product.PriceDescription + "\n" +
												   "INCAT: " + product.FromCatalog + "\n" +
												   "HASPOINTS: " + product.HasPoints + "\n" +
												   "POINTS: " + product.PointsDescription + "\n" +
												   "BRAND: " + product.Brand + "\n" +
												   "BRANDID: " + product.BrandId + "\n" +
												   "IMAGE: " + product.Image + "\n" +
												   "DOSE: " + product.Dosage + "\n" +
												   "PACK: " + product.PackDescription + "\n" +
												   "FF: " + product.FF + "\n" +
												   "PRESENTATION" + product.PresentationDescription + "\n" +
												   "MSRM: " + product.MSRM + "\n" +
												   "CNPEM: " + product.CNPEM + "\n" +
												   "GENERIC: " + product.Generic + "\n"

												  );
				Name = product.Name;
				imageUrl = product.ImageURL;
				MSRM = product.IsMSRM;
				Generic = product.HasGenerics;
				Presentation = product.Presentation;
				PriceDescription = product.PriceDescription;
				this.ProductsVouchers.Add(product);

			}*/


		}
	
		public void LoadImage(double value)
		{
			if (value == 2 || value == 5 || value == 10 || value == 20)
			{
				Value = value;
			}
			else {

				return;
			
			}

			
		}

		#endregion

	}
}
