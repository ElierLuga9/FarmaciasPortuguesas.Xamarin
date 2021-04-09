using System;
using System.Collections.Generic;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class PrescriptionListItem : ContentView
	{
		#region Bindable Objects

		public static readonly BindableProperty ParentViewModelProperty = BindableProperty.Create<PrescriptionListItem, IViewModel>(p => p.ParentViewModel, null);

		public StorePrescriptionViewModel ParentViewModel
		{
			get { return (StorePrescriptionViewModel)GetValue(ParentViewModelProperty); }
			set { SetValue(ParentViewModelProperty, value); }
		}

		#endregion

		#region Custom Events

		public event EventHandler RemoveClicked;
		public event EventHandler QtyChanged;

		#endregion

		public PrescriptionListItem()
		{
			InitializeComponent ();
		}

		#region Event Handlers

		void OnRemoveButtonClicked(object sender, EventArgs args)
		{
			if (RemoveClicked != null) RemoveClicked(BindingContext, args);
		}

		void OnQtyTextChanged(object sender, TextChangedEventArgs args)
		{
			if (QtyChanged != null) QtyChanged(sender, args);
		}

		#endregion
	}
}
