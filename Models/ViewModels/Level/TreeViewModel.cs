using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Level
{
   public class TreeViewModel
    {
        public int key { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public int levelnumber { get; set; }
        public int parentid { get; set; }
    }
}
