using Models.Data;
using Models.EF;
using Models.ViewModels.ActionPlan;
using Models.ViewModels.Comment;
using Models.ViewModels.User;
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
    public interface IActionPlanService : ICommonService<ActionPlan>, IDisposable
    {
        Task<Tuple<List<string[]>, bool>> Approve(int id, int approveby, string KPILevelCode, int CategoryID);
        Task<object> LoadActionPlan(string role, int page, int pageSize);
        Task<object> GetAll(int DataID, int CommentID, int userid);
        Task<CommentForReturnViewModel> Add(ActionPlanParams obj);
        Task<Tuple<List<string[]>, bool>> Done(int id, int userid, string KPILevelCode, int CategoryID);
        Task<bool> UpdateActionPlan(ActionPlanForUpdateParams actionPlan);
        Task<bool> UpdateSheduleDate(string name, string value, string pk, int userid);
        Tuple<List<object[]>, List<UserViewModel>> CheckDeadline();
        Tuple<List<object[]>, List<UserViewModel>> CheckLateOnUpdateData();
    }
}
