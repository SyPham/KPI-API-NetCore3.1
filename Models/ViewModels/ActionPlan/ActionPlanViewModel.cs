using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.ActionPlan
{
   public class ActionPlanViewModel
    {
        public int ID { get; set; }
        public string OC { get; set; }
        public string KPIName { get; set; }
        public string TaskName { get; set; }
        public string URL { get; set; }

        public string Description { get; set; }
        public string PIC { get; set; }
        public string Remark { get; set; }

        public string DueDate { get; set; }
        public string UpdateSheduleDate { get; set; }
        public string ActualFinishDate { get; set; }
        public bool Status { get; set; }
        public bool Approved { get; set; }
        public string SubmitDate { get; set; }
        public int UserTag { get; set; }
    }
}
