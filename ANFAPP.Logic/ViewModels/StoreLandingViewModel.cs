using ANFAPP.Logic.Models.Out.Ecommerce;
using System;
using System.Collections.ObjectModel;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreLandingViewModel : IViewModel
	{

		#region Inner Classes

		public enum GroupType { Highlights, Campaigns };

		public class ProductGroup : ObservableCollection<ProductOut>
		{
			public string Name { get; set; }
			public GroupType Type { get; set; }

			public ProductGroup(string name, GroupType type)
			{
				Name = name;
				Type = type;
			}
		}

		#endregion

		#region Properties

		private ObservableCollection<ProductGroup> _products { get; set; }
		public ObservableCollection<ProductGroup> Products 
		{ 
			get { return _products; }
			set 
			{ 
				_products = value; 
				OnPropertyChanged("Products");
			}
		}

		public bool HasResults
		{
			get { return _products != null; }
		}

		#endregion

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Loaders

		public async void LoadData()
		{
			if (OnLoadStart != null) await OnLoadStart();

			Products = new ObservableCollection<ProductGroup>();
			OnPropertyChanged("HasResults");

			try
			{
				// Get highlighted products
				var hl = await ECommerceWS.FeaturedProd(SessionData.UserAuthentication, 4, false);
				if (hl != null && hl.Products != null && hl.Products.Count > 0) 
				{
					var group1 = new ProductGroup(AppResources.StoreHighlightsLabel, GroupType.Highlights);
					foreach (ProductOut p in hl.Products)
					{
						group1.Add(p);
					}
					Products.Add(group1);
				}
			} 
			catch (Exception e) 
			{ 
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}

			try 
			{
				// Get catalog products
				var ct = await ECommerceWS.FeaturedProd(SessionData.UserAuthentication, 4, true);
				if (ct != null && ct.Products != null && ct.Products.Count > 0) 
				{
					var group2 = new ProductGroup(AppResources.StoreCatalogLabel, GroupType.Campaigns);
					foreach (ProductOut p in ct.Products)
					{
						group2.Add(p);
					}
					Products.Add(group2);
				}
			}
			catch (Exception e) 
			{ 
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}

			if (Products != null && Products.Count == 0) Products = null;
			OnPropertyChanged("HasResults");

			if (OnLoadSuccess != null) OnLoadSuccess();


			// Build highlights dummy data
			/*
			var group = new ProductGroup("Destaques", GroupType.Highlights);
			group.Add(new ProductOut() 
			{
				Name = "Xamarin",
				Brand = "Forms",
				Image = "http://i0.statig.com.br/bancodeimagens/73/5h/2c/735h2cwpav27h7tbwnymrv2x2.jpg",
				Price = 2.15m,
				Generic = true,
				Pack = 15,
				FF = "Injections",
				Dosage = "200mg"
			});
			group.Add(new ProductOut()
			{
				Name = "Xamarin2",
				Brand = "Forms",
				Image = "http://i0.statig.com.br/bancodeimagens/ej/v2/26/ejv2268h0cqrasg2pskesyw6j.jpg",
				Price = 2.15m,
				MSRM = true,
				Pack = 15,
				FF = "Injections",
				Dosage = "200mg"
			});
			group.Add(new ProductOut()
			{
				Name = "Xamarin3",
				Brand = "Forms",
				Image = "http://1.bp.blogspot.com/-kLJRXfnQyTw/T_zI1dLE9kI/AAAAAAAABzA/Qi0UGz0oiC0/s1600/produtos+farm%C3%A1cia+turcifalense.jpg",
				Price = 2.15m,
				CNPEM = 111,
				Pack = 20,
				Points = 100,
				HasPoints = true,
				FF = "Injections",
				Dosage = "200mg"
			});

			// Add highlights
			Products.Add(group);

			group = new ProductGroup("Campanhas", GroupType.Campaigns);
			group.Add(new ProductOut()
			{
				Name = "Zoomarino",
				Brand = "Golf",
				Image = "http://gustavoferreirinha.com.br/saint-germain/produto/wp-content/uploads/2014/11/Farmacia-Saint-Germain-Produtos-Rescue-Sleep-Spray-20ml-caixa-e-frasco.png",
				Price = 3.41m,
				Pack = 15,
				CNPEM = 11,
				MSRM = true,
				FF = "Injections",
				Dosage = "200mg",
				CatGroup = 1
			});

			// Add campaigns
			Products.Add(group);
			*/
		}

		#endregion

	}
}
