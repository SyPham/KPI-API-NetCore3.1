using Models.Data;
using Models.EF;
using Models.Helpers;
using Models.ViewModels.Level;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface ILevelService : IDisposable, ICommonService<Level>
    {
        Task<List<TreeView>> GetListTree();
        Task<bool> AddOrUpdate(Level entity);
        Task<TreeView> GetListTreeForWorkplace(int userid);
        object GetListTreeClient(int userid);
        Task<bool> IsExistsCode(string code);
        Task<bool> Rename(TreeViewModel level);
        string GetNode(string code);
        string GetNode(int id);
    }
   
}
