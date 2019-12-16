using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.ActionPlan
{
   public class CheckActionPlanViewmodel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string KPILevelCode { get; set; }
        public string Area { get; set; }
        public int UserID { get; set; }
        public DateTime Deadline { get; set; }

        public int? Weekly { get; set; }

        public DateTime Monthly { get; set; }
        public DateTime Quarterly { get; set; }
        public DateTime Yearly { get; set; }
    }
}
