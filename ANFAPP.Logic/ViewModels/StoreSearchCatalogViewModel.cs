using System;
using System.Threading;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreSearchCatalogViewModel : StoreSearchViewModel
	{
		private long? _cat;

		public StoreSearchCatalogViewModel (long cat) : base()
		{
			_cat = cat as long?;
		}

		protected override ProductGroup BuildProductGroup()
		{
			if (!string.IsNullOrEmpty(SearchValue)) {
				return base.BuildProductGroup ();
			} else {
				var order = SelectedOrder ?? OrderFilter [0];
				return new ProductGroup(
					AppResources.CatalogSearchResultsHeaderLabel,
					order.Name);
			}
		}

		protected override async Task<SearchOut> InvokeSearch(CancellationToken token)
		{
			var pageStart = SearchResults.Count;

			long? points = SelectedPoints == null || SelectedPoints.Id < 0 ? (long?)null : SelectedPoints.Id;
			long? brand = SelectedBrand == null || SelectedBrand.Id < 0 ? (long?)null : SelectedBrand.Id;
			long? dose = SelectedDosage == null || SelectedDosage.Id < 0 ? (long?)null : SelectedDosage.Id;
			long? ff = SelectedFF == null || SelectedFF.Id < 0 ? (long?)null : SelectedFF.Id;
			long? pp = SelectedPrice == null || SelectedPrice.Id < 0 ? (long?)null : SelectedPrice.Id;
			long? ord = SelectedOrder == null ? 0 : SelectedOrder.Id;

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			var result = await ECommerceWS.Search(SessionData.UserAuthentication, SessionData.StorePharmacyId, DEFAULT_PAGE_SIZE, SearchValue, null, null, pageStart, 
				_cat, points, brand, dose, ff, pp, ord, true); 

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			return result;
		}

	}
}