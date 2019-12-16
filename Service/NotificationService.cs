﻿using Models;
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

namespace Service
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

        public Task<List<NotificationViewModel>> ListNotifications(int userid)
        {
            throw new NotImplementedException();
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
