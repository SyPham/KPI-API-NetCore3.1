using Models;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
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
    public class OCCategoryService : IOCCategoryService
    {
        private readonly DataContext _dbContext;
        private readonly IKPILevelService _kPILlevelService;

        public OCCategoryService(DataContext dbContext,IKPILevelService KPILlevelService)
        {
            this._dbContext = dbContext;
            _kPILlevelService = KPILlevelService;
        }

        public async Task<bool> AddGeneral(int kpilevelID, int categoryID, string picArr, string ownerArr, string managerArr, string sponsorArr, string participantArr)
        {
            if (!picArr.IsNullOrEmpty())
            {
                foreach (var item in picArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var uploader = _kPILlevelService.GetByUsername(item.Trim());
                        if (uploader != 0)
                        {
                            //xoa het xong add moi
                            var listUploader = await _dbContext.Uploaders.Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID).ToListAsync();
                            _dbContext.Uploaders.RemoveRange(listUploader);
                            await _dbContext.SaveChangesAsync();
                            _dbContext.Uploaders.Add(new Uploader { UserID = uploader, KPILevelID = kpilevelID, CategoryID = categoryID });
                        }


                    }

                }
            }
            if (!ownerArr.IsNullOrEmpty())
            {
                foreach (var item in ownerArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var owner = _kPILlevelService.GetByUsername(item.Trim());
                        if (owner != 0)
                        {
                            var listUploader = await _dbContext.Owners.Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID).ToListAsync();
                            _dbContext.Owners.RemoveRange(listUploader);
                            await _dbContext.SaveChangesAsync();

                            _dbContext.Owners.Add(new Owner { UserID = owner, KPILevelID = kpilevelID, CategoryID = categoryID });
                        }
                    }
                }
            }
            if (!sponsorArr.IsNullOrEmpty())
            {
                foreach (var item in sponsorArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var sponsor = _kPILlevelService.GetByUsername(item.Trim());

                        if (sponsor != 0)
                        {
                            var listUploader = await _dbContext.Sponsors.Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID).ToListAsync();
                            _dbContext.Sponsors.RemoveRange(listUploader);
                            await _dbContext.SaveChangesAsync();

                            _dbContext.Sponsors.Add(new Sponsor { UserID = sponsor, KPILevelID = kpilevelID, CategoryID = categoryID });
                        }
                    }
                }
            }
            if (!participantArr.IsNullOrEmpty())
            {
                foreach (var item in participantArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var participant = _kPILlevelService.GetByUsername(item.Trim());
                        if (participant != 0)
                        {
                            var listUploader = await _dbContext.Participants.Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID).ToListAsync();
                            _dbContext.Participants.RemoveRange(listUploader);
                            await _dbContext.SaveChangesAsync();

                            _dbContext.Participants.Add(new Participant { UserID = participant, KPILevelID = kpilevelID, CategoryID = categoryID });
                        }
                    }
                }
            }
            if (!managerArr.IsNullOrEmpty())
            {
                foreach (var item in managerArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var manager = _kPILlevelService.GetByUsername(item.Trim());
                        if (manager != 0)
                        {
                            var listUploader = await _dbContext.Managers.Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID).ToListAsync();
                            _dbContext.Managers.RemoveRange(listUploader);
                            await _dbContext.SaveChangesAsync();

                            _dbContext.Managers.Add(new Manager { UserID = manager, KPILevelID = kpilevelID, CategoryID = categoryID });
                        }
                    }
                }
            }
            try
            {
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;

            }
        }
        public async Task<bool> AddOCCategory(int OCID, int categoryID)
        {
            try
            {
                var item = await _dbContext.OCCategories.FirstOrDefaultAsync(x => x.OCID == OCID && x.CategoryID == categoryID);
                if (item == null)
                {
                    var oc = new OCCategory();
                    oc.OCID = OCID;
                    oc.CategoryID = categoryID;
                    oc.Status = true;
                    _dbContext.OCCategories.Add(oc);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    item.Status = !item.Status;
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public Task<object> GetCategoryByOC(int page, int pageSize, int level, int ocID)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUserByCategoryIDAndKPILevelID(int categoryID, int kpilevelID)
        {
            throw new NotImplementedException();
        }

        public Task<object> LoadDataKPILevel(int levelID, int categoryID, int page, int pageSize = 3)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCategoryKPILevel(int categoryID, int KPILevelID)
        {
            throw new NotImplementedException();
        }
    }
}
