using Models;
using Models.EF;
using Models.ViewModels.ActionPlan;
using Models.ViewModels.Comment;
using Models.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Service.helpers;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
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
    public class ActionPlanService : IActionPlanService
    {
        private readonly DataContext _dbContext;
        private readonly IErrorMessageService _errorService;
        private readonly INotificationService _notificationService;
        private readonly ILevelService _levelService;

        public ActionPlanService(DataContext dbContext,
            IErrorMessageService errorService,
            INotificationService notificationService,
            ILevelService levelService)
        {
            _dbContext = dbContext;
            _errorService = errorService;
            _notificationService = notificationService;
            _levelService = levelService;
        }
        /// <summary>
        /// Khi thêm 1 comment nếu tag nhiều user thì lưu vào bảng Tag đồng thời lưu vào bảng Notification để thông báo đẩy
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="subject"></param>
        /// <param name="auditor"></param>
        /// <returns></returns>
        public async Task<CommentForReturnViewModel> Add(ActionPlanParams obj)//(ActionPlan entity, string subject, string auditor,int catid)
        {
            var subject = obj.Subject;
            var auditor = obj.Auditor;
            var kpilevelcode = obj.KPILevelCode;
            var catid = obj.CategoryID;
            var kpilevelModel = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == kpilevelcode);
            //Kiem tra neu la owner thi moi cho add task(actionPlan)
            if (await _dbContext.Owners.FirstOrDefaultAsync(x => x.CategoryID == catid && x.UserID == obj.OwnerID && x.KPILevelID == kpilevelModel.ID) == null)
                return new CommentForReturnViewModel
                {
                    Status = false,
                    ListEmails = new List<string[]>(),
                    Message = "You are not Owner of this KPI."
                };
            else
            {
                var entity = new ActionPlan();
                entity.Title = obj.Title;
                entity.Description = obj.Description;
                entity.KPILevelCodeAndPeriod = obj.KPILevelCodeAndPeriod;
                entity.KPILevelCode = obj.KPILevelCode;
                entity.Tag = obj.Tag;
                entity.UserID = obj.UserID;
                entity.DataID = obj.DataID;
                entity.CommentID = obj.CommentID;
                entity.Link = obj.Link;
                entity.SubmitDate = obj.SubmitDate.ToDateTime();
                entity.Deadline = obj.Deadline.ToDateTime();

                var user = _dbContext.Users;
                var itemActionPlanDetail = new ActionPlanDetail();
                var listEmail = new List<string[]>();
                var listUserID = new List<int>();
                var listFullNameTag = new List<string>();
                var listTags = new List<Tag>();
                var itemTag = _dbContext.Tags;

                try
                {

                    if (!entity.Description.IsNullOrEmpty())
                    {
                        if (entity.Description.IndexOf(";") == -1)
                        {
                            entity.Description = entity.Description;

                        }
                        else
                        {
                            var des = string.Empty;
                            entity.Description.Split(';').ToList().ForEach(line =>
                            {
                                des += line + "&#13;&#10;";
                            });
                            entity.Description = des;
                        }
                    }
                    else
                    {
                        entity.Description = string.Empty;
                    }
                    var userResult = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == auditor);
                    entity.Auditor = userResult.ID;
                    _dbContext.ActionPlans.Add(entity);
                    await _dbContext.SaveChangesAsync();

                    if (!entity.Tag.IsNullOrEmpty())
                    {
                        string[] arrayString = new string[5];


                        if (entity.Tag.IndexOf(",") == -1)
                        {
                            var userItem = await user.FirstOrDefaultAsync(x => x.Username == entity.Tag);

                            if (userItem != null)
                            {
                                var tag = new Tag();
                                tag.ActionPlanID = entity.ID;
                                tag.UserID = userItem.ID;
                                _dbContext.Tags.Add(tag);
                                await _dbContext.SaveChangesAsync();

                                arrayString[0] = user.FirstOrDefault(x => x.ID == entity.UserID).FullName;
                                arrayString[1] = userItem.Email;
                                arrayString[2] = entity.Link;
                                arrayString[3] = entity.Title;
                                arrayString[4] = entity.Description;
                                listFullNameTag.Add(userItem.FullName);
                                listEmail.Add(arrayString);
                            }
                        }
                        else
                        {
                            var list = entity.Tag.Split(',');
                            var listUsers = await _dbContext.Users.Where(x => list.Contains(x.Username)).ToListAsync();
                            foreach (var item in listUsers)
                            {
                                var tag = new Tag();
                                tag.ActionPlanID = entity.ID;
                                tag.UserID = item.ID;
                                listTags.Add(tag);

                                arrayString[0] = user.FirstOrDefault(x => x.ID == entity.UserID).FullName;
                                arrayString[1] = item.Email;
                                arrayString[2] = entity.Link;
                                arrayString[3] = entity.Title;
                                arrayString[4] = entity.Description;
                                listFullNameTag.Add(item.FullName);
                                listEmail.Add(arrayString);
                            }
                            _dbContext.Tags.AddRange(listTags);
                            await _dbContext.SaveChangesAsync();
                        }
                    }


                    //Add vao Notification
                    var notify = new Notification();
                    notify.ActionplanID = entity.ID;
                    notify.Content = entity.Description;
                    notify.UserID = entity.UserID;
                    //notify.Title = entity.Title;
                    notify.Link = entity.Link;
                    notify.Tag = string.Join(",", listFullNameTag);
                    notify.Title = subject;
                    notify.Action = "Task";
                    notify.TaskName = entity.Title;
                    await _notificationService.Add(notify);

                    //add vao user


                    return new CommentForReturnViewModel
                    {
                        Status = true,
                        ListEmails = listEmail
                    };
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return new CommentForReturnViewModel
                    {
                        Status = true,
                        ListEmails = listEmail
                    };
                }
            }


        }

        public Task<bool> Add(ActionPlan entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Tuple<List<string[]>, bool>> Approve(int id, int approveby, string KPILevelCode, int CategoryID)
        {
            var listTags = new List<Tag>();
            var listEmail = new List<string[]>();
            var user = _dbContext.Users;
            var model = await _dbContext.ActionPlans.FirstOrDefaultAsync(x => x.ID == id);
            model.ApprovedBy = approveby;
            model.ApprovedStatus = !model.ApprovedStatus;
            if (model.ApprovedStatus == true)
                model.Status = true;
            else
                model.Status = false;

            try
            {


                //_dbContext.Tags.AddRange(listTags);
                await _dbContext.SaveChangesAsync();
                //Add vao Notification
                var tags = await _dbContext.Tags.Where(x => x.ActionPlanID == model.ID).ToListAsync();
                foreach (var tag in tags)
                {
                    string[] arrayString = new string[5];
                    arrayString[0] = user.Find(approveby).Alias; //Bi danh
                    arrayString[1] = user.Find(tag.UserID).Email;
                    arrayString[2] = "Approve Task";
                    arrayString[3] = model.Title;
                    listEmail.Add(arrayString);
                }
                var notify = new Notification();
                notify.ActionplanID = model.ID;
                notify.Content = model.Description;
                notify.UserID = approveby; //Nguoi xet duyet
                notify.Title = model.Title;
                notify.Link = model.Link;
                notify.TaskName = model.Title;

                if (model.Status == false && model.ApprovedStatus == false)
                {
                    notify.Action = "UpdateApproval";
                }
                else
                {
                    notify.Action = "Approval";
                }

                await _notificationService.Add(notify);
                return Tuple.Create(listEmail, true);
            }
            catch (Exception ex)
            {
                var a = new ErrorMessage();
                a.Name = ex.Message;
                a.Function = "Approval";
                await _errorService.Add(a);
                return Tuple.Create(new List<string[]>(), true);
            }
        }

        public Task<List<ActionPlan>> GetAllById(int Id)
        {
            return _dbContext.ActionPlans.Where(x => x.ID == Id).ToListAsync();
        }
        public Task<List<ActionPlan>> GetAll()
        {
            return _dbContext.ActionPlans.ToListAsync();
        }
        public async Task<PagedList<ActionPlan>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.ActionPlans.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Title.Contains(keyword));
            }
            return await PagedList<ActionPlan>.CreateAsync(source, page, pageSize);

        }
        public async Task<object> GetAll(int DataID, int CommentID, int userid)
        {
            var userModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.ID == userid);
            var permission = _dbContext.Permissions;
            var data = await _dbContext.ActionPlans
                .Where(x => x.DataID == DataID && x.CommentID == CommentID)
                .Select(x => new
                {
                    x.ID,
                    x.Title,
                    x.Description,
                    x.Tag,
                    x.ApprovedStatus,
                    x.Deadline,
                    x.UpdateSheduleDate,
                    x.ActualFinishDate,
                    x.Status,
                    x.UserID,
                    IsBoss = (int?)permission.FirstOrDefault(a => a.ID == userModel.Permission).ID < 3 ? true : false,
                    CreatedBy = x.UserID,
                    x.Auditor
                })
                .ToListAsync();
            var model = data
            .Select(x => new ActionPlanForChart
            {
                ID = x.ID,
                Title = x.Title,
                Description = x.Description,
                Tag = x.Tag,
                ApprovedStatus = x.ApprovedStatus,
                Deadline = x.Deadline.ToString("MM-dd-yyyy"),
                UpdateSheduleDate = x.UpdateSheduleDate.HasValue ? x.UpdateSheduleDate.Value.ToString("MM/dd/yyyy") : "N/A",
                ActualFinishDate = x.ActualFinishDate.HasValue ? x.ActualFinishDate.Value.ToString("MM/dd/yyyy") : "N/A",
                Status = x.Status,
                IsBoss = x.IsBoss,
                CreatedBy = x.UserID,
                ListUserIDs = _dbContext.Tags.Where(a => a.ActionPlanID == x.ID).Select(a => a.UserID).ToList(),
                Auditor = x.Auditor
            }).ToList();
            return new
            {
                status = true,
                data = model,

            };
        }
        public Task<ActionPlan> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(int Id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateActionPlan(ActionPlanForUpdateParams actionPlan)
        {
            try
            {
                var item = await _dbContext.ActionPlans.FindAsync(actionPlan.ID);
                if (actionPlan.Title.IsNullOrEmpty())
                {
                    item.Title = actionPlan.Title;
                }
                if (actionPlan.Description.IsNullOrEmpty())
                {
                    item.Description = actionPlan.Description;
                }
                if (actionPlan.Tag.IsNullOrEmpty())
                {
                    item.Tag = actionPlan.Tag;
                }
                if (actionPlan.DeadLine.IsNullOrEmpty())
                {
                    item.Deadline = Convert.ToDateTime(actionPlan.DeadLine);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Task<bool> Update(ActionPlan item)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateSheduleDate(string name, string value, string pk, int userid)
        {
            try
            {

                var id = pk.ToSafetyString().ToInt();
                var item = await _dbContext.ActionPlans.FirstOrDefaultAsync(x => x.UserID == userid && x.ID == id);
                if (item == null)
                {
                    return false;
                }
                if (name.ToLower() == "title")
                {
                    item.Title = value;
                }
                if (name.ToLower() == "description")
                {

                    if (value.IndexOf("/n") == -1)
                    {
                        item.Description = value;
                    }
                    else
                    {
                        var des = string.Empty;
                        value.Split('\n').ToList().ForEach(line =>
                        {
                            des += line + "&#13;&#10;";
                        });
                        item.Description = des;
                    }

                }
                if (name.ToLower() == "tag")
                {
                    item.Tag = value;
                }
                if (name.ToLower() == "deadline")
                {
                    item.Deadline = Convert.ToDateTime(value);
                }
                if (name.ToLower() == "updatesheduledate")
                {
                    var dt = Convert.ToDateTime(value);

                    item.UpdateSheduleDate = dt;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Tuple<List<string[]>, bool>> Done(int id, int userid, string KPILevelCode, int CategoryID)
        {
            var listTags = new List<Tag>();
            var model = await _dbContext.ActionPlans.FindAsync(id);
            var listEmail = new List<string[]>();
            var user = _dbContext.Users;
            //Chua duyet thi moi cho update lai status
            if (!model.ApprovedStatus)
            {
                //B1: Update status xong thi thong bao den cac user lien quan va owner
                model.Status = !model.Status;
                if (userid == model.UserID)
                {
                    model.UpdateSheduleDate = DateTime.Now;
                }
                else
                {
                    model.ActualFinishDate = DateTime.Now;

                }
                //B2: Thong bao den owner
                var kpiLevel = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == KPILevelCode);
                var owners = await _dbContext.Owners.Where(x => x.CategoryID == CategoryID && x.KPILevelID == kpiLevel.ID).ToListAsync();
                owners.ForEach(item =>
                {
                    var tag = new Tag();
                    tag.UserID = item.UserID;
                    tag.ActionPlanID = model.ID;
                    listTags.Add(tag);


                });

                try
                {
                    _dbContext.Tags.AddRange(listTags);
                    var tags = await _dbContext.Tags.Where(x => x.ActionPlanID == model.ID).ToListAsync();
                    foreach (var tag in tags)
                    {
                        string[] arrayString = new string[5];
                        arrayString[0] = user.Find(userid).Alias; //Bi danh
                        arrayString[1] = user.Find(tag.UserID).Email;
                        arrayString[2] = "Update Status Task";
                        arrayString[3] = model.Title;
                        listEmail.Add(arrayString);
                    }
                    await _dbContext.SaveChangesAsync();
                    //Add vao Notification
                    var notify = new Notification();
                    notify.ActionplanID = model.ID;
                    notify.Content = model.Description;
                    notify.UserID = userid;//Nguoi update Status task
                    notify.Title = model.Title;
                    notify.Link = model.Link;
                    notify.TaskName = model.Title;
                    notify.Action = "Done";
                    await _notificationService.Add(notify);
                    return Tuple.Create(listEmail, true);
                }
                catch (Exception ex)
                {
                    var a = new ErrorMessage();
                    a.Name = ex.Message;
                    a.Function = "Done";
                    await _errorService.Add(a);
                    return Tuple.Create(new List<string[]>(), false);
                }
            }
            else
            {
                return Tuple.Create(new List<string[]>(), false);
            }
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
        public async Task<object> LoadActionPlan(string role, int page, int pageSize)
        {
            var model = new List<ActionPlanViewModel>();
            switch (role.ToSafetyString().ToUpper())
            {
                case "MAN":
                    model = (await (from d in _dbContext.Datas
                                    join ac in _dbContext.ActionPlans on d.ID equals ac.DataID
                                    join kpilevelcode in _dbContext.KPILevels on d.KPILevelCode equals kpilevelcode.KPILevelCode
                                    join own in _dbContext.Managers on kpilevelcode.ID equals own.KPILevelID
                                    select new
                                    {
                                        TaskName = ac.Title,
                                        Description = ac.Description,
                                        DuaDate = ac.Deadline,
                                        UpdateSheuleDate = ac.UpdateSheduleDate,
                                        ActualFinishDate = ac.ActualFinishDate,
                                        Status = ac.Status,
                                        PIC = ac.Tag,
                                        Approved = ac.ApprovedStatus,
                                        KPIID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).KPIID,
                                        KPILevelID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).ID
                                    }).Distinct()
                      .ToListAsync())
                      .Select(x => new ActionPlanViewModel
                      {
                          TaskName = x.TaskName,
                          Description = x.Description,
                          DueDate = x.DuaDate.ToSafetyString("MM/dd/yyyy"),
                          UpdateSheduleDate = x.UpdateSheuleDate.ToSafetyString("MM/dd/yyyy"),
                          ActualFinishDate = x.ActualFinishDate.ToSafetyString("MM/dd/yyyy"),
                          Status = x.Status,
                          PIC = x.PIC,
                          Approved = x.Approved,
                          KPIName = _dbContext.KPIs.FirstOrDefault(a => a.ID == x.KPIID).Name

                      }).ToList();
                    break;
                case "OWN":

                    model = (await (from d in _dbContext.Datas
                                    join ac in _dbContext.ActionPlans on d.ID equals ac.DataID
                                    join kpilevelcode in _dbContext.KPILevels on d.KPILevelCode equals kpilevelcode.KPILevelCode
                                    join own in _dbContext.Owners on kpilevelcode.ID equals own.KPILevelID
                                    select new
                                    {
                                        TaskName = ac.Title,
                                        Description = ac.Description,
                                        DuaDate = ac.Deadline,
                                        UpdateSheuleDate = ac.UpdateSheduleDate,
                                        ActualFinishDate = ac.ActualFinishDate,
                                        Status = ac.Status,
                                        PIC = ac.Tag,
                                        Approved = ac.ApprovedStatus,
                                        KPIID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).KPIID,
                                        KPILevelID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).ID
                                    }).Distinct()
                     .ToListAsync())
                     .Select(x => new ActionPlanViewModel
                     {
                         TaskName = x.TaskName,
                         Description = x.Description,
                         DueDate = x.DuaDate.ToSafetyString("MM/dd/yyyy"),
                         UpdateSheduleDate = x.UpdateSheuleDate.ToSafetyString("MM/dd/yyyy"),
                         ActualFinishDate = x.ActualFinishDate.ToSafetyString("MM/dd/yyyy"),
                         Status = x.Status,
                         PIC = x.PIC,
                         Approved = x.Approved,
                         KPIName = _dbContext.KPIs.FirstOrDefault(a => a.ID == x.KPIID).Name

                     }).ToList();
                    break;
                case "UPD":

                    model = (await (from d in _dbContext.Datas
                                    join ac in _dbContext.ActionPlans on d.ID equals ac.DataID
                                    join kpilevelcode in _dbContext.KPILevels on d.KPILevelCode equals kpilevelcode.KPILevelCode
                                    join own in _dbContext.Uploaders on kpilevelcode.ID equals own.KPILevelID
                                    select new
                                    {
                                        KPILevelCode = d.KPILevelCode,
                                        TaskName = ac.Title,
                                        Description = ac.Description,
                                        DuaDate = ac.Deadline,
                                        UpdateSheuleDate = ac.UpdateSheduleDate,
                                        ActualFinishDate = ac.ActualFinishDate,
                                        Status = ac.Status,
                                        PIC = ac.Tag,
                                        Approved = ac.ApprovedStatus,
                                        KPIID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).KPIID,
                                        KPILevelID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).ID
                                    }).Distinct()
                     .ToListAsync())
                     .Select(x => new ActionPlanViewModel
                     {
                         TaskName = x.TaskName,
                         Description = x.Description,
                         DueDate = x.DuaDate.ToSafetyString("MM/dd/yyyy"),
                         UpdateSheduleDate = x.UpdateSheuleDate.ToSafetyString("MM/dd/yyyy"),
                         ActualFinishDate = x.ActualFinishDate.ToSafetyString("MM/dd/yyyy"),
                         Status = x.Status,
                         PIC = x.PIC,
                         Approved = x.Approved,
                         KPIName = _dbContext.KPIs.FirstOrDefault(a => a.ID == x.KPIID).Name

                     }).ToList();
                    break;
                case "SPO":

                    model = (await (from d in _dbContext.Datas
                                    join ac in _dbContext.ActionPlans on d.ID equals ac.DataID
                                    join kpilevelcode in _dbContext.KPILevels on d.KPILevelCode equals kpilevelcode.KPILevelCode
                                    join own in _dbContext.Sponsors on kpilevelcode.ID equals own.KPILevelID
                                    select new
                                    {
                                        TaskName = ac.Title,
                                        Description = ac.Description,
                                        DuaDate = ac.Deadline,
                                        UpdateSheuleDate = ac.UpdateSheduleDate,
                                        ActualFinishDate = ac.ActualFinishDate,
                                        Status = ac.Status,
                                        PIC = ac.Tag,
                                        Approved = ac.ApprovedStatus,
                                        KPIID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).KPIID,
                                        KPILevelID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).ID
                                    }).Distinct()
                    .ToListAsync())
                    .Select(x => new ActionPlanViewModel
                    {
                        TaskName = x.TaskName,
                        KPIName = _dbContext.KPIs.FirstOrDefault(a => a.ID == x.KPIID).Name,
                        Description = x.Description,
                        DueDate = x.DuaDate.ToSafetyString("MM/dd/yyyy"),
                        UpdateSheduleDate = x.UpdateSheuleDate.ToSafetyString("MM/dd/yyyy"),
                        ActualFinishDate = x.ActualFinishDate.ToSafetyString("MM/dd/yyyy"),
                        Status = x.Status,
                        PIC = x.PIC,
                        Approved = x.Approved,

                    }).ToList();
                    break;
                case "PAR":

                    model = (await (from d in _dbContext.Datas
                                    join ac in _dbContext.ActionPlans on d.ID equals ac.DataID
                                    join kpilevelcode in _dbContext.KPILevels on d.KPILevelCode equals kpilevelcode.KPILevelCode
                                    join own in _dbContext.Participants on kpilevelcode.ID equals own.KPILevelID
                                    select new
                                    {
                                        TaskName = ac.Title,
                                        Description = ac.Description,
                                        DuaDate = ac.Deadline,
                                        UpdateSheuleDate = ac.UpdateSheduleDate,
                                        ActualFinishDate = ac.ActualFinishDate,
                                        Status = ac.Status,
                                        PIC = ac.Tag,
                                        Approved = ac.ApprovedStatus,
                                        KPIID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).KPIID,
                                        KPILevelID = _dbContext.KPILevels.FirstOrDefault(a => a.KPILevelCode == d.KPILevelCode).ID
                                    }).Distinct()
                    .ToListAsync())
                    .Select(x => new ActionPlanViewModel
                    {
                        TaskName = x.TaskName,
                        Description = x.Description,
                        DueDate = x.DuaDate.ToSafetyString("MM/dd/yyyy"),
                        UpdateSheduleDate = x.UpdateSheuleDate.ToSafetyString("MM/dd/yyyy"),
                        ActualFinishDate = x.ActualFinishDate.ToSafetyString("MM/dd/yyyy"),
                        Status = x.Status,
                        PIC = x.PIC,
                        Approved = x.Approved,
                        KPIName = _dbContext.KPIs.FirstOrDefault(a => a.ID == x.KPIID).Name

                    }).ToList();
                    break;
                default:

                    break;
            }
            int totalRow = model.Count();
            model = model.OrderByDescending(x => x.KPIName)
              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();

            return new
            {
                status = true,
                data = model,
                total = totalRow,
                page = page,
                pageSize = pageSize
            };
        }
        public Tuple<List<object[]>, List<UserViewModel>> CheckLateOnUpdateData()
        {
            var listSendMail = new List<object[]>();
            var listSendMailDetail = new List<string[]>();
            var listNotify = new List<Notification>();
            var listNotifyDetail = new List<NotificationDetail>();
            var listTag = new List<Tag>();

            var dayOfWeek = DateTime.Today.DayOfWeek.ToSafetyString().ToUpper().ConvertStringDayOfWeekToNumber();
            var model2 = (from cat in _dbContext.CategoryKPILevels
                          join kpilevel in _dbContext.KPILevels on cat.KPILevelID equals kpilevel.ID
                          join kpi in _dbContext.KPIs on kpilevel.KPIID equals kpi.ID
                          join level in _dbContext.Levels on kpilevel.LevelID equals level.ID
                          select new CheckActionPlanViewmodel
                          {
                              ID = kpilevel.ID,
                              Title = kpi.Name,
                              Area = level.Name,
                              KPILevelCode = kpilevel.KPILevelCode,
                              Weekly = kpilevel.Weekly ?? 1,
                              Monthly = kpilevel.Monthly ?? DateTime.MinValue,
                              Quarterly = kpilevel.Quarterly ?? DateTime.MinValue,
                              Yearly = kpilevel.Yearly ?? DateTime.MinValue,
                          }).ToList();

            var count = 0;
            model2 = model2.ToList();
            var uploaders = (from a in model2
                             join uploader in _dbContext.Uploaders on a.ID equals uploader.KPILevelID
                             join user in _dbContext.Users on uploader.UserID equals user.ID
                             select new UserViewModel
                             {
                                 ID = user.ID,
                                 Email = user.Email
                             }).ToList();
            var owners = (from a in model2
                          join owner in _dbContext.Owners on a.ID equals owner.KPILevelID
                          join user in _dbContext.Users on owner.UserID equals user.ID
                          select new UserViewModel
                          {
                              ID = user.ID,
                              Email = user.Email
                          }).ToList();

            var listEmails = uploaders.Concat(owners).DistinctBy(x => x.ID);
            foreach (var item in model2)
            {
                var oc = _levelService.GetNode(item.KPILevelCode);
                var time = new TimeSpan(00, 00, 00, 00);

                int month = DateTime.Compare(DateTime.Today.Add(new TimeSpan(00, 00, 00, 00)), item.Monthly);
                int quarter = DateTime.Compare(DateTime.Today.Add(new TimeSpan(00, 00, 00, 00)), item.Quarterly);
                int year = DateTime.Compare(DateTime.Today.Add(new TimeSpan(00, 00, 00, 00)), item.Yearly);
                //less than zero if ToDay is earlier than item.yearly 
                //greater than zero if ToDay is later than item.yearly

                if (month > 0 || quarter > 0 || year > 0 || item.Weekly > dayOfWeek)
                {
                    count++;
                    if (month >= 0 && item.Monthly != DateTime.MinValue)
                    {
                        var itemSendMail = new object[] {
                           item.Title,item.Monthly.ToString(),item.KPILevelCode,oc,"Monthly"
                        };
                        listSendMail.Add(itemSendMail);
                    }
                    if (quarter >= 0 && item.Quarterly != DateTime.MinValue)
                    {
                        var itemSendMail = new object[] {
                           item.Title,item.Quarterly.ToString(),item.KPILevelCode,oc,"Quarterly"
                        };
                        listSendMail.Add(itemSendMail);
                    }
                    if (year >= 0 && item.Yearly != DateTime.MinValue)
                    {
                        var itemSendMail = new object[] {
                            item.Title,item.Yearly.ToString(),item.KPILevelCode,oc,"Yearly"
                        };
                        listSendMail.Add(itemSendMail);
                    }
                    if (item.Weekly >= dayOfWeek && item.Weekly != 1)
                    {
                        var itemSendMail = new object[] {
                            item.Title, item.Weekly.ConvertNumberDayOfWeekToString().ToString(),item.KPILevelCode,oc,"Weekly "
                        };
                        listSendMail.Add(itemSendMail);
                    }


                }
            }
            if (count > 0)
            {
                var notify = new Notification();
                notify.Action = "LateOnUploadData";
                notify.UserID = 1;
                _dbContext.Notifications.Add(notify);
                _dbContext.SaveChanges();

                foreach (var it in listEmails)
                {
                    var notifyDetail = new NotificationDetail();
                    notifyDetail.NotificationID = notify.ID;
                    notifyDetail.Seen = false;
                    notifyDetail.UserID = it.ID;

                    var tag = new Tag();
                    tag.UserID = it.ID;
                    tag.NotificationID = notify.ID;
                    listTag.Add(tag);
                    listNotifyDetail.Add(notifyDetail);
                }
            }
            _dbContext.Tags.AddRange(listTag);
            _dbContext.NotificationDetails.AddRange(listNotifyDetail);
            _dbContext.SaveChanges();
            return Tuple.Create(listSendMail.DistinctBy(x => x[0]).ToList(), listEmails.ToList());
        }

        public bool CreateTagOwnerAndUpdater(List<UserViewModel> uploaders, int notifyID)
        {
            var tags = new List<Tag>();
            var notificationDetails = new List<NotificationDetail>();
            foreach (var item in uploaders)
            {
                var tag = new Tag();
                tag.UserID = item.ID;
                tag.NotificationID = notifyID;
                tags.Add(tag);
            }
            try
            {
                foreach (var value in tags)
                {
                    var itemNotifyDetail = new NotificationDetail();
                    itemNotifyDetail.UserID = value.UserID;
                    itemNotifyDetail.NotificationID = notifyID;
                    itemNotifyDetail.Seen = false;
                    notificationDetails.Add(itemNotifyDetail);
                }
                _dbContext.NotificationDetails.AddRange(notificationDetails);
                _dbContext.Tags.AddRange(tags);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public Tuple<List<object[]>, List<UserViewModel>> CheckDeadline()
        {
            var listSendMail = new List<object[]>();
            var currentDate = DateTime.Now;
            var timeSpan = new TimeSpan(24, 00, 00);
            var date = currentDate - timeSpan;
            var listAc = new List<ActionPlanForCheck>();
            var listAcID = new List<int>();
            var model = _dbContext.ActionPlans.Where(x => x.Status == false && x.ApprovedStatus == false).ToList();
            foreach (var item in model)
            {
                listAcID.Add(item.ID);

            }

            var listUser = (from a in _dbContext.Tags.Where(x => listAcID.Contains(x.ActionPlanID))
                            join c in _dbContext.Users on a.UserID equals c.ID
                            select new UserViewModel
                            {
                                ID = c.ID,
                                Email = c.Email
                            }).ToList();

            var count = 0;


            foreach (var item in model)
            {
                //< 0 date nho hon deadline, > 0 date lon hon deadline
                //deadline "11/06/2019" date "11/07/2019"
                if (DateTime.Compare(item.Deadline, date) < 0)
                {
                    count++;
                    var itemSendMail = new object[] {
                        item.Title,
                        item.Deadline,
                    };
                    listSendMail.Add(itemSendMail);
                }
            }
            if (count > 0)
            {
                foreach (var item in listSendMail)
                {
                    var itemAc = new ActionPlanForCheck();
                    itemAc.UserID = 1;
                    itemAc.Deadline = Convert.ToDateTime(item[1]);
                    itemAc.Email = (string)item[0];
                    listAc.Add(itemAc);
                }
                var notify = new Notification();
                notify.Action = "LateOnTask";
                notify.UserID = 1;
                _dbContext.Notifications.Add(notify);
                _dbContext.SaveChanges();
                CreateTagOwnerAndUpdater(listUser, notify.ID);
            }
            return Tuple.Create(listSendMail, listUser);
        }
    }
}
