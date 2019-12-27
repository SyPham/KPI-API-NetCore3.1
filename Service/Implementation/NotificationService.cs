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

namespace Service.Implementation
{
   
    public class NotificationService : INotificationService
    {
        private readonly DataContext _dbContext;

        public NotificationService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddSendMail(StateSendMail stateSendMail)
        {
            try
            {
                _dbContext.StateSendMails.Add(stateSendMail);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<NotificationViewModel> GetHistoryNotification(int userid)
        {

            var actionPlan = _dbContext.ActionPlans;
            var notification = _dbContext.Notifications;
            var model = from notify in _dbContext.Notifications
                        join notifyDetail in _dbContext.NotificationDetails on notify.ID equals notifyDetail.NotificationID
                        where notifyDetail.UserID == userid
                        join recipient in _dbContext.Users on notifyDetail.UserID equals recipient.ID // recipient
                        join sender in _dbContext.Users on notify.UserID equals sender.ID //sender
                        select new NotificationViewModel
                        {
                            ID = notifyDetail.ID,
                            Title = notify.Title,
                            KPIName = notify.KPIName,
                            Period = notify.Period,
                            CreateTime = notifyDetail.CreateTime,
                            RecipientID = recipient.ID,
                            Recipient = recipient.Alias,
                            Link = notify.Link,
                            Seen = notifyDetail.Seen,
                            Tag = notify.Tag,
                            Deadline = (DateTime?)actionPlan.FirstOrDefault(x => x.ID == notify.ActionplanID).Deadline ?? new DateTime(2001, 1, 1),
                            Sender = sender.Alias,
                            SenderID = notify.ID,
                            Action = notify.Action,
                            Content = notify.Content
                        };
            var model1 = model.OrderByDescending(x => x.CreateTime).ToList();
            return model1;
        }
        public Task<bool> Add(Notification entity)
        {
            throw new NotImplementedException();
        }
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

        public Task<List<Notification>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<Notification>> GetAllPaging(string keyword, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Notification> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NotificationViewModel>> ListNotifications(int userid)
        {
            var kpilevel = _dbContext.KPILevels;
            var kpilevel2 = _dbContext.Notifications.ToList();
            var model = (await (from notify in _dbContext.Notifications
                                join notifyDetail in _dbContext.NotificationDetails.Where(x => x.UserID == userid) on notify.ID equals notifyDetail.NotificationID
                                select new
                                {
                                    NotificationID = notify.ID,
                                    notifyDetail.ID,
                                    notify.Title,
                                    notify.KPIName,
                                    notify.Period,
                                    ContentDetail = notifyDetail.Content,
                                    notifyDetail.URL,
                                    notifyDetail.CreateTime,
                                    notify.Link,
                                    notifyDetail.Seen,
                                    notify.Tag,
                                    notify.Content,
                                    notify.KPILevelCode,
                                    notify.Action,
                                    SenderID = notify.UserID,
                                    RecipientID = notifyDetail.UserID,
                                    notify.TaskName
                                }).ToListAsync())
           .OrderByDescending(x => x.CreateTime)
           .Select(x => new NotificationViewModel
           {
               ID = x.ID,
               NotificationID = x.NotificationID,
               Title = x.Title,
               KPIName = _dbContext.KPIs.FirstOrDefault(kpi => kpi.ID == kpilevel.FirstOrDefault(a => a.KPILevelCode == x.KPILevelCode).KPIID).Name,
               Period = x.Period,
               Sender = _dbContext.Users.FirstOrDefault(a => a.ID == x.SenderID)?.Alias,
               Recipient = _dbContext.Users.FirstOrDefault(a => a.ID == x.RecipientID)?.Alias,
               SenderID = x.SenderID,
               RecipientID = x.RecipientID,
               CreateTime = x.CreateTime,
               Link = x.Link,
               Seen = x.Seen,
               Tag = x.Tag,
               Action = x.Action,
               TaskName = x.TaskName,
               Content = x.Content.ToSafetyString(),
               ContentDetail = x.ContentDetail,
               URL = x.URL
           }).ToList();

            return model;
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Notification entity)
        {
            throw new NotImplementedException();
        }

        public Task<object> Update(int ID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(string listID)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> IsSend()
        {
            TimeSpan timespan = new TimeSpan(00, 00, 00);
            DateTime today = DateTime.Today.Add(timespan);
            return await _dbContext.StateSendMails.FirstOrDefaultAsync(x => x.ToDay == today) == null ? false : true;
        }
    }
}
