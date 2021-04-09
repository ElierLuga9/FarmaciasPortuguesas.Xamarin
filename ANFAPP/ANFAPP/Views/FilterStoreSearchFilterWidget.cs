using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Views
{
    class FilterStoreSearchFilterWidget : StoreSearchFilterOrderWidget
    {
        public FilterStoreSearchFilterWidget() : base()
        {
            // Disable the filter and order button
            SetFilterButtonState(false);
        }

    }
}
