using Models.ViewModels.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.User
{
   public class UserProfileViewModel
    {
        public EF.User User { get; set; }
        public List<MenuViewModel> Menus { get; set; }
    }
}
