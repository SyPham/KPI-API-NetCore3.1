using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
   public class ActionPlan
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int DataID { get; set; }
        public int CommentID { get; set; }

        [Column("Title")]
        public string Title { get; set; }
        public string KPILevelCodeAndPeriod { get; set; }
        public string KPILevelCode { get; set; }
        public string Description { get; set; }
        [Column("Tag")]
        public string Tag { get; set; }
        [Column("TagID")]
        public int TagID { get; set; }
        public int ApprovedBy { get; set; }
       
        public string Link { get; set; }
        [Column(TypeName = "datetime")]
        private DateTime? createTime = null;
        public DateTime CreateTime
        {
            get
            {
                return this.createTime.HasValue
                   ? this.createTime.Value
                   : DateTime.Now;
            }

            set { this.createTime = value; }
        }
        [Column("Deadline", TypeName = "datetime")]
        public DateTime Deadline { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime SubmitDate { get; set; }

        public bool Status { get; set; }
        public bool ApprovedStatus { get; set; }
        public int Auditor { get; set; }
        [Column(TypeName = "datetime")]

        public DateTime? UpdateSheduleDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualFinishDate { get; set; }
    }
}
