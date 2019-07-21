using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreMvcWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options => {
                    int port = 5000;

                    string in_port = args?.FirstOrDefault(p => p.ToLower() == "--port=");
                    if (string.IsNullOrWhiteSpace(in_port) == false)
                    {
                        port = Convert.ToInt16(in_port.Replace("--port=", ""));
                    }

                    options.Limits.MaxRequestBodySize = 1000 * 1000 * 1024; //1GB; kestrel 업로드 용량 제한
                    options.ListenAnyIP(port);
                    try
                    {
                        options.ListenAnyIP(port + 1, (config) =>
                        {
                            config.UseHttps();
                        });
                    }
                    catch { }
                });
    }
}
