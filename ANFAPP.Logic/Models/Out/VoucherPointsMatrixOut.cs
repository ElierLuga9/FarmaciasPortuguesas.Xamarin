using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class VoucherPointsMatrixOut
    {

        #region Properties

        public List<VoucherPointsMatrix> Value { get; set; }

        #endregion

        #region Inner Class

        public class VoucherPointsMatrix
        {
            public int Points { get; set; }
            public decimal Value { get; set; }
            public string Reference { get; set; }
			public string Description { get; set; }
            public string Type { get; set; }

            [JsonIgnore]
            public bool IsAvailable 
            {    
                get
                {
                    return SessionData.IsLogged != null && Points <= SessionData.PharmacyUser.Points;
                }
            }
        }

        #endregion 

    }
}
