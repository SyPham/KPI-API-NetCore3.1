﻿
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRange(string listID)
        {
            return Ok(await _notificationService.UpdateRange(listID));
        }
        [HttpPost]
        public async Task<IActionResult> Update(int ID)
        {
            var obj =await _notificationService.Update(ID);
           // NotificationHub.SendNotifications();
            return Ok(obj);
        }
    }
}