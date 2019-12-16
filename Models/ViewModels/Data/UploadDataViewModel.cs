using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
   public class UploadDataViewModel
    {

        public string KPILevelCode { get; set; }
        public string KPIName { get; set; }
        public string Value { get; set; }
        public int PeriodValue { get; set; }
        public string TargetValue { get; set; }
        public int Year { get; set; }
        public string Area { get; set; }
        public object UpdateTime { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
