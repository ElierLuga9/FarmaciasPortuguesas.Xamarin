using MapKit;
using ANFAPP.Logic.Database.Models;
using CoreLocation;

namespace ANFAPP.iOS.Renderer
{
	public class PharmacyAnnotation : MKAnnotation
	{
		//public override MKAnnotation Coordinate { get;}
		public Pharmacy Pharmacy { get; set; }
		public string Name { get; set; }
		public CLLocationCoordinate2D Coords { get; set; }
		public string Id { get; set; }

		public string _Title { get; set; }
		public string _Subtitle { get; set; }
		
		public PharmacyAnnotation () { }

		public override CLLocationCoordinate2D Coordinate {
			get {
				return Coords;
			}
		}

		public override void SetCoordinate (CLLocationCoordinate2D value)
		{
			this.Coords = value;
		}



		public override string Subtitle {
			get {
				return _Subtitle;
			}
		}

		public override string Title {
			get {
				return _Title;
			}

		}



	}
}

