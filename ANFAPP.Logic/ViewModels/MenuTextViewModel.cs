using System;
using ANFAPP.Logic.ViewModels;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Objects;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using System.Threading.Tasks;

namespace ANFAPP.Logic
{
	public class MenuTextViewModel : IViewModel
	{
		public MenuTextViewModel () 
		{ 
			_lDAO = new LocalizationDAO();
		}

		private LocalizationDAO _lDAO;

		private ObservableCollection<MenuText> _menuTexts;
		public ObservableCollection<MenuText> MenuTexts
		{
			get { return _menuTexts; }
			set
			{
				_menuTexts = value;
				OnPropertyChanged("MenuTexts");
			}
		}

		private ObservableCollection<MenuText> _recentPharmacies;
		public ObservableCollection<MenuText> RecentPharmacies
		{
			get { return _recentPharmacies; }
			set
			{
				_recentPharmacies = value;
				OnPropertyChanged("RecentPharmacies");
			}
		}

		public void SetMenuEntrys() {

			MenuTexts = new ObservableCollection<MenuText> ();

			MenuTexts.Add (new MenuText () { Id = "0", Text = AppResources.NearMeOption, Order = 0 });
			MenuTexts.Add (new MenuText() { Id = "1", Text = AppResources.PlaceMenuOption, Order = 1 });
			MenuTexts.Add (new MenuText() { Id = "2", Text = "Serviços", Order = 2 });
			MenuTexts.Add (new MenuText() { Id = "3", Text = AppResources.Favorites, Order = 3 });
			MenuTexts.Add (new MenuText() { Id = "4", Text = AppResources.Recent, Order = 4 });

			//TODO: order this list remembering the selection

		}


		public async Task<bool> SetFavoritePharmacies() {
			MenuTexts = new ObservableCollection<MenuText> ();

			LocalizationDAO lDAO = new LocalizationDAO ();
			var pharmacies = await lDAO.GetFavourits ();

			foreach (Pharmacy p in pharmacies) {
				MenuText mt = new MenuText () {
					Id = p.Identifier,
					Text = p.Name,
					Order = 0
				};
				MenuTexts.Add (mt);

			}

			return true;
		}
			
		public async Task<bool> UpdateRecentPharmacies() {
			var pharmacies = await _lDAO.GetRecent ();

			var texts = new ObservableCollection<MenuText> ();

			foreach (Pharmacy p in pharmacies) {
				MenuText mt = new MenuText () {
					Id = p.Identifier,
					Text = p.Name,
					Order = 0
				};
				texts.Add (mt);
			}

			this.RecentPharmacies = texts;

			return true;
		}
	}
}

