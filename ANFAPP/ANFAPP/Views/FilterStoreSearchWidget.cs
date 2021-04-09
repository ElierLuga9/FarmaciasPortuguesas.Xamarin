using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Views
{
	public class FilterStoreSearchWidget : StoreSearchWidget
	{

		public FilterStoreSearchWidget() : base()
		{
			// Disable search box button
            SetSearchBoxState(false);
		}
	}
}
