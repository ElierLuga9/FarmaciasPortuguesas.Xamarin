using ANFAPP.Logic;
using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Utils;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class DosingScheduleListItem : ContentView
	{
		
		public DosingScheduleListItem()
		{
			InitializeComponent ();
        }

		#region Button Clicks

		async void OnDeleteDosingScheduleButtonClicked(object sender, EventArgs args)
		{
			if (sender == null || !(sender is Button)) return;

			var context = (sender as Button).BindingContext;
			if (context == null || !(context is DosingSchedule)) return;

			// Deleting the medicine deletes all associated schedules, so we need confirmation 
			// from the user.
			var page = UIUtils.FindParentPage(this);

         
			var accepted = await page.DisplayAlert(null, string.Format(AppResources.ScheduleDeleteMessage,(context as DosingSchedule).Description),
					AppResources.Yes,
					AppResources.No);

			if (accepted)
			{
				// Delete the dosing schedule
				App.DosingScheduleVM.DeleteDosingSchedule(context as DosingSchedule);
			}
		}

		#endregion

    }
}

