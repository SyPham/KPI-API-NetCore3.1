
using Models.Data;
using Models.EF;
using Models.ViewModels.Favourite;
using Microsoft.EntityFrameworkCore;

using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IFavouriteService: IDisposable,ICommonService<Favourite>
    {
        Task<object> GetAllPaging(int userid, int page, int pageSize);
    }
     
}
