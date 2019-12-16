using Models;
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

namespace Service
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
    public class LevelService : ILevelService
    {
        private readonly DataContext _dbContext;

        public LevelService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
      
        public async Task<bool> Add(Level level)
        {

            List<KPILevel> kpiLevelList = new List<KPILevel>();
            if (level.LevelNumber == 0)
                return false;
            try
            {
                level.Code = await CheckLevelNumberAndGenarateCode(level.LevelNumber ?? 0);
                _dbContext.Levels.Add(level);
                await _dbContext.SaveChangesAsync();

                var kpiVM = await _dbContext.KPIs.ToListAsync();
                kpiVM.ForEach(x =>
                {
                    var kpilevel = new KPILevel();
                    kpilevel.LevelID = level.ID;
                    kpilevel.KPIID = x.ID;
                    kpiLevelList.Add(kpilevel);
                });
                _dbContext.KPILevels.AddRange(kpiLevelList);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> Update(Level entity)
        {
            try
            {
                var iteam = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == entity.ID);
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
        public async Task<bool> Remove(int Id)
        {
            var item = await _dbContext.Levels.FindAsync(Id);
            List<int> lsoc = new List<int>();
            try
            {
                _dbContext.Levels.RemoveRange(GetItemFromLevel(item.ID));
                _dbContext.Levels.Remove(item);

                foreach (var oc in GetItemFromLevel(item.ID))
                {
                    lsoc.Add(oc.ID);
                }
                var listkpilevel2 = await _dbContext.KPILevels.Where(x => x.LevelID == item.ID).ToListAsync();
                var listkpilevel = await _dbContext.KPILevels.Where(x => lsoc.Contains(x.LevelID)).ToListAsync();
                _dbContext.KPILevels.RemoveRange(listkpilevel2);
                _dbContext.KPILevels.RemoveRange(listkpilevel);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;

            }
        }
        public async Task<List<Level>> GetAll()
        {
            return await _dbContext.Levels.ToListAsync();
        }
        public Task<List<Level>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }
        public async Task<Level> GetById(int Id)
        {
            return await _dbContext.Levels.FindAsync(Id);
        }
        public async Task<bool> Rename(TreeViewModel level)
        {
            var item = await _dbContext.Levels.FindAsync(level.key);
            item.Name = level.title;
            item.Code = level.code;
            var kpilevels = await _dbContext.KPILevels.Where(x => x.KPILevelCode.Contains(item.Code)).ToListAsync();
            kpilevels.ForEach(x => x.KPILevelCode = x.KPILevelCode.Replace(item.Code, level.code));

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
       
        public bool Update(int key, string title, string code)
        {
            var item = _dbContext.Levels.FirstOrDefault(x => x.ID == key);

            if (item == null)
                return false;
            else
            {
                item.Code = code;
                item.Name = title;
                try
                {
                    _dbContext.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

       
        public IEnumerable<Level> GetItemFromLevel(int parentID)
        {
            foreach (var item in _dbContext.Levels.ToList())
            {
                if (item.ParentID == parentID)
                    yield return item;
            }
        }

        /// <summary>
        /// Lấy ra danh sách OC theo tree view, Sử dụng đệ quy
        /// </summary>
        /// <returns>Trả về danh sách tree view</returns>
        public async Task<List<TreeView>> GetListTree()
        {
            var listLevels = await _dbContext.Levels.OrderBy(x => x.LevelNumber).ToListAsync();
            var levels = new List<TreeView>();
            foreach (var item in listLevels)
            {
                var levelItem = new TreeView();
                levelItem.key = item.ID;
                levelItem.title = item.Name;
                levelItem.code = item.Code;
                levelItem.state = item.State;
                levelItem.levelnumber = item.LevelNumber;
                levelItem.parentid = item.ParentID;
                levels.Add(levelItem);
            }

            List<TreeView> hierarchy = new List<TreeView>();

            hierarchy = levels.Where(c => c.parentid == 0)
                            .Select(c => new TreeView()
                            {
                                key = c.key,
                                title = c.title,
                                code = c.code,
                                levelnumber = c.levelnumber,
                                parentid = c.parentid,
                                children = GetChildren(levels, c.key)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }

        public async Task<bool> AddOrUpdate(Level entity)
        {
            entity.Code = entity.Code.ToUpper();
            List<KPILevel> kpiLevelList = new List<KPILevel>();
            if (entity.ID == 0)
            {
                if (await _dbContext.Levels.FirstOrDefaultAsync(x => x.Code == entity.Code) != null)
                {
                    return false;
                }
                try
                {
                    var level = new Level()
                    {
                        Name = entity.Name,
                        Code = entity.Code,
                        LevelNumber = entity.LevelNumber,
                        ParentCode = entity.ParentCode,
                        ParentID = entity.ParentID
                    };
                    _dbContext.Levels.Add(level);

                    await _dbContext.SaveChangesAsync();

                    var kpiVM = await _dbContext.KPIs.ToListAsync();
                    kpiVM.ForEach(x =>
                    {
                        var kpilevel = new KPILevel();
                        kpilevel.LevelID = level.ID;
                        kpilevel.KPIID = x.ID;
                        kpiLevelList.Add(kpilevel);
                    });
                    
                    _dbContext.KPILevels.AddRange(kpiLevelList);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    var item = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == entity.ID);
                    item.Code = entity.Code;
                    item.Name = entity.Name;
                    item.LevelNumber = entity.LevelNumber;
                    item.ParentID = entity.ParentID;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return false;
                }
            }
        }

        public async Task<TreeView> GetListTreeForWorkplace(int userid)
        {
            var levels = new List<TreeView>();
            List<TreeView> hierarchy = new List<TreeView>();

            var listLevels = await _dbContext.Levels.OrderBy(x => x.LevelNumber).ToListAsync();

            var user = _dbContext.Users.FirstOrDefault(x => x.ID == userid);


            var levelNumber = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == user.LevelID);

            if (levelNumber == null)
            {
                return new TreeView();
            }

            listLevels = listLevels.Where(x => x.LevelNumber >= levelNumber.LevelNumber).OrderBy(x => x.LevelNumber).ToList();
            foreach (var item in listLevels)
            {
                var levelItem = new TreeView();
                levelItem.key = item.ID;
                levelItem.title = item.Name;
                levelItem.code = item.Code;
                levelItem.state = item.State;
                levelItem.levelnumber = item.LevelNumber;
                levelItem.parentid = item.ParentID;
                levels.Add(levelItem);
            }
            var itemLevel = await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == user.LevelID);
            hierarchy = levels.Where(c => c.parentid == itemLevel.ParentID)
                       .Select(c => new TreeView()
                       {
                           key = c.key,
                           title = c.title,
                           code = c.code,
                           levelnumber = c.levelnumber,
                           parentid = c.parentid,
                           children = GetChildren(levels, c.key)
                       })
                       .ToList();

            HieararchyWalk(hierarchy);
            var obj = new TreeView();
            foreach (var item in hierarchy)
            {
                if (item.key == itemLevel.ID)
                {
                    obj = item;
                    break;
                }
            }
            var model = hierarchy.Where(x => x.key == itemLevel.ID).FirstOrDefault();
            return model;
        }
        /// <summary>
        /// Lấy ra danh sách OC theo tree view, Sử dụng đệ quy
        /// </summary>
        /// <returns>Trả về danh sách tree view theo userId</returns>
        public object GetListTreeClient(int userid)
        {
            var levels = new List<TreeView>();
            List<TreeView> hierarchy = new List<TreeView>();

            var listLevels = _dbContext.Levels.OrderBy(x => x.LevelNumber).ToList();

            var user = _dbContext.Users.FirstOrDefault(x => x.ID == userid);

            if (user.Permission == 1)
            {
                listLevels = listLevels.OrderBy(x => x.LevelNumber).ToList();
                foreach (var item in listLevels)
                {
                    var levelItem = new TreeView();
                    levelItem.key = item.ID;
                    levelItem.title = item.Name;
                    levelItem.code = item.Code;
                    levelItem.state = item.State;
                    levelItem.levelnumber = item.LevelNumber;
                    levelItem.parentid = item.ParentID;
                    levels.Add(levelItem);
                }

                hierarchy = levels.Where(c => c.parentid == 0)
                           .Select(c => new TreeView()
                           {
                               key = c.key,
                               title = c.title,
                               code = c.code,
                               levelnumber = c.levelnumber,
                               parentid = c.parentid,
                               children = GetChildren(levels, c.key)
                           })
                           .ToList();

                HieararchyWalk(hierarchy);
                return hierarchy;
            }
            else
            {
                var levelNumber = _dbContext.Levels.FirstOrDefault(x => x.ID == user.LevelID);

                if (levelNumber == null)
                {
                    return new List<TreeView>();
                }

                listLevels = listLevels.Where(x => x.LevelNumber >= levelNumber.LevelNumber).OrderBy(x => x.LevelNumber).ToList();
                foreach (var item in listLevels)
                {
                    var levelItem = new TreeView();
                    levelItem.key = item.ID;
                    levelItem.title = item.Name;
                    levelItem.code = item.Code;
                    levelItem.state = item.State;
                    levelItem.levelnumber = item.LevelNumber;
                    levelItem.parentid = item.ParentID;
                    levels.Add(levelItem);
                }
                var itemLevel = _dbContext.Levels.FirstOrDefault(x => x.ID == user.LevelID);
                hierarchy = levels.Where(c => c.parentid == itemLevel.ParentID)
                           .Select(c => new TreeView()
                           {
                               key = c.key,
                               title = c.title,
                               code = c.code,
                               levelnumber = c.levelnumber,
                               parentid = c.parentid,
                               children = GetChildren(levels, c.key)
                           })
                           .ToList();

                HieararchyWalk(hierarchy);
                var obj = new TreeView();
                foreach (var item in hierarchy)
                {
                    if (item.key == itemLevel.ID)
                    {
                        obj = item;
                        break;
                    }
                }
                var model = hierarchy.Where(x => x.key == itemLevel.ID).ToList();
                return model;
            }
        }

        public async Task<bool> IsExistsCode(string code)
        {
            return await _dbContext.Levels.AnyAsync(x => x.Code == code);
        }
        #region Helper
        public async Task<string> CheckLevelNumberAndGenarateCode(int levelNumber)
        {
            var code = string.Empty;
            switch (levelNumber)
            {
                case (int)OC.GR:
                    code = OC.GR.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.DI:
                    code = OC.DI.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.FA:
                    code = OC.FA.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.CE:
                    code = OC.CE.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.DE:
                    code = OC.DE.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.BU:
                    code = OC.BU.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.TE:
                    code = OC.TE.ToString();
                    code = await GenarateCode(code);
                    break;
                case (int)OC.CEll:
                    code = OC.CEll.ToString();
                    code = await GenarateCode(code);
                    break;
                default:
                    break;
            }
            return code;
        }
        public async Task<string> GenarateCode(string code)
        {
            var value = code;
            for (int i = 1; i < 9999; i++)
            {
                if (!await IsExistsCode(value))
                {
                    value += i.ToString("D4");
                    break;
                }
            }
            return value;
        }
        public string GetNode(string code)
        {
            try
            {
                var id = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == code).LevelID;
                var list = new List<Level>();
                list = _dbContext.Levels.ToList();
                var list2 = new List<Level>();
                list2.Add(list.FirstOrDefault(x => x.ID == id));
                var parentID = list.FirstOrDefault(x => x.ID == id).ParentID.Value;
                foreach (var item in list)
                {
                    if (parentID == 0)
                        break;
                    if (parentID != 0)
                    {
                        //add vao list1
                        list2.Add(list.FirstOrDefault(x => x.ID == parentID));
                    }
                    //cap nhat lai parentID
                    parentID = list.FirstOrDefault(x => x.ID == parentID).ParentID.Value;

                }
                return string.Join("->", list2.OrderBy(x => x.ParentID).Select(x => x.Name).ToArray());
            }
            catch (Exception)
            {

                return "#N/A";
            }

        }
        public string GetNode(int id)
        {
            var list = new List<Level>();
            list = _dbContext.Levels.ToList();
            var list2 = new List<Level>();
            list2.Add(list.FirstOrDefault(x => x.ID == id));
            var parentID = list.FirstOrDefault(x => x.ID == id).ParentID.Value;
            foreach (var item in list)
            {
                if (parentID == 0)
                    break;
                if (parentID != 0)
                {
                    //add vao list1
                    list2.Add(list.FirstOrDefault(x => x.ID == parentID));
                }
                //cap nhat lai parentID
                parentID = list.FirstOrDefault(x => x.ID == parentID).ParentID.Value;

            }
            return string.Join("->", list2.OrderBy(x => x.ParentID).Select(x => x.Name).ToArray());
        }
        public List<TreeView> GetChildren(List<TreeView> levels, int parentid)
        {
            return levels
                    .Where(c => c.parentid == parentid)
                    .Select(c => new TreeView()
                    {
                        key = c.key,
                        title = c.title,
                        code = c.code,
                        levelnumber = c.levelnumber,
                        parentid = c.parentid,
                        children = GetChildren(levels, c.key)
                    })
                    .ToList();
        }
        private void HieararchyWalk(List<TreeView> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    //Console.WriteLine(string.Format("{0} {1}", item.Id, item.Text));
                    HieararchyWalk(item.children);
                }
            }
        }
        #endregion
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

        public Task<PagedList<Level>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
