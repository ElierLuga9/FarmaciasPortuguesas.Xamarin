using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class MenuListItem : ContentView
	{
		#region Properties


		private readonly string DEFAULT_USER_IMAGE = "ic_menu_utilizador.png";

		//testes
		private List<PromotionsOut> _promoList { get; set; }

		public async void PromoList()
		{
			var result = await ECommerceWS.GetPromotions(SessionData.UserAuthentication);
			_promoList = result.PromotionsList;

		}

		#endregion

		public MenuListItem()
		{
			//PromoList();
			InitializeComponent();
			Icon.IsVisible = true;

		}

		//protected override void OnParentSet()
		//{
		//    base.OnParentSet();
		//}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			SetUserIcon();
			SetUserPointsBackgroundImage();

		}
		/// <summary>
		/// Shows user image or a default user icon.
		/// </summary>
		/// <param name="UserImage"></param>
		public void SetUserImage()
		{

			if (SessionData.PharmacyUser != null)
			{
				var userImage = UserAreaViewModel.GetPlatformPhotoLocation();
				ButtonMask.IsVisible = true;
				ButtonImage.Source = !string.IsNullOrEmpty(userImage) ? userImage : DEFAULT_USER_IMAGE;

			}
			else
			{
				// Not logged
				ButtonMask.IsVisible = false;
				ButtonMask.Source = "ic_menu_utilizador.png";
			}
		}

		public void SetUserIcon()
		{
			if (this.ButtonImage.Source != null)
			{
				SetUserImage();
				Icon.IsVisible = false;

			}
			else
				Icon.IsVisible = true;
		}


		public void SetUserPointsBackgroundImage()
		{
			if (SessionData.PharmacyUser != null && UserPoints.Text != null)
			{
				if (UserPoints.Text.Length <= 2)
				{
					UserPointsBGImage.Source = "points_bg4.png";
				}
				else if (UserPoints.Text.Length == 3)
				{
					UserPointsBGImage.Source = "points_bg5.png";
				}
				else
					UserPointsBGImage.Source = "points_bg6.png";
			}
		}

		public void SetPromoCount()
		{
			if (PromoCount.Text.Length <= 2)
			{
				PromoCountImage.Source = "points_bg2.png";
			}
			else
			{
				PromoCountImage.Source = "points_bg3.png";
			}

		}
	}
}
