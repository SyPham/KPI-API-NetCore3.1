using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Menu
{
   public class MenuViewModel
    {
        public string Code { get; set; }
        public string Link { get; set; }
        public string FontAwesome { get; set; }
        public string BackgroudColor { get; set; }
        public int RoleID { get; set; }
        public int Index { get; set; }
    }
}
