using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Abstract
{
    public class General : IGeneral
    {
        public General()
        {
            this.CreatedTime = DateTime.Now;
        }
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int KPILevelID { get; set; }
        public string KPILevelCode { get; set; }
        public string CategoryCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
        public int UserID { get ; set; }
    }
}
