
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using API.Helpers;
using Microsoft.Extensions.Configuration;
using Service.helpers;
using System.Linq;
using Models.EF;
using System.Collections.Generic;
using Models.ViewModels.Notification;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuaration;
        private readonly IActionPlanService _actionPlanService;
        private readonly ISettingService _settingService;
        private readonly IMailHelper _mailHelper;
        private readonly IErrorMessageService _errorMessageService;
        private readonly IConfiguration _configuration;

        public HomeController(INotificationService notificationService,
            IConfiguration configuaration,
            IActionPlanService actionPlanService,
            ISettingService settingService,
            IMailHelper mailHelper,
            IErrorMessageService errorMessageService,
            IConfiguration configuration)
        {
            _notificationService = notificationService;
            _configuaration = configuaration;
            _actionPlanService = actionPlanService;
            _settingService = settingService;
            _mailHelper = mailHelper;
            _errorMessageService = errorMessageService;
            _configuration = configuration;
        }

        public IActionResult ChangeLanguage(String Langby)
        {
            //if (Langby != null)
            //{
            //    var userProfile = Session["UserProfile"] as UserProfileVM;

            //    var sibars= new UserAdminDAO().Sidebars(userProfile.User.Permission,Langby);
            //    var userVM = new UserProfileVM();
            //    userVM.User = userProfile.User;
            //    userVM.Menus = sibars;

            //    Session["UserProfile"] = userVM;
            //    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Langby);
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(Langby);
            //}

            //HttpCookie cookie = new HttpCookie("Lang");
            //cookie.Value = Langby;
            //Response.Cookies.Add(cookie);
            //string urlReferrer = Request.UrlReferrer.ToString();

            //return Redirect(Request.UrlReferrer.ToString());
            return Ok();
        }
        public async Task<bool> SendMail()
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
                    await _mailHelper.SendEmailRangeAsync(model2.Item2.Select(x => x.Email).ToList(), "[KPI System] Late on upload data", content2);


                }

                if (await _settingService.IsSendMail("CHECKDEADLINE"))
                {
                    foreach (var item in model.Item1)
                    {
                        //string content = "Please note that the action plan we are overdue on " + item.Deadline;
                        count++;
                        html += @"<tr>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{no}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{kpiname}}</td>
                            <td valign='top' style='padding:5px; font-family: Arial,sans-serif; font-size: 16px; line-height:20px;'>{{deadline}}</td>
                             </tr>"
                                .Replace("{{no}}", count.ToString())
                                .Replace("{{kpiname}}", item[0].ToSafetyString())
                                .Replace("{{deadline}}", item[1].ToSafetyString("MM/dd/yyyy"));
                    }
                    content = content.Replace("{{{html-template}}}", html);
                    await _mailHelper.SendEmailRangeAsync(model.Item2.Select(x => x.Email).ToList(), "[KPI System] Late on task", content);

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
       

        public async Task<IActionResult> GetNotifications()
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.DecodeToken(token).FirstOrDefault(x => x.Type == "nameid").Value.ToInt();
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
            return Ok(new { arrayID = listID.ToArray(), total = total, data = listNotifications });
        }
        public ActionResult ListHistoryNotification()
        {
            string token = Request.Headers["Authorization"];
            var userID = Extensions.DecodeToken(token).FirstOrDefault(x => x.Type == "nameid").Value.ToInt();
            if (userID > 0)
                return Ok();
            IEnumerable<NotificationViewModel> model = _notificationService.GetHistoryNotification(userID);
            return BadRequest();
        }


    }
}

