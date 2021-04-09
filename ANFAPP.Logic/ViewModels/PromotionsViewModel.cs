using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Database.DAOs;

namespace ANFAPP.Logic.ViewModels
{
    public class PromotionsViewModel : IViewModel
    {
		
		public OnErrorEventHandler OnError;
		public OnErrorEventHandler OnLoadStart;
        #region Properties
		public event OnSuccessEventHandler OnSuccess;
		public Voucher voucher;
		private bool _userIn = true;
        public bool UserIn
        {
            get { return _userIn; }
            set
            {
                _userIn = value;
                OnPropertyChanged("UserIn");
            }
        }

        private bool _hasPromo = true;
        public bool HasPromo
        {
            get { return _hasPromo; }
            set
            {
                _hasPromo = value;
                OnPropertyChanged("HasPromo");
            }
        }
		#endregion

		public List<PromotionsOut> _promtoListCrm;
        private List<PromotionsOut> _promoList { get; set; }


		public List<PromotionsOut> PromoList
        {
            get { return _promoList; }
            set
            {
                _promoList = value;
                OnPropertyChanged("PromoList");

                // Update has orders
                HasPromo = _promoList != null && _promoList != null && _promoList.Count > 0;
            }
        }

        //testes
        private static int _promoTotals { get; set; }
        public static int PromoTotals
        {
            get { return _promoTotals; }
            set
            {
                _promoTotals = value;
                //OnPropertyChanged("PromoTotal");
            }
        }

        private int _promoTotal { get; set; }
        public int PromoTotal
        {
            get { return _promoTotal; }
            set
            {
                _promoTotal = value;
                OnPropertyChanged("PromoTotal");
            }
        }

        #region Events

        public OnErrorEventHandler OnLoadError;
        public OnSuccessEventHandler OnLoadSuccess;

        #endregion

        #region Loader
        /// <summary>
        /// Load promotions list and change some items
        /// </summary>
        /// <returns></returns>
		/// 
		public PromotionsViewModel()
		{
		}
        public async Task LoadList(bool fromHome)
        {
			List<string> tempList = new List<string>();
			_promtoListCrm = new List<PromotionsOut>();
			int contador = 0;
			bool hasCrmPromo = false;

            try
            {
                var result = await ECommerceWS.GetPromotions(SessionData.UserAuthentication);




				//var crmList = userCrmPromo.PromotionsEvent;
				//crmList = new List<Models.Out.Ecommerce.ClientePromoCrmOut.Promotions>();


				//userCrmPromo.PromotionsEvent = new List<Models.Out.Ecommerce.ClientePromoCrmOut.Promotions>();
				//var first6elements = result.PromotionsList.Take(6).ToList();
				try
				{


					foreach (var promo in result.PromotionsList)
					{

						if ((result.PromotionsList.Count != 0 || result.PromotionsList != null) && fromHome)
						{
							PromotionsDAO pDAO = new PromotionsDAO();
							var dataBasePromos = await pDAO.GetAllPromo();

							var containID = dataBasePromos.Where(i => i.Id == promo.Id).SingleOrDefault();
							if (containID == null)
							{
								var promoId = new Promotion()
								{
									Id = promo.Id,
									Read = false

								};

								await pDAO.InsertOrUpdate(promoId);

							}

						}
						/*if (promo.OnlyImage)
						{
							promo.Promo1IsActive = true;
							promo.Promo2IsActive = false;
							promo.Promo3IsActive = false;
							promo.Promo4IsActive = false;
						}*/
						/*else
						{*/

						//Have Image and Lead
						if ((promo.Image != null) && (promo.Lead != null))
						{
							promo.Promo1IsActive = false;
							promo.Promo2IsActive = false;
							promo.Promo3IsActive = true;
							promo.Promo4IsActive = false;
						}

						//No Image and have Lead(voucher)
						if ((promo.Image == null) && (promo.Lead != null))
						{
							hasCrmPromo = true;
							promo.Promo1IsActive = false;
							promo.Promo2IsActive = true;
							promo.Promo3IsActive = false;
							promo.Promo4IsActive = false;
						}


						if (promo.OnlyImage)
						{
							promo.Promo1IsActive = true;
							promo.Promo2IsActive = false;
							promo.Promo3IsActive = false;
							promo.Promo4IsActive = false;
						}
						else
						{

							promo.Promo1IsActive = false;
							promo.Promo2IsActive = false;
							promo.Promo3IsActive = true;
							promo.Promo4IsActive = false;

						}


						//No image and no Lead
						if ((promo.Image == null) && (promo.Lead == null))
						{
							promo.Promo1IsActive = false;
							promo.Promo2IsActive = false;
							promo.Promo3IsActive = false;
							promo.Promo4IsActive = true;
						}
						//}

						if (promo.PromoType == 0)
						{
							promo.ButtonLabel = "Saber Mais";
						}


						if (promo.PromoType == 1)
						{
							if (promo.Description != null)
							{
								promo.ButtonLabel = "Saber Mais";
							}
							else
								promo.ButtonLabel = "Ver Produto";
						}


						if (promo.PromoType == 2)
						{
							if (promo.Description != null)
							{
								promo.ButtonLabel = "Saber Mais";
							}
							else
								promo.ButtonLabel = "Obter Vale";
							if (SessionData.HasPharmacyCard)
							{
								hasCrmPromo = true;
							}

						}


						if (promo.PromoType == 3)
						{
							if (promo.Description != null)
							{
								promo.ButtonLabel = "Saber Mais";
							}
							else
								promo.ButtonLabel = "Ver Produtos";
						}

						if (promo.DateStart != null && promo.DateEnd != null)
						{
							if (promo.DateStart.Value.Month == promo.DateEnd.Value.Month)
							{
								promo.DateLabel = "Válido de " + promo.DateStart.Value.Day + " a " + promo.DateEnd.Value.Day + " de " + formatMonth(promo.DateEnd.Value.Month);

								if (promo.DateStart.Value.Day == promo.DateEnd.Value.Day)
								{
									promo.DateLabel = "Válido até às " + promo.DateEnd.Value.Hour + "h" + promo.DateEnd.Value.Minute + " do dia de hoje";
								}

							}
							else
							{
								promo.DateLabel = "Válido de " + promo.DateStart.Value.Day + " de " + formatMonth(promo.DateStart.Value.Month) + " a " + promo.DateEnd.Value.Day + " de " + formatMonth(promo.DateEnd.Value.Month);
							}
							if (promo.DateEnd.Value.Month != 5) 
							{
								System.Diagnostics.Debug.WriteLine("OK");

							}
						}
						else
						{
							promo.DateLabel = "Sem data de validade";
						}
					}
				}catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Exception on PromotionViewModel: " + ex.Message.ToString()); }

				//esta lista tem de ser alterada se houver cartao e se houver campanhas crm
				if (result.PromotionsList != null || !result.PromotionsList.Equals(0)) 
				{ 
					if (hasCrmPromo == false)
					{
						PromoList = result.PromotionsList;
					}
					else if (SessionData.HasPharmacyCard == false)
					{
						PromoList = result.PromotionsList;
					}
					else if (hasCrmPromo == true && SessionData.HasPharmacyCard == true)
					{
						try {
							//call WS for user promos
							var userCrmPromo = await VouchersWS.GetVoucherPromo(SessionData.PharmacyUser.CardNumber, SessionData.UserAuthentication);
							foreach (var promoEvent in userCrmPromo.PromotionsEvent)
							{


								tempList.Add(promoEvent.Code);


							}
							foreach (var promo in result.PromotionsList)
							{
								if (promo.PromoType == 2 && userCrmPromo != null)
								{

									if (tempList.Contains(promo.PromoTypePar))
									{

										_promtoListCrm.Add(promo);

									}

								}
								else if (promo.PromoType != 2)
								{
									_promtoListCrm.Add(promo);
								}
							}


							PromoList = _promtoListCrm;
						
						
						
						
						}
						catch (Exception e)
						{
							PromoList = result.PromotionsList;
						}
					}
	               //TODO : RETIRAR O COMENT CASO DER PROBLEMAS
					PromotionsDAO pDAO2 = new PromotionsDAO();
					var listsize = await pDAO2.GetAllPromo();

					foreach (var a in listsize)
					{
						if (a.Read == false)
						{
							contador = contador + 1;
							
						}
						
					}

						//PromoTotal = contador;
					PromoTotal = PromoTotals = contador;
			}
				//PromoTotal = PromoTotals = 0;
				


            }
            catch (Exception ex)
            {
                HasPromo = false;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (OnLoadError != null)
                    OnLoadError("", AppResources.GenericErrorMessage);
            }

            if (OnLoadSuccess != null) OnLoadSuccess();

        }

		public async void DataSave()
		{
			PromotionsDAO pDAO2 = new PromotionsDAO();
			var listsize = await pDAO2.GetAllPromo();
			foreach (var a in listsize)
			{
				var promoId = new Promotion()
				{
					Id = a.Id,
					Read = true

				};
				await pDAO2.Update(promoId);

			}



		}

		/// <summary>
		/// Format the month from int to string
		/// </summary>
		/// <param name="Month"></param>
		/// <returns></returns>
		public string formatMonth(int Month)
        {
            string Monthstr = "";

            switch (Month)
            {
                case 1:
                    Monthstr = "Janeiro";
                    break;
                case 2:
                    Monthstr = "Fevereiro";
                    break;
                case 3:
                    Monthstr = "Março";
                    break;
                case 4:
                    Monthstr = "Abril";
                    break;
                case 5:
                    Monthstr = "Maio";
                    break;
                case 6:
                    Monthstr = "Junho";
                    break;
                case 7:
                    Monthstr = "Julho";
                    break;
                case 8:
                    Monthstr = "Agosto";
                    break;
                case 9:
                    Monthstr = "Setembro";
                    break;
                case 10:
                    Monthstr = "Outubro";
                    break;
                case 11:
                    Monthstr = "Novembro";
                    break;
                case 12:
                    Monthstr = "Dezembro";
                    break;

            }

            return Monthstr;
        }

        /// <summary>
        /// Load´s voucher 
        /// </summary>
        /// <param name="promoType"></param>
		public async void LoadVoucher(string promoType)
		{
			
			try
			{
				var wallet = await UserCardWS.GetUserVoucher(SessionData.PharmacyUser.CardNumber, promoType);
				var getWallet = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);
				var voucherOut = getWallet.Vouchers[getWallet.Vouchers.Count - 1];
				voucher = new Voucher(voucherOut);

			}

				catch (Exception e)
			{
				if (OnError != null)
					OnError(AppResources.VouchersMessageTitle, e.Message);

			}
		}


		#endregion
	}
}