using System;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Wrappers;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreSelectPAViewModel : IViewModel
	{
		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#region Properties

		public GetPAOut Result { get; set; }

		private PAListItem _selectedItem;
		public PAListItem SelectedItem
		{
			get { return _selectedItem; }
			set { 
				_selectedItem = value;
				OnPropertyChanged ("SelectedItem");
			}
		}

		private ObservableCollection<DosageItem> _dosageFilter;
		public ObservableCollection<DosageItem> DosageFilter
		{
			get { return _dosageFilter; }
			set
			{
				_dosageFilter = value;
				OnPropertyChanged("DosageFilter");
			}
		}

		private ObservableCollection<PackItem> _packFilter;
		public ObservableCollection<PackItem> PackFilter
		{
			get { return _packFilter; }
			set
			{
				_packFilter = value;
				OnPropertyChanged("PackFilter");
			}
		}

		private DosageItem _selectedDosage;
		public DosageItem SelectedDosage
		{
			get { return _selectedDosage; }
			set
			{
				_selectedDosage = value;
				OnPropertyChanged("SelectedDosage");

				if (_selectedDosage != null) {
					LoadPA(SelectedItem.Id, SelectedDosage.Id);		
				}
			}
		}

		private PackItem _selectedPack;
		public PackItem SelectedPack
		{
			get { return _selectedPack; }
			set
			{
				_selectedPack = value;

				OnPropertyChanged("SelectedPack");
			}
		}

		#endregion

		public async Task LoadPA(int paId, int? dosId) 
		{
			if (OnLoadStart != null) await OnLoadStart();

			try {
				var result = await ECommerceWS.GetPA(SessionData.UserAuthentication, paId, dosId, SessionData.StorePharmacyId);
				if (null != result.Packs && SelectedDosage != null) {
					// After selecting the pack, the returned dosage filter list contains the element with the matching dosage identifier.
					PackFilter = new ObservableCollection<PackItem>(result.Packs);
				} else {
					DosageFilter = new ObservableCollection<DosageItem>(result.Dosages);
				}

				Result = result;

			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}

			if (OnLoadSuccess != null) OnLoadSuccess ();
		}
	}
}
