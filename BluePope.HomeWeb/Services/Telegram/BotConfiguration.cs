using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Services.Telegram
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public string ProxySocks5Host { get; set; }

        public int ProxySocks5Port { get; set; }

        public string WebHookUrl { get; set; }

        public static BotConfiguration GetFromJson()
        {
            var path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "telegrambot-config.json");

            if (System.IO.File.Exists(path))
            {
                return JsonConvert.DeserializeObject<BotConfiguration>(System.IO.File.ReadAllText(path));
            }
            else
            {
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(new BotConfiguration()));

                throw new Exception($"{path} is empty");
            }
        }
    }
}
