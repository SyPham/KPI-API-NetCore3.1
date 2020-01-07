using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Abstract
{
    public class Inheritance : IInheritance
    {
        public Inheritance()
        {
            CreateTime = DateTime.Now;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LevelID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }

        public string ParentID { get; set; }
    }
}
