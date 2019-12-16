using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class DataCompareViewModel
    {
        public ChartViewModel list1 { get; set; }
        public ChartViewModel list2 { get; set; }
        public ChartViewModel list3 { get; set; }
        public ChartViewModel list4 { get; set; }
        public int Standard { get; set; }
        public string Unit { get; set; }
        public string Period { get; set; }
    }
}
