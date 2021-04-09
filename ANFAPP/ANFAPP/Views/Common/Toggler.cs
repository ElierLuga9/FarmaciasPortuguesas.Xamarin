using System;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
	public class Toggler : CustomImage
	{
		#region Bindable Properties

		public static readonly BindableProperty StateProperty = BindableProperty.Create<Toggler, bool>(p => p.State, false);
		
        #endregion

        #region Properties

        public bool State
		{
			get { return (bool)GetValue(StateProperty); }
			set 
			{ 
				SetValue(StateProperty, value);
				//SetState(value);
			}
		}

        #endregion 

        public Toggler() : base()
        {
            // Set the default state as false.
            //State = false;
            SetState(State = false);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (this.GestureRecognizers.Count > 0) return;

			if (!InputTransparent) { 
				this.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(OnTogglerTapped) });
			}
        }

        public void OnTogglerTapped(object sender)
        {
            State = !State;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if ("State".Equals(propertyName))
            {
                SetState(State);
            }
        }

        /// <summary>
        /// Sets the toggle state.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(bool state) {

			if (!state) 
            {
				this.Source = "switch_left";
			} 
            else 
            {
				this.Source = "switch_right";
			}
		}
	}
}

