using Models;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
   public interface ISettingService : IDisposable,ICommonService<Setting>
    {
        Task<bool> IsSendMail(string code);
    }
    public class SettingService : ISettingService
    {
        private readonly DataContext _dbContext;
        public SettingService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> Add(Setting entity)
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


        public Task<List<Setting>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Setting>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<Setting>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Setting> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Setting entity)
        {
            throw new NotImplementedException();
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
    }
}
