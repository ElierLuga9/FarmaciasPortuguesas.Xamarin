using System;
using System.Threading;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreGenericSearchViewModel : StoreSearchViewModel
	{
		private int? _cnpem;
		private string _baseName;

		public StoreGenericSearchViewModel (int cnpem, string name) : base()
		{
			_baseName = name;
			_cnpem = cnpem as int?;
		}

		protected override ProductGroup BuildProductGroup()
		{
			var order = SelectedOrder;
			return new ProductGroup(
				string.Format("{0} - {1}", AppResources.GenericsStoreSearchResultsHeaderLabel, _baseName),
				order.Name);
		}

		protected override void AddToSearchResults(ProductOut prod)
		{
			if (prod.Generic.HasValue && prod.Generic.Value) {
				SearchResults.Add (prod);
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

			var result = await ECommerceWS.Search(SessionData.UserAuthentication, SessionData.StorePharmacyId, DEFAULT_PAGE_SIZE, SearchValue, _cnpem, null, pageStart, 
				0, points, brand, dose, ff, pp, ord); 

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			return result;
		}

	}
}

