using Models.Data;
using Models.EF;
using Models.ViewModels.Notification;
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
    public interface INotificationService : ICommonService<Notification>, IDisposable
    {
        Task<bool> UpdateRange(string listID);
        Task<object> Update(int ID);
        
        Task<List<NotificationViewModel>> ListNotifications(int userid);
        Task<bool> IsSend();
        Task<bool> AddSendMail(StateSendMail stateSendMail);
        List<NotificationViewModel> GetHistoryNotification(int userid);
    }
     
}
