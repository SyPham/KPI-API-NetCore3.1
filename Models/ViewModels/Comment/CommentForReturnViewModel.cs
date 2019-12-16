using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.Comment
{
    public class CommentForReturnViewModel
    {
        public bool Status { get; set; }
        public List<string[]> ListEmails { get; set; }
        public string Message { get; set; }
    }
}
