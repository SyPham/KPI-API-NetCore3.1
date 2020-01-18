using Models.EF;
using Models.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using System.Linq;
using Service.Helpers;
using System.Collections.Generic;
using Models.ViewModels.ActionPlan;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API.SignalR;
using System.Threading;
using API.Dto;
using API.Dto.ChartPeriod;

namespace API.Controllers
{
    [Authorize]
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
        private readonly IHubContext<HenryHub> _hubContext;
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
            IHubContext<HenryHub> hubContext,
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
            _hubContext = hubContext;
            _notificationService = notificationService;
        }
        [HttpGet("{kpilevelcode}/{catid}/{period}/{year}/{start}/{end}")]
        public async Task<IActionResult> ListDatas(string kpilevelcode, int? catid, string period, int? year, int? start, int? end)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var model =await _dataService.ListDatas(kpilevelcode, period, year, start, end, catid, userID);
            return Ok(model);
        }
        [HttpGet("{code}/{period}")]
        public async Task<IActionResult> GetItemInListOfWorkingPlan(string code, string period)
        {
            return Ok(await _kPILevelService.GetItemInListOfWorkingPlan(code, period));
        }
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody]AddCommentViewModel entity)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();

            var levelNumberOfUserComment = Extensions.GetDecodeTokenByProperty(token, "LevelId").ToInt();

            var data = await _commentService.AddComment(entity, levelNumberOfUserComment);
            
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", "message");


            
            var tos = new List<string>();
            
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", "message");

            
            if (data.ListEmails.Count > 0 && await _settingService.IsSendMail("ADDCOMMENT"))
            {
                var model = data.ListEmails.DistinctBy(x => x);
                //string from = ConfigurationManager.AppSettings["FromEmailAddress"].ToSafetyString();
                string content = @"<p><b>*PLEASE DO NOT REPLY* this email was automatically sent from the KPI system.</b></p> 
                                   <p>The account <b>" + model.First()[0] + "</b> mentioned you in KPI System Apps. </p>" +
                                  "<p>Content: " + model.First()[4] + "</p>" +
                                  "<p>Link: <a href='" + data.QueryString + "'>Click Here</a></p>";
                Thread thread = new Thread(async () =>
                {
                    await _mailHelper.SendEmailRange(model.Select(x => x[1]).ToList(), "[KPI System-02] Comment", content);
                });
                thread.Start();
            }
            return Ok(new { status = data.Status, isSendmail = true });
        
        }
        [AllowAnonymous]
        [HttpGet("{dataid}/{userid}")]
        public async Task<IActionResult> LoadDataComment(int dataid, int userid)
        {
            return Ok(await _commentService.ListComments(dataid, userid));
        }
        [HttpPost]
        public async Task<IActionResult> AddCommentHistory([FromBody]int userid, int dataid)
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
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", "message");


            
            return Ok(new { status = data.Status, isSendmail = true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _actionPlanService.Remove(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteComment2(int id)
        {
            return Ok(await _commentService.Remove(id));
        }

        [HttpGet("{DataID}/{CommentID}/{UserID}")]
        public async Task<IActionResult> GetAll(int DataID, int CommentID, int UserID)
        {
            //var userprofile = Session["UserProfile"] as UserProfileVM;
            return Ok(await _actionPlanService.GetAll(DataID, CommentID, UserID));
        }
        [HttpPost("{DataID}/{CommentID}/{UserID}/{keyword}")]
        [HttpPost("{DataID}/{CommentID}/{UserID}/{keyword}/{page}/{pageSize}")]
        public async Task<IActionResult> GetAllPaging(int DataID, int CommentID, int UserID, string keyword, int? page, int? pageSize)
        {
            //var userprofile = Session["UserProfile"] as UserProfileVM;
            return Ok(await _actionPlanService.GetAll(DataID, CommentID, UserID, keyword, page ?? 1, pageSize ?? 5));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            return Ok(await _actionPlanService.GetById(id));
        }
        [HttpPost]
        public async Task<IActionResult> Approval([FromBody]ApprovalDto obj)
        {
            var model = await _actionPlanService.Approve(obj.id, obj.approveby, obj.KPILevelCode, obj.CategoryID);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", "message");


            if (model.Item1.Count > 0 && await _settingService.IsSendMail("APPROVAL"))
            {
                Thread thread = new Thread(async () =>
                {
                    string URL = _configuaration.GetSection("AppSettings:URL").ToSafetyString();
                    var data = model.Item1.DistinctBy(x => x);
                    string content = @"<p><b>*PLEASE DO NOT REPLY* this email was automatically sent from the KPI system.</b></p> 
                                   <p>The account <b>" + data.First()[0].ToTitleCase() + "</b> approved the task <b>'" + data.First()[3] + "'</b> </p>" +
                                 "<p>Link: <a href='" + model.Item3 + "'>Click Here</a></p>";
                    await _mailHelper.SendEmailRange(data.Select(x => x[1]).ToList(), "[KPI System-05] Approved", content);
                });
                thread.Start();
            }
            return Ok(new { status = model.Item2, isSendmail = true });
        }
        [HttpPost]
        public async Task<IActionResult> Done([FromBody]DoneDto obj)
        {

            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();

            var model = await _actionPlanService.Done(obj.id, userID, obj.KPILevelCode, obj.CategoryID);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", "message");


            if (model.Item1.Count > 0 && await _settingService.IsSendMail("DONE"))
            {
                Thread thread = new Thread(async () =>
                {
                    string URL = _configuaration.GetSection("AppSettings:URL").ToSafetyString();

                    var data = model.Item1.DistinctBy(x => x);
                    string content = @"<p><b>*PLEASE DO NOT REPLY* this email was automatically sent from the KPI system.</b></p> 
                                    <p>The account <b>" + data.First()[0].ToTitleCase() + "</b> has finished the task name <b>'" + data.First()[3] + "'</b></p>" +
                                  "<p>Link: <a href='" + model.Item3 + "'>Click Here</a></p>";
                    await _mailHelper.SendEmailRange(data.Select(x => x[1]).ToList(), "[KPI System-04] Action Plan (Finished Task)", content);
                });
                thread.Start();
            }
            return Ok(new { status = model.Item2, isSendmail = true });
        }
        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody]Notification notification)
        {
            var status = await _notificationService.Add(notification);
            await _hubContext.Clients.All.SendAsync("SendMessage");

            return Ok(status);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateActionPlan([FromBody]ActionPlanForUpdateParams actionPlan)
        {
            return Ok(await _actionPlanService.UpdateActionPlan(actionPlan));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpdateSheduleDate([FromForm]UpdateActionPlanDto obj)
        {
            //string token = Request.Headers["Authorization"];
            //var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            return Ok(await _actionPlanService.UpdateSheduleDate(obj.name, obj.value, obj.pk, obj.userid));
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            return Ok(await _commentService.DeleteComment(id, userID));
        }

        [AllowAnonymous]
        [HttpPost("{code}")]
        [HttpPost("{code}/{page}/{pageSize}")]
        public async Task<ActionResult> ListTasks(string code, int? page, int? pageSize)
        {
            var pagedList = await _dataService.ListTasks(code, page, pageSize);
            return Ok(new
            {
                data = pagedList,
                total = pagedList.Count,
                pageCount = pagedList.TotalPages,
                status = true,
                page,
                pageSize
            });
        }


        //public async Task<IActionResult> GetAllDataByCategory(int catid, string period, int? year)
        //{
        //    var currenYear = year ?? DateTime.Now.Year;
        //    return Ok(new DataChartDAO().GetAllDataByCategory(catid, period, currenYear));

        //}
    }
}