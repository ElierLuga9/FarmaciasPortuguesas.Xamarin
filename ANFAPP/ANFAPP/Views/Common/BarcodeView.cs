using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class BarcodeView : Image
    {

        #region Bindable Properties

        public static readonly BindableProperty CodeProperty = BindableProperty.Create(nameof(Code), typeof(string), typeof(BarcodeView), null);

        #endregion

        #region Bindable Objects

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        #endregion

    }
}
