using System;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
	public class NativeMap : Map
	{
		public static readonly BindableProperty RouteCoordinatesProperty =
		BindableProperty.Create<NativeMap, List<Position>>(p => p.RouteCoordinates, new List<Position>());
		private ICollection<Pharmacy> _pharmacyList;
		public ICollection<Pharmacy> PharmacyList { 
			get { return _pharmacyList; } 
			set { 
				_pharmacyList = value;
				OnPropertyChanged ("PharmacyList");
			}
		}


		public List<Position> RouteCoordinates
		{
			get { return (List<Position>)GetValue(RouteCoordinatesProperty); }
			set { SetValue(RouteCoordinatesProperty, value); }
		}
		public bool AdjustRegionToAnnotations = true;
		public Action PinToAnnotations;

		public NativeMap () : base ()
		{
			RouteCoordinates = new List<Position>();
		}
	}
}

