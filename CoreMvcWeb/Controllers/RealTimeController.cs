using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using CoreMvcWeb.Hubs.Chat;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace CoreMvcWeb.Controllers
{
    public class RealTimeController : Controller
    {
        static IHubContext<ChatHub> ChatHub;

        public RealTimeController(IHubContext<ChatHub> chatHub)
        {
            ChatHub = chatHub;
        }

        public IActionResult Index()
        {
            return Redirect("/realtime/chat");
        }

        public IActionResult Chat()
        {
            //https://docs.microsoft.com/ko-kr/aspnet/core/signalr/javascript-client?view=aspnetcore-2.2

            return View();
        }


        [Authorize(Roles = "ADMIN")]
        public IActionResult AdminMessage(string message)
        {
            if (message.IsNull() == false)
            {
                ChatHub.Clients.All.SendAsync("ReceiveMessage", "관리자", message);

                return Json(new { msg = "OK" });
            }

            return Json(new { msg = "메세지를 입력해주세요" });
        }
    }
}
