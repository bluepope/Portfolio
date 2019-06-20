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
                    options.Limits.MaxRequestBodySize = 1000 * 1000 * 1024; //1GB; kestrel 업로드 용량 제한
                    if (args != null)
                    {
                        foreach (var arg in args)
                        {
                            if (arg.Trim().ToLower().IndexOf("--port=") == 0)
                            {
                                options.ListenAnyIP(Convert.ToInt16(arg.Trim().ToLower().Replace("--port=", "")));
                            }
                        }
                    }
                });
    }
}
