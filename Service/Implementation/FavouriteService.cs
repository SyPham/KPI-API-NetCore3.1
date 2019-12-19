
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

namespace Service.Implementation
{
   
    public class FavouriteService : IFavouriteService
    {
        private readonly DataContext _dbContext;
        public FavouriteService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Add(Favourite entity)
        {

            try
            {
                _dbContext.Favourites.Add(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {

                return false;
            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<List<Favourite>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Favourite>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<Favourite>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.Favourites.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Period.Contains(keyword));
            }
            return await PagedList<Favourite>.CreateAsync(source, page, pageSize);
        }

        public Task<Favourite> GetById(int Id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Remove(int id)
        {
            try
            {
                var item = await _dbContext.Favourites.FirstOrDefaultAsync(x => x.ID == id);
                _dbContext.Favourites.Remove(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }
        

        public Task<bool> Update(Favourite entity)
        {
            throw new NotImplementedException();
        }
        public async Task<object> GetAllPaging(int userid, int page, int pageSize)
        {

            try
            {

                var model = (await _dbContext.Favourites
               .Where(x => x.UserID == userid).ToListAsync())
               .Select(x => new FavouriteViewModel
               {
                   KPIName = _dbContext.KPIs.FirstOrDefault(k => k.ID == _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == x.KPILevelCode).KPIID).Name,
                   Username = _dbContext.Users.FirstOrDefault(u => u.ID == x.UserID).Username,
                   TeamName = _dbContext.Levels.FirstOrDefault(l => l.ID == _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == x.KPILevelCode).LevelID).Name,
                   Level = _dbContext.KPIs.FirstOrDefault(k => k.ID == _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == x.KPILevelCode).KPIID).LevelID,
                   CreateTime = x.CreateTime,
                   KPILevelCode = x.KPILevelCode,
                   Period = x.Period,
                   ID = x.ID
               })
               .Distinct()
               .OrderByDescending(x => x.CreateTime)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToList();
                int totalRow = model.Count();
                return new
                {
                    status = true,
                    data = model,
                    total = totalRow
                };

            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }

        }
    }
}
