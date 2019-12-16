using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
   public class ListKPIUploadViewModel
    {
        public int KPILevelID { get; set; }
        public string KPIName { get; set; }
        public bool Checked { get; set; }
        public bool WeeklyChecked { get; set; }
        public bool MonthlyChecked { get; set; }
        public bool QuarterlyChecked { get; set; }
        public bool YearlyChecked { get; set; }
        public int UserID { get; set; }

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
