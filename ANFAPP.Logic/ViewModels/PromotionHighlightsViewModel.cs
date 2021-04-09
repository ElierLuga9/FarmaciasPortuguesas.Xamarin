using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{
	public class PromotionHighlightsViewModel : StoreSearchableViewModel
	{
		public OnLoadStartEventHandler OnLoadStart;
		public OnErrorEventHandler OnError;
		//public OnErrorEventHandler
		public OnSuccessEventHandler OnLoadSuccess;
		public OnSuccessEventHandler OnLoadError;

		private ObservableCollection<PromoItem> _list;
		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged();

			}
		}
		private ObservableCollection<PromoItem> _promoList { get; set; }
		public ObservableCollection<PromoItem> PromoList
		{
			get { return _promoList; }
			set
			{
				_promoList = value;
				OnPropertyChanged("PromoList");
		
			}
		}

		public class PromoItem : ObservableCollection<ProductOut>
		{
			public bool? MSRM{ get; set; }
			public string MainImageURL{ get; set; }
			public string CatIcon{ get; set; }
			public bool HasGenerics{ get; set; }
			public string Presentation{ get; set; }
			public Color CatColor{ get; set; }
			public string PriceDescription{ get; set; }
			public bool? Generic{ get; set; }
			public string Name { get; set; }
			public int? Cnp { get; set; }

		

			public PromoItem(int? cnp,bool? mSRM,string name, string mainImageURL, string catIcon, bool hasGenerics, string presentation, Color catColor, string priceDescription, bool? generic)
			{
				Cnp = cnp;
				Name = name;
				MSRM = mSRM;
				MainImageURL = mainImageURL;
				CatIcon = catIcon;
				HasGenerics = hasGenerics;
				Presentation = presentation;
				CatColor = catColor;
				PriceDescription = priceDescription;
				Generic = generic;
			}
		}
		private List<Product> _cnpList;
		public PromotionHighlightsViewModel(List<Product> cnpList) : base()
		{
			_cnpList = cnpList;

		}

		public PromotionHighlightsViewModel()
		{
		}

		public async void LoadData()
		{
			int errorContador = 0;
			await OnLoadStart();
			_list = new ObservableCollection<PromoItem>();
			PromoList = new ObservableCollection<PromoItem>();
			try{
				foreach (var cnp in _cnpList)
				{
					int p = Int32.Parse(cnp.CNP);
				try
				{
					var result = await ECommerceWS.ProdDetail(SessionData.UserAuthentication, SessionData.StorePharmacyId, p);
					if (result != null)
					{

						var product = new PromoItem(result.CNP, result.MSRM, result.Name, result.MainImageURL, result.CatIcon, result.HasGenerics, result.Presentation,
													result.CatColor, result.PriceDescription, result.Generic);

						_list.Add(product);
					}


				}
				catch
				{
						errorContador = errorContador + 1;
						if (errorContador == _cnpList.Count)
						{

							OnLoadError();
							
						}
				}
					
				
					

				}
				PromoList = _list;
				if (OnLoadSuccess != null) OnLoadSuccess();

			}
			catch(Exception e)
			{
				OnLoadSuccess();

				if (OnError != null)
				{
					throw new ServiceErrorException("Servidor Indisponível.");

				}


			}


		}
	}
}


