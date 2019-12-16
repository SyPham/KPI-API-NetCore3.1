using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class KPIUploadViewModel
    {
        public string KPIName { get; set; }
        public string Area { get; set; }
        public bool StateW { get; set; }
        public bool StateM { get; set; }
        public bool StateQ { get; set; }
        public bool StateY { get; set; }


        public bool StateDataW { get; set; }
        public bool StateDataM { get; set; }
        public bool StateDataQ { get; set; }
        public bool StateDataY { get; set; }
    }
}
