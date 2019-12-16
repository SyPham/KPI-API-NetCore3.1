using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Abstract
{
     interface IGeneral
    {
        int ID { get; set; }
        int UserID { get; set; }
        int CategoryID { get; set; }
        int KPILevelID { get; set; }
        string KPILevelCode { get; set; }
        string CategoryCode { get; set; }
        [Column(TypeName = "datetime")]
        DateTime CreatedTime { get; set; }
    }
}
