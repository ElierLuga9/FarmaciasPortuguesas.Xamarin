using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
    public class UserNotLoggedViewModel : IViewModel
    {

		#region Properties
        private string _imagemBanner;
		public string imagemBanner
		{
			get { return _imagemBanner; }
			set
			{
				_imagemBanner = value;
				OnPropertyChanged("imagemBanner");
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
        public async void GetPromoBanner()
		{
			//string json = "";

			try
			{
				var response =  await VouchersWS.GetUserCardPromo();

				if (response.promo == 1)
				{
					imagemBanner = "cardpromotionalbanner.png";
				}
				else
				{
					imagemBanner = "bg_card_benefits.png";
				}


			}
			catch (NullReferenceException e)
			{
				imagemBanner = "bg_card_benefits.png";
			}
			catch (JsonException e)
			{
				imagemBanner = "bg_card_benefits.png";
			}
			catch (Exception e2)
			{
				imagemBanner = "bg_card_benefits.png";
			}



		}
		#endregion

	}
}
