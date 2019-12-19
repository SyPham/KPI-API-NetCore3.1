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

namespace Service.Implementation
{
    
    public class CategoryKPILevelService : ICategoryKPILevelService
    {
        private readonly DataContext _dbContext;
        private readonly IKPILevelService _KPILevelService;
        public CategoryKPILevelService(DataContext dbContext, IKPILevelService KPILevelService)
        {
            _dbContext = dbContext;
            _KPILevelService = KPILevelService;
        }
        public async Task<bool> Add(CategoryKPILevel entity)
        {
            var item = await _dbContext.CategoryKPILevels.FirstOrDefaultAsync(x => x.KPILevelID == entity.KPILevelID && x.CategoryID == entity.CategoryID);

            if (item == null)
            {
                entity.Status = true;
                _dbContext.CategoryKPILevels.Add(entity);
            }
            else
            {
                item.Status = !item.Status;
            }

            try
            {
                _dbContext.SaveChanges();
                return true;
            }
            catch
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
        public async Task<List<CategoryKPILevel>> GetAll()
        {
            return await _dbContext.CategoryKPILevels.ToListAsync();

        }
        public async Task<object> GetAllKPIlevels(int page, int pageSize)
        {
            var model = await _dbContext.KPILevels
               .Where(x => x.UserCheck == "1")
               .Join(_dbContext.KPIs,
               kpilevel => kpilevel.KPIID,
               kpi => kpi.ID,
               (kpilevel, kpi) => new
               {
                   ID = kpilevel.ID,
                   Name = kpi.Name,
                   kpilevel.KPILevelCode,
                   CreateTime = kpilevel.CreateTime,
               })
               .ToListAsync();
            int totalRow = model.Count();
            model = model.OrderByDescending(x => x.CreateTime)

              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();
            return new
            {
                data = model,
                total = totalRow,
                status = true,
                page,
                pageSize
            };
        }

        public Task<List<CategoryKPILevel>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Helpers.PagedList<CategoryKPILevel>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryKPILevel> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(CategoryKPILevel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddGeneral(int kpilevelID, int categoryID, string picArr, string ownerArr, string managerArr, string sponsorArr, string participantArr)
        {
            if (!picArr.IsNullOrEmpty())
            {
                foreach (var item in picArr.Split(','))
                {
                    if (!item.IsNullOrEmpty())
                    {
                        var uploader = _KPILevelService.GetByUsername(item.Trim());
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
                        var owner = _KPILevelService.GetByUsername(item.Trim());
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
                        var sponsor = _KPILevelService.GetByUsername(item.Trim());

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
                        var participant = _KPILevelService.GetByUsername(item.Trim());
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
                        var manager = _KPILevelService.GetByUsername(item.Trim());
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

        public async Task<object> GetCategoryByOC(int page, int pageSize, int level, int ocID)
        {
            var ocCategories = _dbContext.OCCategories;
            var model = await _dbContext.Categories
                .Select(x => new
                {
                    x.Name,
                    x.ID,
                    x.LevelID,
                    x.CreateTime,
                    Status = ocCategories.FirstOrDefault(a => a.CategoryID == x.ID && a.OCID == ocID) == null ? false : ocCategories.FirstOrDefault(a => a.CategoryID == x.ID && a.OCID == ocID).Status
                }).Where(x => x.Status == true).ToListAsync();

            if (level > 0)
            {
                model = model.Where(x => x.LevelID == level).ToList();
            }
            int totalRow = model.Count();

            model = model.OrderByDescending(x => x.CreateTime)

              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();
            return new
            {
                data = model,
                total = totalRow,
                status = true,
                page,
                pageSize
            };
        }

        public async Task<object> GetAll(int page, int pageSize, int level, int OCID)
        {
            var ocCategories = _dbContext.OCCategories;
            var model = await _dbContext.Categories
                .Select(x => new
                {
                    x.Name,
                    x.ID,
                    x.LevelID,//LevelNumber
                    Total = _dbContext.CategoryKPILevels.Where(a => a.CategoryID == x.ID && a.Status == true).Count(),
                    x.CreateTime,
                    Status = ocCategories.FirstOrDefault(a => a.CategoryID == x.ID) != null ? ocCategories.FirstOrDefault(a => a.CategoryID == x.ID).Status : false
                }).Where(x => x.Status == true).ToListAsync();
            int totalRow = model.Count();
            if (level > 0)
            {
                model = model.Where(x => x.LevelID == level).ToList();
            }
            model = model.OrderByDescending(x => x.CreateTime)

              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();
            return new
            {
                data = model,
                total = totalRow,
                status = true,
                page,
                pageSize
            };
        }

        public async Task<bool> RemoveCategoryKPILevel(int categoryID, int KPILevelID)
        {
            try
            {
                var itemCateKPILevel = await _dbContext.CategoryKPILevels.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemCateKPILevel != null)
                {
                    _dbContext.CategoryKPILevels.Remove(itemCateKPILevel);
                    await _dbContext.SaveChangesAsync();
                }
                var itemManager = await _dbContext.Managers.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemManager != null)
                {
                    _dbContext.Managers.Remove(itemManager);
                    await _dbContext.SaveChangesAsync();
                }
                var itemOwner = await _dbContext.Owners.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemOwner != null)
                {
                    _dbContext.Owners.Remove(itemOwner);
                    await _dbContext.SaveChangesAsync();
                }
                var itemUpdater = await _dbContext.Uploaders.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemUpdater != null)
                {
                    _dbContext.Uploaders.Remove(itemUpdater);
                    await _dbContext.SaveChangesAsync();
                }
                var itemSponsor = await _dbContext.Sponsors.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemSponsor != null)
                {
                    _dbContext.Sponsors.Remove(itemSponsor);
                    await _dbContext.SaveChangesAsync();
                }
                var itemPaticipant = await _dbContext.Participants.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.KPILevelID == KPILevelID);
                if (itemPaticipant != null)
                {
                    await _dbContext.SaveChangesAsync();
                    _dbContext.Participants.Remove(itemPaticipant);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<object> GetUserByCategoryIDAndKPILevelID(int categoryID, int kpilevelID)
        {
            var manager = await _dbContext.Managers
                       .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                           cat => cat.UserID,
                           user => user.ID,
                           (cat, user) => new
                           {
                               user.Username
                           }).Select(x => "@" + x.Username).ToArrayAsync();
            var owner = await _dbContext.Owners
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.Username
                        }).Select(x => "@" + x.Username).ToArrayAsync();
            var updater = await _dbContext.Uploaders
                         .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.Username
                        }).Select(x => "@" + x.Username).ToArrayAsync();
            var sponsor = await _dbContext.Sponsors
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                       .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.Username
                        }).Select(x => "@" + x.Username).ToArrayAsync();
            var participant = await _dbContext.Participants
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                       .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.Username
                        }).Select(x => "@" + x.Username).ToArrayAsync();

            return new
            {
                Owner = string.Join(" ", owner),
                Manager = string.Join(" ", manager),
                Updater = string.Join(" ", updater),
                Sponsor = string.Join(" ", sponsor),
                Participant = string.Join(" ", participant),
            };
        }

        public async Task<object> LoadDataKPILevel(int levelID, int categoryID, int page, int pageSize = 3)
        {
            //Lấy các tuần tháng quý năm hiện tại
            var weekofyear = DateTime.Now.GetIso8601WeekOfYear();
            var monthofyear = DateTime.Now.Month;
            var quarterofyear = DateTime.Now.GetQuarterOfYear();
            var year = DateTime.Now.Year;
            var currentweekday = DateTime.Now.DayOfWeek.ToSafetyString().ToUpper().ConvertStringDayOfWeekToNumber();
            var currentdate = DateTime.Now.Date;
            var dt = new DateTime(2019, 8, 1);
            var value = DateTime.Compare(currentdate, dt);
            try
            {
                //Lấy ra danh sách data từ trong db
                var datas = _dbContext.Datas;
                var categoryKPILevels = _dbContext.CategoryKPILevels;
                var model = await (//from cat in _dbContext.CategoryKPILevels.Where(x => x.Status == true && x.CategoryID == categoryID)
                                   from kpiLevel in _dbContext.KPILevels.Where(x => x.LevelID == levelID && x.Checked == true)
                                   join kpi in _dbContext.KPIs on kpiLevel.KPIID equals kpi.ID
                                   join level in _dbContext.Levels on kpiLevel.LevelID equals level.ID

                                   select new KPILevelViewModel
                                   {
                                       ID = kpiLevel.ID,
                                       KPILevelCode = kpiLevel.KPILevelCode,
                                       UserCheck = kpiLevel.KPILevelCode,
                                       KPIID = kpiLevel.KPIID,
                                       KPICode = kpi.Code,
                                       LevelID = kpiLevel.LevelID,
                                       LevelNumber = kpi.LevelID,
                                       Period = kpiLevel.Period,

                                       Weekly = kpiLevel.Weekly,
                                       Monthly = kpiLevel.Monthly,
                                       Quarterly = kpiLevel.Quarterly,
                                       Yearly = kpiLevel.Yearly,

                                       Checked = kpiLevel.Checked,

                                       WeeklyChecked = kpiLevel.WeeklyChecked,
                                       MonthlyChecked = kpiLevel.MonthlyChecked,
                                       QuarterlyChecked = kpiLevel.QuarterlyChecked,
                                       YearlyChecked = kpiLevel.YearlyChecked,
                                       CheckedPeriod = kpiLevel.CheckedPeriod,

                                       //true co du lieu false khong co du lieu
                                       StatusEmptyDataW = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.WeeklyChecked == true ? "W" : "")) != null ? true : false,
                                       StatusEmptyDataM = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.MonthlyChecked == true ? "M" : "")) != null ? true : false,
                                       StatusEmptyDataQ = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.QuarterlyChecked == true ? "Q" : "")) != null ? true : false,
                                       StatusEmptyDataY = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.YearlyChecked == true ? "Y" : "")) != null ? true : false,

                                       TimeCheck = kpiLevel.TimeCheck,
                                       CreateTime = kpiLevel.CreateTime,

                                       //CategoryID = kpi.CategoryID,
                                       KPIName = kpi.Name,
                                       LevelCode = level.Code,
                                       //Nếu tuần hiện tại - tuần MAX trong bảng DATA > 1 return false,
                                       //ngược lại nếu == 1 thi kiểm tra thứ trong bảng KPILevel,
                                       //Nếu thứ nhỏ hơn thứ hiện tại thì return true,
                                       //ngược lại reutrn false
                                       //StatusUploadDataW = weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week) > 1 ? false : ((weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week)) == 1 ? (kpiLevel.Weekly < currentweekday ? true : false) : false),

                                       //StatusUploadDataM = monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) > 1 ? false : monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Monthly.Value) < 0 ? true : false) : false,

                                       //StatusUploadDataQ = quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) > 1 ? false : quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Quarterly.Value) < 0 ? true : false) : false, //true dung han flase tre han

                                       //StatusUploadDataY = year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) > 1 ? false : year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Yearly.Value) < 0 ? true : false) : false,

                                       StatusUploadDataW = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.WeeklyChecked == true ? "W" : "")) == null ? false : kpiLevel.Weekly == null ? false : kpiLevel.Weekly < currentweekday ? true : false,
                                       StatusUploadDataM = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.MonthlyChecked == true ? "M" : "")) == null ? false : kpiLevel.Monthly == null ? false : DateTime.Compare(currentdate, kpiLevel.Monthly.Value) < 0 ? true : false,
                                       StatusUploadDataQ = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.QuarterlyChecked == true ? "Q" : "")) == null ? false : kpiLevel.Quarterly == null ? false : DateTime.Compare(currentdate, kpiLevel.Quarterly.Value) < 0 ? true : false,
                                       StatusUploadDataY = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.YearlyChecked == true ? "Y" : "")) == null ? false : kpiLevel.Yearly == null ? false : DateTime.Compare(currentdate, kpiLevel.Yearly.Value) < 0 ? true : false,
                                       CheckCategory = categoryKPILevels.FirstOrDefault(a => a.CategoryID == categoryID && a.KPILevelID == kpiLevel.ID) != null ? categoryKPILevels.FirstOrDefault(a => a.CategoryID == categoryID && a.KPILevelID == kpiLevel.ID).Status : false
                                       //CheckCategory = cat.Status

                                   }).ToListAsync();



                //if (categoryID != 0)
                //{
                //    model = model.Where(x => x.CategoryID == categoryID);
                //}

                int totalRow = model.Count();

                model = model.OrderByDescending(x => x.CreateTime)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize).ToList();


                return new
                {
                    data = model,
                    total = totalRow,
                    status = true,
                    page,
                    pageSize
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return "";
            }

        }

        public async Task<object> LoadKPILevel(int categoryID, int page, int pageSize = 3)
        {
            //Lấy các tuần tháng quý năm hiện tại
            var weekofyear = DateTime.Now.GetIso8601WeekOfYear();
            var monthofyear = DateTime.Now.Month;
            var quarterofyear = DateTime.Now.GetQuarterOfYear();
            var year = DateTime.Now.Year;
            var currentweekday = DateTime.Now.DayOfWeek.ToSafetyString().ToUpper().ConvertStringDayOfWeekToNumber();
            var currentdate = DateTime.Now.Date;
            var dt = new DateTime(2019, 8, 1);
            var value = DateTime.Compare(currentdate, dt);
            try
            {
                var categoryKPILevels = _dbContext.CategoryKPILevels;
                //Lấy ra danh sách data từ trong db
                var datas = _dbContext.Datas;
                var model = await (from cat in _dbContext.CategoryKPILevels.Where(x => x.Status == true && x.CategoryID == categoryID)
                                   join kpiLevel in _dbContext.KPILevels on cat.KPILevelID equals kpiLevel.ID
                                   join kpi in _dbContext.KPIs on kpiLevel.KPIID equals kpi.ID

                                   select new KPILevelViewModel
                                   {
                                       ID = cat.ID,
                                       KPILevelCode = kpiLevel.KPILevelCode,
                                       UserCheck = kpiLevel.KPILevelCode,
                                       KPIID = kpiLevel.KPIID,
                                       KPICode = kpi.Code,
                                       LevelID = kpiLevel.LevelID,
                                       LevelNumber = kpi.LevelID,
                                       Period = kpiLevel.Period,

                                       Weekly = kpiLevel.Weekly,
                                       Monthly = kpiLevel.Monthly,
                                       Quarterly = kpiLevel.Quarterly,
                                       Yearly = kpiLevel.Yearly,

                                       Checked = kpiLevel.Checked,

                                       WeeklyChecked = kpiLevel.WeeklyChecked,
                                       MonthlyChecked = kpiLevel.MonthlyChecked,
                                       QuarterlyChecked = kpiLevel.QuarterlyChecked,
                                       YearlyChecked = kpiLevel.YearlyChecked,
                                       CheckedPeriod = kpiLevel.CheckedPeriod,

                                       //true co du lieu false khong co du lieu
                                       StatusEmptyDataW = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.WeeklyChecked == true ? "W" : "")) != null ? true : false,
                                       StatusEmptyDataM = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.MonthlyChecked == true ? "M" : "")) != null ? true : false,
                                       StatusEmptyDataQ = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.QuarterlyChecked == true ? "Q" : "")) != null ? true : false,
                                       StatusEmptyDataY = datas.FirstOrDefault(x => x.KPILevelCode == kpiLevel.KPILevelCode && x.Period == (kpiLevel.YearlyChecked == true ? "Y" : "")) != null ? true : false,

                                       TimeCheck = kpiLevel.TimeCheck,
                                       CreateTime = kpiLevel.CreateTime,

                                       CategoryID = kpi.CategoryID,
                                       KPIName = kpi.Name,
                                       //Nếu tuần hiện tại - tuần MAX trong bảng DATA > 1 return false,
                                       //ngược lại nếu == 1 thi kiểm tra thứ trong bảng KPILevel,
                                       //Nếu thứ nhỏ hơn thứ hiện tại thì return true,
                                       //ngược lại reutrn false
                                       StatusUploadDataW = weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week) > 1 ? false : ((weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week)) == 1 ? (kpiLevel.Weekly < currentweekday ? true : false) : false),

                                       StatusUploadDataM = monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) > 1 ? false : monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Monthly.Value) < 0 ? true : false) : false,

                                       StatusUploadDataQ = quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) > 1 ? false : quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Quarterly.Value) < 0 ? true : false) : false, //true dung han flase tre han

                                       StatusUploadDataY = year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) > 1 ? false : year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Yearly.Value) < 0 ? true : false) : false,
                                       //CheckCategory = categoryKPILevels.FirstOrDefault(a => a.CategoryID == categoryID && a.KPILevelID == kpiLevel.ID) != null ? categoryKPILevels.FirstOrDefault(a => a.CategoryID == categoryID && a.KPILevelID == kpiLevel.ID).Status : false
                                       CheckCategory = cat.Status
                                   }).ToListAsync();



                int totalRow = model.Count();

                model = model.Where(x => x.CheckCategory == true).OrderByDescending(x => x.CreateTime)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize).ToList();


                return new
                {
                    data = model,
                    total = totalRow,
                    status = true
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return new
                {
                    status = false,
                };
            }
        }
    }
}
