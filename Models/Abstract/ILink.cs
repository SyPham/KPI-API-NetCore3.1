using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Abstract
{
    interface ILink
    {
        int ID { get; set; }
        string CodeTableA { get; set; }
        string CodeTableB { get; set; }
    }
}
