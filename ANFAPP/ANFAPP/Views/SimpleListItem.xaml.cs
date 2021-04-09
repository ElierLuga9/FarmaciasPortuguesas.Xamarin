using System;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class SimpleListItem : ContentView
	{
		public event EventHandler Clicked;

		public SimpleListItem ()
		{
			InitializeComponent ();
		}

		void OnClicked(object sender, EventArgs args)
		{
			if (Clicked != null) Clicked(BindingContext, args);
		}
	}
}
