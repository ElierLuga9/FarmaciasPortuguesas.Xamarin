
namespace ANFAPP.Logic.Models.In
{
    class NewPlanToMedIn
    {
		public string UserID { get; set; }
        public string MedID { get; set; }
        public string Nota { get; set; }
        public string NomeUser { get; set; }
        public string DataInicio { get; set; }
        public int IntervaloQtd { get; set; }
        public int IntervaloEscala { get; set; }
        public int DuracaoQtd { get; set; }
        public int DuracaoEscala { get; set; }
        public double Qtd { get; set; }
    }
}