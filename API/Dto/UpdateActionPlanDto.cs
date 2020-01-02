using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class UpdateActionPlanDto
    {
      
        public string pk { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public int userid { get; set; }
    }
}
