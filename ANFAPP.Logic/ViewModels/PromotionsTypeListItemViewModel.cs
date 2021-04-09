using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class PromotionsTypeListItemViewModel : IViewModel
    {
		#region Properties
		private Product _item = new Product();

        private PromotionsOut _promotion { get; set; }
        public PromotionsOut Promotion
        {
            get { return _promotion; }
            set
            {
                _promotion = value;
                OnPropertyChanged("Promotion");
            }
        }
		private bool _titleOnlyImage { get; set; }
		public bool TitleOnlyImage
		{
			get { return _titleOnlyImage; }
			set
			{
				_titleOnlyImage = value;
				OnPropertyChanged("TitleOnlyImage");
			}
		}
		private bool _descriptionOnlyImage { get; set; }
		public bool DescriptionOnlyImage
		{
			get { return _descriptionOnlyImage; }
			set
			{
				_descriptionOnlyImage = value;
				OnPropertyChanged("DescriptionOnlyImage");
			}
		}

		#endregion

		#region Event Handler
		public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;
        #endregion

        /// <summary>
        /// Load the voucher for data binding.
        /// </summary>
        /// <param name="promotion"></param>
        public void LoadData(PromotionsOut promotion)
        {
			Promotion = promotion;
            //testes
            if (OnSuccess != null) OnSuccess();
        }


	}
}
