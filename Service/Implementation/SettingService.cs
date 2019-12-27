using Models.Data;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Service.Implementation
{
  
    public class SettingService : ISettingService
    {
        private readonly DataContext _dbContext;
        public SettingService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(Setting entity)
        {

            try
            {
                _dbContext.Settings.Add(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {

                return false;
            }
        }

        public async Task<bool> Remove(int Id)
        {
            try
            {
                var setting = await GetById(Id);
                _dbContext.Remove(setting);
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


        public async Task<List<Setting>> GetAll()
        {
            return await _dbContext.Settings.ToListAsync();
        }

        public async Task<List<Setting>> GetAllById(int Id)
        {
            return await _dbContext.Settings.Where(x=> x.ID ==Id).ToListAsync();
        }

        public async Task<PagedList<Setting>> GetAllPaging(string keyword, int page, int pageSize)
        {
            return PagedList<Setting>.Create(await GetAll(), page, pageSize);
        }

        public async Task<Setting> GetById(int Id)
        {
            return await _dbContext.Settings.FirstOrDefaultAsync(x => x.ID == Id );
        }

       
        public async Task<bool> Update(Setting entity)
        {
            try
            {
                var item = await GetById(entity.ID);
                item.Name = entity.Name;
                item.State = entity.State;
                item.CreatedTime = entity.CreatedTime;
                item.Code = entity.Code;
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
        public async Task<bool> IsSendMail(string code)
        {
            try
            {
                var item = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Code.Equals(code));
                return item.State;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Setting> ShowInfo()
        {
            return await _dbContext.Settings.FirstOrDefaultAsync(x => x.State == true && x.Code == "INFO");
        }

        public async Task<Setting> Maintain()
        {
            return await _dbContext.Settings.FirstOrDefaultAsync(x => x.State == true && x.Code == "MAINTAIN");

        }
    }
}
