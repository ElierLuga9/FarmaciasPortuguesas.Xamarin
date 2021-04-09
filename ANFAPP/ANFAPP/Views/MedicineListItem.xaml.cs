using System;
using Xamarin.Forms;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;

namespace ANFAPP.Views
{
    public partial class MedicineListItem : ContentView
    {
        public MedicineListItem ()
        {
            InitializeComponent ();
        }

        #region Button Clicks

        async void OnDeleteMedicineButtonClicked(object sender, EventArgs args)
        {
            if (sender == null || !(sender is Button)) return;

            var context = (sender as Button).BindingContext;
            if (context == null || !(context is Medicine)) return;

            // Deleting the medicine deletes all associated schedules, so we need confirmation 
            // from the user.
            var page = UIUtils.FindParentPage(this);

            var accepted = await page.DisplayAlert(null, AppResources.MedicineDeleteConfirmation, 
                    AppResources.Yes, 
                    AppResources.No);

            if (accepted) {                
                await App.ListDrugsVM.DeleteMedicine (context as Medicine);
            }
        }

        #endregion
    }
}
