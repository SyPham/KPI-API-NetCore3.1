using Models.Data;
using Models.EF;
using Models.ViewModels.KPILevel;
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
    public interface IKPILevelService : ICommonService<KPILevel>, IDisposable
    {
        int GetByUsername(string username);
        Task<bool> UpdateKPILevel(KPILevelForUpdate entity);
        Task<bool> Update(KPILevelForUpdate entity);
        Task<object> GetAll(int page, int pageSize);
        Task<object> GetAllPaging(int levelID, int categoryID, int page, int pageSize = 3);
		Task<object> GetItemInListOfWorkingPlan(string code, string period);
		Task<bool> AddWorkingPlan(WorkingPlan entity);
		Task<object> GetDetail(int ID);
		/// <summary>
		/// +) Lấy ra danh sách tbl.KPIs kết hợp với tbl.Levels dựa vào tbl.KPILevels
		/// Select 
		///       kl.ID,
		///       kl.KPILevelCode,
		///       kl.KPILevelCode,
		///       kl.KPIID,
		///       KPIs.Code,
		///       kl.LevelID,
		///       KPIs.LevelID as LevelNumber,
		///       kl.Period,
		///       kl.Weekly,
		///       kl.Monthly,
		///       kl.Quarterly,
		///       kl.Yearly,
		///       kl.Checked,
		///       kl.WeeklyChecked,
		///       kl.MonthlyChecked,
		///       kl.QuarterlyChecked,
		///       kl.YearlyChecked,
		///       kl.CheckedPeriod,
		///       kl.TimeCheck,
		///       kl.CreateTime,
		///       Units.Name,
		///       KPIs.CategoryID,
		///       KPIs.Name,
		///       Levels.Code
		///       From KPILevels as kl
		///       Join KPIs on kl.KPIID = KPIs.ID
		///       Join Units on KPIs.Unit = Units.ID
		///       Join Levels on kl.LevelID = Levels.ID
		///       Where kl.LevelID = @levelID or KPIs.CategoryID = @categoryID
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="levelID"></param>
		/// <param name="categoryID"></param>
		/// <returns></returns>
		Task<object> LoadDataForUser(int levelID, int categoryID, int page, int pageSize = 3);
        /// <summary>
        /// +) Lấy ra danh sách tbl.KPIs kết hợp với tbl.Levels dựa vào tbl.KPILevels
        /// Select 
	    ///       kl.ID,
	    ///       kl.KPILevelCode,
	    ///       kl.KPILevelCode,
	    ///       kl.KPIID,
	    ///       KPIs.Code,
	    ///       kl.LevelID,
	    ///       KPIs.LevelID as LevelNumber,
	    ///       kl.Period,
	    ///       kl.Weekly,
	    ///       kl.Monthly,
	    ///       kl.Quarterly,
	    ///       kl.Yearly,
	    ///       kl.Checked,
	    ///       kl.WeeklyChecked,
	    ///       kl.MonthlyChecked,
	    ///       kl.QuarterlyChecked,
	    ///       kl.YearlyChecked,
	    ///       kl.CheckedPeriod,
	    ///       kl.TimeCheck,
	    ///       kl.CreateTime,
	    ///       Units.Name,
	    ///       KPIs.CategoryID,
	    ///       KPIs.Name,
	    ///       Levels.Code
	    ///       From KPILevels as kl
	    ///       Join KPIs on kl.KPIID = KPIs.ID
	    ///       Join Units on KPIs.Unit = Units.ID
	    ///       Join Levels on kl.LevelID = Levels.ID
        ///       Where kl.LevelID = @levelID or KPIs.CategoryID = @categoryID
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="levelID"></param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        Task<object> LoadData(int levelID, int categoryID, int page, int pageSize = 3);
        Task<PagedList<KPILevel>> GetAllPagingByLevelIdAndCategoryId(int levelId, int catId, int page, int pageSize);
        Task<object> LoadDataProvide(string obj, int page, int pageSize);
    }
     
}
