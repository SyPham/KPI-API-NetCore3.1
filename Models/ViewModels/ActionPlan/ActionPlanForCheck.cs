using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.ActionPlan
{
   public class ActionPlanForCheck
    {

        public int ActionplanID { get; set; }
        public DateTime Deadline { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public int KPILevelID { get; set; }
        public bool Sent { get; set; }
    }
}
