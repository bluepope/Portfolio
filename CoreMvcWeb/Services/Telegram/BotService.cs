using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoreMvcWeb.Services.Telegram
{
    public class BotService : IBotService
    {
        public BotConfiguration Config { get; }
        public TelegramBotClient Bot { get; }

        public BotService(IOptions<BotConfiguration> config)
        {
            this.Config = config.Value;

            if (this.Config.BotToken.IsNull())
                return;

            if (this.Config.ProxySocks5Host.IsNull())
            {
                this.Bot = new TelegramBotClient(this.Config.BotToken);
            }
            else
            {
                this.Bot = new TelegramBotClient(this.Config.BotToken, new WebProxy(this.Config.ProxySocks5Host, this.Config.ProxySocks5Port));
            }

            if (this.Config.WebHookUrl.IsNull())
            {
                this.Bot.DeleteWebhookAsync().Wait();

                var me = Bot.GetMeAsync().Result;
                //me.Username;

                Bot.OnUpdate += Bot_OnUpdate;
                //Bot.OnReceiveError += BotOnReceiveError;
                Bot.StartReceiving(Array.Empty<UpdateType>());
                //Console.WriteLine($"Start listening for @{me.Username}");
                //Bot.StopReceiving();
            }
            else
            {
                this.Bot.SetWebhookAsync(this.Config.WebHookUrl).Wait();
            }
        }

        private void Bot_OnUpdate(object sender, UpdateEventArgs e)
        {
            this.ReceiveMessageAsync(e.Update).Wait();
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }

        public async Task ReceiveMessageAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;

            //승인된 chat id 처리 해야하나?

            //명령의 연속성은 어떻게 관리하지?


            var list = new List<KeyboardButton[]>();
            list.Add(new KeyboardButton[] { "row1-1", "row1-2" });
            list.Add(new KeyboardButton[] { "row2-1", "row2-2" });
            list.Add(new KeyboardButton[] { "row3-1", "row3-2" });

            var replykey = new ReplyKeyboardMarkup(list);
            replykey.OneTimeKeyboard = true;

            if (message.Type == MessageType.Text)
            {
                if (message.Text.Trim() == "/명령")
                {
                    await this.Bot.SendTextMessageAsync(message.Chat.Id, message.Text, ParseMode.Default, false, false, 0, replykey);
                }
                else
                {
                    await this.Bot.SendTextMessageAsync(message.Chat.Id, message.Text);
                }
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var fileId = message.Photo.LastOrDefault()?.FileId;
                var file = await this.Bot.GetFileAsync(fileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await this.Bot.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await this.Bot.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }

        public async Task SendTextMessageAsync(long chatId, string message)
        {
            await this.Bot.SendTextMessageAsync(chatId, message);
        }
    }
}
