using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ANFAPP.Logic.Models.Out
{

    public class AuthorizationOut
    {

        /// <summary>
        /// Authorization Object
        /// </summary>
        public AuthorizationModel Authorization { get; set; }
    
        /// <summary>
        /// Inner Class
        /// </summary>
        public class AuthorizationModel
        {
            public int ExpiresIn { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string UserName { get; set; }
        }
    }

}
