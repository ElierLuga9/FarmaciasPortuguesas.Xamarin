using System;
using System.Threading;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
	public class CategoryStoreSearchViewModel : StoreSearchViewModel
	{
		#region Properties

		private string _catName;

		private long _baseCategory;
		public long BaseCategory
		{
			get { return _baseCategory; }
			set
			{
				_baseCategory = value;
				OnPropertyChanged("BaseCategory");
			}
		}

		#endregion

		#region Constructors

		public CategoryStoreSearchViewModel(long baseCategory, string catName)
		{
			BaseCategory = baseCategory;
			_catName = catName;
		}

		#endregion

		#region Search

		/// <summary>
		/// Builds the product group
		/// </summary>
		/// <returns></returns>
		protected override ProductGroup BuildProductGroup()
		{
			return new ProductGroup(_catName, SelectedOrder.Name);
		}

		/// <summary>
		/// Invoke the WS
		/// </summary>
		/// <returns></returns>
		protected override async Task<SearchOut> InvokeSearch(CancellationToken token)
		{
			var pageStart = SearchResults.Count;

			long? points = SelectedPoints == null ? 0 : SelectedPoints.Id;
			long? brand = SelectedBrand == null ? 0 : SelectedBrand.Id;
			long? dose = SelectedDosage == null ? 0 : SelectedDosage.Id;
			long? ff = SelectedFF == null ? 0 : SelectedFF.Id;
			long? pp = SelectedPrice == null ? 0 : SelectedPrice.Id;
			long? ord = SelectedOrder == null ? 0 : SelectedOrder.Id;

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			var result = await ECommerceWS.Search(
				SessionData.UserAuthentication, 
				SessionData.StorePharmacyId, 
				DEFAULT_PAGE_SIZE, 
				SearchValue, 
				null,
				null,
				pageStart,
				BaseCategory,
				points, 
				brand, 
				dose, 
				ff, 
				pp, 
				ord); 

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			return result;
		}

		#endregion
	}
}

