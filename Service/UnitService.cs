using Models;
using Models.EF;
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
   public interface IUnitService: IDisposable, ICommonService<Unit>
    {
    }
    public class UnitService : IUnitService
    {
        private readonly DataContext _dbContext;

        public UnitService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(Unit entity)
        {

            try
            {
                _dbContext.Units.Add(entity);
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
                var category = await GetById(Id);
                _dbContext.Units.Remove(category);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
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

        public async Task<List<Unit>> GetAll()
        {
            return await _dbContext.Units.ToListAsync() ;
        }

        public List<Unit> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<Unit> GetById(int Id)
        {
            return await _dbContext.Units.FindAsync(Id);
        }

        public async Task<bool> Update(Unit entity)
        {
            try
            {
                var iteam = await _dbContext.Units.FirstOrDefaultAsync(x => x.ID == entity.ID);
                iteam.Name = entity.Name;
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

        public Task<List<Unit>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        Task<PagedList<Unit>> ICommonService<Unit>.GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
