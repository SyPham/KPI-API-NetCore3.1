using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto.CategoryKPILevelAdmin
{
    public class AddGeneralDto
    {
       public int kpilevel { get; set; }
        public int category { get; set; }
        public string pic { get; set; }
        public string owner { get; set; }
        public string manager { get; set; }
        public string sponsor { get; set; }
        public string participant { get; set; }
    }
}
