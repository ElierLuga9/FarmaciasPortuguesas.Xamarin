using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreSearchableViewModel : IViewModel
	{

		#region Inner Classes

		public class SearchFilter
		{
			public long Id { get; set; }
			public string Name { get; set; }

			public SearchFilter(long id, string name)
			{
				Id = id;
				Name = name;
			}

			public override string ToString()
			{
				return Name;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is SearchFilter)) return false;
				return ((obj as SearchFilter).Id == Id);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		#endregion

		#region Unbindable Properties

		public bool InvalidSearchResults = true;
		public bool IsLoading = false;

		#endregion

		#region Search Filters

		private string _searchValue;
		public string SearchValue
		{
			get { return _searchValue; }
			set
			{
				_searchValue = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SearchValue");
			}
		}

		private SearchFilter _selectedOrder = _orderFilter[0];
		public SearchFilter SelectedOrder
		{
			get { return _selectedOrder; }
			set
			{
				if (value == null || _selectedOrder == value) return;
				_selectedOrder = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedOrder");
			}
		}

		private SearchFilter _selectedPrice = _priceFilter[0];
		public SearchFilter SelectedPrice
		{
			get { return _selectedPrice; }
			set
			{
				_selectedPrice = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedPrice");
			}
		}

		private SearchFilter _selectedPoints = _pointsFilter[0];
		public SearchFilter SelectedPoints
		{
			get { return _selectedPoints; }
			set
			{
				_selectedPoints = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedPoints");
			}
		}

		private SearchFilter _selectedDosage = new SearchFilter(0, "Todos");
		public SearchFilter SelectedDosage
		{
			get { return _selectedDosage; }
			set
			{
				_selectedDosage = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedDosage");
			}
		}

		private SearchFilter _selectedBrand = new SearchFilter(0, "Todos");
		public SearchFilter SelectedBrand
		{
			get { return _selectedBrand; }
			set
			{
				_selectedBrand = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedBrand");
			}
		}

		private SearchFilter _selectedFF = new SearchFilter(0, "Todos");
		public SearchFilter SelectedFF
		{
			get { return _selectedFF; }
			set
			{
				_selectedFF = value;

				InvalidSearchResults = true;
				OnPropertyChanged("SelectedFF");
			}
		}

		#endregion

		#region Filter Lists

		public List<SearchFilter> OrderFilter { get { return _orderFilter; } }
		private static List<SearchFilter> _orderFilter = new List<SearchFilter>() 
		{
			new SearchFilter(1, AppResources.StoreSearchOrderMostPopularLabel),
			new SearchFilter(2, AppResources.StoreSearchOrderAZLabel),
			new SearchFilter(3, AppResources.StoreSearchOrderZALabel),
			new SearchFilter(4, AppResources.StoreSearchOrderCheapestLabel),
			new SearchFilter(5, AppResources.StoreSearchOrderMostExpensiveLabel)
		};

		public List<SearchFilter> PointsFilter { get { return _pointsFilter; } }
		private static List<SearchFilter> _pointsFilter = new List<SearchFilter>() 
		{
			new SearchFilter(0, "Todos"),
			new SearchFilter(1, AppResources.StoreSearchPointsFirstTierLabel),
			new SearchFilter(2, AppResources.StoreSearchPointsSecondTierLabel),
			new SearchFilter(3, AppResources.StoreSearchPointsThirdTierLabel)
		};

		public List<SearchFilter> PriceFilter { get { return _priceFilter; } }
		private static List<SearchFilter> _priceFilter = new List<SearchFilter>() 
		{
			new SearchFilter(0, "Todos"),
			new SearchFilter(1, AppResources.StoreSearchPriceFirstTierLabel),
			new SearchFilter(2, AppResources.StoreSearchPriceSecontTierLabel),
			new SearchFilter(3, AppResources.StoreSearchPriceThirdTierLabel),
			new SearchFilter(4, AppResources.StoreSearchPriceFourthTierLabel)
		};

		private ObservableCollection<SearchFilter> _dosageFilter;
		public ObservableCollection<SearchFilter> DosageFilter
		{
			get { return _dosageFilter; }
			set
			{
				_dosageFilter = value;
				OnPropertyChanged("DosageFilter");
			}
		}

		private ObservableCollection<SearchFilter> _brandsFilter;
		public ObservableCollection<SearchFilter> BrandsFilter
		{
			get { return _brandsFilter; }
			set
			{
				_brandsFilter = value;
				OnPropertyChanged("BrandsFilter");
			}
		}

		private ObservableCollection<SearchFilter> _ffFilter;
		public ObservableCollection<SearchFilter> FFFilter
		{
			get { return _ffFilter; }
			set
			{
				_ffFilter = value;
				OnPropertyChanged("FFFilter");
			}
		}

		#endregion
		
	}
}
