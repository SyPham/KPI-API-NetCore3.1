using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Helpers;
using API.SignalR;
using Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Service;
using Service.Helpers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using AutoMapper;
using Newtonsoft.Json.Serialization;

namespace API
{
    public class Startup
    {
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            CurrentEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<Helpers.AppSettings>(appSettingsSection);

            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            var conn = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataContext>(x => x.UseSqlServer(conn));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                                      {
                                          options.TokenValidationParameters = new TokenValidationParameters
                                          {
                                              ValidateIssuerSigningKey = true,

                                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                                  .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                                              ValidateIssuer = false,
                                              ValidateAudience = false
                                          };
                                      });
            // Code omitted for brevity
            services.AddSignalR();
            // configure DI for application services

            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IActionPlanService, ActionPlanService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryKPILevelService, CategoryKPILevelService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IErrorMessageService, ErrorMessageService>();
            services.AddScoped<IFavouriteService, FavouriteService>();
            services.AddScoped<IKPIService, KPIService>();
            services.AddScoped<IKPILevelService, KPILevelService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOCCategoryService, OCCategoryService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IUnitService, UnitService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            //context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            IdentityModelEventSource.ShowPII = true;
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<HenryHub>("/henryHub");
                endpoints.MapControllers();
            });

        }
    }
}
