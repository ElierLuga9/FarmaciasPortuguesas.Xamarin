using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreImageDetailViewModel : IViewModel
	{
		private uint _selectedIdx = 0;
		private ProductDetailOut _product;
		public ProductDetailOut Product {
			get { return _product; }
			set { 
				_product = value;
				OnPropertyChanged ("Product");
			}
		}

		private List<UriImageSource> _thumbnails;
		public List<UriImageSource> Thumbnails { 
			get { return _thumbnails; }
			set {
				_thumbnails = value;
				OnPropertyChanged ("Thumbnails");
				OnPropertyChanged("HasEnoughThumbnails");
			}
		}

		public bool HasEnoughThumbnails {
			get
			{
				return _thumbnails != null && _thumbnails.Count > 1;
			}
		}

		private UriImageSource _selectedImage;
		public UriImageSource SelectedImage {
			get { return _selectedImage; }
			set { 
				_selectedImage = value;
				OnPropertyChanged ("SelectedImage");
			}
		}

		private StoreImageDetailViewModel () : base()
		{
		}

		public StoreImageDetailViewModel (ProductDetailOut product) : base()
		{
			Product = product;

			if (Product.ImageList != null && Product.ImageList.Count > 0) {

				var thumbnails = new List<UriImageSource> ();
				foreach (ProductDetailImage item in Product.ImageList) {
					thumbnails.Add (item.ImageSource1);
				}

				_selectedIdx = 0;
				foreach (ProductDetailImage item in Product.ImageList) {
					if (item.Princ) {
						SelectedImage = item.ImageSource2;
						break;
					}
					++_selectedIdx;
				}

				if (SelectedImage == null) SelectedImage = Product.ImageList[0].ImageSource2;

				Thumbnails = thumbnails;
			}
		}

		public void SelectImageAtIndex(uint idx) 
		{
			if (idx >= Product.ImageList.Count || idx == _selectedIdx)
				return;

			SelectedImage = Product.ImageList [(int)idx].ImageSource2;
			_selectedIdx = idx;
		}

		public void SelectImage(Image image) 
		{
			var source = image.Source.GetValue (UriImageSource.UriProperty);

			foreach (ProductDetailImage detail in Product.ImageList) {
				if (source.Equals (detail.ImageSource1.Uri)) {
					SelectedImage = detail.ImageSource2;
					break;
				}
			}
		}
	}
}

