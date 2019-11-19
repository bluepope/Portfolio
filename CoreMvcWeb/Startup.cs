using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using CoreMvcWeb.Services.Telegram;
using CoreMvcWeb.Services.BatchJob;
using CoreMvcWeb.Services;
using CoreMvcWeb.Services.Server;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

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
            //Cross Origin 허용
            services.AddCors((options) => {
                //options.AddPolicy("", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy() {  });
            });

            //services.AddTransient //모든 요청에 대해 생성 -- TagHelper 등으로 생성시 요청마다 
            //services.AddScoped //연결시 1회
            //services.AddSingleton //딱 1번

            //쿠키인증
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

            //쿠키인증시 백엔드 변경점을 체크하기 위한 커스텀 인증 이벤트
            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true; //쿠키정책 사용시 허용 아니면 쿠키 사용 못함
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

            var userConfig = UserSettings.GetFromJson();

            //CoreLib.DataBase.SqlDapperHelper.ConnectionString = userConfig.ConnectionString["MsSql"];
            CoreLib.DataBase.MySqlDapperHelper.ConnectionString = userConfig.ConnectionString["MySql"];
            CoreLib.DataBase.PgSqlDapperHelper.ConnectionString = userConfig.ConnectionString["PostgreSql"];
            
            if (userConfig.TelegramBot != null)
            {
                services.Configure<BotConfiguration>(options =>
                {
                    options.BotToken = userConfig.TelegramBot.BotToken;
                    options.ProxySocks5Host = userConfig.TelegramBot.ProxySocks5Host;
                    options.ProxySocks5Port = userConfig.TelegramBot.ProxySocks5Port;
                    options.WebHookUrl = userConfig.TelegramBot.WebHookUrl;
                });

                services.AddSingleton<IBotService, BotService>(); //최초 생성 후 유지
            }

            if (userConfig.ReCaptcha != null)
            {
                Models.Login.ReCaptchaModel.SiteKey = userConfig.ReCaptcha.SiteKey;
                Models.Login.ReCaptchaModel.PrivateKey = userConfig.ReCaptcha.PrivateKey;
            }
            //services.AddSingleton<ITimerBatchService, TimerBatchService>(); //타이머 싱글톤
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            //app.UseCookiePolicy(); //쿠키정책 (사용허가 승인같은거?) 사용여부
            //app.UseSession(); //세션쓰려면 session 관련 nuget 에서 추가해야함
            app.UseAuthentication(); //인증 사용
            
            //var provider = new FileExtensionContentTypeProvider();
            //provider.Mappings[".png"] = "application/x-msdownload";
            //provider.Mappings.Remove(".png");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images")),
                RequestPath = "/MyImages",
                //ContentTypeProvider = provider
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


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

            //서버 시작시 서비스 호출
            app.ApplicationServices.GetService<IBotService>(); //텔레그램 봇 생성
            //app.ApplicationServices.GetService<ITimerBatchService>(); //타이머 생성
        }
    }
}
