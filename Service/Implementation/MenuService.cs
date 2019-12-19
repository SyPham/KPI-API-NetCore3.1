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
    
    public class MenuService : IMenuService
    {
        private readonly DataContext _dbContext;

        public MenuService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(Menu entity)
        {

            try
            {
                _dbContext.Menus.Add(entity);
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
                _dbContext.Menus.Remove(category);
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

        public async Task<List<Menu>> GetAll()
        {
            return await _dbContext.Menus.ToListAsync() ;
        }

        //public List<Menu> GetAllPaging(string keyword, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Menu> GetById(int Id)
        {
            return await _dbContext.Menus.FindAsync(Id);
        }

        public async Task<bool> Update(Menu entity)
        {
            try
            {
                var item = await _dbContext.Menus.FirstOrDefaultAsync(x => x.ID == entity.ID);
                item.Name = entity.Name;
                item.Link = entity.Link;
                item.Permission = entity.Permission;
                item.Position = entity.Position;
                item.BackgroudColor = entity.BackgroudColor;
                item.FontAwesome = entity.FontAwesome;
                item.MenuLangs = entity.MenuLangs;
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

        public async Task<List<Menu>> GetAllById(int Id)
        {
            return await _dbContext.Menus.Where(x=>x.ID == Id).ToListAsync();

        }

        public async Task<PagedList<Menu>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.Menus.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<Menu>.CreateAsync(source, page, pageSize);
        }

        public async Task<List<Permission>> GetPermissions()
        {
            return await _dbContext.Permissions.ToListAsync();
        }
    }
}
