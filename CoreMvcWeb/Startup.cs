using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMvcWeb.Services.Telegram;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CoreMvcWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => { options.LoginPath = "/Login"; });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //identity role 사용 등록
            services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>();

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()) //return Json 에서 camelcase 강제 적용 안함
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            

            //SignalR 추가
            services.AddSignalR();

            //전송 제한
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });

            //Telegram Bot Service추가
            
            services.Configure<BotConfiguration>(options =>
            {
                var config = BotConfiguration.GetFromJson();
                options.BotToken = config.BotToken;
                options.ProxySocks5Host = config.ProxySocks5Host;
                options.ProxySocks5Port = config.ProxySocks5Port;
                options.WebHookUrl = config.WebHookUrl;
            });

            //services.AddScoped<IUpdateService, UpdateService>(); //request 마다 생성
            services.AddSingleton<IBotService, BotService>(); //최초 생성 후 유지

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //인증을 사용한다는 선언
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //SignalR 추가
            app.UseSignalR(routes =>
            {
                routes.MapHub<Hubs.Chat.ChatHub>("/hubs/chathub");
            });

            //서버 시작시 텔레그램 봇 최초 생성
            app.ApplicationServices.GetService<IBotService>();
        }
    }
}
