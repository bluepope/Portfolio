using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BluePope.HomeWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    
                    if (System.IO.File.Exists("hyunmo.pfx"))
                    {
                        webBuilder.UseKestrel(options =>
                        {
                            options.ListenAnyIP(5000);
                            options.ListenAnyIP(5001, config =>
                            {
                                config.UseHttps("hyunmo.pfx");
                            });
                        });
                    }
                });
    }
}
