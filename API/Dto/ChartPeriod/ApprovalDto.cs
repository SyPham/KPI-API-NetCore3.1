using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto.ChartPeriod
{
    public class ApprovalDto
    {
        public int id { get; set; }
        public int approveby { get; set; }
        public string KPILevelCode { get; set; }
        public int CategoryID { get; set; }

    }
}
