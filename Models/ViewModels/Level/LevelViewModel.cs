using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Level
{
   public class LevelViewModel
    {
        public LevelViewModel()
        {
            CreateTime = DateTime.Now;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ParentID { get; set; }
        public string ParentCode { get; set; }
        public int? LevelNumber { get; set; }
        public bool State { get; set; }
        public DateTime CreateTime { get; set; }
        public List<LevelViewModel> Levels { get; set; }
    }
}
