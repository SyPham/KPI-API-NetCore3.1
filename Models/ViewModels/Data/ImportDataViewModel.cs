using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Data
{
    public class ImportDataViewModel
    {


        public List<UploadKPIViewModel> ListUploadKPIVMs { get; set; }
        public List<UploadKPIViewModel> ListDataSuccess { get; set; }
        public bool Status { get; set; }
        public List<string> ListSendMail { get; set; }

    }
}
