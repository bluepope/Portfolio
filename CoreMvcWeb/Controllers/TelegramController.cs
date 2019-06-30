using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using Microsoft.AspNetCore.Authorization;
using CoreMvcWeb.Services.Telegram;
using Telegram.Bot.Types;

namespace CoreMvcWeb.Controllers
{
    [Authorize]
    public class TelegramController : Controller
    {
        private readonly IBotService _botService;
        private readonly IUpdateService _updateService;

        public TelegramController(IUpdateService updateService, IBotService botService)
        {
            _updateService = updateService;
            _botService = botService;
        }

        /// <summary>
        /// 텔레그램 webhook 등록시 동작
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody]Update update)
        {
            await _updateService.EchoAsync(update);

            return Ok();
        }
        public IActionResult Index()
        {
            return Redirect("/telegram/chatlist");
        }

        public IActionResult ChatList()
        {
            return View();
        }

        public IActionResult GetStatus()
        {
            //_botService.Config.BotToken
            //_botService.Config.Socks5Host
            //_botService.Config.Socks5Port
            //_botService.Config.WebHookUrl

            return View();
        }
    }
}
