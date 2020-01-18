using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.EF
{
    [Table("WorkingPlans")]
    public class WorkingPlan
    {
        public WorkingPlan()
        {
            CreatedTime = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
