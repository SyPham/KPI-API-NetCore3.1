using Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class UserProfileDto
    {
        public UserForReturnLogin User { get; set; }
        public string Menus { get; set; }
    }
}
