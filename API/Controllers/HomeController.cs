
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using API.Helpers;
using Microsoft.Extensions.Configuration;
using Service.Helpers;
using System.Linq;
using Models.EF;
using System.Collections.Generic;
using Models.ViewModels.Notification;
using Microsoft.AspNetCore.Authorization;
using Models.ViewModels.User;
using System.Threading;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuaration;
        private readonly IActionPlanService _actionPlanService;
        private readonly ISettingService _settingService;
        private readonly IMailHelper _mailHelper;
        private readonly IDataService _dataService;
        private readonly IErrorMessageService _errorMessageService;
        private readonly IConfiguration _configuration;

        public HomeController(INotificationService notificationService,
            IConfiguration configuaration,
            IActionPlanService actionPlanService,
            ISettingService settingService,
            IMailHelper mailHelper,
            IDataService dataService,
            IErrorMessageService errorMessageService,
            IConfiguration configuration)
        {
            _notificationService = notificationService;
            _configuaration = configuaration;
            _actionPlanService = actionPlanService;
            _settingService = settingService;
            _mailHelper = mailHelper;
            _dataService = dataService;
            _errorMessageService = errorMessageService;
            _configuration = configuration;
        }

        #region (*) Method Helper For SendMail()
        private async Task CheckLateOnUpdate(Tuple<List<object[]>, List<UserViewModel>> auditUploadModel)
        {

            if (await _settingService.IsSendMail("CHECKLATEONUPDATEDATA") && auditUploadModel.Item1.Count > 0)
            {
                string host = Request.Host.ToSafetyString();
                string contentForAuditUpload = System.IO.File.ReadAllText(host + "/Templates/LateOnUpDateData.html");
                contentForAuditUpload = contentForAuditUpload
                                    .Replace("{{{href}}}", host)
                                    .Replace("{{{content}}}", @"<b style='color:red'>Late On Update Data</b><br/>Your KPIs have expired as below list: ");
                var htmlForUpload = string.Empty;
                var count = 0;


                foreach (var item2 in auditUploadModel.Item1)
                {
                    count++;
                    htmlForUpload += @"<tr>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{no}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{area}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{ockpicode}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{kpiname}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{year}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{deadline}}</td>
                             </tr>"
                            .Replace("{{no}}", count.ToSafetyString())
                            .Replace("{{area}}", item2[3].ToSafetyString())
                            .Replace("{{kpiname}}", item2[0].ToSafetyString())
                            .Replace("{{ockpicode}}", item2[2].ToSafetyString())
                            .Replace("{{year}}", item2[4].ToSafetyString())
                            .Replace("{{deadline}}", item2[1].ToSafetyString());
                }
                contentForAuditUpload = contentForAuditUpload.Replace("{{{html-template}}}", htmlForUpload);
                await _mailHelper.SendEmailRange(auditUploadModel.Item2.Select(x => x.Email).ToList(), "[KPI System-07] Late on upload data", contentForAuditUpload);

            }
        }

        private async Task CheckLateOnTask(Tuple<List<object[]>, List<UserViewModel>> model)
        {
            if (await _settingService.IsSendMail("CHECKDEADLINE") && model.Item1.Count > 0)
            {
                var count = 0;
                string host = Request.Host.ToSafetyString();
                var htmlForDeadLine = string.Empty;
                string contentForDeadline = System.IO.File.ReadAllText(host + "/Templates/LateOnTask.html");
                contentForDeadline = contentForDeadline
                                    .Replace("{{{href}}}", host)
                                    .Replace("{{{content}}}", @"<b style='color:red'>Late On Task</b><br/>Your task have expired as below list: ");
                foreach (var item in model.Item1)
                {
                    //string content = "Please note that the action plan we are overdue on " + item.Deadline;
                    count++;
                    htmlForDeadLine += @"<tr>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{no}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{oc}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{kpi}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{task}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{deadline}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'><a href='{{link}}'>Click Here</a></td>
                             </tr>"
                            .Replace("{{no}}", count.ToString())
                            .Replace("{{oc}}", item[3].ToSafetyString())
                            .Replace("{{kpi}}", item[4].ToSafetyString())
                            .Replace("{{task}}", item[0].ToSafetyString())
                            .Replace("{{deadline}}", item[1].ToSafetyString())
                            .Replace("{{link}}", item[2].ToSafetyString());
                }
                contentForDeadline = contentForDeadline.Replace("{{{html-template}}}", htmlForDeadLine);

                await _mailHelper.SendEmailRange(model.Item2.Select(x => x.Email).ToList(), "[KPI System-06] Late on task", contentForDeadline);

            }
        }
        #endregion

        private async Task SendMail()
        {
            if (!await _notificationService.IsSend())
            {
                #region *) Field
                string token = Request.Headers["Authorization"];
                var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();

                var auditUploadModel = _actionPlanService.CheckLateOnUpdateData(userID);
                var model = _actionPlanService.CheckDeadline();
                #endregion

                #region 1) Late On Update
                Thread lateOnTask = new Thread(async () => await CheckLateOnUpdate(auditUploadModel));
                lateOnTask.Start();

                #endregion

                #region 2) Check Late On Task
                Thread lateOnUpload = new Thread(async () => await CheckLateOnTask(model));
                lateOnUpload.Start();
                #endregion

                #region *) Thông báo để biết gửi mail hay chưa

                var itemSendMail = new StateSendMail();
                await _notificationService.AddSendMail(itemSendMail);
                await _errorMessageService.Add(new ErrorMessage
                {
                    Function = "Send Mail",
                    Name = "[KPI System] Late on task, [KPI System] Late on upload data"
                });
                #endregion
            }
        }
        private async Task<bool> SendMail2()
        {
            string URL = _configuaration.GetSection("AppSettings:URL").ToSafetyString();
            string token = Request.Headers["Authorization"];
            var userID = Extensions.DecodeToken(token).FirstOrDefault(x => x.Type == "nameid").Value.ToInt();
            if (!await _notificationService.IsSend())
            {

                string content2 = System.IO.File.ReadAllText(URL + "/Templates/LateOnUpDateData.html");
                content2 = content2.Replace("{{{content}}}", @"<b style='color:red'>Late On Update Data</b><br/>Your KPIs have expired as below list: ");

                string content = System.IO.File.ReadAllText(URL + "/Templates/LateOnTask.html");
                content = content.Replace("{{{content}}}", @"<b style='color:red'>Late On Task</b><br/>Your task have expired as below list: ");
                var html = string.Empty;
                var html2 = string.Empty;

                var count = 0;
                var model2 = _actionPlanService.CheckLateOnUpdateData();
                var model = _actionPlanService.CheckDeadline();
                if (await _settingService.IsSendMail("CHECKLATEONUPDATEDATA"))
                {
                    foreach (var item2 in model2.Item1)
                    {
                        count++;
                        html += @"<tr>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{no}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{area}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{ockpicode}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{kpiname}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{year}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{deadline}}</td>
                             </tr>"
                                .Replace("{{no}}", count.ToSafetyString())
                                .Replace("{{area}}", item2[3].ToSafetyString())
                                .Replace("{{kpiname}}", item2[0].ToSafetyString())
                                .Replace("{{ockpicode}}", item2[2].ToSafetyString())
                                .Replace("{{year}}", item2[4].ToSafetyString())
                                .Replace("{{deadline}}", item2[1].ToSafetyString());
                    }
                    content2 = content2.Replace("{{{html-template}}}", html);


                }

                if (await _settingService.IsSendMail("CHECKDEADLINE"))
                {
                    foreach (var item in model.Item1)
                    {
                        //string content = "Please note that the action plan we are overdue on " + item.Deadline;
                        count++;
                        html2 += @"<tr>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{no}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{kpiname}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{deadline}}</td>
                             </tr>"
                                .Replace("{{no}}", count.ToString())
                                .Replace("{{kpiname}}", item[0].ToSafetyString())
                                .Replace("{{deadline}}", item[1].ToSafetyString("MM/dd/yyyy"));
                    }
                    content = content.Replace("{{{html-template}}}", html2);
                    await _mailHelper.SendEmailRange(model.Item2.Select(x => x.Email).ToList(), "[KPI System] Late on task", content);

                }
                var itemSendMail = new StateSendMail();
                await _notificationService.AddSendMail(itemSendMail);

                int hh = _configuration.GetSection("AppSettings:hh").ToInt();
                int mm = _configuration.GetSection("AppSettings:mm").ToInt();
                await _errorMessageService.Add(new ErrorMessage
                {
                    Function = "Test window service " + hh + ":" + mm,
                    Name = "EmailJob"
                });
            }

            return true;
        }
        [HttpGet]
        public async Task<IActionResult> CheckEmailEveryday()
        {
            await SendMail();
            return Ok(200);
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            if (userID == 1)
            {
                return Ok(new { arrayID = new List<int>(), total = 0, data = new List<NotificationViewModel>() });

            }
            var listNotifications = await _notificationService.ListNotifications(userID);
            var total = 0;
            var listID = new List<int>();
            foreach (var item in listNotifications)
            {
                if (item.Seen == false)
                {
                    total++;
                    listID.Add(item.ID);
                }

            }
            return Ok(new { arrayID = listID.ToArray(), total, data = listNotifications });
        }
        [HttpGet]
        public IActionResult ListHistoryNotification()
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            if (userID > 0)
            {
                IEnumerable<NotificationViewModel> model = _notificationService.GetHistoryNotification(userID);
                return Ok(model);

            }

            return BadRequest();
        }
        [HttpPost("{notificationId}")]
        [HttpPost("{notificationId}/{page}/{pageSize}")]
        public IActionResult LateOnUpload(int notificationId, int? page, int? pageSize)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var pagedList = _dataService.LateOnUpLoads(userID, notificationId, page, pageSize);
            return Ok(new
            {
                notificationId,
                data = pagedList,
                total = pagedList.Count,
                pageCount = pagedList.TotalPages,
                status = true,
                page,
                pageSize
            });
        }
        [HttpPost("{notificationId}")]
        [HttpPost("{notificationId}/{page}/{pageSize}")]
        public async Task<IActionResult> ListSubNotificationDetail(int notificationId, int? page, int? pageSize)
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var pagedList = await _dataService.GetAllSubNotificationsByIdAsync(userID, notificationId, page, pageSize);
            return Ok(new
            {
                notificationId,
                data = pagedList,
                total = pagedList.Count,
                pageCount = pagedList.TotalPages,
                status = true,
                page,
                pageSize
            });
        }
    }
}

