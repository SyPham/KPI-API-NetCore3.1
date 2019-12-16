using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class UploadKPIViewModel
    {
        public string KPILevelCode { get; set; }
        public string Area { get; set; }
        public string KPIName { get; set; }
        public string Email { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
        public bool Status { get; set; }

    }
}
