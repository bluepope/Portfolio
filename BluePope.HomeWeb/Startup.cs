using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluePope.HomeWeb.Models.Login;
using BluePope.HomeWeb.Services;
using BluePope.HomeWeb.Services.Server;
using BluePope.HomeWeb.Services.Telegram;
using BluePope.Lib.DataBase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
            services
                .AddControllersWithViews()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });

            /*
            //Cross Origin ���
            services.AddCors((options) => {
                //options.AddPolicy("", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy() {  });
            });
            */

            //��Ű����
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

            //services.AddTransient //��� ��û�� ���� ���� -- TagHelper ������ ������ ��û���� 
            //services.AddScoped //����� 1ȸ - ��Ʈ�ѷ� ������ ����
            //services.AddSingleton //1ȸ ���� �� ������


            //��Ű������ �鿣�� �������� üũ�ϱ� ���� Ŀ���� ���� �̺�Ʈ
            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true; //��Ű��å ���� ��� �ƴϸ� ��Ű ��� ����
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //SignalR �߰�
            services.AddSignalR();

            //���� ����
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });

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

                services.AddSingleton<IBotService, BotService>(); //���� ���� �� ����
            }

            if (userConfig.ReCaptcha != null)
            {
                MReCaptcha.SiteKey = userConfig.ReCaptcha.SiteKey;
                MReCaptcha.PrivateKey = userConfig.ReCaptcha.PrivateKey;
            }
            //services.AddSingleton<ITimerBatchService, TimerBatchService>(); //Ÿ�̸� �̱���
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
                app.UseExceptionHandler("/Home/Error");
            }

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

            //app.UseCookiePolicy(); //��Ű��å (����㰡 ���ΰ�����?) ��뿩��
            //app.UseSession(); //���Ǿ����� session ���� nuget ���� �߰��ؾ���


            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<Hubs.Chat.ChatHub>("/hubs/chatHub");
            });

            //���� ���۽� ���� ȣ��
            //app.ApplicationServices.GetService<IBotService>(); //�ڷ��׷� �� ����
            //app.ApplicationServices.GetService<ITimerBatchService>(); //Ÿ�̸� ����
        }
    }
}
