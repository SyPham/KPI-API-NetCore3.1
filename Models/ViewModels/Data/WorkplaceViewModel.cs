using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
   public class WorkplaceViewModel
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public List<KPIUploadViewModel> KPIUpLoads { get; set; }
    }
 
}
