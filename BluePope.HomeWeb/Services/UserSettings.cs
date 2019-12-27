using BluePope.HomeWeb.Services.Telegram;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Services
{
    public class UserSettings
    {
        public Dictionary<string, string> ConnectionString { get; set; }
        public BotConfiguration TelegramBot { get; set; }
        public dynamic ReCaptcha { get; set; }

        public static UserSettings GetFromJson()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usersettings.json");

            if (File.Exists(filePath) == false)
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(new UserSettings()));
            }

            return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(filePath));
        }

    }
}
