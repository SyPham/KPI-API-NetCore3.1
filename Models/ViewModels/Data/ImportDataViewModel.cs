using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class ImportDataViewModel
    {


        public List<UploadKPIViewModel> ListDatasBelowTarget { get; set; }
        public List<UploadKPIViewModel> ListDatasSuccess { get; set; }
        public bool Status { get; set; }
        public List<string> ListSendMail { get; set; }

    }
}
