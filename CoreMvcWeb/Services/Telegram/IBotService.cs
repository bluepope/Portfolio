using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CoreMvcWeb.Services.Telegram
{
    public interface IBotService
    {
        BotConfiguration Config { get; }
        TelegramBotClient Bot { get; }
    }
}