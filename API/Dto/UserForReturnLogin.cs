using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class UserForReturnLogin
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Alias { get; set; }
        public int Role { get; set; }
    }
}
