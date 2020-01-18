using Models.EF;
using Models.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data;
using System.Text.RegularExpressions;
using System.Threading;

namespace Service.Implementation
{

    public class CommentService : ICommentService
    {
        private readonly DataContext _dbContext;
        private readonly INotificationService _notificationService;
        private readonly IMailExtension _mailHelper;
        private readonly ISettingService _settingService;

        public CommentService(DataContext dbContext,
            IMailExtension mailHelper,
            ISettingService settingService,
            INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
            _mailHelper = mailHelper;
            _settingService = settingService;
        }

        #region Common
        public Task<bool> Add(Comment entity)
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
        public Task<List<Comment>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetAllById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<Comment>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _dbContext.Comments.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Title.Contains(keyword));
            }
            return await PagedList<Comment>.CreateAsync(source, page, pageSize);
        }

        public Task<Comment> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Remove(int Id)
        {
            var item = await _dbContext.Comments.FindAsync(Id);
            _dbContext.Comments.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {

                return false;

            }
        }

        public Task<bool> Update(Comment entity)
        {
            throw new NotImplementedException();
        }
        #endregion
        public async Task<bool> CheckLevelNumberOfUser(int OCIDOfUserComment, int OCIDOfOwner)
        {
            var leveNumberOfUserComment = await _dbContext.Levels.FindAsync(OCIDOfUserComment);
            var leveNumberOfOwner = await _dbContext.Levels.FindAsync(OCIDOfOwner);
            if (leveNumberOfUserComment.LevelNumber > leveNumberOfOwner.LevelNumber)
                return true;
            return false;
        }
        public async Task<Comment> CreateComment(Comment comment)
        {
            try
            {
                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();
                return comment;
            }
            catch (Exception)
            {

                return comment;
            }
        }
        public async Task<Tuple<List<string[]>, List<string>, List<Tag>>> CreateTag(string users, int userID, Comment comment)
        {
            List<string[]> listEmail = new List<string[]>();
            List<Tag> listTags = new List<Tag>();
            List<string> listFullNameTag = new List<string>();
            var user = _dbContext.Users;
            if (users.IndexOf(',') == -1) //Neu tag 1 nguoi
            {
                if (users.IndexOf("@") != -1)
                {
                    users = users.Replace("@", "").Trim();
                }
                var username = users;

                var recipient = await user.FirstOrDefaultAsync(x => x.Username == username);// nguoi nhan
                if (recipient != null)
                {
                    var itemtag = new Tag();
                    itemtag = new Tag();
                    itemtag.UserID = recipient.ID;
                    itemtag.CommentID = comment.ID;

                    string[] arrayString = new string[5];
                    arrayString[0] = user.Find(userID).Alias;
                    arrayString[1] = recipient.Email;
                    arrayString[2] = comment.Link;
                    arrayString[3] = comment.Title;
                    arrayString[4] = comment.CommentMsg;

                    listFullNameTag.Add(recipient.Alias);

                    listEmail.Add(arrayString);
                    listTags.Add(itemtag);
                }

            }
            else//Tag nhieu nguoi
            {
                if (users.IndexOf("@") != -1)
                {
                    Regex pattern = new Regex("s[@]");
                    pattern.Replace(users, "");
                }

                var list = users.Split(',');

                var commentID = comment.ID;
                var listUserID = await _dbContext.Tags.Where(x => x.ActionPlanID == comment.ID).Select(x => x.UserID).ToListAsync();
                var listUsers = await _dbContext.Users.Where(x => list.Contains(x.Username)).ToListAsync();
                foreach (var item in listUsers)
                {
                    string[] arrayString = new string[5];
                    var itemtag = new Tag();
                    itemtag.CommentID = comment.ID;
                    itemtag.UserID = item.ID;

                    arrayString[0] = user.Find(userID).Alias;
                    arrayString[1] = item.Email;
                    arrayString[2] = comment.Link;
                    arrayString[3] = comment.Title;
                    arrayString[4] = comment.CommentMsg;

                    listTags.Add(itemtag);
                    listEmail.Add(arrayString);
                    listFullNameTag.Add(item.Alias);
                }
            }
            return Tuple.Create(listEmail, listFullNameTag, listTags);

        }
        public async Task<SeenComment> CreateSeenComment(SeenComment seenComment)
        {
            try
            {
                _dbContext.SeenComments.Add(seenComment);
                await _dbContext.SaveChangesAsync();
                return seenComment;
            }
            catch (Exception)
            {

                return seenComment;
            }
        }
        public async Task<CommentForReturnViewModel> AddComment(AddCommentViewModel entity, int levelIDOfUserComment)
        {
            var listEmail = new List<string[]>();
            var listTags = new List<Tag>();
            var listFullNameTag = new List<string>();
            var user = _dbContext.Users;
            var dataModel = _dbContext.Datas;
            string queryString = string.Empty;
            try
            {
                //add vao comment

                var comment = await CreateComment(new Comment
                {
                    CommentMsg = entity.CommentMsg,
                    DataID = entity.DataID,
                    UserID = entity.UserID,//sender
                    Link = entity.Link,
                    Title = entity.Title
                });
                if (comment.ID > 0)
                {
                    var updateLink = await _dbContext.Comments.FirstOrDefaultAsync(x => x.ID == comment.ID);

                    if (!updateLink.Link.Contains("title"))
                    {
                        //Replace Remark become Action Plan
                        var title = Regex.Replace(comment.Title.Split('-')[0].ToSafetyString(), @"\s+", "-");
                        updateLink.Link = updateLink.Link + $"/remark/{comment.ID}/{comment.DataID}/{title}";
                        await _dbContext.SaveChangesAsync();
                        queryString = updateLink.Link;

                    }
                }
                //B1: Xu ly viec gui thong bao den Owner khi nguoi gui cap cao hon comment
                //Tim levelNumber cua user comment
                var kpilevelIDResult = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == entity.KPILevelCode);
                var userIDResult = await _dbContext.Owners.FirstOrDefaultAsync(x => x.KPILevelID == kpilevelIDResult.ID && x.CategoryID == entity.CategoryID);
                var userModel = await _dbContext.Users.FindAsync(userIDResult.UserID);

                //Lay ra danh sach owner thuoc categoryID va KPILevelCode
                var owners = await _dbContext.Owners.Where(x => x.KPILevelID == kpilevelIDResult.ID && x.CategoryID == entity.CategoryID).ToListAsync();

                //Neu nguoi comment ma la cap cao hon owner thi moi gui thong bao va gui mail cho owner
                if (await CheckLevelNumberOfUser(levelIDOfUserComment, userModel.LevelID))
                {
                    owners.ForEach(userItem =>
                    {
                        //Add Tag gui thong bao den cac owner
                        if (entity.UserID != userItem.ID) //Neu chinh owner do binh luan thi khong gui thong bao
                        {
                            var itemtag = new Tag();
                            itemtag = new Tag();
                            itemtag.UserID = userItem.ID;
                            itemtag.CommentID = comment.ID;

                            listTags.Add(itemtag); //Day la danh sach tag
                            //Add vao list gui mail
                            string[] arrayString = new string[5];
                            arrayString[0] = user.Find(entity.UserID).Alias; //Bi danh
                            arrayString[1] = user.Find(entity.UserID).Email;
                            arrayString[2] = comment.Link;
                            arrayString[3] = comment.Title;
                            arrayString[4] = comment.CommentMsg;

                            listEmail.Add(arrayString);
                        }

                    });
                    //B2: Neu ma nguoi cap cao hon owner tag ai do vao comment cua ho thi se gui mail va thong bao den nguoi do
                    if (!entity.Tag.IsNullOrEmpty())
                    {
                        var result = await CreateTag(entity.Tag, entity.UserID, comment);
                        listEmail = result.Item1;
                        listFullNameTag = result.Item2;
                        listTags = result.Item3;
                    }
                }
                else //Neu user co level nho hon owner commnent thi gui den owner 
                {
                    //B1: Gui thong bao den cac owner
                    owners.ForEach(x =>
                    {
                        //Add vao Tag de gui thong 
                        if (entity.UserID != x.UserID)
                        {
                            var itemtag = new Tag();
                            itemtag = new Tag();
                            itemtag.UserID = x.UserID;
                            itemtag.CommentID = comment.ID;

                            listTags.Add(itemtag); //Day la danh sach tag
                        }

                    });

                    //B2: Neu tag ai thi gui thong bao den nguoi do
                    if (!entity.Tag.IsNullOrEmpty())
                    {
                        var result = await CreateTag(entity.Tag, entity.UserID, comment);
                        listEmail = result.Item1;
                        listFullNameTag = result.Item2;
                        listTags = result.Item3;
                    }
                }
                //Add vao seencomment
                var seenComment = new SeenComment();
                seenComment.CommentID = comment.ID;
                seenComment.UserID = entity.UserID;
                seenComment.Status = true;

                _dbContext.Tags.AddRange(listTags);
                await _dbContext.SaveChangesAsync();
                await CreateSeenComment(seenComment);

                if (listTags.Count > 0)
                {
                    //Add vao Notification
                    var notify = new Notification();
                    notify.CommentID = comment.ID;
                    notify.Content = comment.CommentMsg;
                    notify.UserID = entity.UserID; //sender
                    notify.Title = comment.Title;
                    notify.Link = comment.Link;
                    notify.Action = "Comment";
                    notify.Tag = string.Join(",", listFullNameTag);
                    await _notificationService.Add(notify);
                }

                if (listEmail.Count > 0 && await _settingService.IsSendMail("ADDCOMMENT"))
                {
                    var model = listEmail.DistinctBy(x => x);
                    //string from = ConfigurationManager.AppSettings["FromEmailAddress"].ToSafetyString();
                    string content = @"<p><b>*PLEASE DO NOT REPLY* this email was automatically sent from the KPI system.</b></p> 
                                   <p>The account <b>" + model.First()[0] + "</b> mentioned you in KPI System Apps. </p>" +
                                      "<p>Content: " + model.First()[4] + "</p>" +
                                      "<p>Link: <a href='" + queryString + "'>Click Here</a></p>";
                    Thread thread = new Thread(async () =>
                    {
                        await _mailHelper.SendEmailRange(model.Select(x => x[1]).ToList(), "[KPI System-02] Comment", content);
                    });
                    thread.Start();
                }

                return new CommentForReturnViewModel
                {
                    Status = true,
                    ListEmails = listEmail,
                    QueryString = queryString
                };
            }
            catch (Exception ex)
            {
                return new CommentForReturnViewModel { Status = false };
            }
        }

        public async Task<bool> AddCommentHistory(int userid, int dataid)
        {
            try
            {
                var comments = await _dbContext.Comments.Where(x => x.DataID == dataid).ToListAsync();
                foreach (var comment in comments)
                {
                    var item = await _dbContext.SeenComments.FirstOrDefaultAsync(x => x.UserID == userid && x.CommentID == comment.ID);
                    if (item == null)
                    {
                        var seencmt = new SeenComment();
                        seencmt.CommentID = comment.ID;
                        seencmt.UserID = userid;
                        seencmt.Status = true;
                        _dbContext.SeenComments.Add(seencmt);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Lấy ra các danh sách comment theo từng Value của KPILevel
        /// </summary>
        /// <param name="dataid">Là giá trị của KPILevel upload</param>
        /// <returns>Trả về các comment theo dataid</returns>
        public async Task<object> ListComments(int dataid, int userid)
        {

            var actionPlan = _dbContext.ActionPlans;
            //Cat chuoi
            //lay tat ca comment cua kpi
            var listcmts = await _dbContext.Comments.Where(x => x.DataID == dataid).ToListAsync();

            //Tong tat ca cac comment cua kpi
            var totalcomment = listcmts.Count();

            //Lay ra tat ca lich su cmt
            var seenCMT = _dbContext.SeenComments;

            //Lay ra tat ca lich su cmt
            var user = _dbContext.Users;

            //Lay ra tat ca cac comment cua kpi(userid nao post comment len thi mac dinh userid do da xem comment cua chinh minh roi)
            var data = await _dbContext.Comments.Where(x => x.DataID == dataid)
               .Select(x => new CommentViewModel
               {
                   CommentID = x.ID,
                   UserID = x.UserID,
                   CommentMsg = x.CommentMsg,
                   //KPILevelCode = x.KPILevelCode,
                   CommentedDate = x.CommentedDate,
                   FullName = user.FirstOrDefault(a => a.ID == x.UserID).FullName,
                   //Period = x.Period,
                   Read = seenCMT.FirstOrDefault(a => a.CommentID == x.ID && a.UserID == userid) == null ? true : false,
                   IsHasTask = actionPlan.FirstOrDefault(a => a.DataID == dataid && a.CommentID == x.ID) == null ? false : true,
                   Task = actionPlan.FirstOrDefault(a => a.DataID == dataid && a.CommentID == x.ID) == null ? false : true
               })
               .OrderByDescending(x => x.CommentedDate)
               .ToListAsync();

            return new
            {
                data,
                total = _dbContext.Comments.Where(x => x.DataID == dataid).Count()
            };

        }
        public async Task<object> DeleteComment(int id, int userid)
        {
            try
            {
                var item = await _dbContext.Comments.FirstOrDefaultAsync(x => x.ID == id && x.UserID == userid);

                if (userid != item.UserID || item == null)
                {
                    return new
                    {
                        status = false,
                        message = "You are not authorized to delete this comment!"
                    };
                }
                else
                {
                    _dbContext.Comments.Remove(item);
                    await _dbContext.SaveChangesAsync();
                    return new
                    {
                        status = true,
                        message = "Successfully!"
                    };
                }


            }
            catch
            {

                return new
                {
                    message = "You are not authorized to delete this comment!",
                    status = false
                };
            }

        }

    }
}
