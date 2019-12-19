using Microsoft.EntityFrameworkCore;
using Models.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Data
{
    public class DataContext : DbContext
    {
        public DbSet<KPI> KPIs { get; set; }
        public DbSet<KPILevel> KPILevels { get; set; }

        public DbSet<Models.EF.Data> Datas { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Level> Levels { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<SeenComment> SeenComments { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Unit> Units { get; set; }

        public DbSet<Revise> Revises { get; set; }
        public DbSet<ActionPlan> ActionPlans { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActionPlanDetail> ActionPlanDetails { get; set; }
        public DbSet<NotificationDetail> NotificationDetails { get; set; }
        public DbSet<Tag> Tags { get; set; }
        //public DbSet<Language> Languages { get; set; }
        public DbSet<ErrorMessage> ErrorMessages { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Uploader> Uploaders { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<CategoryKPILevel> CategoryKPILevels { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<OCCategory> OCCategories { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<StateSendMail> StateSendMails { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<MenuLang> MenuLangs { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


        }
    }
}
