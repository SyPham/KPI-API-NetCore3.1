using Models.Data;
using Models.EF;
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

    public class CategoryService : ICategoryService
    {
        private readonly DataContext _dbContext;

        public CategoryService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Add(Category entity)
        {
            entity.Code = entity.Code.ToUpper();

            try
            {
                _dbContext.Categories.Add(entity);
                await _dbContext.SaveChangesAsync();
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

        public async Task<List<Category>> GetAll()
        {
            return await _dbContext.Categories.ToListAsync();
        }


        public async Task<List<Category>> GetAllById(int Id)
        {
            return await _dbContext.Categories.Where(x => x.ID == Id).ToListAsync();
        }

        public async Task<PagedList<Category>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.Categories.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<Category>.CreateAsync(source, page, pageSize);
        }

        public async Task<Category> GetById(int Id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.ID == Id);
        }

        public async Task<bool> Remove(int Id)
        {
            try
            {
                var category = await GetById(Id);
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update(Category entity)
        {
            entity.Code = entity.Code.ToUpper();
            try
            {
                var iteam = await _dbContext.Categories.FirstOrDefaultAsync(x => x.ID == entity.ID);
                iteam.Name = entity.Name;
                iteam.Code = entity.Code;
                iteam.LevelID = entity.LevelID;
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

        public async Task<object> GetAllByCategory(int page, int pageSize, int level, int ocID)
        {
            var ocCategories = _dbContext.OCCategories;
            var model = await _dbContext.Categories
                .Select(x => new
                {
                    x.Name,
                    x.ID,
                    x.LevelID,
                    x.CreateTime,
                    Total = _dbContext.CategoryKPILevels.Join(_dbContext.KPILevels,
                                cat => cat.KPILevelID,
                                kpil => kpil.ID,
                                (cat, kpil) => new { cat.CategoryID, cat.Status, kpil.Checked }
                            ).Where(a => a.CategoryID == x.ID && a.Status == true && a.Checked == true).Count(),
                    Status = ocCategories.FirstOrDefault(a => a.CategoryID == x.ID && a.OCID == ocID) == null ? false : ocCategories.FirstOrDefault(a => a.CategoryID == x.ID && a.OCID == ocID).Status
                }).Where(x => x.Status == true && x.Total > 0).ToListAsync();

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
                }).ToListAsync();

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
                pageSize,
                totalPage = (int)Math.Ceiling((double)totalRow / pageSize)
            };
        }
    }
}
