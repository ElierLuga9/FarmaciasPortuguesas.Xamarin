using System;
using Xamarin.Forms;
using ANFAPP.Pages;
using ANFAPP.Logic;

namespace ANFAPP.Pages.Store
{
	public class ANFStorePage : ANFPage
	{
		protected override bool HasBasketButton()
		{
			return SessionData.IsAuthenticated;
		}

		protected override bool HasFavoritesButton()
		{
			return SessionData.IsAuthenticated;
		}

        protected override bool HasCardButton()
        {
            return SessionData.IsAuthenticated;
        }

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePageTitle;
		}

		protected override string GetAppBarSubtitle()
		{
			return SessionData.StorePharmacyName;
		}
	}
}
