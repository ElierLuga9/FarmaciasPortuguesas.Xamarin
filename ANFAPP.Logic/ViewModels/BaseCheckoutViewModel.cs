using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
	public abstract class BaseCheckoutViewModel : IViewModel
	{

		#region Properties

		private CheckoutStartOut _basket;
		public CheckoutStartOut Basket
		{
			get { return _basket; }
			set
			{
				SessionData.MasterCheckoutBasket = _basket = value;
				OnPropertyChanged("Basket");
				OnPropertyChanged("ShowVoucherControl");
			}
		}

		#endregion

		#region Event Handlers

		public OnErrorEventHandler OnLoadError = delegate { };
		public OnSuccessEventHandler OnLoadSuccess = delegate { };
		public OnLoadStartEventHandler OnLoadStart;

		#endregion

		#region Constructors

		public BaseCheckoutViewModel() { }

		public BaseCheckoutViewModel(CheckoutStartOut basket)
		{
			Basket = basket;
		}

		public virtual void RefreshBasket()
		{
			Basket = SessionData.MasterCheckoutBasket;
		}

		#endregion

	}
}
