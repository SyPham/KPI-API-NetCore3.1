using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
   public class ErrorMessage
    {
        public ErrorMessage()
        {

            this.CreateTime = DateTime.Now;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }
        public string Function { get; set; }
    }
}
