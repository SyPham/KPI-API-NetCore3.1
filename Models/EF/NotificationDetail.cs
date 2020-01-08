using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
    public class NotificationDetail
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int NotificationID { get; set; }
        public bool Seen { get; set; }
        public string Content { get; set; }
        public string URL { get; set; }
        public string Action { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }

        public NotificationDetail()
        {
            CreateTime = DateTime.Now;
        }

       
    }
}
