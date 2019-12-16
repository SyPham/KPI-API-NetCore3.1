using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Comment
{
    public class AddCommentViewModel
    {
        public string CommentMsg { get; set; }
        public int UserID { get; set; }
        public int DataID { get; set; }
        public string Tag { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string KPILevelCode { get; set; }
        public int CategoryID { get; set; }
    }
}
