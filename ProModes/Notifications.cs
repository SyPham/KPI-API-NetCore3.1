﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ProModes
{
    public partial class Notifications
    {
        public Notifications()
        {
            SubNotifications = new HashSet<SubNotifications>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Kpiname { get; set; }
        public string Period { get; set; }
        public string Action { get; set; }
        public string Link { get; set; }
        public DateTime CreateTime { get; set; }
        public string Tag { get; set; }
        public string KpilevelCode { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CommentId { get; set; }
        public int ActionplanId { get; set; }
        public string TaskName { get; set; }

        public virtual ICollection<SubNotifications> SubNotifications { get; set; }
    }
}