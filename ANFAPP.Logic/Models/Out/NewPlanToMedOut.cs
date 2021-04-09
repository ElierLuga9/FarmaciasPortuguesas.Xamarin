using System.Collections.Generic;

namespace ANFAPP.Logic.Models.Out
{
    public class TomaOut
    {
        public int TomaID { get; set; }
        public double Qtd { get; set; }
        public string DataHora { get; set; }
    }

    public class NewPlanToMedOut : WSTomasOut
    {
        public int PlanID { get; set; }
        public List<TomaOut> Tomas { get; set; }
    }
}