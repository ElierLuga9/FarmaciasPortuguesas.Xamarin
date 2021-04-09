using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using ANFAPP.Views.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic;

namespace ANFAPP.Views
{
    public partial class TutorialNavigationWidget : ContentView
    {

		#region Bindable Properties

		public static readonly BindableProperty SelectedStepProperty = BindableProperty.Create<TutorialNavigationWidget, int>(p => p.SelectedStep, 0);
		public int SelectedStep
		{
			get { return (int) GetValue(SelectedStepProperty); }
			set
			{
				SetValue(SelectedStepProperty, value);
				SetSelectedStep(SelectedStep);
			}
		}

		#endregion

		public TutorialNavigationWidget()
        {
            InitializeComponent ();
        }

		#region Initializers

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (string.Equals(propertyName, "SelectedStep"))
			{
				SetSelectedStep(SelectedStep);
			}
		}

		/// <summary>
		/// Changes the selection state for all tabs.
		/// </summary>
		/// <param name="selectedTab"></param>
		public void SetSelectedStep(int selectedStep)
		{
			switch (selectedStep)
			{
				case 1: MarkStepAsSelected(Step1Image); LeftArrow.Opacity = 0.5; break;
				case 2: MarkStepAsSelected(Step2Image); break;
				case 3: MarkStepAsSelected(Step3Image); break;
				case 4: MarkStepAsSelected(Step4Image); break;
				case 5: MarkStepAsSelected(Step5Image); RightArrow.Opacity = 0.5; break;
			}
		}

		/// <summary>
		/// Marks the referenced step as selected.
		/// </summary>
		/// <param name="view"></param>
		private void MarkStepAsSelected(View view)
		{
			
			if (view == null) return;
			view.Opacity = 1;
		}

		#endregion

    }
}