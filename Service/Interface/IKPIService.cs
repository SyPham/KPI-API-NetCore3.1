using Models.Data;
using Models.EF;
using Models.ViewModels.Comment;
using Models.ViewModels.KPI;
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
   public interface IKPIService: IDisposable, ICommonService<KPI>
    {
        Task<bool> AddKPILevel(KPILevel entity);
        int Total();
        List<Category> GetCategoryCode();
        Task<bool> Delete(int id);
        Task<KPI> GetbyId(int ID);
        Task<object> GetAllPaging(int? categoryID, string name, int page, int pageSize = 3);
        Task<object> Autocomplete(string search);
        Task<object> GetAllAjax(string kpilevelcode, string period);
        Task<object> ListComments(int dataid, int userid);
    }
     
}
