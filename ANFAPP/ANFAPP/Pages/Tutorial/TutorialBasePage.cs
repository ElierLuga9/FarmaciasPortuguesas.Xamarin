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
    public abstract class TutorialBasePage : ANFPage
    {
	
		#region Constants

		protected int SLIDESHOW_DELAY = 5000;
		protected int MAX_SLIDESHOW_STEPS { get { return GetMaxSlideshowSteps(); } }

		#endregion

		#region Properties

		protected int _currentStep = -1;
		protected bool _isActive = false;
		protected View _visibleStep = null;

		#endregion

        #region Page Initialization

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// Start slideshow
			_isActive = true;
			GoToNextSubstep();
			StartTutorialSlideshow();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_isActive = false;
		}

        #endregion

		#region Animation Helpers

		/// <summary>
		/// Starts the tutorial slideshow
		/// </summary>
		/// <returns></returns>
		protected async Task StartTutorialSlideshow() 
		{
			if (MAX_SLIDESHOW_STEPS > 1 && !_isActive) return;
			await Task.Delay(SLIDESHOW_DELAY);
			
			GoToNextSubstep();
			StartTutorialSlideshow();
		}

		/// <summary>
		/// Increment the current step and animate to it.
		/// </summary>
		protected void GoToNextSubstep()
		{
			_currentStep = (++_currentStep % MAX_SLIDESHOW_STEPS);
			AnimateToSubstep(_currentStep);
		}

		/// <summary>
		/// Animate from the current step to the referenced step.
		/// </summary>
		/// <param name="step"></param>
		protected void AnimateToSubstep(int step)
		{
			AnimateViewOut(_visibleStep);
			AnimateViewIn(_visibleStep = GetSubstepView(step));

			ThemeDismissButton(step);
		}

		/// <summary>
		/// Animate a view out of visibility
		/// </summary>
		/// <param name="view"></param>
		protected void AnimateViewOut(View view)
		{
			if (view == null) return;
			view.ScaleTo(0.95, 2000, Easing.CubicOut);
			view.FadeTo(0, 1500, Easing.CubicOut);
		}

		/// <summary>
		/// Animate a view into visibility
		/// </summary>
		/// <param name="view"></param>
		protected void AnimateViewIn(View view)
		{
			if (view == null) return;
			view.FadeTo(1, 1500, Easing.CubicIn);
			view.ScaleTo(1, 1000, Easing.CubicIn);
		}

		#endregion

		#region Event Handlers

		protected void OnDismissTutorial(object sender, EventArgs args) 
		{
			if (!SessionData.IsLogged)
			{
				NavigationUtils.PushPageAndClearHistory(new QuickRegisterPage(), Navigation);
			}
			else
			{
				NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);
			}
		}

		#endregion

		#region Abstract

		protected abstract void NavigateToPrevious(object sender, EventArgs args);

		protected abstract void NavigateToNext(object sender, EventArgs args);

		/// <summary>
		/// Returns the appropriate view for the referenced step.
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		protected abstract View GetSubstepView(int step);

		protected abstract int GetMaxSlideshowSteps();

		protected abstract void ThemeDismissButton(int step);

		#endregion

	}
}
