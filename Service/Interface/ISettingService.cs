using Models.Data;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface ISettingService : IDisposable,ICommonService<Setting>
    {
        Task<bool> IsSendMail(string code);

        Task<Setting> ShowInfo();
        Task<Setting> Maintain();

    }

}
