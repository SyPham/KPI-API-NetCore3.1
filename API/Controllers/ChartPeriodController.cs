
using Models.EF;
using Models.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using System.Linq;
using Service.Helpers;
using API.SignalR;
using System.Collections.Generic;
using Models.ViewModels.ActionPlan;
using Microsoft.Extensions.Configuration;


namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChartPeriodController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ICommentService _commentService;
        private readonly IActionPlanService _actionPlanService;
        private readonly ISettingService _settingService;
        private readonly IMailHelper _mailHelper;
        private readonly IKPILevelService _kPILevelService;
        private readonly IFavouriteService _favouriteService;
        private readonly IConfiguration _configuaration;
        private readonly INotificationService _notificationService;

        // GET: Month

        public ChartPeriodController(IDataService dataService,
            ICommentService commentService,
            IActionPlanService actionPlanService,
            ISettingService settingService,
            IMailHelper mailHelper,
            IKPILevelService kPILevelService,
            IFavouriteService favouriteService,
            IConfiguration configuaration,
            INotificationService notificationService)
        {
            _dataService = dataService;
            _commentService = commentService;
            _actionPlanService = actionPlanService;
            _settingService = settingService;
            _mailHelper = mailHelper;
            _kPILevelService = kPILevelService;
            _favouriteService = favouriteService;
            _configuaration = configuaration;
            _notificationService = notificationService;
        }
        [HttpGet("{kpilevelcode}/{catid}/{period}/{year}/{start}/{end}")]
        public IActionResult ListDatas(string kpilevelcode, int? catid, string period, int? year, int? start, int? end)
        {
            var model = _dataService.ListDatas(kpilevelcode, period, year, start, end, catid);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody]AddCommentViewModel entity)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();

            var levelNumberOfUserComment = Extensions.GetDecodeTokenByProperty(token, ClaimTypeEnum.LevelId.ToString()).ToInt();

            var data = await _commentService.AddComment(entity, levelNumberOfUserComment);
            var tos = new List<string>();
            //HenryHub.SendNotifications();
            if (data.ListEmails.Count > 0 && await _settingService.IsSendMail("ADDCOMMENT"))
            {
                var model = data.ListEmails.DistinctBy(x => x);
                //string from = ConfigurationManager.AppSettings["FromEmailAddress"].ToSafetyString();
                string content = "The account" + model.First()[0] + " mentioned you in KPI System Apps. Content: " + model.First()[4] + ". " + model.First()[3] + " Link: " + model.First()[2];
                await _mailHelper.SendEmailRangeAsync(model.Select(x => x[1]).ToList(), "[KPI System] Comment", content);
            }
            return Ok(new { status = data.Status, isSendmail = true });
        }
        [HttpGet("{dataid}/{userid}")]
        public async Task<IActionResult> LoadDataComment(int dataid, int userid)
        {
            return Ok(await _commentService.ListComments(dataid, userid));
        }
        [HttpPost]
        public async Task<IActionResult> AddCommentHistory([FromBody]int userid,int dataid)
        {
            return Ok(await _commentService.AddCommentHistory(userid, dataid));
        }
        [HttpGet("{dataid}")]
        public async Task<IActionResult> Remark(int dataid)
        {
            return Ok(await _dataService.Remark(dataid));
        }
        [HttpPost]

        public async Task<IActionResult> AddFavourite([FromBody]Favourite entity)
        {
            return Ok(await _favouriteService.Add(entity));
        }
        [HttpGet("{obj}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadDataProvide(string obj, int page, int pageSize)
        {
            return Ok(await _kPILevelService.LoadDataProvide(obj, page, pageSize));
        }
        [HttpGet("{dataid}/{remark}")]
        public async Task<IActionResult> UpdateRemark(int dataid, string remark)
        {
            return Ok(await _dataService.UpdateRemark(dataid, remark));
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]ActionPlan item)
        {
            return Ok(await _actionPlanService.Update(item));
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]ActionPlanParams obj)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            obj.OwnerID = userID;
            var data = await _actionPlanService.Add(obj);//(item, obj.Subject, obj.Auditor, obj.CategoryID);
            //NotificationHub.SendNotifications();
            if (data.ListEmails.Count > 0 && await _settingService.IsSendMail("ADDTASK"))
            {
                string content = "The account " + data.ListEmails.First()[0] + " mentioned you in KPI System Apps. Content: " + data.ListEmails.First()[4] + ". " + data.ListEmails.First()[3] + " Link: " + data.ListEmails.First()[2];
                await _mailHelper.SendEmailRangeAsync(data.ListEmails.Select(x => x[1]).ToList(), "[KPI System] Action Plan (Add task)", content);
            }
            return Ok(new { status = data.Status, isSendmail = true });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _actionPlanService.Remove(id));
        }
        [HttpGet("{DataID}/{CommentID}/{UserID}")]
        public async Task<IActionResult> GetAll(int DataID, int CommentID, int UserID)
        {
            //var userprofile = Session["UserProfile"] as UserProfileVM;
            return Ok(await _actionPlanService.GetAll(DataID, CommentID, UserID));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            return Ok(await _actionPlanService.GetById(id));
        }
        [HttpPost]
        public async Task<IActionResult> Approval([FromBody]int id, int approveby, string KPILevelCode, int CategoryID)
        {
            var model = await _actionPlanService.Approve(id, approveby, KPILevelCode, CategoryID);
            //NotificationHub.SendNotifications();
            if (model.Item1.Count > 0 && await _settingService.IsSendMail("APPROVAL"))
            {
                string URL = _configuaration.GetSection("AppSettings:URL").ToSafetyString();
                var data = model.Item1.DistinctBy(x => x);
                string content = "The account " + data.First()[0] + " was approved the task " + data.First()[3] + " Link: " + URL + "/Workplace";
                await _mailHelper.SendEmailRangeAsync(data.Select(x => x[1]).ToList(), "[KPI System] Approved", content);

            }
            return Ok(new { status = model.Item2, isSendmail = true });
        }
        [HttpPost]
        public async Task<IActionResult> Done([FromBody]int id, string KPILevelCode, int CategoryID)
        {

            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();

            var model = await _actionPlanService.Done(id, userID, KPILevelCode, CategoryID);
            //NotificationHub.SendNotifications();
            if (model.Item1.Count > 0 && await _settingService.IsSendMail("DONE"))
            {
                string URL = _configuaration.GetSection("AppSettings:URL").ToSafetyString();

                var data = model.Item1.DistinctBy(x => x);
                string content = "The account " + data.First()[0] + " has finished the task" + data.First()[3] + " Link: " + URL + "/Workplace";
                await _mailHelper.SendEmailRangeAsync(data.Select(x => x[1]).ToList(), "[KPI System] Action Plan (Done)", content);

            }
            return Ok(new { status = model.Item2, isSendmail = true });
        }
        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody]Notification notification)
        {
            var status = await _notificationService.Add(notification);
            //NotificationHub.SendNotifications();
            return Ok(status);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateActionPlan([FromBody]ActionPlanForUpdateParams actionPlan)
        {
            return Ok(await _actionPlanService.UpdateActionPlan(actionPlan));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSheduleDate([FromBody]string name, string value,string pk)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            return Ok(await _actionPlanService.UpdateSheduleDate(name, value, pk, userID));
        }

        //public async Task<IActionResult> GetAllDataByCategory(int catid, string period, int? year)
        //{
        //    var currenYear = year ?? DateTime.Now.Year;
        //    return Ok(new DataChartDAO().GetAllDataByCategory(catid, period, currenYear));

        //}
    }
}