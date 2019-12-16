using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.KPILevel
{
   public class CompareViewModel
    {
        public int? LevelNumber { get; set; }
        public string Area { get; set; }
        public bool Status { get; set; }
        public string KPILevelCode { get; set; }
        public bool StatusPublic { get; set; }
    }
}
