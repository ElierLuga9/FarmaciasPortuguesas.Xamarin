using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Views
{

    public class HighlightsSearchWidget : StoreSearchFilterOrderWidget
	{

		public HighlightsSearchWidget() : base()
		{
			// Disable filter button
			SetFilterButtonState(false);
		}
	}
}
