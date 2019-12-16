using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Abstract
{
   interface IInheritance
    {
        int ID { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        [Column(TypeName = "datetime")]
        DateTime CreateTime { get; set; }
        string ParentID { get; set; }
    }
}
