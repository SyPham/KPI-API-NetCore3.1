using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class ManagerOwnerUpdaterSponsorParticipantViewModel
    {
        public int KPILevelID { get; set; }
        public string KPILevelCode { get; set; }
        public int CategoryID { get; set; }
        public string KPIName { get; set; }
        public string Owner { get; set; }
        public string Manager { get; set; }
        public string Updater { get; set; }
        public string Sponsor { get; set; }
        public string Participant { get; set; }
    }
}
