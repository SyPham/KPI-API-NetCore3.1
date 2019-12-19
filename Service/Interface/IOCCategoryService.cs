using Models.Data;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IOCCategoryService: IDisposable
    {
        /// <summary>
        /// +) Kiểm tra đã tồn tại params trong bảng OCCategories, 
        /// Nếu true thì Cập nhật Status, False thì thêm mới
        /// </summary>
        /// <param name="OCID"></param>
        /// <param name="categoryID"></param>
        /// <returns>True or False</returns>
        Task<bool> AddOCCategory(int OCID, int categoryID);
        Task<object> GetCategoryByOC(int page, int pageSize, int level, int ocID);
        Task<bool> RemoveCategoryKPILevel(int categoryID, int KPILevelID);
        Task<object> GetUserByCategoryIDAndKPILevelID(int categoryID, int kpilevelID);
        Task<object> LoadDataKPILevel(int levelID, int categoryID, int page, int pageSize = 3);
        Task<bool> AddGeneral(int kpilevelID, int categoryID, string picArr, string ownerArr, string managerArr, string sponsorArr, string participantArr);
    }
 
}
