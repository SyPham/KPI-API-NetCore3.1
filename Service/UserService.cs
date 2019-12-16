using Models;
using Models.EF;
using Microsoft.EntityFrameworkCore;
using Service;
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
    public interface IUserService: IDisposable,ICommonService<User>
    {
        Task<bool> AddUserToLevel(int id, int levelid);
        Task<object> LoadDataUser(int levelid, string code, int page, int pageSize);
        Task<bool> LockUser(int id);
        Task<bool> ChangePassword(string username, string newpass);
        object GetAllMenusByPermissionID(int id);
        Task<bool> Checkpermisson(int userid);
        Task<object> GetListAllPermissions(int userid);
    }
    public class UserService : IUserService
    {
        private readonly DataContext _dbContext;
        public UserService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Checkpermisson(int userid)
        {
            var model = await (_dbContext.Permissions.Join(
                _dbContext.Users,
               p => p.ID,
               u => u.Permission,
               (p, u) => new
               {
                   UserID = u.ID,
                   PermissionID = p.ID,

               })).Where(x => x.UserID == userid).FirstOrDefaultAsync();

            return model != null ? true : false;
        }
        public async Task<object> GetListAllPermissions(int userid)
        {
            var model = await _dbContext.Permissions.Select(x => new
            {
                x.ID,
                x.PermissionName,
                State = _dbContext.Users.FirstOrDefault(a => a.ID == userid && a.Permission == x.ID) != null ? true : false
            }).ToListAsync();
            return model;
        }
        public object GetAllMenusByPermissionID(int id)
        {
            return _dbContext.Menus.Where(x => x.Permission == id).Select(x => new
            {
                x.ID,
                x.Link,
                x.Name,
                x.Permission,
                State = _dbContext.Resources.FirstOrDefault(a => a.Menu == x.ID) != null ? true : false
            }).ToList();
        }
        public async Task<bool> ChangePassword(string username, string newpass)
        {
            var item = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            try
            {
                var pass = newpass.ToSafetyString().SHA256Hash();
                item.Password = pass;
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
        public async Task<bool> LockUser(int id)
        {
            var item = await _dbContext.Users.FirstOrDefaultAsync(x => x.ID == id);

            item.IsActive = !item.IsActive;
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
        public async Task<bool> AddUserToLevel(int id, int levelid)
        {
            var itemUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.ID == id);
            if (itemUser != null)
            {
                if (itemUser.LevelID == levelid)
                {
                    itemUser.LevelID = 0;
                }
                else
                {
                    itemUser.LevelID = levelid;
                }
            }


            try
            {
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
        public async Task<bool> Add(User user)
        {
            _dbContext.Add(user);
            try
            {
             await   _dbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception)
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
        public async Task<bool> Remove(int ID)
        {
            var user =await GetById(ID);
            _dbContext.Remove(user);
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

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.Where(x=>x.State == true).ToListAsync();
        }
        public async Task<object> LoadDataUser(int levelid, string code, int page, int pageSize)
        {
            var model = await _dbContext.Users.Where(x => x.State == true).Select(x => new
            {
                x.ID,
                x.Username,
                x.LevelID,
                x.Role,
                x.TeamID,
                FullName = x.Alias,
                Status = x.LevelID == levelid ? true : false
            }).ToListAsync();
            if (!string.IsNullOrEmpty(code))
            {
                model = model.Where(a => a.Username.Contains(code)).ToList();
            }
            int totalRow = model.Count();

            model = model.OrderBy(x => x.LevelID)
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
        public Task<User> GetById(int ID)
        {
            return  _dbContext.Users.FirstOrDefaultAsync(x=>x.ID == ID);
        }

        public async Task<bool> Update(User user)
        {
            var item = GetById(user.ID);
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

     
        public Task<List<User>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }


        public Task<PagedList<User>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
