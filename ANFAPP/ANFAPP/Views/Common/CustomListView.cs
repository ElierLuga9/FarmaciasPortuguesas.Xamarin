using System;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class CustomListView : ListView
    {
        public static readonly BindableProperty SeparatorInsetProperty = BindableProperty.Create<CustomListView, Thickness>(p => p.SeparatorInset, new Thickness(0));

        public Thickness SeparatorInset
        {
            get { return (Thickness)GetValue(SeparatorInsetProperty); }
            set { SetValue(SeparatorInsetProperty, value); }
        }
    }
}
