using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CoreMvcWeb.Services.Telegram
{
    public class BotService : IBotService
    {
        public TelegramBotClient Client { get; }

        public BotService(IOptions<BotConfiguration> config)
        {
            if (config.Value.Socks5Host.IsNull())
            {
                this.Client = new TelegramBotClient(config.Value.BotToken);
            }
            else
            {
                this.Client = new TelegramBotClient(config.Value.BotToken, new WebProxy(config.Value.Socks5Host, config.Value.Socks5Port));
            }

            if (config.Value.WebHookUrl.IsNull())
            {
                this.Client.DeleteWebhookAsync().Wait();
            }
            else
            {
                this.Client.SetWebhookAsync(config.Value.WebHookUrl).Wait();
            }
        }

    }
}
