using System;
using System.Linq;
using System.Collections.ObjectModel;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Helper;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
	public class PharmacyServicesViewModel : IViewModel
	{

		#region EventHandlers

		public OnSuccessEventHandler OnLoadFinished;

		#endregion 

		private ObservableCollection<Grouping<char, PharmacyService>> _servicesGroupedByName;
		public ObservableCollection<Grouping<char, PharmacyService>> ServicesGroupedByName
		{
			get { return _servicesGroupedByName; }
			set
			{
				_servicesGroupedByName = value;
				OnPropertyChanged("ServicesGroupedByName");
			}
		}

		private ObservableCollection<PharmacyService> _services;
		public ObservableCollection<PharmacyService> Services
		{
			get { return _services; }
			set
			{
				_services = value;
				OnPropertyChanged("Services");
			}
		}

		public async void LoadData() 
		{
			Services = new ObservableCollection<PharmacyService> ();

			var result = await ECommerceWS.ListPharmacyServices (SessionData.UserAuthentication);

			foreach (PharmacyService ls in result.Services) {
				Services.Add (ls);
			}
				
			var shorted = from service in Services
				orderby service.SortKey
				group service by service.SortKey into ServicesGroup
				select new Grouping<char, PharmacyService> (ServicesGroup.Key, ServicesGroup);


			ServicesGroupedByName = new ObservableCollection<Grouping<char, PharmacyService>> (shorted);

			if (OnLoadFinished != null) OnLoadFinished();
		}

		public void FilterData(string userText) 
		{
			if (Services == null) return;

			var shorted = from service in Services
				where service.Description.Contains(userText)
				orderby service.SortKey
				group service by service.SortKey into ServicesGroup
				select new Grouping<char, PharmacyService> (ServicesGroup.Key, ServicesGroup);


			ServicesGroupedByName = new ObservableCollection<Grouping<char, PharmacyService>> (shorted);
		}

	}
}
