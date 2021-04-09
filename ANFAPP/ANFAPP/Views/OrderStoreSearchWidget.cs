using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Views
{

	public class OrderStoreSearchWidget : StoreSearchFilterOrderWidget
	{

		public OrderStoreSearchWidget() : base()
		{
			// Disable the order button
			SetOrderButtonState(false);
		}
	}
}
