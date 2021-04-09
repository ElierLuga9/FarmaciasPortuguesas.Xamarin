using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class AssociationRequestOut
    {
        public DateTime Date { get; set; }
        public DateTime ResponseDate { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }

        public FamilyMemberOut Requester { get; set; }

        [JsonIgnore]
        public int Order { get; set; }
    }
}
