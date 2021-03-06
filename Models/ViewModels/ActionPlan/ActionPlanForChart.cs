﻿using Models.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.ActionPlan
{
   public class ActionPlanForChart
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string Deadline { get; set; }
        public bool ApprovedStatus { get; set; }
        public bool Status { get; set; }
        public bool IsBoss { get; set; }
        public int CreatedBy { get; set; }
        public string UpdateSheduleDate { get; set; }
        public string ActualFinishDate { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public List<int> ListUserIDs { get; set; }
        public int Auditor { get; set; }
        public List<int> ListAuditorIDs { get; set; }
        
        public string CreatedByName { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
