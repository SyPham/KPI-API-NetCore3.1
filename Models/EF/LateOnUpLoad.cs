using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.EF
{
   public class LateOnUpLoad
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int NotificationID { get; set; }
        public string Area { get; set; }
        public string DeadLine { get; set; }
        public string KPIName { get; set; }
        public string Code { get; set; }
        public string Year { get; set; }
    }
}
