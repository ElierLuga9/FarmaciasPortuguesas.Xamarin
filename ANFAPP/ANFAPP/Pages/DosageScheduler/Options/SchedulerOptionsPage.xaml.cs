using System;
using System.Collections.Generic;
using Acr.BarCodes;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.DosageScheduler.Options
{
    public class Option
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public object ActionIdentifier { get; set; }
    }

    public partial class SchedulerOptionsPage : ANFPage
    {
        private List<Option> _options;
        public List<Option> Options { 
            get { return _options; }
            set {
                _options = value;
                OnPropertyChanged("Options");
            }
        }

		private bool _onlyActiveScheduleReports = true;

        #region Page Initialization

        public SchedulerOptionsPage () : base() {}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }


        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
            return AppResources.DosageSchedulerPageTitle;
        }

        #endregion

        #region Event Handlers

        public async void TogglerViewTypeAction (object sender, EventArgs args)
        {
			var pnEnabled = !Settings.AppSettings.GetValueOrDefault("Scheduler.PN", true); 

            PNToggle.IsEnabled = false;
 
            try
            {
                var result = await SchedulerWS.PNConfig(SessionData.PharmacyUser.Username, SessionData.ParseInstallationId, pnEnabled);
                if (result.OK)
                {
                    PNToggle.State = pnEnabled;
                    Settings.AppSettings.AddOrUpdateValue("Scheduler.PN", pnEnabled);

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, AppResources.OK);
            }
        }

		public async void FilterToggleAction(object sender, EventArgs args) 
		{
			FilterToggle.State = _onlyActiveScheduleReports = !_onlyActiveScheduleReports;
		}

        async void OnOptionSelected (object sender, SelectedItemChangedEventArgs args)
        {
			Option opt = args.SelectedItem as Option;
			if (opt == null)
				return;
            LoadingView.IsVisible = true;
			try {
				var result = await SchedulerWS.SendReport(SessionData.PharmacyUser.Username, opt.Order, _onlyActiveScheduleReports);
                if (result.OK) 
                {
                    await DisplayAlert("", AppResources.ShedulerOptionsSuccess, AppResources.OK);

				}
				else
				{
					await DisplayAlert("", result.ErrorMessage, AppResources.OK);
				}
			} 
			catch (Exception ex) 
			{
				DisplayAlert("", ex.Message, AppResources.OK);
			}
            finally
            {
                LoadingView.IsVisible = false;
            }
            
			OptionList.SelectedItem = null;
        }

        #endregion

        void OnLoadComplete() 
        {

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing ();

            OptionList.BindingContext = this;
            Options = new List<Option>() {
                new Option() {
                    Order = 1, // To start with ANFLightBlue... see TableOrderBackgroundConverter.
                    Name = AppResources.SchedulerReport30Days,  
                },
                new Option() {
                    Order = 2, 
                    Name = AppResources.SchedulerReport60Days,  
                },
                new Option() {
                    Order = 3,
                    Name = AppResources.SchedulerReport90Days,  
                },
                new Option() {
                    Order = 4,
                    Name = AppResources.SchedulerReportAllDays,  
                },
            };
                
            TabBar.SelectedTab = DosageSchedulerTabbedBar.SelectedTabEnum.Options;

            var pnEnabled = Settings.AppSettings.GetValueOrDefault("Scheduler.PN", true); 
            PNToggle.State = pnEnabled;
        }
    }
}
