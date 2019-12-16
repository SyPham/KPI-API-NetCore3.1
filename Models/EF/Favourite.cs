using Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
    public class Favourite
    {
        public int ID { get; set; }
        public string KPILevelCode { get; set; }
        public int UserID { get; set; }
        public string Period { get; set; }
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
        //public string Like { get; set; }


    }
}
