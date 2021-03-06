﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
   public class DatasetViewModel
    {
        public string CategoryCode { get; set; }

        public string KPIName { get; set; }
        public string KPILevelCode { get; set; }
        public int Target { get; set; }
        public string Period { get; set; }
        public string CategoryName { get; set; }
        public object Datasets { get; set; }
        public string Owner { get; set; }
        public string Manager { get; set; }
        public string Updater { get; set; }
        public string Sponsor { get; set; }
        public string Participant { get; set; }
        public object KPIObj { get; set; }
    }
}
