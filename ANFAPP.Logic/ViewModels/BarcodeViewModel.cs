
namespace ANFAPP.Logic.ViewModels
{
    public class BarcodeViewModel : IViewModel
    {

        #region Properties

        private string _cardName { get; set; }
        public string CardName 
        {
            get
            {
                return _cardName;
            }
            set
            {
                _cardName = value;
                OnPropertyChanged("CardName");
            }
        }

        private string _cardNumber { get; set; }
        public string CardNumber {
            get
            {
                return _cardNumber;
            }
            set
            {
                _cardNumber = value;
                OnPropertyChanged("CardNumber");
            }
        }

        private string _optNumber { get; set; }
        public string OptNumber {
            get 
            { 
                return _optNumber; 
            }
            set 
            {
                _optNumber = value;
                OnPropertyChanged("OptNumber");
            }
        }

        #endregion

    }
}
