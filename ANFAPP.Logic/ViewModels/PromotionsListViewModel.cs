using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{
	public class PromotionsListViewModel : IViewModel
	{
		List<Product> _cnpList;

		private List<Product> _promoList { get; set; }
		public List<Product> PromoList
		{
			get { return _promoList; }
			set
			{
				_promoList = value;
				OnPropertyChanged("PromoList");

				// Update has orders
				//HasPromo = _promoList != null && _promoList != null && _promoList.Count > 0;
			}
		}


		public PromotionsListViewModel()
		{
	
		}

		public PromotionsListViewModel(List<Product> cnpList)
		{
			_cnpList = cnpList;
		}

		public async void Load()
		{

			foreach (var c in _cnpList)
			{
				string cnp = c.CNP;
				int cnpParse = Int32.Parse(cnp);
				var result = await ECommerceWS.ProdDetail(SessionData.UserAuthentication, SessionData.StorePharmacyId, cnpParse);


			}

		}
	}
}


