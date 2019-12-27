using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.EF
{
  public class MenuRole
    {
        [Key, Column(Order = 1)]
        public int MenuID { get; set; }
        [Key, Column(Order = 2)]
        public int RoleID { get; set; }
        public int Index { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual Role Role { get; set; }

    }
}
