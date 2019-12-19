using Models.Data;
using Models.EF;
using System;
using System.Threading.Tasks;

namespace Service.Interface
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
   
}
