using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
   public class StateSendMail
    {
        public StateSendMail()
        {
            ToDay = DateTime.Today;
            Status = true;
            Description = "IsSended";
        }

        public int ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ToDay { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
    }
}
