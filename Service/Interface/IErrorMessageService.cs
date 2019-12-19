using Models.Data;
using Models.EF;

using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IErrorMessageService:ICommonService<ErrorMessage>,IDisposable
    {
    }
    
}
