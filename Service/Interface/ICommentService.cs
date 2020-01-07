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

namespace Service.Interface
{
    public interface ICommentService: IDisposable,ICommonService<Comment>
    {
        Task<CommentForReturnViewModel> AddComment(AddCommentViewModel entity, int levelIDOfUserComment);
        Task<object> ListComments(int dataid, int userid);
        Task<bool> AddCommentHistory(int userid, int dataid);
        Task<object> DeleteComment(int id, int userid);
    }
    
}
