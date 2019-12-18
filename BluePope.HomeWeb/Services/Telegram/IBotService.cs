using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BluePope.HomeWeb.Services.Telegram
{
    public interface IBotService
    {
        BotConfiguration Config { get; }
        TelegramBotClient Bot { get; }


        Task ReceiveMessageAsync(Update update);
        Task SendTextMessageAsync(long chatId, string message);
    }
}