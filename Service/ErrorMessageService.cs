using Models;
using Models.EF;
using Service.helpers;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
   public interface IErrorMessageService:ICommonService<ErrorMessage>,IDisposable
    {
    }
    public class ErrorMessageService : IErrorMessageService
    {
        private readonly DataContext _dbContext;

        public ErrorMessageService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(ErrorMessage entity)
        {
            _dbContext.Add(entity);
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

        public Task<List<ErrorMessage>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<ErrorMessage>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<ErrorMessage>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.ErrorMessages.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<ErrorMessage>.CreateAsync(source, page, pageSize);
        }

        public Task<ErrorMessage> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(ErrorMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
