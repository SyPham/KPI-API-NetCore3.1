using Models.Data;
using Models.EF;
using Models.ViewModels.KPILevel;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Helpers;

namespace Service.Interface
{
    public interface ICategoryKPILevelService : ICommonService<CategoryKPILevel>, IDisposable
    {
        Task<object> LoadKPILevel(int categoryID, int page, int pageSize = 3);
        Task<object> LoadDataKPILevel(int levelID, int categoryID, int page, int pageSize = 3);
        Task<object> GetUserByCategoryIDAndKPILevelID(int categoryID, int kpilevelID);
        Task<bool> RemoveCategoryKPILevel(int categoryID, int KPILevelID);
        Task<object> GetAll(int page, int pageSize, int level, int OCID);
        Task<object> GetCategoryByOC(int page, int pageSize, int level, int ocID);
        Task<object> GetAllKPIlevels(int page, int pageSize);
        Task<bool> AddGeneral(int kpilevelID, int categoryID, string picArr, string ownerArr, string managerArr, string sponsorArr, string participantArr);
    }
    
}
