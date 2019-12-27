using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Models.ViewModels.Menu;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<User> Login(string username, string password);
        Task<User> Register(User user, string password);
        Task<User> FindByNameAsync(string username);
        Task<User> GetById(int Id);
        Task<Role> GetRolesAsync(int role);
        Task<List<MenuViewModel>> GetMenusAsync(int role);

        Task<User> Edit(string username);
    }

}
