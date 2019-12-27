using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
   public class Menu
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string Link { get; set; }
        public string FontAwesome { get; set; }
        public string BackgroudColor { get; set; }
        public int Position { get; set; }
        public virtual ICollection<MenuLang> MenuLangs { get; set; }

    }
}
