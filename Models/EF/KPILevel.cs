using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Abstract;
namespace Models.EF
{
    public class KPILevel
    {
        public KPILevel()
        {
            CreateTime = DateTime.Now;
        }

        public int ID { get; set; }
        public string KPILevelCode { get; set; }
        public string UserCheck { get; set; }
        public int KPIID { get; set; }
        public int LevelID { get; set; }
        public int? TeamID { get; set; }
        public string Period { get; set; }
        public int? Weekly { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Monthly { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Quarterly { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Yearly { get; set; }
        public bool? Checked { get; set; }
        public bool? WeeklyChecked { get; set; }
        public bool? MonthlyChecked { get; set; }
        public bool? QuarterlyChecked { get; set; }
        public bool? YearlyChecked { get; set; }
        public bool? CheckedPeriod { get; set; }
        public bool? WeeklyPublic { get; set; }
        public bool? MonthlyPublic { get; set; }
        public bool? QuarterlyPublic { get; set; }
        public bool? YearlyPublic { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TimeCheck { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }

        public int LevelNumber { get; set; }
        public int WeeklyStandard { get; set; }
        public int MonthlyStandard { get; set; }
        public int QuarterlyStandard { get; set; }
        public int YearlyStandard { get; set; }
        public int WeeklyTarget { get; set; }
        public int MonthlyTarget { get; set; }
        public int QuarterlyTarget { get; set; }
        public int YearlyTarget { get; set; }
        public int PIC { get; set; }
        public int Owner { get; set; }
        public int OwnerManagerment { get; set; }
    }
}
