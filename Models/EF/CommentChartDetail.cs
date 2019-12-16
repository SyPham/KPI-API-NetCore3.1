using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EF
{
   public class CommentChartDetail
    {
        public int ID { get; set; }
        public int DataID { get; set; }
        public int UserID { get; set; }
        public int? Parent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public string FileMimeType { get; set; }
        public string Content { get; set; }
        public string Pings { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int Creator { get; set; }
        public bool CreatedByAdmin { get; set; }
        public bool CreatedByCurrentUser { get; set; }
        public string File { get; set; }
        public string FileURL{ get; set; }
        public int Item { get; set; }
        public string CreateByStaff { get; set; }
        public int UpvoteCount { get; set; }
        public bool userHasUpvoted { get; set; }
        public bool IsNew { get; set; }
    }
}
