using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Helper;
using System.ServiceModel;
using System.Linq;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.EventHandlers;

namespace ANFAPP.ViewModel
{
	public class LocalizationViewModel : IViewModel
	{
		//public ObservableCollection<LocalizationShort> Locations { get; set; }
        //public ObservableCollection<Grouping<string, LocalizationShort>> LocationsGroupedByName { get; set; }

        #region EventHandlers

        public OnSuccessEventHandler OnLoadFinished;

        #endregion 


        public LocalizationViewModel ()
		{


		}

		private ObservableCollection<Grouping<char, LocalizationShort>> _locationsGroupedByName;
		public ObservableCollection<Grouping<char, LocalizationShort>> LocationsGroupedByName
		{
			get { return _locationsGroupedByName; }
			set
			{
				_locationsGroupedByName = value;
				OnPropertyChanged("LocationsGroupedByName");
			}
		}

		private ObservableCollection<LocalizationShort> _locations;
		public ObservableCollection<LocalizationShort> Locations
		{
			get { return _locations; }
			set
			{
				_locations = value;
				OnPropertyChanged("Locations");
			}
		}

		public async void LoadData(LocatorDataType dataType, string idDistrict = null, string idCounty = null, string idParish = null) {

			Locations = new ObservableCollection<LocalizationShort> ();

			LocalizationDAO lDAO = new LocalizationDAO ();
			List<LocalizationShort> result = new List<LocalizationShort>();
			switch (dataType) {
				case LocatorDataType.DISTRITOS:
					result = await lDAO.GetAllDistritosDistinct ();
					break;
				case LocatorDataType.CONCELHOS:
					result = await lDAO.GetConcelhosById (idDistrict);
					break;
				case LocatorDataType.FREGUESIA:
					result = await lDAO.GetFreguesiasById (idDistrict, idCounty);
					break;

			}




			//List<LocalizationShort> result = loc.Result;

			foreach (LocalizationShort ls in result) {
				Locations.Add (ls);
			}

			/*
			LocalizationShort ls;
			for (int i = 0; i < Convert.ToInt32(loc.Count()); i++) {
				ls = loc [i];
				Locations.Add (ls);
			}*/

			var shorted = from localization in Locations
				orderby localization.ShortByDescrition
				group localization by localization.ShortByDescrition into LocationsGroup
				select new Grouping<char, LocalizationShort> (LocationsGroup.Key, LocationsGroup);


			LocationsGroupedByName = new ObservableCollection<Grouping<char, LocalizationShort>> (shorted);

            if (OnLoadFinished != null) OnLoadFinished();
		}

		public void FilterData(string userText) 
		{
			if (Locations == null) return;

			var shorted = from localization in Locations
				where localization.Description.Contains(userText)
				orderby localization.ShortByDescrition
				group localization by localization.ShortByDescrition into LocationsGroup
				select new Grouping<char, LocalizationShort> (LocationsGroup.Key, LocationsGroup);


			LocationsGroupedByName = new ObservableCollection<Grouping<char, LocalizationShort>> (shorted);
		}

	}
}

