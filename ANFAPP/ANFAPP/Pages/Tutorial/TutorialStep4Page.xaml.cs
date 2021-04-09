using ANFAPP.Logic;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.Tutorial
{
    public partial class TutorialStep4Page : TutorialBasePage
    {

        #region Page Initialization

		public TutorialStep4Page() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

		#region Animation Helpers

		/// <summary>
		/// Returns the appropriate view for the referenced step.
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		protected override View GetSubstepView(int step)
		{
			switch (step)
			{
				case 0: return Tutorial1Container;
				case 1: return Tutorial2Container;
				case 2: return Tutorial3Container;
				case 3: return Tutorial4Container;
				default: return null;
			}
		}

		/// <summary>
		/// Returns the amount of slideshow steps in the page.
		/// </summary>
		/// <returns></returns>
		protected override int GetMaxSlideshowSteps()
		{
			return 4;
		}

		/// <summary>
		/// Changes the theme of the dismiss button according to the selected slideshow step.
		/// </summary>
		/// <param name="step"></param>
		protected override void ThemeDismissButton(int step) { }

		#endregion

		#region Event Handlers

		protected override void NavigateToPrevious(object sender, EventArgs args)
		{
			NavigationUtils.PushPageAndClearHistory(new TutorialStep3Page(), Navigation);
		}

		protected override void NavigateToNext(object sender, EventArgs args)
		{
			NavigationUtils.PushPageAndClearHistory(new TutorialStep5Page(), Navigation);
		}

		#endregion

	}
}
