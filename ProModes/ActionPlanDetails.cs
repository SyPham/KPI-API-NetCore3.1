﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ProModes
{
    public partial class ActionPlanDetails
    {
        public int Id { get; set; }
        public int ActionPlanId { get; set; }
        public int UserId { get; set; }
        public bool Sent { get; set; }
        public bool Seen { get; set; }
        public DateTime CreateTime { get; set; }
    }
}