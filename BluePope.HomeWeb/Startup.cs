using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BluePope.HomeWeb.Models.Login;
using BluePope.HomeWeb.Models.User;
using BluePope.HomeWeb.Services;
using BluePope.HomeWeb.Services.Server;
using BluePope.HomeWeb.Services.Telegram;
using BluePope.Lib.DataBase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BluePope.HomeWeb
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
#if DEBUG
            services
                .AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });
#else
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            services
                .AddControllersWithViews()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });
#endif

            /*
            //Cross Origin 허용
            services.AddCors((options) => {
                //options.AddPolicy("", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy() {  });
            });
            */

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true; //쿠키정책 사용시 허용 아니면 쿠키 사용 못함
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //SignalR 추가
            services.AddSignalR();

            //전송 제한
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });

            //사용자 설정 가져오기
            var userConfig = UserSettings.GetFromJson();
            MySqlDapperHelper.ConnectionString = userConfig.ConnectionString["MySql"];
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
                MReCaptcha.SiteKey = userConfig.ReCaptcha.SiteKey;
                MReCaptcha.PrivateKey = userConfig.ReCaptcha.PrivateKey;
            }

            //services.AddTransient //모든 요청에 대해 생성 -- TagHelper 등으로 생성시 요청마다 
            //services.AddScoped //연결시 1회 - 컨트롤러 생성자 생각
            //services.AddSingleton //1회 생성 후 유지됨

            //쿠키인증시 백엔드 변경점을 체크하기 위한 커스텀 인증 이벤트
            services.AddScoped<CustomCookieAuthenticationEvents>();

            //httpcontext inject
            services.AddHttpContextAccessor();

            //로그인정보
            services.AddScoped<IUserInfo, MLoginUser>();

            //Db
            services.AddScoped<IDapperHelper, MySqlDapperHelper>();

            //services.AddSingleton<ITimerBatchService, TimerBatchService>(); //타이머 싱글톤
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    RequireHeaderSymmetry = false,
                    ForwardLimit = null,
                    KnownNetworks = { new IPNetwork(IPAddress.Parse("::ffff:172.17.0.1"), 104) }
                });

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler("/Error");

            /*
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".png"] = "application/x-msdownload";
            provider.Mappings.Remove(".png");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images")),
                RequestPath = "/MyImages",
                //ContentTypeProvider = provider
            });
            */
            app.UseStaticFiles();
            app.UseRouting();

            //app.UseCookiePolicy(); //쿠키정책 (사용허가 승인같은거?) 사용여부
            //app.UseSession(); //세션쓰려면 session 관련 nuget 에서 추가해야함

            app.UseAuthentication(); //이게 먼저와야 인증 처리됨
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<Hubs.Chat.ChatHub>("/hubs/chatHub");
            });

            //서버 시작시 서비스 호출
            //app.ApplicationServices.GetService<IBotService>(); //텔레그램 봇 생성
            //app.ApplicationServices.GetService<ITimerBatchService>(); //타이머 생성
        }
    }
}
