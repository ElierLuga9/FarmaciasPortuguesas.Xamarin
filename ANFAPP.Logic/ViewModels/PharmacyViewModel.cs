using System;
using ANFAPP.Logic.ViewModels;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace ANFAPP.Logic
{
	public class PharmacyViewModel : IViewModel
	{
		public void SetData(List<Pharmacy> pList) {
			this.Pharmacies = new List<Pharmacy>(pList);

			var inService = new List<Pharmacy>();
            var longSchedulesOrOpenFull = new List<Pharmacy>();
            foreach (Pharmacy p in pList) {
                // farmacias horario alargada ou abertas 24h que estejam abertas ou farmacias de serviço
			
				if (PharmacyUtils.GetIsInService(p.Code)) {
					p.OpenStatusTxt = AppResources.InService;
					p.OpenStatusTextColor = Color.Aqua;

					inService.Add(p);

				}
                else if (((p.LongSchedule || p.OpenFull) && p.IsOpen))
                {
					
                    longSchedulesOrOpenFull.Add(p);

                }
            }

			this.ServicePharmacies = inService;
            this.LongSchudlePharmacies = longSchedulesOrOpenFull;
        }

        private List<Pharmacy> _pharmacies;
		public List<Pharmacy> Pharmacies
		{
			get { return _pharmacies; }
			set
			{
				_pharmacies = value;
				OnPropertyChanged("Pharmacies");
				OnPropertyChanged("ListHasItems");
			}
		}

		private List<Pharmacy> _servicePharmacies;
		public List<Pharmacy> ServicePharmacies
		{
			get { return _servicePharmacies; }
			set
			{
				_servicePharmacies = value;
				OnPropertyChanged("ServicePharmacies");
				OnPropertyChanged("ListHasItems");
			}
		}

        private List<Pharmacy> _longSchudlePharmacies;
        public List<Pharmacy> LongSchudlePharmacies
        {
            get { return _longSchudlePharmacies; }
            set
            {
                _longSchudlePharmacies = value;
                OnPropertyChanged("LongSchudlePharmacies");
                OnPropertyChanged("ListHasItems");
            }
        }
		private string _OpenTextPharm;
		public string OpenTextPharm
		{
			get { return _OpenTextPharm; }
			set
			{
				_OpenTextPharm = value;
				OnPropertyChanged("OpenStatusTxt");

			}
		}
		private Color _OpenTextPharmColor;
		public Color OpenTextPharmColor
		{
			get { return _OpenTextPharmColor; }
			set
			{
				_OpenTextPharmColor = value;
				OnPropertyChanged("OpenStatusTextColor");

			}
		}

        public bool ListHasItems
		{
			
			get
			{
                if (SessionData.IsAllPharmacies)
                {
                    return (Pharmacies != null && Pharmacies.Count > 0);
                }
                else
                {
                    if (SessionData.IsOnlyServicePharmacies)
                    {
                        return (ServicePharmacies != null && ServicePharmacies.Count > 0);
                    }
                    else
                    {
                        return (LongSchudlePharmacies != null && LongSchudlePharmacies.Count > 0);
                    }
                }
				/*return SessionData.IsAllPharmacies ? 
					(Pharmacies != null && Pharmacies.Count > 0) : 
					(ServicePharmacies != null && ServicePharmacies.Count > 0 );*/
			}
		}

		public void Notify()
		{
			OnPropertyChanged("ListHasItems");
		}
	}
}

