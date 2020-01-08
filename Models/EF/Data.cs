using Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
    public class Data
    {
        public int ID { get; set; }
        public string KPILevelCode { get; set; }
        public string Period { get; set; }
        public string Value { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
        public int Yearly { get; set; }
        public string DateUpload { get; set; }
        public string Remark { get; set; }
        public string Target { get; set; }

        public Data()
        {
            CreateTime = DateTime.Now;
        }

        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }

    }
}
