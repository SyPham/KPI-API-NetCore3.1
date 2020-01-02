using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto.Category
{
    public class LoadDataDto
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string name { get; set; }
    }
}
