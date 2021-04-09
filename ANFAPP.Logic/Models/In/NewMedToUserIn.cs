
namespace ANFAPP.Logic.Models.In
{
    class NewMedToUserIn
    {
        public string UserID { get; set; }
        public string CNP { get; set; }
        public string NomeMed { get; set; }
        public string AvisoMedicamento { get; set; }
        public string FormaFarmaceutica { get; set; }
        public string Nota { get; set; }
        public string Dosagem { get; set; }
        public string Embalagem { get; set; }
		public string PrincipioAtivo { get; set; }
    }
}
