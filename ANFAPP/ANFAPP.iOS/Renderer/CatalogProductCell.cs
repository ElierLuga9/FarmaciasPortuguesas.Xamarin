
using System;

using Foundation;
using UIKit;

namespace ANFAPP.iOS
{
	public partial class CatalogProductCell : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CatalogProductCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CatalogProductCell");

		public CatalogProductCell (IntPtr handle) : base (handle)
		{
		}

		public static CatalogProductCell Create ()
		{
			return (CatalogProductCell)Nib.Instantiate (null, null) [0];
		}
	}
}

