using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class FamilyMemberOut
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public bool IsFamilyHead { get; set; }

        [JsonIgnore]
        public int Order { get; set; }

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (IsFamilyHead)
                {
                    return Name + " " + AppResources.FamilyAccountMasterLabel;
                }
                else
                {
                    return Name;
                }
            }
        }
    }
}
