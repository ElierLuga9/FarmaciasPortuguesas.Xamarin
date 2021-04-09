using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreSearchViewModel : StoreSearchableViewModel
	{
		private CancellationTokenSource _cancellationSource;

		#region Constants

		protected static readonly int DEFAULT_PAGE_SIZE = 20;

		#endregion

		#region Inner Classes

		public class ProductGroup : ObservableCollection<ProductOut>
		{
			public string Description { get ; set; }
			public string Order { get; set; }

			public ProductGroup(string description, string order)
			{
				Description = description;
				Order = order;
			}
		}

		#endregion

		#region Properties

		private ProductGroup _searchResults;
		public ProductGroup SearchResults
		{
			get { return _searchResults; }
			set
			{
				_searchResults = value;
				OnPropertyChanged("SearchResults");
			}
		}

		private ObservableCollection<ProductGroup> _searchGroups;
		public ObservableCollection<ProductGroup> SearchGroups
		{
			get { return _searchGroups; }
			set
			{
				_searchGroups = value;
				OnPropertyChanged("SearchGroups");
			}
		}

		private int _totalResults;
		public int TotalResults
		{
			get { return _totalResults; }
			set
			{
				_totalResults = value;
				OnPropertyChanged("TotalResults");
			}
		}

		public bool HasMore
		{
			get 
			{  
				if (_searchResults == null || _searchResults.Count == 0 || TotalResults == 0) return false;
				return _searchResults.Count < TotalResults;
			}
		}

		private bool _showNavigation;
		public bool ShowNavigation
		{
			get { return _showNavigation; }
			set
			{
				_showNavigation = value;
				OnPropertyChanged("ShowNavigation");
			}
		}

		public bool HasResults
		{
			get
			{
				return IsLoading || (SearchResults != null && SearchResults.Count > 0);
			}
		}

		#endregion

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Constructors

		public StoreSearchViewModel() 
		{
			_showNavigation = true;
		}

		public StoreSearchViewModel(string searchValue) : this()
		{
			SearchValue = searchValue;
		}

		#endregion

		#region Search

		/// <summary>
		/// Adds to the search results.
		/// 
		/// Subclasses should override this to filter which products get added to the results. 
		/// 
		/// </summary>
		/// <param name="prod">A product.</param>
		protected virtual void AddToSearchResults(ProductOut prod)
		{
			SearchResults.Add (prod);
		}

		/// <summary>
		/// Perform a new search.
		/// </summary>
		public async virtual Task PerformSearch()
		{
			if (_cancellationSource != null && !_cancellationSource.IsCancellationRequested) {
				_cancellationSource.Cancel ();
			}

			if (OnLoadStart != null) await OnLoadStart();
			IsLoading = true;

			// Reset the search. We need the empty option to reset the filter.
			DosageFilter = new ObservableCollection<SearchFilter>(new SearchFilter[]{ new SearchFilter( 0, "Todos") });
			BrandsFilter = new ObservableCollection<SearchFilter>(new SearchFilter[]{ new SearchFilter( 0, "Todos") });
			FFFilter = new ObservableCollection<SearchFilter>(new SearchFilter[]{ new SearchFilter( 0, "Todos") });

			SearchGroups = new ObservableCollection<ProductGroup>();
			SearchGroups.Add(SearchResults = BuildProductGroup());

			TotalResults = 0;
			SearchResults.Clear ();
			OnPropertyChanged("HasMore");
			OnPropertyChanged("HasResults");

			// Call Webservice
			SearchOut result = null;
			try 
			{ 
				var source = new CancellationTokenSource();
				_cancellationSource = source;
				result = await InvokeSearch(source.Token);

				if (_cancellationSource == source) {
					_cancellationSource = null;
					source.Dispose();
				}

				if (result == null || result.Products == null || result.Products.Count == 0) 
				{
					IsLoading = false;
					OnPropertyChanged("HasResults");
					if (OnLoadSuccess != null) OnLoadSuccess();
					return;
				}
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				OnPropertyChanged("HasResults");
				IsLoading = false;
				return;
			}

			// Init products
			TotalResults = result.Results;
			foreach (var prod in result.Products)
			{
				AddToSearchResults (prod);
			}
			
			// Brand Filters
			if (result.BrandFilters != null && result.BrandFilters.Count > 0) 
			{
				foreach (var filter in result.BrandFilters)
				{
					BrandsFilter.Add(new SearchFilter(filter.ID, filter.Name));
				}
			}

			// Dosage Filters
			if (result.DosageFilters != null && result.DosageFilters.Count > 0)
			{
				foreach (var filter in result.DosageFilters)
				{
					DosageFilter.Add(new SearchFilter(filter.ID, filter.Name));
				}
			}

			// FF Filters
			if (result.FFFilters != null && result.FFFilters.Count > 0)
			{
				foreach (var filter in result.FFFilters)
				{
					FFFilter.Add(new SearchFilter(filter.ID, filter.Name));
				}
			}

			// Set search results as valid
			IsLoading = false;
			InvalidSearchResults = false;
			OnPropertyChanged("HasMore");
			OnPropertyChanged("HasResults");
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		/// <summary>
		/// Loads the next page
		/// </summary>
		public async void LoadNextPage()
		{
			IsLoading = true;

			// Call Webservice
			SearchOut result = null;
			try 
			{
				CancellationTokenSource source = new CancellationTokenSource();
				_cancellationSource = source;
				result = await InvokeSearch(source.Token);

				if (_cancellationSource == source) {
					_cancellationSource = null;
					source.Dispose();
				}
			} 
			catch (Exception ex) 
			{
				IsLoading = false;
				return;
			}

			if (result == null || result.Products == null || result.Products.Count == 0) return;

			// Add products
			foreach (var prod in result.Products)
			{
				AddToSearchResults (prod);
			}

			IsLoading = false;
			OnPropertyChanged("HasMore");
		}

		/// <summary>
		/// Builds the product group
		/// </summary>
		/// <returns></returns>
		protected virtual ProductGroup BuildProductGroup()
		{
			var order = SelectedOrder ?? OrderFilter [0];
			return new ProductGroup(
				string.Format(AppResources.StoreSearchResultsHeaderLabel, SearchValue),
				order.Name);
		}

		/// <summary>
		/// Invoke the WS
		/// </summary>
		/// <returns></returns>
		protected virtual async Task<SearchOut> InvokeSearch(CancellationToken token)
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
				0, points, brand, dose, ff, pp, ord); 

			// Handle cancellation...
			if (token != CancellationToken.None && token.IsCancellationRequested) return null;

			return result;
		}

		#endregion

	}
}
