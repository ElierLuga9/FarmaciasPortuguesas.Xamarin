using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;

namespace ANFAPP.Logic.Network.Services
{
    public class ProductSearchWS : AnfWS
    {
        public static async Task<MedicineSearchOut> Search(string name)
        {
            // DEVELOPMENT DUMMY DATA.
			List<MedicineSearchOut.MedicineInOut> results = new List<MedicineSearchOut.MedicineInOut>()
            {
                new MedicineSearchOut.MedicineInOut() {
                    CNP = "1",
                    Name = "Xamarin Forms",
                    Dosage = "3",
                    Qty = "1",
                    Form = "Injectable"
                },
                new MedicineSearchOut.MedicineInOut() {
                    CNP = "2",
                    Name = "Xamarin iOS",
                    Dosage = "1",
                    Qty = "1",
                    Form = "Injectable",
                },
                new MedicineSearchOut.MedicineInOut() {
                    CNP = "3",
                    Name = "Xamarin Android",
                    Dosage = "1",
                    Qty = "1",
                    Form = "Injectable",
                },
                new MedicineSearchOut.MedicineInOut() {
                    CNP = "4",
                    Name = "Xamarin Window Phone",
                    Dosage = "1",
                    Qty = "1",
                    Form = "Injectable",
                },
            };

            return new MedicineSearchOut() { Drugs = results } ;
        }
    }
}
