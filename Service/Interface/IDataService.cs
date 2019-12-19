using Models.Data;
using Models.EF;
using Models.ViewModels.Auth;
using Models.ViewModels.Data;
using Models.ViewModels.Level;
using Models.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Helpers;

namespace Service.Interface
{
   public interface IDataService: IDisposable
    {
        Task<object> GetAllDataByCategory(int categoryid, string period, int? start, int? end, int? year);
        Task<DataUserViewModel> GetAllOwner(int categoryID, int kpilevelID);
        ChartViewModel ListDatas(string kpilevelcode, string period, int? year, int? start, int? end, int? catid);
        Task<object> Remark(int dataid);
        Task<bool> UpdateRemark(int dataid, string remark);
        DataCompareViewModel Compare(string obj);
        ChartViewModel Compare(string kpilevelcode, string period);
        public List<ChartViewModel2> Compare2(string obj);
        ChartViewModel2 Compare2(string kpilevelcode, string period);
        Task<object> ListKPIUpload(int updaterid, int page, int pageSize);
        Task<object> UpLoadKPILevel(int userid, int page, int pageSize);
        string GetTargetData(string KPILevelCode, string CharacterPeriod, int period);
        List<DataExportViewModel> DataExport(int userid);
        string GetValueData(string KPILevelCode, string CharacterPeriod, int period);
        Task<bool> IsUpdater(int id);
        Task<ImportDataViewModel> ImportData(List<UploadDataViewModel> entity, string userUpdate, int userId);
        Task<object> KPIRelated(int levelId, int page, int pageSize);
        Task<object> UpLoadKPILevelTrack(int userid, int page, int pageSize);
    }
 
}
