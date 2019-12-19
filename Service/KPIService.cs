using Models;
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

namespace Service
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
    public class KPIService : IKPIService
    {
        private readonly DataContext _dbContext;
        public KPIService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(KPI entity)
        {
            try
            {
                for (int i = 1; i < 10000; i++)
                {
                    string code = i.ToString("D4");
                    if (await _dbContext.KPIs.FirstOrDefaultAsync(x => x.Code == code) == null)
                    {
                        entity.Code = code;
                        break;
                    }
                }

                _dbContext.KPIs.Add(entity);
                await _dbContext.SaveChangesAsync();

                List<KPILevel> kpiLevelList = new List<KPILevel>();
                var levels = _dbContext.Levels.ToList();

                foreach (var level in levels)
                {
                    var kpilevel = new KPILevel();
                    kpilevel.LevelID = level.ID;
                    kpilevel.KPIID = entity.ID;
                    kpiLevelList.Add(kpilevel);
                }

                _dbContext.KPILevels.AddRange(kpiLevelList);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> AddKPILevel(KPILevel entity)
        {
            _dbContext.KPILevels.Add(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public int Total()
        {
            return _dbContext.KPIs.ToList().Count();
        }
        public async Task<bool> Update(KPI entity)
        {
            entity.Code = entity.Code.ToSafetyString().ToUpper();
            try
            {
                var iteam = await _dbContext.KPIs.FirstOrDefaultAsync(x => x.ID == entity.ID);
                iteam.Name = entity.Name;
                //iteam.Code = entity.Code;
                iteam.LevelID = entity.LevelID;
                iteam.CategoryID = entity.CategoryID;
                iteam.Unit = entity.Unit;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //logging
                return false;
            }

        }
        public List<Category> GetCategoryCode()
        {
            return _dbContext.Categories.ToList();
        }
        public async Task<bool> Delete(int id)
        {

            try
            {
                var kpi = await _dbContext.KPIs.FindAsync(id);
                _dbContext.KPIs.Remove(kpi);

                var kpiLevel = await _dbContext.KPILevels.Where(x => x.KPIID == id).ToListAsync();
                _dbContext.KPILevels.RemoveRange(kpiLevel);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }

        }
        public async Task<object> GetAllAjax()
        {
            return await _dbContext.KPIs.Select(x => new {
                x.ID,
                x.Code,
                x.Name,
                x.LevelID,
                CategoryName = _dbContext.Categories.FirstOrDefault(a => a.ID == x.CategoryID),
                Unit = _dbContext.Units.FirstOrDefault(u => u.ID == x.Unit)

            }).ToListAsync();
        }
        public async Task<KPI> GetbyId(int ID)
        {
            return await _dbContext.KPIs.FirstOrDefaultAsync(x => x.ID == ID);
        }
        public async Task<object> ListCategory()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<object> GetAllPaging(int? categoryID, string name, int page, int pageSize = 3)
        {
            categoryID = categoryID.ToInt();
            name = name.ToSafetyString();
            var model = await _dbContext.KPIs.Select(
                x => new KPIViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Code = x.Code,
                    LevelID = x.LevelID,
                    CategoryID = x.CategoryID,
                    CategoryName = _dbContext.Categories.FirstOrDefault(c => c.ID == x.CategoryID).Name,
                    Unit = _dbContext.Units.FirstOrDefault(u => u.ID == x.Unit).Name,
                    CreateTime = x.CreateTime
                }
                ).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                model = model.Where(x => x.Name.Contains(name)).ToList();
            }

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

        public async Task<object> Autocomplete(string search)
        {
            if (search != "")
                return await _dbContext.KPIs.Where(x => x.Name.Contains(search)).Select(x => x.Name).Take(5).ToListAsync();
            else
                return "";
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

        public async Task<bool> Remove(int Id)
        {

            try
            {
                var kpi = await _dbContext.KPIs.FindAsync(Id);
                _dbContext.KPIs.Remove(kpi);

                var kpiLevel = await _dbContext.KPILevels.Where(x => x.KPIID == Id).ToListAsync();
                _dbContext.KPILevels.RemoveRange(kpiLevel);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }

        }

        public Task<KPI> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<KPI>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<KPI>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.KPIs.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<KPI>.CreateAsync(source, page, pageSize);
        }
        public async Task<object> GetAll(string kpilevelcode, string period)
        {
            if (!string.IsNullOrEmpty(kpilevelcode) && !string.IsNullOrEmpty(period))
            {
                //label chartjs
                var item = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == kpilevelcode);
                var modelLevel = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == item.LevelID);
                var label = modelLevel.Name;
                //datasets chartjs
                var model = await _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode).ToListAsync();

                if (period == "W".ToUpper())
                {

                    var datasets = model.Where(x => x.Period == "W").OrderBy(x => x.Week).Select(x => x.Value).ToArray();

                    //data: labels chartjs
                    var labels = model.Where(x => x.Period == "W").OrderBy(x => x.Week).Select(x => x.Week).ToArray();


                    return new
                    {
                        datasets,
                        labels,
                        label
                    };
                }
                else if (period == "M".ToUpper())
                {

                    var datasets = model.Where(x => x.Period == "M").OrderBy(x => x.Month).Select(x => x.Value).ToArray();

                    //data: labels chartjs
                    var labels = model.Where(x => x.Period == "M").OrderBy(x => x.Month).Select(x => x.Month).ToArray();
                    return new
                    {
                        datasets,
                        labels,
                        label
                    };
                }
                else if (period == "Q".ToUpper())
                {
                    var datasets = model.Where(x => x.Period == "Q").OrderBy(x => x.Quarter).Select(x => x.Value).ToArray();

                    //data: labels chartjs
                    var labels = model.Where(x => x.Period == "Q").OrderBy(x => x.Quarter).Select(x => x.Quarter).ToArray();
                    return new
                    {
                        datasets,
                        labels,
                        label
                    };
                }
                else if (period == "Y".ToUpper())
                {

                    var datasets = model.Where(x => x.Period == "Y").OrderBy(x => x.Year).Select(x => x.Value).ToArray();

                    //data: labels chartjs
                    var labels = model.Where(x => x.Period == "Y").OrderBy(x => x.Year).Select(x => x.Year).ToArray();
                    return new
                    {
                        datasets,
                        labels,
                        label
                    };
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public async Task<object> ListComments(int dataid, int userid)
        {

            var actionPlan = _dbContext.ActionPlans;
            //Cat chuoi
            //lay tat ca comment cua kpi
            var listcmts = await _dbContext.Comments.Where(x => x.DataID == dataid).ToListAsync();

            //Tong tat ca cac comment cua kpi
            var totalcomment = listcmts.Count();

            //Lay ra tat ca lich su cmt
            var seenCMT = _dbContext.SeenComments;

            //Lay ra tat ca lich su cmt
            var user = _dbContext.Users;

            //Lay ra tat ca cac comment cua kpi(userid nao post comment len thi mac dinh userid do da xem comment cua chinh minh roi)
            var data = await _dbContext.Comments.Where(x => x.DataID == dataid)
               .Select(x => new CommentViewModel
               {
                   CommentID = x.ID,
                   UserID = x.UserID,
                   CommentMsg = x.CommentMsg,
                   //KPILevelCode = x.KPILevelCode,
                   CommentedDate = x.CommentedDate,
                   FullName = user.FirstOrDefault(a => a.ID == x.UserID).FullName,
                   //Period = x.Period,
                   Read = seenCMT.FirstOrDefault(a => a.CommentID == x.ID && a.UserID == userid) == null ? true : false,
                   IsHasTask = actionPlan.FirstOrDefault(a => a.DataID == dataid && a.CommentID == x.ID) == null ? false : true,
                   Task = actionPlan.FirstOrDefault(a => a.DataID == dataid && a.CommentID == x.ID) == null ? false : true
               })
               .OrderByDescending(x => x.CommentedDate)
               .ToListAsync();

            return new
            {
                data,
                total = _dbContext.Comments.Where(x => x.DataID == dataid).Count()
            };

        }

        public Task<object> GetAllAjax(string kpilevelcode, string period)
        {
            throw new NotImplementedException();
        }

        public async Task<List<KPI>> GetAll()
        {
            return await _dbContext.KPIs.ToListAsync();
        }
    }
}
