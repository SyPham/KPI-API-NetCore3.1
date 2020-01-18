using Models.Data;
using Models.EF;
using Models.ViewModels.KPILevel;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
  
    public class KPILevelService : IKPILevelService
    {
        private readonly DataContext _dbContext;

        public KPILevelService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<bool> Add(KPILevel entity)
        {
            throw new NotImplementedException();
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
        /// <summary>
        /// Lấy ra danh sách tất cả các KPILevel
        /// </summary>
        /// <param name="levelID"></param>
        /// <param name="categoryID"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Danh sách KPI theo điều kiện</returns>
        public async Task<object> LoadData(int levelID, int categoryID, int page, int pageSize = 3)
        {
            var model = await (from kpiLevel in _dbContext.KPILevels
                               where kpiLevel.LevelID == levelID
                               join kpi in _dbContext.KPIs on kpiLevel.KPIID equals kpi.ID
                               join unit in _dbContext.Units on kpi.Unit equals unit.ID
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

                                   TimeCheck = kpiLevel.TimeCheck,

                                   CreateTime = kpiLevel.CreateTime,
                                   Unit = unit.Name,
                                   CategoryID = kpi.CategoryID,
                                   KPIName = kpi.Name,
                                   LevelCode = level.Code,
                               }).ToListAsync();
            if (categoryID != 0)
            {
                model = model.Where(x => x.CategoryID == categoryID).ToList();
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

        public Task<List<KPILevel>> GetAll()
        {
            throw new NotImplementedException();
        }
        public async Task<object> GetAll(int page, int pageSize)
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
        public Task<List<KPILevel>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }
        public async Task<object> GetDetail(int ID)
        {
            var item = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.ID == ID);
            var codeW = item.KPILevelCode + (item.WeeklyChecked ?? false == true ? "W" : "");
            var codeM = item.KPILevelCode + (item.MonthlyChecked ?? false == true ? "M" : "");
            var codeQ = item.KPILevelCode + (item.QuarterlyChecked ?? false == true ? "Q" : "");
            var codeY = item.KPILevelCode + (item.YearlyChecked ?? false == true ? "Y" : "");

            string WorkingPlanOfWeekly = (await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == codeW))?.Content ?? "Not avaiable!";
            string WorkingPlanOfMonthly = (await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == codeM))?.Content ?? "Not avaiable!";
            string WorkingPlanOfQuarterly = (await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == codeQ))?.Content ?? "Not avaiable!";
            string WorkingPlanOfYearly = (await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == codeY))?.Content ?? "Not avaiable!";

            return new
            {
                status = true,
                data = item,
                WorkingPlanOfWeekly,
                WorkingPlanOfMonthly,
                WorkingPlanOfQuarterly,
                WorkingPlanOfYearly,
            };
        }
        public Task<PagedList<KPILevel>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<KPILevel> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public int GetByUsername(string username)
        {
            try
            {

                return _dbContext.Users.FirstOrDefault(x => x.Username == username).ID;

            }
            catch (Exception)
            {

                return 0;

            }
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> AddWorkingPlan(WorkingPlan entity)
        {
            var item = await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == entity.Code);
            if (item != null)
            {
                item.Content = entity.Content;
            }
            else
            {
                _dbContext.WorkingPlans.Add(entity);

            }
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update(KPILevelForUpdate entity)
        {
            var kpiLevel = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.ID == entity.ID);
            if(kpiLevel==null)
                return false;

            var kpiModel = await _dbContext.KPIs.FirstOrDefaultAsync(x => x.ID == kpiLevel.KPIID);
            var ocModel = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == kpiLevel.LevelID);
            if (!entity.Target.IsNullOrEmpty())
            {
                string code = ocModel.LevelNumber + ocModel.Code + kpiModel.Code + entity.Period;
                var status = await AddWorkingPlan(new WorkingPlan { Code = code, Content = entity.Target });
            }
            if (entity.Weekly != null)
            {
                kpiLevel.Weekly = entity.Weekly;
            }
            if (!entity.Monthly.IsNullOrEmpty())
            {
                kpiLevel.Monthly = Convert.ToDateTime(entity.Monthly);
            }
            if (!entity.Quarterly.IsNullOrEmpty())
            {
                kpiLevel.Quarterly = Convert.ToDateTime(entity.Quarterly);
            }
            if (!entity.Yearly.IsNullOrEmpty())
            {
                kpiLevel.Yearly = Convert.ToDateTime(entity.Yearly);
            }
            if (entity.WeeklyChecked != null)
            {
                kpiLevel.WeeklyChecked = entity.WeeklyChecked;
            }
            if (entity.MonthlyChecked != null)
            {
                kpiLevel.MonthlyChecked = entity.MonthlyChecked;
            }
            if (entity.QuarterlyChecked != null)
            {
                kpiLevel.QuarterlyChecked = entity.QuarterlyChecked;
            }
            if (entity.MonthlyChecked != null)
            {
                kpiLevel.MonthlyChecked = entity.MonthlyChecked;
            }
            if (entity.YearlyChecked != null)
            {
                kpiLevel.YearlyChecked = entity.YearlyChecked;
            }
            if (entity.WeeklyPublic != null)
            {
                kpiLevel.WeeklyPublic = entity.WeeklyPublic;
            }
            if (entity.MonthlyPublic != null)
            {
                kpiLevel.MonthlyPublic = entity.MonthlyPublic;
            }
            if (entity.QuarterlyPublic != null)
            {
                kpiLevel.QuarterlyPublic = entity.QuarterlyPublic;
            }
            if (entity.YearlyPublic != null)
            {
                kpiLevel.YearlyPublic = entity.YearlyPublic;
            }
            if (entity.Checked != null)
            {
                kpiLevel.Checked = entity.Checked;
                kpiLevel.KPILevelCode = ocModel.LevelNumber + ocModel.Code + kpiModel.Code;
            }

            try
            {
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //logging
                return false;
            }

        }
        public async Task<bool> UpdateKPILevel(KPILevelForUpdate entity)
        {
            var kpiLevel = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.ID == entity.ID);
            var kpiModel = await _dbContext.KPIs.FirstOrDefaultAsync(x => x.ID == kpiLevel.KPIID);
            var ocModel = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == kpiLevel.LevelID);
            if (entity.Weekly != null)
            {
                kpiLevel.Weekly = entity.Weekly;
            }
            if (!entity.Monthly.IsNullOrEmpty())
            {
                kpiLevel.Monthly = Convert.ToDateTime(entity.Monthly);
            }
            if (!entity.Quarterly.IsNullOrEmpty())
            {
                kpiLevel.Quarterly = Convert.ToDateTime(entity.Quarterly);
            }
            if (!entity.Yearly.IsNullOrEmpty())
            {
                kpiLevel.Yearly = Convert.ToDateTime(entity.Yearly);
            }
            if (entity.WeeklyChecked != null)
            {
                kpiLevel.WeeklyChecked = entity.WeeklyChecked;
            }
            if (entity.MonthlyChecked != null)
            {
                kpiLevel.MonthlyChecked = entity.MonthlyChecked;
            }
            if (entity.QuarterlyChecked != null)
            {
                kpiLevel.QuarterlyChecked = entity.QuarterlyChecked;
            }
            if (entity.MonthlyChecked != null)
            {
                kpiLevel.MonthlyChecked = entity.MonthlyChecked;
            }
            if (entity.YearlyChecked != null)
            {
                kpiLevel.YearlyChecked = entity.YearlyChecked;
            }
            if (entity.WeeklyPublic != null)
            {
                kpiLevel.WeeklyPublic = entity.WeeklyPublic;
            }
            if (entity.MonthlyPublic != null)
            {
                kpiLevel.MonthlyPublic = entity.MonthlyPublic;
            }
            if (entity.QuarterlyPublic != null)
            {
                kpiLevel.QuarterlyPublic = entity.QuarterlyPublic;
            }
            if (entity.YearlyPublic != null)
            {
                kpiLevel.YearlyPublic = entity.YearlyPublic;
            }

            if (entity.Checked != null)
            {
                kpiLevel.Checked = entity.Checked;
                kpiLevel.KPILevelCode = ocModel.LevelNumber + ocModel.Code + kpiModel.Code;
            }

            if (kpiLevel.WeeklyChecked == false
                && kpiLevel.MonthlyChecked == false
                && kpiLevel.QuarterlyChecked == false
                && kpiLevel.YearlyChecked == false)
            {
                kpiLevel.Checked = false;
                kpiLevel.KPILevelCode = ocModel.LevelNumber + ocModel.Code + kpiModel.Code;
            }
            else
            {
                kpiLevel.Checked = true;
                kpiLevel.KPILevelCode = ocModel.LevelNumber + ocModel.Code + kpiModel.Code;
            }
            try
            {
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //logging
                return false;
            }

        }
        public async Task<object> GetAllPaging(int levelID, int categoryID, int page, int pageSize = 3)
        {
            var model = await (from kpiLevel in _dbContext.KPILevels
                               where kpiLevel.LevelID == levelID
                               join kpi in _dbContext.KPIs on kpiLevel.KPIID equals kpi.ID
                               join unit in _dbContext.Units on kpi.Unit equals unit.ID
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

                                   TimeCheck = kpiLevel.TimeCheck,

                                   CreateTime = kpiLevel.CreateTime,
                                   Unit = unit.Name,
                                   CategoryID = kpi.CategoryID,
                                   KPIName = kpi.Name,
                                   LevelCode = level.Code,
                               }).ToListAsync();
            if (categoryID != 0)
            {
                model = model.Where(x => x.CategoryID == categoryID).ToList();
            }

            //int totalRow = model.Count();
            var data = PagedList<KPILevelViewModel>.Create(model, page, pageSize);

            //model = model.OrderByDescending(x => x.CreateTime)
            //  .Skip((page - 1) * pageSize)
            //  .Take(pageSize).ToList();

            return new
            {
                data = data,
                CurrentPage = data.CurrentPage,
                PageSize = data.PageSize,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount,



            };
        }

        public Task<bool> Update(KPILevel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<object> LoadDataForUser(int levelID, int categoryID, int page, int pageSize = 3)
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
                var catKPILevel = _dbContext.CategoryKPILevels;
                var model = await (from kpiLevel in _dbContext.KPILevels
                                   where kpiLevel.LevelID == levelID && kpiLevel.Checked == true
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
                                       StatusUploadDataW = weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week) > 1 ? false : ((weekofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "W").Max(x => x.Week)) == 1 ? (kpiLevel.Weekly < currentweekday ? true : false) : false),

                                       StatusUploadDataM = monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) > 1 ? false : monthofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "M").Max(x => x.Month) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Monthly.Value) < 0 ? true : false) : false,

                                       StatusUploadDataQ = quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) > 1 ? false : quarterofyear - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Q").Max(x => x.Quarter) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Quarterly.Value) < 0 ? true : false) : false, //true dung han flase tre han

                                       StatusUploadDataY = year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) > 1 ? false : year - datas.Where(a => a.KPILevelCode == kpiLevel.KPILevelCode && a.Period == "Y").Max(x => x.Year) == 1 ? (DateTime.Compare(currentdate, kpiLevel.Yearly.Value) < 0 ? true : false) : false,

                                       CheckCategory = catKPILevel.FirstOrDefault(x => x.KPILevelID == kpiLevel.ID && x.CategoryID == categoryID) == null ? false : catKPILevel.FirstOrDefault(x => x.Status == true && x.KPILevelID == kpiLevel.ID && x.CategoryID == categoryID).Status == true ? true : false

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
        /// <summary>
        /// Lấy ra danh sách để so sánh chart với nhau.
        /// </summary>
        /// <param name="obj">Chuỗi dữ liệu gồm KPIlevelcode, Period của các KPILevel</param>
        /// <returns>Trả về danh sách so sánh dữ liệu cùng cấp. So sánh tối đa 4 KPILevel</returns>
        public async Task<object> LoadDataProvide(string obj, int page, int pageSize)
        {
            var listCompare = new List<CompareViewModel>();
            var value = obj.ToSafetyString().Split(',');
            var kpilevelcode = value[0].ToSafetyString();
            var period = value[1].ToSafetyString();

            var itemkpilevel = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == kpilevelcode);
            var itemlevel = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == itemkpilevel.LevelID);
            var levelNumber = itemlevel.LevelNumber;
            var kpiid = itemkpilevel.KPIID;
            //Lay ra tat ca kpiLevel cung levelNumber

            if (period == "W")
            {

                listCompare = await _dbContext.KPILevels.Where(x => x.KPIID == kpiid && x.WeeklyChecked == true && !x.KPILevelCode.Contains(kpilevelcode))
                    .Join(_dbContext.Levels,
                    x => x.LevelID,
                    a => a.ID,
                    (x, a) => new CompareViewModel
                    {
                        KPILevelCode = x.KPILevelCode + ",W",
                        LevelNumber = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).LevelNumber,
                        Area = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).Name,
                        Status = _dbContext.Datas.FirstOrDefault(henry => henry.KPILevelCode == x.KPILevelCode) == null ? false : true,
                        StatusPublic = (bool?)x.WeeklyPublic ?? false
                    }).
                    Where(c => c.LevelNumber == levelNumber).ToListAsync();

                int totalRow = listCompare.Count();
                listCompare = listCompare.OrderByDescending(x => x.LevelNumber)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new
                {
                    total = totalRow,
                    listCompare
                };
            }

            if (period == "M")
            {
                listCompare = await _dbContext.KPILevels.Where(x => x.KPIID == kpiid && x.MonthlyChecked == true && !x.KPILevelCode.Contains(kpilevelcode))
                    .Join(_dbContext.Levels,
                    x => x.LevelID,
                    a => a.ID,
                    (x, a) => new CompareViewModel
                    {
                        KPILevelCode = x.KPILevelCode + ",W",
                        LevelNumber = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).LevelNumber,
                        Area = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).Name,
                        Status = _dbContext.Datas.FirstOrDefault(henry => henry.KPILevelCode == x.KPILevelCode) == null ? false : true,
                        StatusPublic = (bool?)x.MonthlyPublic ?? false
                    }).
                    Where(c => c.LevelNumber == levelNumber)
                    .ToListAsync();

                int totalRow = listCompare.Count();
                listCompare = listCompare.OrderByDescending(x => x.LevelNumber)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new
                {
                    total = totalRow,
                    listCompare
                };
            }

            if (period == "Q")
            {
                listCompare = await _dbContext.KPILevels.Where(x => x.KPIID == kpiid && x.QuarterlyChecked == true && !x.KPILevelCode.Contains(kpilevelcode))
                    .Join(_dbContext.Levels,
                    x => x.LevelID,
                    a => a.ID,
                    (x, a) => new CompareViewModel
                    {
                        KPILevelCode = x.KPILevelCode + ",W",
                        LevelNumber = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).LevelNumber,
                        Area = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).Name,
                        Status = _dbContext.Datas.FirstOrDefault(henry => henry.KPILevelCode == x.KPILevelCode) == null ? false : true,
                        StatusPublic = (bool?)x.QuarterlyPublic ?? false
                    }).
                    Where(c => c.LevelNumber == levelNumber)
                    .ToListAsync();

                int totalRow = listCompare.Count();
                listCompare = listCompare.OrderByDescending(x => x.LevelNumber)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new
                {
                    total = totalRow,
                    listCompare
                };
            }

            if (period == "Y")
            {
                listCompare = await _dbContext.KPILevels.Where(x => x.KPIID == kpiid && x.YearlyChecked == true && !x.KPILevelCode.Contains(kpilevelcode))
                    .Join(_dbContext.Levels,
                    x => x.LevelID,
                    a => a.ID,
                    (x, a) => new CompareViewModel
                    {
                        KPILevelCode = x.KPILevelCode + ",W",
                        LevelNumber = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).LevelNumber,
                        Area = _dbContext.Levels.FirstOrDefault(l => l.ID == x.LevelID).Name,
                        Status = _dbContext.Datas.FirstOrDefault(henry => henry.KPILevelCode == x.KPILevelCode) == null ? false : true,
                        StatusPublic = (bool?)x.YearlyPublic ?? false
                    }).
                    Where(c => c.LevelNumber == levelNumber)
                    .ToListAsync();

                int totalRow = listCompare.Count();
                listCompare = listCompare.OrderByDescending(x => x.LevelNumber)
                    .Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new
                {
                    total = totalRow,
                    listCompare
                };
            }
            //Lay tat ca kpilevel cung period

            return new
            {
                listCompare
            };
        }
        public async Task<PagedList<KPILevel>> GetAllPagingByLevelIdAndCategoryId(int levelId, int catId, int page, int pageSize)
        {
            var source = _dbContext.KPILevels.AsQueryable();
            if (levelId > 0 || catId > 0)
            {
                source = source.Where(x => x.LevelID.Equals(levelId));
            }
            return await PagedList<KPILevel>.CreateAsync(source, page, pageSize);
        }

        public async Task<object> GetItemInListOfWorkingPlan(string code, string period)
        {
            var kpilevelcode = code + period;
            if (kpilevelcode.IsNullOrEmpty())
            {
                return new
                {
                    status = false,
                    message = "Error!",
                    data = new WorkingPlan { Content = "Not avaiable!" }
                };
            }
            return new
            {
                status = true,
                message = "Successfully!",
                data = await _dbContext.WorkingPlans.FirstOrDefaultAsync(x => x.Code == kpilevelcode)
            };

        }
    }
}
