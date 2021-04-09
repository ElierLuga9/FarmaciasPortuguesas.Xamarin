using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class PromotionListItem : ContentView
	{
		#region Bindable Properties

        //public static readonly BindableProperty DoseProperty = BindableProperty.Create(nameof(Dose), typeof(string), typeof(PromotionListItem), null);
        //public static readonly BindableProperty DrugNameProperty = BindableProperty.Create(nameof(DrugName), typeof(string), typeof(PromotionListItem), null);
        //public static readonly BindableProperty ImageVisableProperty = BindableProperty.Create(nameof(ImageVisable), typeof(bool), typeof(PromotionListItem), false);
        //public static readonly BindableProperty DrugImageProperty = BindableProperty.Create(nameof(DrugImage), typeof(ImageSource), typeof(PromotionListItem), null);

		public event EventHandler OnMessageClicked = delegate { };
		#endregion


        //public string Dose
        //{
        //    get { return (string)GetValue(DoseProperty); }
        //    set { SetValue(DoseProperty, value); }
        //}
        //public string DrugName
        //{
        //    get { return (string)GetValue(DrugNameProperty); }
        //    set { SetValue(DrugNameProperty, value); }
        //}
        //public bool ImageVisable
        //{
        //    get { return (bool)GetValue(ImageVisableProperty); }
        //    set { SetValue(ImageVisableProperty, value); }
        //}
        //public ImageSource DrugImage
        //{
        //    get { return (ImageSource)GetValue(DrugImageProperty); }
        //    set { SetValue(DrugImageProperty, value); }
        //}
        //public PromotionListItem()
        //{
        //    InitializeComponent();
        //}

        //protected override void OnPropertyChanged(string propertyName = null)
        //{
        //    base.OnPropertyChanged(propertyName);

        //    if (propertyName == DoseProperty.PropertyName)
        //    {
        //        Dosage.Text = Dose;



        //    }
        //    else if (propertyName == DrugNameProperty.PropertyName)
        //    {

        //        Name.Text = DrugName;


        //    }
        //    else if (propertyName == ImageVisableProperty.PropertyName)
        //    {
        //        //ImageNull.IsVisible = ImageVisable;



        //    }
        //    else if (propertyName == DrugImageProperty.PropertyName)
        //    {
        //        //Image.Source = DrugImage;



        //    }
        //}
	}
}